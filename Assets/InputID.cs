using UnityEngine;
using TMPro;

public class InputID : MonoBehaviour
{
    public int id;                   // Unique ID for the object
    public TMP_Text idText;          // Reference to the TextMeshPro text component

    void Start()
    {
        // Ensure the text component is assigned
        if (idText == null)
        {
            idText = GetComponentInChildren<TMP_Text>();
        }
    }

    public void UpdateIDText()
    {
        // Update the text to display the ID
        if (idText != null)
        {
            idText.text = id.ToString();
        }
        else
        {
            Debug.LogError("ID Text component is not assigned.");
        }
    }
}
