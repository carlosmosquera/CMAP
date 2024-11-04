using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSpawnerV2 : MonoBehaviour
{
    public GameObject imagePrefab; // Prefab of the image to be instantiated
    public float radius = 5f; // Radius of the circle
    public int numberOfImages = 10; // Number of images to instantiate (can be set via inspector or script)

    // Start is called before the first frame update
    void Start()
    {

        imagePrefab.SetActive(true);

        SpawnImagesInCircle(numberOfImages);
    }

    void SpawnImagesInCircle(int count)
    {
        float angleStep = 360f / count;
        for (int i = 0; i < count; i++)
        {
            float angle = i * angleStep;
            float angleRad = angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(angleRad) * radius;
            float y = Mathf.Sin(angleRad) * radius;

            Vector3 position = new Vector3(x, y, 0f);

            GameObject newImage = Instantiate(imagePrefab, position, Quaternion.identity, transform);

            // Optional: Rotate the image to face outward
            newImage.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Debug.Log($"Image {i + 1} Angle: {angle} degrees");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Example of changing the number of images dynamically
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            numberOfImages++;
            DestroyAllImages();
            SpawnImagesInCircle(numberOfImages);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            numberOfImages = Mathf.Max(1, numberOfImages - 1);
            DestroyAllImages();
            SpawnImagesInCircle(numberOfImages);
        }
    }

    void DestroyAllImages()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
