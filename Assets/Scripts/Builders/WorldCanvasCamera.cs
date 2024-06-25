using UnityEngine;
using UnityEngine.UI;

public class WorldCanvasCamera : MonoBehaviour
{
    [SerializeField]
    GameObject[] uiCamerasforBuilder;

    [Header("Landscape Canvas")]
    public Button AnimationBtn;
    public Button FavBtn;
    public GameObject CircullerScrollBtns;
    public GameObject AnimationPanel;

    [Header("Portrait Canvas")]
    public Button AnimationBtnPortrait;
    public Button FavBtnPortrait;
    public GameObject CircullerScrollBtnsPortrait;
    public GameObject AnimationPanelPortrait;


    void OnEnable()
    {
        BuilderEventManager.EnableWorldCanvasCamera += EnableCamera;
        BuilderEventManager.DisableAnimationsButtons += DisableAnimationsButtons;

    }
    void OnDisable()
    {
        BuilderEventManager.EnableWorldCanvasCamera -= EnableCamera;
        BuilderEventManager.DisableAnimationsButtons -= DisableAnimationsButtons;

    }

    void EnableCamera()
    {
        foreach (GameObject camObj in uiCamerasforBuilder)
        {
            camObj.SetActive(true);
            camObj.GetComponent<Camera>().farClipPlane = 12f;
        }
    }

    void DisableAnimationsButtons(bool state)
    {
        AnimationBtn.interactable = state;
        FavBtn.interactable = state;
        AnimationBtnPortrait.interactable = state;
        FavBtnPortrait.interactable = state;

        //Close circullerscroll btns
        if(!state && (CircullerScrollBtns.activeInHierarchy || CircullerScrollBtnsPortrait.activeInHierarchy))
        {
            FavBtn.onClick.Invoke();
            FavBtnPortrait.onClick.Invoke();
        }

        if (!state && (AnimationPanel.activeInHierarchy || AnimationPanelPortrait.activeInHierarchy))
        {
            GamePlayButtonEvents.inst.CloseEmoteSelectionPanel();
        }
        if (ActionManager.IsAnimRunning && !state)
        {
            //// EmoteAnimationHandler.Instance.StopAnimation();
            // EmoteAnimationHandler.Instance.StopAllCoroutines();
            ActionManager.StopActionAnimation?.Invoke();
        }
    }

}
