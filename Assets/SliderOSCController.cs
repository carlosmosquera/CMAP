using UnityEngine;
using UnityEngine.UI;
using extOSC;

public class SliderOSCController : MonoBehaviour
{
    private string oscAddress = "/MasterFader";

    [Header("UI Components")]
    public Slider masterFaderSlider;
    public Text valueDisplay;  // Reference to a Text UI component to display the dB value

    public OSCTransmitter oscTransmitter;

    private void Start()
    {
        // Send the initial OSC message with the current slider value
        UpdateFaderValue(masterFaderSlider.value);
        masterFaderSlider.onValueChanged.AddListener(UpdateFaderValue);
    }

    private void UpdateFaderValue(float sliderValue)
    {
        // Map the slider value (0-1) to the dB range (-70 to 0)
        float dBValue = Mathf.Lerp(-70, 0, Mathf.Pow(sliderValue, 2));

        // Display the dB value on the canvas
        valueDisplay.text = $"{dBValue:F1} dB";

        // Send OSC message with the dB value
        SendOSCMessage(dBValue);
    }

    private void SendOSCMessage(float dBValue)
    {
        var message = new OSCMessage(oscAddress);
        message.AddValue(OSCValue.Float(dBValue));
        oscTransmitter.Send(message);

        Debug.Log($"Sent OSC Message with dB Value: {dBValue}");
    }

    private void OnDestroy()
    {
        masterFaderSlider.onValueChanged.RemoveListener(UpdateFaderValue);
    }
}
