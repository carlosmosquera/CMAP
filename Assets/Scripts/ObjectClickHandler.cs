using UnityEngine;

public class ObjectClickHandler : MonoBehaviour
{
    //private PanelController panelController;

    public GameObject SpkPanel;

    //public InputFieldHandler InputFieldHandler;

    void Start()
    {

    }

    void OnMouseDown()
    {

        SpkPanel.SetActive(!SpkPanel.activeSelf);

        //InputFieldHandler.ReadInputFields();

    }


}
