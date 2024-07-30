using UnityEngine.UI;
using UnityEngine;

public class SummitCarUIHandler : MonoBehaviour
{
    public static SummitCarUIHandler instance;

    [SerializeField]
    private GameObject JoyStick, UIObjects;
    public GameObject carCanvas;
    public Button exitButton;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else DestroyImmediate(this);
    }

    public void UpdateUIelement(bool enable)
    {
        JoyStick.SetActive(enable);
        UIObjects.SetActive(enable);
    }

}
