using UnityEngine;
using UnityEngine.UI;
using extOSC;

public class SliderOSCController : MonoBehaviour
{
    private string oscAddress = "/ReverbFader";
    // Set to your desired port

    [Header("UI Components")]
    public Slider masterFaderSlider;

    public OSCTransmitter oscTransmitter;

    private void Start()
    {
        // Send the initial OSC message with the current slider value
        SendOSCMessage(masterFaderSlider.value);

        // Add listener to slider to send OSC message when value changes
        masterFaderSlider.onValueChanged.AddListener(SendOSCMessage);
    }

    private void SendOSCMessage(float value)
    {
        // Create an OSC message with the specified address
        var message = new OSCMessage(oscAddress);

        // Clamp value to 0.0 - 1.0 range
        value = Mathf.Clamp(value, 0.0f, 1.0f);

        // Add the slider value to the OSC message
        message.AddValue(OSCValue.Float(value));

        // Send the message
        oscTransmitter.Send(message);

        Debug.Log(message);
    }

    private void OnDestroy()
    {
        // Remove listener to avoid errors when the object is destroyed
        masterFaderSlider.onValueChanged.RemoveListener(SendOSCMessage);
    }
}
