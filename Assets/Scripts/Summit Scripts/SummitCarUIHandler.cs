using UnityEngine.UI;
using UnityEngine;

public class SummitCarUIHandler : MonoBehaviour
{
    //Public variables
    public static SummitCarUIHandler SummitCarUIHandlerInstance;
    public GameObject CarCanvas;
    public Button ExitButton;

    //private variables 
    [SerializeField]
    private GameObject joyStick;
    [SerializeField]
    private GameObject uiObjects;

   

    private void Awake()
    {
        if (SummitCarUIHandlerInstance == null)
            SummitCarUIHandlerInstance = this;
        else DestroyImmediate(this);
    }

    public void UpdateUIelement(bool enable)
    {
        joyStick.SetActive(enable);
        uiObjects.SetActive(enable);
    }

}
