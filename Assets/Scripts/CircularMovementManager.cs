using UnityEngine;
using System.Collections.Generic;
using TMPro;
using extOSC;

public class CircularMovementManager : MonoBehaviour
{
    public OSCTransmitter Transmitter;




    [System.Serializable]
    public class CircularObject
    {
        public Transform objectTransform;
        public Transform centralPoint;
        public float radius = 5.0f;
      

        [HideInInspector]
        public bool isDragging = false;
        [HideInInspector]
        public float angle = 0.0f;

        public GameObject Zone1;
        public GameObject Zone2;
        public GameObject Zone3;
        public GameObject CustomZone;



    }

    public List<CircularObject> circularObjects = new List<CircularObject>();

    void Start()
    {
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
            textMesh.fontSize = 5;
            textMesh.color = Color.black;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.sortingOrder = 9;

            // Adjust the position if necessary
            numberObject.transform.localPosition = new Vector3(0, 0, -1);
        }
    }


    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            // Check if the mouse is over any of the objects
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                foreach (var obj in circularObjects)
                {
                    if (hit.collider.transform == obj.objectTransform)
                    {
                        obj.isDragging = true;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            foreach (var obj in circularObjects)
            {
                obj.isDragging = false;
            }
        }

        foreach (var obj in circularObjects)
        {
            if (obj.isDragging)
            {

                if (obj.CustomZone.activeSelf)
                {
                    // ------ TO MOVE WITHIN A CIRCLE --------

                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 offset = mousePosition - (Vector2)obj.centralPoint.position;

                    if (offset.magnitude > obj.radius)
                    {
                        offset = offset.normalized * obj.radius;
                    }

                    obj.objectTransform.position = (Vector2) obj.centralPoint.position + offset;

                    float cartCoordsX = obj.objectTransform.position.x;
                    float cartCoordsY = obj.objectTransform.position.y;
                    float cartCoordsZ = obj.objectTransform.position.z;

                    float outPolar;  outPolar = Mathf.Atan2(cartCoordsY, cartCoordsX);
                    outPolar = (-outPolar * Mathf.Rad2Deg + 450) % 360;


                    float distance = Mathf.Sqrt((cartCoordsX * cartCoordsX) + (cartCoordsY * cartCoordsY) + (cartCoordsZ * cartCoordsZ));

                    float maxRadius = obj.radius;  // Replace with the appropriate maximum radius if different
                    float outRadiusFloat = Mathf.Clamp01(distance / maxRadius) * 90f;
                    int outRadius = Mathf.RoundToInt(90f - outRadiusFloat);

                    int ObjectNumber = circularObjects.IndexOf(obj) + 1;

                    string _panAddress = "" + ObjectNumber;

                    var messagePan = new OSCMessage(_panAddress);
                    messagePan.AddValue(OSCValue.Float(outPolar));
                    messagePan.AddValue(OSCValue.Float(outRadius));

                    Debug.Log($"Object {ObjectNumber} position: {outPolar} elevation: {outRadius}");

                    Transmitter.Send(messagePan);

                }
                //------- TO MOVE AROUND A CIRCLE ----

                else
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 direction = mousePosition - (Vector2)obj.centralPoint.position;

                    // Normalize direction to keep the object on the circumference
                    direction.Normalize();

                    // Set the object's position to the edge of the circle
                    obj.objectTransform.position = (Vector2)obj.centralPoint.position + direction * obj.radius;

                    float cartCoordsX = obj.objectTransform.position.x;
                    float cartCoordsY = obj.objectTransform.position.y;
                    float cartCoordsZ = obj.objectTransform.position.z;

                    float outRadius;
                    float outPolar;

                    outPolar = Mathf.Atan2(cartCoordsY, cartCoordsX);
                    outPolar = (-outPolar * Mathf.Rad2Deg + 450) % 360;

                    outRadius = Mathf.Sqrt((cartCoordsX * cartCoordsX)
                                    + (cartCoordsY * cartCoordsY));
                                    //+ (cartCoordsZ * cartCoordsZ));

                    int ObjectNumber = circularObjects.IndexOf(obj) + 1;

                    string _panAddress = "" + ObjectNumber;

                    var messagePan = new OSCMessage(_panAddress);
                    messagePan.AddValue(OSCValue.Float(outPolar));

                    Debug.Log($"Object {ObjectNumber} position: {outRadius}");

                    Transmitter.Send(messagePan);
                }

            }

          
        }

       
    }
}
