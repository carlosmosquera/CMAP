using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab;  // The prefab to duplicate
    public TMP_InputField inputField;  // The TMP input field for the user to input the number
    public Button spawnButton;       // The button to trigger the spawn process

    private int currentID = 1;       // Counter to assign unique IDs, starting from 1
    private List<GameObject> spawnedObjects = new List<GameObject>(); // List to store references to spawned objects

    void Start()
    {
        // Add a listener to the button to call the SpawnObjects method when clicked
        spawnButton.onClick.AddListener(SpawnObjects);
    }

    void SpawnObjects()
    {
        // Clear existing spawned objects
        foreach (GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
        spawnedObjects.Clear();

        // Reset the ID counter
        currentID = 1;

        // Parse the input field text to get the number of objects to spawn
        if (int.TryParse(inputField.text, out int numberOfObjects))
        {
            for (int i = 0; i < numberOfObjects; i++)
            {
                // Instantiate the object prefab
                GameObject newObj = Instantiate(objectPrefab, transform);

                // Ensure the new object is active
                newObj.SetActive(true);

                // Position the new object (all at 0, 3 on x and y axes)
                newObj.transform.localPosition = new Vector3(0, 3, 0);

                // Assign a unique ID and display it
                ObjectID objectID = newObj.GetComponent<ObjectID>();
                objectID.id = currentID++;

                objectID.UpdateIDText();

                // Optionally, set the name of the new object to include its ID
                newObj.name = "Object_" + objectID.id;

                // Add the new object to the list of spawned objects
                spawnedObjects.Add(newObj);
            }

            // Optionally, hide the original prefab after creating all the duplicates
             objectPrefab.SetActive(false);
        }
        else
        {
            Debug.LogError("Invalid input! Please enter a valid number.");
        }
    }
}
