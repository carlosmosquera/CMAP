using System.Collections.Generic;
using UnityEngine;
using TMPro;
using extOSC;
using UnityEngine.UI; // For Button UI
using System.Collections;

public class CircularMovementManagerv2 : MonoBehaviour
{
    public OSCTransmitter Transmitter;

    public OSCReceiver Receiver;



    public CustomZoneSpawner zoneSpawner; // Reference to CustomZoneSpawner to access degreeAngles

    [System.Serializable]
    public class CircularObject
    {
        public Transform objectTransform;
        public SpriteRenderer spriteRenderer;

        [HideInInspector]
        public bool isDragging = false;
        [HideInInspector]
        public int angle = 0;  // Store angle as an integer
        [HideInInspector]
        public float snappedAngle = float.NaN; // Store snapped angle
    }

    public List<CircularObject> circularObjects = new List<CircularObject>();
    public List<TMP_Text> sliderTexts = new List<TMP_Text>(); // Reference to the TMP_Text of each slider

    public Button snapButton; // Reference to the Snap Button

    private CircularObject lastSelectedObject = null; // Track the last selected object
    private TMP_Text lastSelectedSliderText = null; // Track the last selected slider text

    public Transform slidersParent; // Reference to the parent of all sliders


    private IEnumerator DelayedSendPosition()
    {
        yield return new WaitForSeconds(0.5f); // Delay of 0.1 seconds (adjust if necessary)

        if (Transmitter == null)
        {
            Debug.LogError("OSC Transmitter is not assigned in the Inspector!");
            yield break; // Stop the coroutine if Transmitter is missing
        }

        if (circularObjects.Count == 0)
        {
            Debug.LogWarning("CircularObjects list is empty. Nothing to send.");
        }
        else
        {
            SendPosition(); // Call SendPosition after the delay
        }
    }

    void Start()
    {

        StartCoroutine(DelayedSendPosition());


        // Retrieve the TMP_Text components from each child of the parent folder
        for (int i = 0; i < slidersParent.childCount; i++)
        {
            Transform sliderChild = slidersParent.GetChild(i);
            //Debug.Log($"Child {i}: {sliderChild.name}");

            Transform objNumberTransform = sliderChild.Find("Obj Number");
            if (objNumberTransform != null)
            {
                // Check for TextMeshPro or TextMeshProUGUI components
                TMP_Text textMeshPro = objNumberTransform.GetComponent<TMP_Text>();

                if (textMeshPro != null)
                {
                    sliderTexts.Add(textMeshPro);
                    //Debug.Log($"Slider Text Added: {textMeshPro.text}");

                    // Add BoxCollider2D to Obj Number if it doesn't exist
                    BoxCollider2D collider = objNumberTransform.GetComponent<BoxCollider2D>();
                    if (collider == null)
                    {
                        collider = objNumberTransform.gameObject.AddComponent<BoxCollider2D>();
                    }

                    // Set collider properties
                    collider.isTrigger = true;
                    collider.offset = new Vector2(100, 0);
                    collider.size = new Vector2(450, 72);
                }
                else
                {
                    Debug.LogWarning($"TextMeshPro component not found in child 'Obj Number' of {sliderChild.name}");
                }
            }
            else
            {
                Debug.LogWarning($"Child named 'Obj Number' not found in {sliderChild.name}");
            }
        }

        Debug.Log($"Total Slider Texts Found: {sliderTexts.Count}");

        // Initialize the number for each object
        for (int i = 0; i < circularObjects.Count; i++)
        {
            CircularObject obj = circularObjects[i];

            // Create a new GameObject for the number
            GameObject numberObject = new GameObject("Number");
            numberObject.transform.SetParent(obj.objectTransform);
            numberObject.transform.localPosition = Vector3.zero;

            // Add and configure the TextMesh component
            TextMeshPro textMesh = numberObject.AddComponent<TextMeshPro>();
            textMesh.text = (i + 1).ToString();
            textMesh.fontSize = 3;
            textMesh.color = Color.white;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.sortingOrder = 12; // Ensure text is always on top

            // Adjust the position if necessary
            numberObject.transform.localPosition = new Vector3(0, 0, -1);

            // Add a SpriteRenderer if not already set
            if (obj.spriteRenderer == null)
            {
                obj.spriteRenderer = obj.objectTransform.GetComponent<SpriteRenderer>();
                if (obj.spriteRenderer == null)
                {
                    obj.spriteRenderer = obj.objectTransform.gameObject.AddComponent<SpriteRenderer>();
                }
            }

            // Set the initial appearance of the circular object
            obj.spriteRenderer.color = new Color(0.2830189f, 0.1081346f, 0.1081346f); // Unselected
            obj.spriteRenderer.sortingOrder = 8;


        }

        snapButton.onClick.AddListener(SnapObjectsToClosestAngle); // Assign Snap Button click event



    }





    void SendPosition()
    {


        foreach (var obj in circularObjects)
        {


        // Convert position to polar angle in degrees (clockwise), normalize to [0, 360) with 0 degrees at the top
        float outPolarFloat = Mathf.Atan2(-obj.objectTransform.position.y, obj.objectTransform.position.x) * Mathf.Rad2Deg;
        int outPolar = Mathf.RoundToInt((outPolarFloat + 90 + 360) % 360); // Add 90 degrees to offset the zero to the top

        int ObjectNumber = circularObjects.IndexOf(obj) + 1;

        // Create an OSC message with the address and polar angle
        var messagePan = new OSCMessage("/objectPosition");
        messagePan.AddValue(OSCValue.Int(ObjectNumber));
        messagePan.AddValue(OSCValue.Int(outPolar)); // Send outPolar as an integer

        Debug.Log($"Object {ObjectNumber} position: {outPolar} degrees");

        Transmitter.Send(messagePan);

        }
    }   



        void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Get all hits under the mouse position
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hits.Length > 0)
            {
                bool isObjNumberSelected = false;
                CircularObject topmostObject = null;
                float highestSortingOrder = float.MinValue;

                // Find the topmost object or TMP_Text that matches a hit
                foreach (var hit in hits)
                {
                    // Check if the hit is on a TMP_Text for the slider (Obj Number)
                    foreach (var text in sliderTexts)
                    {
                        if (hit.collider.transform == text.transform)
                        {
                            SelectSliderText(text);
                            isObjNumberSelected = true;

                            // Select corresponding circular object without enabling dragging
                            int index = sliderTexts.IndexOf(text);
                            if (index != -1)
                            {
                                HighlightObjectWithoutDragging(circularObjects[index]);
                            }

                            break;
                        }
                    }

                    // Skip selecting a CircularObject if Obj Number was selected
                    if (isObjNumberSelected) break;

                    // Check if the hit is on a CircularObject
                    foreach (var obj in circularObjects)
                    {
                        if (hit.collider.transform == obj.objectTransform && obj.spriteRenderer.sortingOrder > highestSortingOrder)
                        {
                            topmostObject = obj;
                            highestSortingOrder = obj.spriteRenderer.sortingOrder;
                        }
                    }
                }

                // Select the topmost object if found
                if (topmostObject != null)
                {
                    SelectObject(topmostObject);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            foreach (var obj in circularObjects)
            {
                if (obj.isDragging)
                {
                    obj.isDragging = false;
                    // Keep the darker color for the currently selected object
                    if (obj == lastSelectedObject)
                    {
                        obj.spriteRenderer.color = new Color(1.0f, 0.8591049f, 0.0f); // highlighted
                    }
                    else
                    {
                        obj.spriteRenderer.color = new Color(0.2830189f, 0.1081346f, 0.1081346f); // Unselected
                    }
                }
            }
        }


    



        foreach (var obj in circularObjects)
        {
            if (obj.isDragging)
            {
                // Move object in a circular path
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = mousePosition.normalized;

                // Set the object's position to the edge of the circle with a radius of 3.0f
                obj.objectTransform.position = direction * 3.0f;

                // Convert position to polar angle in degrees (clockwise), normalize to [0, 360) with 0 degrees at the top
                float outPolarFloat = Mathf.Atan2(-obj.objectTransform.position.y, obj.objectTransform.position.x) * Mathf.Rad2Deg;
                int outPolar = Mathf.RoundToInt((outPolarFloat + 90 + 360) % 360); // Add 90 degrees to offset the zero to the top

                int ObjectNumber = circularObjects.IndexOf(obj) + 1;

                // Create an OSC message with the address and polar angle
                var messagePan = new OSCMessage("/objectPosition");
                messagePan.AddValue(OSCValue.Int(ObjectNumber));
                messagePan.AddValue(OSCValue.Int(outPolar)); // Send outPolar as an integer

                Debug.Log($"Object {ObjectNumber} position: {outPolar} degrees");

                Transmitter.Send(messagePan);
            }
        }
    }

    void SelectSliderText(TMP_Text text)
    {
        Debug.Log("SelectSliderText called");
        int index = sliderTexts.IndexOf(text);
        Debug.Log($"Index of clicked text: {index}");

        if (index != -1)
        {
            Debug.Log("Valid index found, highlighting text");

            // Highlight the text
            if (lastSelectedSliderText != null)
            {
                lastSelectedSliderText.color = Color.white; // Revert previous slider text to black
            }

            lastSelectedSliderText = sliderTexts[index];
            lastSelectedSliderText.color = new Color(1.0f, 0.8591049f, 0.0f); // Highlighted
            Debug.Log($"Highlighting slider text for object {index + 1}");
        }
        else
        {
            Debug.LogWarning($"Index {index} is invalid or not found in sliderTexts");
        }
    }

    void HighlightObjectWithoutDragging(CircularObject obj)
    {
        if (lastSelectedObject != null && lastSelectedObject != obj)
        {
            // Revert the color and sorting order of the previously selected object
            lastSelectedObject.spriteRenderer.color = new Color(0.2830189f, 0.1081346f, 0.1081346f); // Unselected
            lastSelectedObject.spriteRenderer.sortingOrder = 8; // Reset sorting order to original value

            // Change the color of the previously selected TextMeshPro back to white
            Transform numberTransform = lastSelectedObject.objectTransform.Find("Number");
            if (numberTransform != null)
            {
                var textMeshPro = numberTransform.GetComponent<TextMeshPro>();
                if (textMeshPro != null)
                {
                    textMeshPro.color = Color.white; // Deselect text
                    textMeshPro.sortingOrder = 9; // Lower sorting order
                }
            }
        }

        // Highlight the current selected object
        obj.spriteRenderer.color = new Color(1.0f, 0.8591049f, 0.0f);  // highlighted
        obj.spriteRenderer.material.SetFloat("_Glossiness", 0.4f); // Optional: Add glossiness for effect
        obj.spriteRenderer.sortingOrder = 11; // Set higher sorting order to bring it on top

        // Change the color of the currently selected TextMeshPro to black
        Transform currentNumberTransform = obj.objectTransform.Find("Number");
        if (currentNumberTransform != null)
        {
            var currentTextMeshPro = currentNumberTransform.GetComponent<TextMeshPro>();
            if (currentTextMeshPro != null)
            {
                currentTextMeshPro.color = Color.black; // Select text
                currentTextMeshPro.sortingOrder = 12; // Higher sorting order to keep text on top
            }
        }

        lastSelectedObject = obj; // Update the last selected object
    }


    void SelectObject(CircularObject obj)
    {
        if (lastSelectedObject != null && lastSelectedObject != obj)
        {
            // Revert the color and sorting order of the previously selected object
            lastSelectedObject.spriteRenderer.color = new Color(0.2830189f, 0.1081346f, 0.1081346f); // Unselected
            lastSelectedObject.spriteRenderer.sortingOrder = 8; // Reset sorting order to original value

            // Change the color of the previously selected TextMeshPro back to white
            Transform numberTransform = lastSelectedObject.objectTransform.Find("Number");
            if (numberTransform != null)
            {
                var textMeshPro = numberTransform.GetComponent<TextMeshPro>();
                if (textMeshPro != null)
                {
                    textMeshPro.color = Color.white; // Deselect text
                    textMeshPro.sortingOrder = 9; // Lower sorting order
                }
            }
        }

        // Highlight the current selected object
        obj.spriteRenderer.color = new Color(1.0f, 0.8591049f, 0.0f); // highlighted
        obj.spriteRenderer.material.SetFloat("_Glossiness", 0.4f); // Optional: Add glossiness for effect
        obj.spriteRenderer.sortingOrder = 11; // Set higher sorting order to bring it on top

        // Change the color of the currently selected TextMeshPro to black
        Transform currentNumberTransform = obj.objectTransform.Find("Number");
        if (currentNumberTransform != null)
        {
            var currentTextMeshPro = currentNumberTransform.GetComponent<TextMeshPro>();
            if (currentTextMeshPro != null)
            {
                currentTextMeshPro.color = Color.black; // Select text
                currentTextMeshPro.sortingOrder = 12; // Higher sorting order to keep text on top
            }
        }

        lastSelectedObject = obj; // Update the last selected object
        obj.isDragging = true; // Enable dragging for the selected object
    }


    void SnapObjectsToClosestAngle()
    {
        if (lastSelectedObject != null)
        {
            // Calculate the current angle (clockwise orientation with offset)
            float outPolarFloat = Mathf.Atan2(-lastSelectedObject.objectTransform.position.y, lastSelectedObject.objectTransform.position.x) * Mathf.Rad2Deg;
            float currentAngle = (outPolarFloat + 90 + 360) % 360; // Add 90 degrees to offset zero to the top

            Debug.Log($"Current Angle: {currentAngle}");
            float closestAngle = float.NaN;
            float minDifference = float.MaxValue;

            // Find the closest predefined angle
            foreach (float angle in zoneSpawner.degreeAngles)
            {
                float difference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, angle));
                if (difference < minDifference)
                {
                    minDifference = difference;
                    closestAngle = angle;
                }
            }

            // Snap the object to the closest angle
            lastSelectedObject.snappedAngle = closestAngle;

            // Adjust the snapped angle to place it correctly on the circle (clockwise, with 90-degree offset)
            float radiansClosest = (closestAngle - 90) * Mathf.Deg2Rad; // Subtract 90 to place 0 degrees at the top
            float xClosest = Mathf.Cos(radiansClosest) * 3.0f;
            float yClosest = -Mathf.Sin(radiansClosest) * 3.0f; // Negate y to maintain clockwise orientation

            lastSelectedObject.objectTransform.position = new Vector2(xClosest, yClosest);

            int ObjectNumberSnapped = circularObjects.IndexOf(lastSelectedObject) + 1;

            Debug.Log(ObjectNumberSnapped);
            var messagePan = new OSCMessage("/objectPosition");
            messagePan.AddValue(OSCValue.Int(ObjectNumberSnapped));
            messagePan.AddValue(OSCValue.Float(closestAngle)); // Send outPolar as an integer

            //Debug.Log($"Object {ObjectNumber} position: {outPolar} degrees");

            Transmitter.Send(messagePan);

            Debug.Log($"Object snapped to {closestAngle} degrees (adjusted for correct clockwise position)");
        }
    }
}