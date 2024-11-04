using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using extOSC;
using System.IO;
using System.Collections;

public class SnapshotController : MonoBehaviour
{
    public Button saveButton;
    public Button loadButton;
    public OSCTransmitter Transmitter;

    private List<Vector3> savedPositions = new List<Vector3>();
    private Transform[] childTransforms;

    private string saveFilePath;

    void Start()
    {
        // Initialize child transforms array with all children of the parent
        childTransforms = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            childTransforms[i] = transform.GetChild(i);
        }

        // Set the file path for saving positions
        saveFilePath = Path.Combine(Application.persistentDataPath, "savedPositions.json");

        // Assign button click events
        saveButton.onClick.AddListener(SavePositions);
        loadButton.onClick.AddListener(LoadPositions);

        // Start coroutine to load positions after a delay
        StartCoroutine(LoadPositionsWithDelay(0.5f));
    }

    // Coroutine to load positions with a delay
    private IEnumerator LoadPositionsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadPositionsFromFile();
        LoadPositions();
    }

    // Save the current positions of all child objects
    void SavePositions()
    {
        savedPositions.Clear();
        foreach (Transform child in childTransforms)
        {
            savedPositions.Add(child.position);
        }
        SavePositionsToFile();
        Debug.Log("Positions saved");
    }

    // Load the saved positions and apply them to the child objects
    void LoadPositions()
    {
        if (savedPositions.Count != childTransforms.Length)
        {
            Debug.LogWarning("No saved positions or number of children has changed");
            return;
        }

        for (int i = 0; i < childTransforms.Length; i++)
        {
            childTransforms[i].position = savedPositions[i];
        }

        // Send OSC messages for each object's position
        StartCoroutine(SendPositionsViaOSC());
        Debug.Log("Positions loaded and sent via OSC");
    }

    private IEnumerator SendPositionsViaOSC()
    {
        for (int i = 0; i < savedPositions.Count; i++)
        {
            // Calculate the polar float and send OSC
            float outPolarFloat = Mathf.Atan2(savedPositions[i].y, savedPositions[i].x) * Mathf.Rad2Deg;
            int outPolar = Mathf.RoundToInt((450 - outPolarFloat) % 360);
            int objectNumber = i + 1;

            var message = new OSCMessage("/objectPosition");
            message.AddValue(OSCValue.Int(objectNumber));
            message.AddValue(OSCValue.Int(outPolar));

            Transmitter.Send(message);
            Debug.Log($"Object {objectNumber} position: {outPolar} degrees");

            // Optionally add a small delay between sends if needed
            yield return new WaitForSeconds(0.1f); // Adjust this delay as needed
        }
    }

    // Save positions to a file
    void SavePositionsToFile()
    {
        string json = JsonUtility.ToJson(new PositionData { positions = savedPositions });
        File.WriteAllText(saveFilePath, json);
    }

    // Load positions from a file
    void LoadPositionsFromFile()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            PositionData data = JsonUtility.FromJson<PositionData>(json);
            savedPositions = data.positions;
            Debug.Log("Positions loaded from file");
        }
    }

    [System.Serializable]
    private class PositionData
    {
        public List<Vector3> positions;
    }
}
