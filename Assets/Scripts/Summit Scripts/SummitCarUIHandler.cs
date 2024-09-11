using UnityEngine.UI;
using UnityEngine;

public class SummitCarUIHandler : MonoBehaviour
{
    //Public variables
    public static SummitCarUIHandler SummitCarUIHandlerInstance;
    public GameObject CarCanvas;
    public Button ExitButtonLandscape;
    public Button ExitButtonPotrait;

    public Button ExitButton
    {
        get
        {
            if (!ScreenOrientationManager._instance.isPotrait)
                return ExitButtonLandscape;
            else
                return ExitButtonPotrait;
        }
    }

    [SerializeField]
    private GameObject[] objectsToEnableDisable;
    [SerializeField]
    private RectTransform inviteBtnPortait;
    [SerializeField]
    private RectTransform peopleRect;

    private void Awake()
    {
        if (SummitCarUIHandlerInstance == null)
            SummitCarUIHandlerInstance = this;
        else DestroyImmediate(this);
    }

    public void UpdateUIelement(bool enable)
    {
        objectsToEnableDisable.SetActive(enable);
        if (!enable)
        {
            inviteBtnPortait.anchoredPosition = new Vector2(50f, -63.30005f);
            peopleRect.anchoredPosition = new Vector2(-78.3f, -3.399902f);
        }
        else
        {
            inviteBtnPortait.anchoredPosition = new Vector2(-31.5f, -63.30005f);
            peopleRect.anchoredPosition = new Vector2(2.699951f, -3.399902f);
        }
    }

}
