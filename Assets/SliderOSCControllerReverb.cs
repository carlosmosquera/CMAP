using UnityEngine;
using UnityEngine.UI;
using extOSC;

public class SliderOSCControllerReverb : MonoBehaviour
{
    private string oscAddress = "/ReverbFader";

    [Header("UI Components")]
    public Slider masterFaderSlider;
    public Text valueDisplay;  // Reference to a Text UI component to display the reverb dB value

    public OSCTransmitter oscTransmitter;

    private void Start()
    {
        // Send the initial OSC message with the current slider value
        UpdateReverbValue(masterFaderSlider.value);
        masterFaderSlider.onValueChanged.AddListener(UpdateReverbValue);
    }

    private void UpdateReverbValue(float sliderValue)
    {
        // Map the slider value (0-1) to a dB range (-70 to 0) using a logarithmic scale
        float reverbDB = Mathf.Lerp(-70, 0, Mathf.Log10(1 + 9 * sliderValue) / Mathf.Log10(10));

        // Display the dB value on the canvas
        valueDisplay.text = $"{reverbDB:F0} dB";

        // Send OSC message with the mapped dB value
        SendOSCMessage(reverbDB);
    }

    private void SendOSCMessage(float reverbDB)
    {
        int reverbDBInt = (int)reverbDB;

        var message = new OSCMessage(oscAddress);
        message.AddValue(OSCValue.Float(reverbDBInt));
        oscTransmitter.Send(message);

        // Debug.Log($"Sent OSC Message with Reverb dB: {reverbDBInt} dB");
    }

    private void OnDestroy()
    {
        masterFaderSlider.onValueChanged.RemoveListener(UpdateReverbValue);
    }
}
