using UnityEngine;
using TMPro;

public class ObjectID : MonoBehaviour
{
    public int id;                    // Unique ID for the object
    public TMP_Text idText;           // Reference to the TextMeshPro text component
    public Canvas canvas;             // Reference to the Canvas component

    void Start()
    {
        // Ensure the text component and canvas are assigned
        if (idText == null)
        {
            idText = GetComponentInChildren<TMP_Text>();
        }

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
    }

    public void UpdateIDText()
    {
        // Update the text to display the ID
        if (idText != null)
        {
            idText.text = id.ToString();
            idText.color = Color.red;  // Set the text color to black
            idText.alignment = TextAlignmentOptions.Center;  // Center the text

            // Ensure the Canvas is assigned before setting sortingOrder
            if (canvas != null)
            {
                canvas.sortingOrder = 9;
            }
            else
            {
                Debug.LogError("Canvas component is not assigned.");
            }
        }
        else
        {
            Debug.LogError("ID Text component is not assigned.");
        }
    }
}
