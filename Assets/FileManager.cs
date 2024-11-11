using UnityEngine;
using UnityEngine.UI;
using TMPro; // Ensure you have this for TextMeshPro
using extOSC;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class FileManager : MonoBehaviour
{
    public GameObject content; // Drag and drop the Content parent object here
    public Button saveButton;
    public Button loadButton;
    public Button deleteButton; // Button to delete selected file
    public InputField fileNameInput; // For saving a new file
    public Dropdown fileDropdown; // Dropdown for loading files
    public OSCTransmitter Transmitter;

    private List<Vector3> savedPositions = new List<Vector3>(); // To store the positions of the child objects
    private List<string> savedTexts = new List<string>(); // To store the text from InputFields
    private Transform[] objTransforms;
    private Transform[] textTransforms;

    void Start()
    {
        // Initialize child transforms array for positions
        objTransforms = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            objTransforms[i] = transform.GetChild(i);
        }

        // Initialize child transforms array for text fields
        textTransforms = new Transform[content.transform.childCount];
        for (int i = 0; i < content.transform.childCount; i++)
        {
            textTransforms[i] = content.transform.GetChild(i);
        }

        // Assign button click events
        saveButton.onClick.AddListener(SaveData);
        loadButton.onClick.AddListener(LoadSelectedData);
        deleteButton.onClick.AddListener(DeleteSelectedFile);

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

    // Save both the positions and the texts of all child objects with a specified file name
    void SaveData()
    {
        string fileName = fileNameInput.text.Trim();
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogWarning("File name is empty!");
            return;
        }

        // Clear previous saved data
        savedPositions.Clear();
        savedTexts.Clear();

        // Save positions
        foreach (Transform child in objTransforms)
        {
            savedPositions.Add(child.position);
        }

        // Save texts
        foreach (Transform child in textTransforms)
        {
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
            if (!string.IsNullOrEmpty(selectedFileName) && fileNames.Contains(selectedFileName))
            {
                fileNameInput.text = selectedFileName;
                fileDropdown.value = fileNames.IndexOf(selectedFileName);
            }
            else
            {
                fileNameInput.text = fileNames[0];
                fileDropdown.value = 0;
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
        if (savedPositions.Count != objTransforms.Length || savedTexts.Count != textTransforms.Length)
        {
            Debug.LogWarning("No saved data or number of children has changed");
            return;
        }

        // Apply positions
        for (int i = 0; i < objTransforms.Length; i++)
        {
            objTransforms[i].position = savedPositions[i];
        }

        // Apply texts
        for (int i = 0; i < textTransforms.Length; i++)
        {
            TMP_InputField inputField = textTransforms[i].GetComponentInChildren<TMP_InputField>();
            if (inputField != null)
            {
                inputField.text = savedTexts[i];
            }
        }
        StartCoroutine(SendPositionsViaOSC());

        Debug.Log("Data loaded and applied");
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
            //Debug.Log($"Object {objectNumber} position: {outPolar} degrees");

            // Optionally add a small delay between sends if needed
            yield return new WaitForSeconds(0.1f); // Adjust this delay as needed
        }
    }

    // Save both positions and texts to a single file with the specified file name
    void SaveDataToFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        Data data = new Data
        {
            positions = savedPositions.ToArray(),
            texts = savedTexts.ToArray()
        };
        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, jsonData);
    }

    // Load both positions and texts from a specified file
    void LoadDataFromFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            Data data = JsonUtility.FromJson<Data>(jsonData);

            // Check if the texts array is null, and handle it
            if (data.texts != null)
            {
                savedTexts = new List<string>(data.texts);
            }
            else
            {
                savedTexts = new List<string>(); // Initialize as an empty list if it's null
            }

            // Check if the positions array is null, and handle it
            if (data.positions != null)
            {
                savedPositions = new List<Vector3>(data.positions);
            }
            else
            {
                savedPositions = new List<Vector3>(); // Initialize as an empty list if it's null
            }

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
