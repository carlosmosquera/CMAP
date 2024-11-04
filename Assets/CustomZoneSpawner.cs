using System.Collections.Generic;
using UnityEngine;

public class CustomZoneSpawner : MonoBehaviour
{
    public GameObject objectPrefab; // The prefab of the object to be spawned
    public int numberOfObjects = 5; // Number of objects to be spawned
    private float radius = 3f; // Radius of the circle

    public List<float> degreeAngles; // List of degree angles provided by the user

    private List<GameObject> spawnedObjects; // List to store spawned objects


    void Start()
    {

        spawnedObjects = new List<GameObject>();

        if (degreeAngles.Count != numberOfObjects)
        {
            Debug.LogError("The number of angles provided does not match the number of objects.");
            return;
        }

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = degreeAngles[i] + 90f;
            SpawnObjectAtAngle(angle);
        }
    }

    void SpawnObjectAtAngle(float angle)
    {
        float radians = angle * Mathf.Deg2Rad; // Convert degrees to radians
        float x = Mathf.Cos(radians) * radius; // Calculate x position
        float y = Mathf.Sin(radians) * radius; // Calculate y position

        Vector2 spawnPosition = new Vector2(x, y); // Set the spawn position

        //Instantiate(objectPrefab, spawnPosition, Quaternion.identity); // Instantiate the object

        GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition, Quaternion.identity); // Instantiate the object
        spawnedObjects.Add(spawnedObject);
    }

    //void OnDisable()
    //{
    //    foreach (GameObject obj in spawnedObjects)
    //    {
    //        if (obj != null)
    //        {
    //            obj.SetActive(false); // Hide the object
    //        }
    //    }
    //}

    //void OnEnable()
    //{
    //    foreach (GameObject obj in spawnedObjects)
    //    {
    //        if (obj != null)
    //        {
    //            obj.SetActive(true); // Unhide the object
    //        }
    //    }
    //}
}
