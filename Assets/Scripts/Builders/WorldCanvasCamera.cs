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

    [Header("Portrait Canvas")]
    public Button AnimationBtnPortrait;
    public Button FavBtnPortrait;
    public GameObject CircullerScrollBtnsPortrait;

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
        }
    }

    void DisableAnimationsButtons(bool state)
    {
        AnimationBtn.interactable = state;
        FavBtn.interactable = state;
        AnimationBtnPortrait.interactable = state;
        FavBtnPortrait.interactable = state;
        CircullerScrollBtns.SetActive(state);
        CircullerScrollBtnsPortrait.SetActive(state);
        if (EmoteAnimationPlay.Instance.isAnimRunning && !state)
        {
            EmoteAnimationPlay.Instance.StopAnimation();
            EmoteAnimationPlay.Instance.StopAllCoroutines();
        }
    }

}
