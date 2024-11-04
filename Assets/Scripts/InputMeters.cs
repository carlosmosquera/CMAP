using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using extOSC;

public class InputMeters : MonoBehaviour
{
    public OSCReceiver OSCReceiver;
    public OSCTransmitter OSCTransmitter;
    public int NumberOfChannels = 8; // User-defined number of channels, editable in the Inspector
    public Button soloClearButton; // Reference to the Solo Clear button

    private List<string> channelLevelIn = new List<string>();
    private List<Slider> channelSliders = new List<Slider>();
    private List<Button> soloButtons = new List<Button>(); // List to hold the Solo buttons
    private List<bool> soloStates = new List<bool>(); // To track the state of each Solo button (on/off)

    // Start is called before the first frame update
    void Start()
    {
        InitializeChannelsAndSliders();
        if (soloClearButton != null)
        {
            soloClearButton.onClick.AddListener(ClearSolo); // Add listener to the Solo Clear button
        }
        else
        {
            Debug.LogWarning("Solo Clear button is not assigned in the inspector.");
        }
    }

    private void InitializeChannelsAndSliders()
    {
        for (int i = 1; i <= NumberOfChannels; i++)
        {
            // Set channel string paths
            channelLevelIn.Add($"/channelIn/{i}");

            // Find corresponding sliders
            Slider channelSlider = GameObject.Find($"InCh{i}").GetComponent<Slider>();
            if (channelSlider != null)
            {
                channelSliders.Add(channelSlider);
            }
            else
            {
                Debug.LogWarning($"Slider for InCh{i} not found. Make sure the GameObject is named correctly.");
            }

            // Find corresponding Solo buttons
            Button soloButton = GameObject.Find($"InCh{i}/Solo").GetComponent<Button>();
            if (soloButton != null)
            {
                soloButtons.Add(soloButton);
                soloStates.Add(false); // Default to "off" state
                int index = i - 1; // Closure issue prevention
                soloButton.onClick.AddListener(() => ToggleSolo(index)); // Add the listener for the Solo button
            }
            else
            {
                Debug.LogWarning($"Solo button for InCh{i} not found. Make sure the child 'Solo' exists.");
            }

            // Bind OSC messages to corresponding handlers
            int channelIndex = i - 1;
            OSCReceiver.Bind(channelLevelIn[channelIndex], message => ReceivedInput(channelIndex, message));
        }
    }

    // Generalized input handler
    private void ReceivedInput(int index, OSCMessage message)
    {
        if (message.ToFloat(out var value))
        {
            if (index >= 0 && index < channelSliders.Count)
            {
                channelSliders[index].value = value;
            }
        }
    }

    // Toggle the solo button state
    private void ToggleSolo(int index)
    {
        if (index >= 0 && index < soloButtons.Count)
        {
            // If the button is being turned on
            if (!soloStates[index])
            {
                // Turn off all other buttons
                for (int i = 0; i < soloStates.Count; i++)
                {
                    if (i != index && soloStates[i]) // Check if other buttons are on
                    {
                        soloStates[i] = false; // Turn off other buttons
                        Image otherButtonImage = soloButtons[i].GetComponent<Image>();
                        otherButtonImage.color = Color.white; // Change the color to white
                        SendSoloOSC(i + 1, 0); // Send OSC message to turn off
                    }
                }
            }

            // Toggle the state for the current button
            soloStates[index] = !soloStates[index];

            // Get the Image component of the button to change color
            Image buttonImage = soloButtons[index].GetComponent<Image>();

            if (soloStates[index])
            {
                // Set button color to green when toggled on
                buttonImage.color = Color.green;
                SendSoloAllOSC(0);
                SendSoloOSC(index + 1, 1); // Send OSC message with "1" for solo ON
            }
            else
            {
                // Set button color to white when toggled off
                buttonImage.color = Color.white;
                SendSoloOSC(index + 1, 0); // Send OSC message with "0" for solo OFF
            }

            // Check if all buttons are off
            if (AllSoloButtonsOff())
            {
                SendSoloAllOSC(1); // Send "/soloAll" with argument "1" if all are off
            }
        }
    }

    // Check if any solo button is ON
    private bool AnySoloButtonOn()
    {
        foreach (var state in soloStates)
        {
            if (state) // If any button is ON
            {
                return true; // At least one button is ON
            }
        }
        return false; // No buttons are ON
    }

    // Check if all solo buttons are off
    private bool AllSoloButtonsOff()
    {
        foreach (var state in soloStates)
        {
            if (state) // If any button is ON
            {
                return false; // At least one button is ON
            }
        }
        return true; // All buttons are OFF
    }

    // Clear all Solo buttons
    private void ClearSolo()
    {
        for (int i = 0; i < soloButtons.Count; i++)
        {
            // Set each solo button's state to OFF
            soloStates[i] = false;

            // Get the Image component of the button to change color
            Image buttonImage = soloButtons[i].GetComponent<Image>();
            buttonImage.color = Color.white; // Set to white for OFF state

            // Send OSC message for each channel to indicate it is OFF
            SendSoloOSC(i + 1, 0); // Send OSC message with "0" for solo OFF
        }

        // Send "/soloAll" with argument "1" after all buttons are turned off
        SendSoloAllOSC(1);
    }

    // Send OSC message for solo
    private void SendSoloOSC(int channelNumber, int state)
    {
        var message = new OSCMessage(state == 1 ? "/soloOn" : "/soloOff");
        message.AddValue(OSCValue.Int(channelNumber)); // Add the channel number
        OSCTransmitter.Send(message); // Send the OSC message
    }

    // Send OSC message for solo all
    private void SendSoloAllOSC(int state)
    {
        var message = new OSCMessage("/soloAll");
        message.AddValue(OSCValue.Int(state)); // Add the state (0 or 1)
        OSCTransmitter.Send(message); // Send the OSC message
    }
}
