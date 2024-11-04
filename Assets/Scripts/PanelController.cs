using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject infoButton;

    void Start()
    {
        // Ensure the panel is initially inactive
        infoPanel.SetActive(false);
        infoButton.SetActive(false);
    }

    // Call this method to show the panel
    public void ShowPanel()
    {
        infoPanel.SetActive(true);
        infoButton.SetActive(true);
    }

    // Call this method to hide the panel
    public void HidePanel()
    {
        infoPanel.SetActive(false);
        infoButton.SetActive(false);
    }
}
