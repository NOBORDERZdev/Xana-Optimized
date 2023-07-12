using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingScreenScrolllerActivity : MonoBehaviour
{
    public ScrollRect ScrollController;
    public float normalized;
    public OnEnableDisable enableDisable;
    public Button CloseButton;
    public bool CanClose;

    private void Awake()
    {
        enableDisable = GetComponent<OnEnableDisable>();
        ScrollController.verticalNormalizedPosition = 0f;
    }
    void Start()
    {

    }
    private void OnDisable()
    {
        DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 0f, 0.2f);
    }

    private void OnEnable()
    {
        CancelInvoke(nameof(EnableCloseButton));
        Invoke(nameof(EnableCloseButton), .4f);
    }
    void Update()
    {
        normalized = ScrollController.verticalNormalizedPosition;
        OnSettingScreenScroll();
    }

    public void OnSettingScreenScroll()
    {
        if (ScrollController.verticalNormalizedPosition > 0f)
        {
            if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)))
            {
                ClosePopUp();
            }
        }
    }
    Coroutine IEClosePopUpCoroutine;
    public void ClosePopUp()
    {
        Debug.Log("ClosePopup");
        if (CanClose)
        {
            IEClosePopUpCoroutine = StartCoroutine(IEClosePopUp());
        }
    }
    public void OnClickBlackBG()
    {
        if (ScrollController.verticalNormalizedPosition <= 0)
        {
            ClosePopUp();
        }
    }
    public IEnumerator IEClosePopUp()
    {
        DisableCloseButton();
        Debug.LogError("IEClosePopUp Start");
        DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 1f, 0.2f);
        yield return new WaitForSeconds(.2f);
        enableDisable.ClosePopUp();
    }
    public void EnableCloseButton()
    {
        CanClose = CloseButton.interactable = true;
    }
    public void DisableCloseButton()
    {
        CanClose = CloseButton.interactable = false;
    }
}
