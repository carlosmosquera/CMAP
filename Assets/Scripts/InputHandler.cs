using UnityEngine;
using TMPro;
using extOSC;
public class InputFieldHandler : MonoBehaviour
{
    public TMP_InputField[] inputFields;

    public OSCTransmitter OSCTransmitter;

    // You can use this array to store the parsed numbers
    //private float[] numbers;

    void Start()
    {
        //numbers = new float[inputFields.Length];

        foreach (TMP_InputField inputField in inputFields)
        {
            inputField.onValueChanged.AddListener(delegate { OnInputFieldChanged(inputField); });
        }
    }


    void OnInputFieldChanged(TMP_InputField changedInputField)
    {
        // Get the value from the changed input field
        string newValue = changedInputField.text;

        // Find the index of the changed input field in the array (if needed)
        int index = System.Array.IndexOf(inputFields, changedInputField);

        // Do something with the new value
        Debug.Log($"Input field at index {index + 1} changed to: {newValue}");

        
        var messageChanSel = new OSCMessage("/chSel/");
        messageChanSel.AddValue(OSCValue.Int(index + 1));
        messageChanSel.AddValue(OSCValue.String(newValue));
        OSCTransmitter.Send(messageChanSel);

        // Example: Perform additional logic here
    }

    // Optional: Remove listeners when the script is destroyed
    void OnDestroy()
    {
        foreach (TMP_InputField inputField in inputFields)
        {
            inputField.onValueChanged.RemoveListener(delegate { OnInputFieldChanged(inputField); });
        }
    }
}



    //void OnInputFieldEndEdit(string input)
    //{
    //    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
    //    {
    //        ReadInputFields();
    //    }
    //}

    // Call this method when you want to read the input fields, e.g., on a button click
    //public void ReadInputFields()
    //{
    //    for (int i = 0; i < inputFields.Length; i++)
    //    {
    //        if (float.TryParse(inputFields[i].text, out float result))
    //        {
    //            numbers[i] = result;
    //            Debug.Log($"Speaker {i + 1} value: {result}");
    //        }
    //        else
    //        {
    //            Debug.LogError($"Speaker {i + 1} contains invalid number!");
    //        }
    //    }
    //}