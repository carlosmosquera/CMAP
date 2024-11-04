using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class CircleSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // The object to spawn
    private int numberOfObjects; // Number of objects to spawn
    private float radius = 3; // Radius of the circle

    private List<GameObject> spawnedObjects = new List<GameObject>(); // List to keep track of spawned objects

    public TMP_InputField inputField; // Reference to the TMP_InputField


    void Start()
    {
        objectToSpawn.SetActive(true);



        if (inputField == null)
        {
            inputField = GetComponent<TMP_InputField>();
        }

        // Add listener to handle input change
        inputField.onValueChanged.AddListener(HandleInputChange);




    }


    void HandleInputChange(string input)
    {
        if (int.TryParse(input, out int number))
        {
            numberOfObjects = number;

            Debug.Log("Input number: " + numberOfObjects);


            UpdateNumberOfObjects(numberOfObjects);


            // Do something with the number

        }
        else
        {
            Debug.LogWarning("Invalid input! Please enter a valid number.");
        }
    }

    void SpawnObjectsInCircle(int count)
    {
      

        float angleStep = 360f / numberOfObjects;

        for (int i = 0; i < numberOfObjects; i++)
        {
            // Calculate the angle for this object
            //float angle = i * Mathf.PI * 2 / numberOfObjects;

            float angle = i * angleStep;
            float angleRad = angle * Mathf.Deg2Rad;

            // Determine the position for this object
            float x = Mathf.Cos(angleRad) * radius;
            float y = Mathf.Sin(angleRad) * radius;

            // Create the object
            Vector3 position = new Vector3(x, y, 0f);
            GameObject spawnedObject = Instantiate(objectToSpawn, position, Quaternion.identity, transform);

            // Add the spawned object to the list
            spawnedObjects.Add(spawnedObject);

                Debug.Log($"Image {i + 1} Angle: {angle} degrees");

        }
    }


    public void UpdateNumberOfObjects(int newNumber)
    {
        // Remove previously spawned objects
        RemovePreviousObjects();

        // Update the number of objects
        numberOfObjects = newNumber;

        // Spawn new objects
        SpawnObjectsInCircle(numberOfObjects);
    }

    void RemovePreviousObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
        spawnedObjects.Clear();
    }
}
