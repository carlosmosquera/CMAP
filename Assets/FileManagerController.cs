using UnityEngine;
using UnityEngine.UI;
using TMPro; // Ensure you have this for TextMeshPro
using System.Collections.Generic;
using extOSC;
using System.IO;
using System.Collections;

public class FileManagerController : MonoBehaviour
{
    public Button saveButton;
    public Button loadButton;
    public Button deleteButton; // Button to delete selected file
    public TMP_InputField fileNameInput; // For saving a new file
    public TMP_Dropdown fileDropdown; // Dropdown for loading files
    public OSCTransmitter Transmitter;

    private List<Vector3> savedPositions = new List<Vector3>();
    private Transform[] childTransforms;

    void Start()
    {
        // Initialize child transforms array with all children of the parent
        childTransforms = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            childTransforms[i] = transform.GetChild(i);
        }

        // Assign button click events
        saveButton.onClick.AddListener(SavePositions);
        loadButton.onClick.AddListener(LoadSelectedPosition);
        deleteButton.onClick.AddListener(DeleteSelectedFile); // Add listener for delete button

        // Populate the dropdown with saved files on start
        UpdateFileDropdown();

        // Optional: Set the input field to the first dropdown option if available
        if (fileDropdown.options.Count > 0)
        {
            fileNameInput.text = fileDropdown.options[0].text;
        }

        // Update input field when dropdown selection changes
        fileDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // Update input field when the dropdown value changes
    private void OnDropdownValueChanged(int index)
    {
        fileNameInput.text = fileDropdown.options[index].text;
    }

    // Save the current positions of all child objects with a specified file name
    void SavePositions()
    {
        // Use the current input field text as the filename, regardless of dropdown selection
        string fileName = fileNameInput.text.Trim(); // Trim whitespace
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogWarning("File name is empty!");
            return;
        }

        // Clear previous saved positions and store current positions
        savedPositions.Clear();
        foreach (Transform child in childTransforms)
        {
            savedPositions.Add(child.position);
        }

        SavePositionsToFile(fileName);
        Debug.Log($"Positions saved to {fileName}");

        // Update the dropdown after saving to reflect changes, but keep the current selection
        UpdateFileDropdown(fileName);
    }

    // Load the selected file from the dropdown
    void LoadSelectedPosition()
    {
        int selectedIndex = fileDropdown.value;
        string fileName = fileDropdown.options[selectedIndex].text;

        LoadPositionsFromFile(fileName);
        ApplyLoadedPositions();
    }

    // Delete the selected file from the dropdown
    void DeleteSelectedFile()
    {
        int selectedIndex = fileDropdown.value;
        string fileName = fileDropdown.options[selectedIndex].text;

        string filePath = Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"File {fileName} deleted.");
            UpdateFileDropdown(); // Refresh the dropdown after deletion
        }
        else
        {
            Debug.LogWarning($"File {fileName} does not exist!");
        }
    }

    // Update the dropdown options with available saved files
    private void UpdateFileDropdown(string selectedFileName = null)
    {
        fileDropdown.ClearOptions();
        List<string> fileNames = new List<string>();

        // Load all JSON files from the persistent data path
        string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");
        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            fileNames.Add(fileName);
        }

        fileDropdown.AddOptions(fileNames);

        // Reset input field if there are options available
        if (fileNames.Count > 0)
        {
            // If a specific file name was provided, maintain that selection in the dropdown
            if (!string.IsNullOrEmpty(selectedFileName) && fileNames.Contains(selectedFileName))
            {
                fileNameInput.text = selectedFileName; // Set input field to the selected file
                fileDropdown.value = fileNames.IndexOf(selectedFileName); // Set dropdown to the current file
            }
            else
            {
                fileNameInput.text = fileNames[0]; // Default to the first available option
                fileDropdown.value = 0; // Set dropdown to the first file
            }
        }
        else
        {
            fileNameInput.text = ""; // Clear input field if no files exist
        }
    }

    // Apply the loaded positions to the child objects
    private void ApplyLoadedPositions()
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

    // Save positions to a file with the specified file name
    void SavePositionsToFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        string json = JsonUtility.ToJson(new PositionData { positions = savedPositions });
        File.WriteAllText(filePath, json);
    }

    // Load positions from a file with the specified file name
    void LoadPositionsFromFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PositionData data = JsonUtility.FromJson<PositionData>(json);
            savedPositions = data.positions;
            Debug.Log($"Positions loaded from {fileName}");
        }
        else
        {
            Debug.LogWarning($"File {fileName} does not exist!");
        }
    }

    [System.Serializable]
    private class PositionData
    {
        public List<Vector3> positions;
    }
}
