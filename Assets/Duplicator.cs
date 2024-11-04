using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class ObjectDuplicator : MonoBehaviour
{
    public GameObject objectPrefab; // Assign the prefab in the Inspector
    public TMP_InputField inputField; // Assign the input field in the Inspector
    public Button createButton; // Assign the button in the Inspector
    public CircularMovementManager movementManager; // Reference to CircularMovementManager

    private void Start()
    {
        // Add listener to button
        createButton.onClick.AddListener(CreateDuplicates);
    }

    private void CreateDuplicates()
    {
        // Get the number of duplicates from the input field
        if (int.TryParse(inputField.text, out int numberOfDuplicates))
        {
            // Deactivate the original prefab
            objectPrefab.SetActive(false);

            // Clear previous objects in CircularMovementManager
            movementManager.circularObjects.Clear();

            for (int i = 0; i < numberOfDuplicates; i++)
            {
                // Instantiate a new object from the prefab
                GameObject newObject = Instantiate(objectPrefab, new Vector2(0, 3), Quaternion.identity);

                // Activate the newly created object
                newObject.SetActive(true);

                // Get the TextMeshPro component and set the ID
                TextMeshPro textMeshPro = newObject.GetComponentInChildren<TextMeshPro>();
                textMeshPro.text = (i + 1).ToString();

                // Create a new CircularObject and set its properties
                CircularMovementManager.CircularObject circularObject = new CircularMovementManager.CircularObject
                {
                    objectTransform = newObject.transform,
                    centralPoint = newObject.transform, // Set centralPoint appropriately if needed
                    radius = 5.0f // Set radius or other properties as needed
                };

                // Add the new CircularObject to the CircularMovementManager
                movementManager.circularObjects.Add(circularObject);
            }
        }
        else
        {
            Debug.LogError("Invalid number entered in the input field.");
        }
    }
}
