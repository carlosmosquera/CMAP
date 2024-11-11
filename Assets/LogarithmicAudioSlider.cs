using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class LogarithmicAudioSlider : MonoBehaviour
{
    private Slider audioSlider;

    [SerializeField] private float minDb = -70f;
    [SerializeField] private float maxDb = 0f;

    void Awake()
    {
        audioSlider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        audioSlider.onValueChanged.AddListener(UpdateVolume);
    }

    void OnDisable()
    {
        audioSlider.onValueChanged.RemoveListener(UpdateVolume);
    }

    private void UpdateVolume(float sliderValue)
    {
        // Convert slider value to dB
        float dBValue = Mathf.Lerp(minDb, maxDb, Mathf.Pow(sliderValue, 2f));

        // Convert dB to linear scale for use with audio source volume
        float linearValue = Mathf.Pow(10f, dBValue / 20f);

        // Apply the linearValue to your audio settings, e.g.,
        // audioSource.volume = linearValue;
    }
}
