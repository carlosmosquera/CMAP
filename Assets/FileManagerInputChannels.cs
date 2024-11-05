using UnityEngine;
using UnityEngine.UI;
using TMPro; // Ensure you have this for TextMeshPro
using System.Collections.Generic;
using extOSC;
using System.IO;
using System.Collections;

public class FileManagerInputChannels : MonoBehaviour
{
    public GameObject content; // Drag and drop the Content parent object here
    public Button saveButton;
    public Button loadButton;
    public Button deleteButton; // Button to delete selected file
    public TMP_InputField fileNameInput; // For saving a new file
    public TMP_Dropdown fileDropdown; // Dropdown for loading files
    public OSCTransmitter Transmitter;

    private List<Vector3> savedPositions = new List<Vector3>();
    private List<string> savedTexts = new List<string>(); // To store the text from InputFields
    private Transform[] childTransforms;

    void Start()
    {
        // Initialize child transforms array with all children of the Content parent
        childTransforms = new Transform[content.transform.childCount];
        for (int i = 0; i < content.transform.childCount; i++)
        {
            childTransforms[i] = content.transform.GetChild(i);
        }

        // Assign button click events
        saveButton.onClick.AddListener(SaveData);
        loadButton.onClick.AddListener(LoadSelectedData);
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

    // Save the current positions and texts of all child objects with a specified file name
    void SaveData()
    {
        // Use the current input field text as the filename, regardless of dropdown selection
        string fileName = fileNameInput.text.Trim(); // Trim whitespace
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogWarning("File name is empty!");
            return;
        }

        // Clear previous saved positions and texts
        savedPositions.Clear();
        savedTexts.Clear();

        foreach (Transform child in childTransforms)
        {
            savedPositions.Add(child.position);
            TMP_InputField inputField = child.GetComponentInChildren<TMP_InputField>();
            if (inputField != null)
            {
                savedTexts.Add(inputField.text);
            }
            else
            {
                savedTexts.Add(""); // In case there's no TMP_InputField, store an empty string
            }
        }

        SaveDataToFile(fileName);
        Debug.Log($"Data saved to {fileName}");

        // Update the dropdown after saving to reflect changes, but keep the current selection
        UpdateFileDropdown(fileName);
    }

    // Load the selected file from the dropdown
    void LoadSelectedData()
    {
        int selectedIndex = fileDropdown.value;
        string fileName = fileDropdown.options[selectedIndex].text;

        LoadDataFromFile(fileName);
        ApplyLoadedData();
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

    // Apply the loaded positions and texts to the child objects
    private void ApplyLoadedData()
    {
        if (savedPositions.Count != childTransforms.Length || savedTexts.Count != childTransforms.Length)
        {
            Debug.LogWarning("No saved data or number of children has changed");
            return;
        }

        for (int i = 0; i < childTransforms.Length; i++)
        {
            childTransforms[i].position = savedPositions[i];
            TMP_InputField inputField = childTransforms[i].GetComponentInChildren<TMP_InputField>();
            if (inputField != null)
            {
                inputField.text = savedTexts[i];
            }
        }

        // Send OSC messages for each object's position
        StartCoroutine(SendPositionsViaOSC());
        Debug.Log("Data loaded and sent via OSC");
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

    // Save positions and texts to a file with the specified file name
    void SaveDataToFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        Data data = new Data { positions = savedPositions.ToArray(), texts = savedTexts.ToArray() };
        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, jsonData);
    }

    // Load data from a specified file
    void LoadDataFromFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            Data data = JsonUtility.FromJson<Data>(jsonData);
            savedPositions = new List<Vector3>(data.positions);
            savedTexts = new List<string>(data.texts);
            Debug.Log($"Data loaded from {fileName}");
        }
        else
        {
            Debug.LogWarning($"File {fileName} not found!");
        }
    }

    // Data structure to hold positions and texts for saving/loading
    [System.Serializable]
    public class Data
    {
        public Vector3[] positions;
        public string[] texts;
    }
}
