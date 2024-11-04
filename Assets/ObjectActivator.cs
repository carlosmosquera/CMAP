using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectActivator : MonoBehaviour
{
    public TMP_InputField inputField; // Reference to the TextMeshPro Input Field
    public Button activateButton; // Reference to the Button
    public GameObject parentObject; // Reference to the parent GameObject

    private void Start()
    {
        activateButton.onClick.AddListener(OnActivateButtonClick);
    }

    private void OnActivateButtonClick()
    {
        if (int.TryParse(inputField.text, out int x))
        {
            ActivateChildrenUpTo(x);
        }
        else
        {
            Debug.LogWarning("Invalid input. Please enter a valid number.");
        }
    }

    private void ActivateChildrenUpTo(int count)
    {
        // Get all child objects
        Transform[] children = parentObject.GetComponentsInChildren<Transform>();

        // Deactivate all children first
        foreach (Transform child in children)
        {
            if (child != parentObject.transform) // Skip the parent itself
            {
                child.gameObject.SetActive(false);
            }
        }

        // Activate up to the specified number of children
        for (int i = 1; i <= count; i++)
        {
            if (i < children.Length)
            {
                children[i].gameObject.SetActive(true);
            }
            else
            {
                break; // Exit loop if there are fewer children than specified
            }
        }
    }
}
