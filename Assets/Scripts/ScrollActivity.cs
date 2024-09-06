using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DigitalRubyShared;
using System.Linq;

public class ScrollActivity : MonoBehaviour
{
    [Header("For World Icons Scroll")]
    public ScrollRect ScrollController, worlddetailScrollContrl;
    public Transform m_parent;
    public GameObject btnback;
    public float parentNormVal, ChildNormVal;
    [SerializeField]
    CanvasGroup canvasGroup;
    public WorldDescriptionPopupPreview worldDetailParentRef;
    public RectTransform fakeBGRectTransform;
    public RectTransform contentRectTransform;

    private SwipeGestureRecognizer swipe1 = new SwipeGestureRecognizer();
    private SwipeGestureRecognizer swipe2 = new SwipeGestureRecognizer();
    public SwipeGestureRecognizerDirection lastSwipeMovement;
    public float lastSwipeYDistance, lastSwipeSpeed;
    public bool firstTimePlay = true;

    private void Awake()
    {
        ScrollController.verticalNormalizedPosition = 3.5f;
        firstTimePlay = true;
    }
    private void OnDisable()
    {
        SetComponentsDefaultSetting();
        UnRegTouchInput();
    }
    private void OnEnable()
    {
        ScrollController.movementType = ScrollRect.MovementType.Elastic;
        RegisterNewTouchInput();
    }

    void RegisterNewTouchInput()
    {
        swipe1.StateUpdated += Swipe_Updated;
        swipe1.AllowSimultaneousExecution(null);
        swipe1.DirectionThreshold = 0f;
        swipe1.MinimumSpeedUnits = 0.1f;
        swipe1.PlatformSpecificView = ScrollController.gameObject;
        swipe1.MinimumNumberOfTouchesToTrack = 1;
        swipe1.ThresholdSeconds = 0f;
        swipe1.MinimumDistanceUnits = 0f;
        swipe1.EndMode = SwipeGestureRecognizerEndMode.EndWhenTouchEnds;
        FingersScript.Instance.AddGesture(swipe1);
    }

    void UnRegTouchInput()
    {
        if (FingersScript.HasInstance)
        {
            swipe1.StateUpdated -= Swipe_Updated;
            FingersScript.Instance.RemoveGesture(swipe1);
        }
    }

    void SetComponentsDefaultSetting()
    {
        worlddetailScrollContrl.GetComponent<CanvasGroup>().blocksRaycasts = false;
        worldDetailParentRef.WorldDetailContentrRef.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        worldDetailParentRef.backButton.SetActive(false);
        ScrollController.enabled = false;
        ScrollController.verticalNormalizedPosition = 3.5f;
        worlddetailScrollContrl.verticalNormalizedPosition = 1f;
        ScrollController.movementType = ScrollRect.MovementType.Elastic;
        firstTimePlay = true;
    }

    Coroutine IEBottomToTopCoroutine;

    public void BottomToTop()
    {
        if (IEBottomToTopCoroutine == null)
        {
            IEBottomToTopCoroutine = StartCoroutine(IEBottomToTop());
        }
    }

    public IEnumerator IEBottomToTop()
    {
        ScrollController.verticalNormalizedPosition = 3.5f;
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.2f);
        yield return new WaitForSeconds(0.2f);
        DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 1, 0.2f).SetEase(Ease.Linear).OnComplete(WaitForOpenWorldPage);
        IEBottomToTopCoroutine = null;
    }

    IEnumerator ExampleCoroutine()
    {
        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0, 0.1f);
        DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 3.5f, 0.2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.2f);
        this.gameObject.SetActive(false);
        if (ConstantsHolder.xanaConstants.isFromHomeTab)
        {
            GameManager.Instance.HomeCameraInputHandler(true);
            ConstantsHolder.xanaConstants.isFromHomeTab = false;

        }
        GameManager.Instance.UiManager.ShowFooter(true);
    }

    public void WaitForOpenWorldPage()
    {
        DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 0.97f, 0.1f).SetEase(Ease.InSine).OnComplete(BounceBack);

    }

    void BounceBack()
    {
        DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 1, 0.1f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            worldDetailParentRef.CreatorDescriptionTxt.transform.parent.GetComponent<ParentHeightAdjuster>().SetParentHeight();
            contentRectTransform.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ScrollController.transform.parent.GetComponent<ScrollActivity>().enabled = true;
            ScrollController.enabled = true;
        });
    }

    public void Swipe_Updated(DigitalRubyShared.GestureRecognizer gesture)
    {
        firstTimePlay = false;
        swipe2 = gesture as SwipeGestureRecognizer;

        if (swipe2.EndDirection == SwipeGestureRecognizerDirection.Down || swipe2.EndDirection == SwipeGestureRecognizerDirection.Up)
        {
            lastSwipeMovement = swipe2.EndDirection;
        }

        if (swipe2.DistanceY > 0 || swipe2.DistanceY < 0)
        {
            lastSwipeYDistance = swipe2.DistanceY;
            lastSwipeSpeed = swipe2.Speed;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("Last swipe direction and distance Y Values: " + lastSwipeMovement + ": Distance: " + lastSwipeYDistance + ": Speed: " + lastSwipeSpeed);
            if (ScrollController.verticalNormalizedPosition < 1f)
            {
                if (lastSwipeMovement == SwipeGestureRecognizerDirection.Down)
                {
                    if (worlddetailScrollContrl.verticalNormalizedPosition >= 1f)
                    {
                        if (lastSwipeSpeed >= 8000f)
                        {
                            OnClickBackButton();
                        }
                        else
                        {
                            WorldDescriptionPopupPreview.OndescriptionPanelSwipUp?.Invoke(false);
                            DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 1, 0.1f).SetEase(Ease.Linear);
                            worlddetailScrollContrl.GetComponent<CanvasGroup>().blocksRaycasts = false;
                        }
                    }
                }
                else if (lastSwipeMovement == SwipeGestureRecognizerDirection.Up && lastSwipeYDistance > 0)
                {
                    WorldDescriptionPopupPreview.OndescriptionPanelSwipUp?.Invoke(true);
                    DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 0, 0.1f).SetEase(Ease.Linear);
                    worlddetailScrollContrl.GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
            }
            else if (ScrollController.verticalNormalizedPosition >= 1.000001f)
            {
                if (lastSwipeMovement == SwipeGestureRecognizerDirection.Down && lastSwipeYDistance < 0)
                {
                    ScrollController.movementType = ScrollRect.MovementType.Unrestricted;
                    StartCoroutine(ExampleCoroutine());
                }
            }
        }

    }

    //Called From Inspector Scroller event
    public void Closer()
    {
        parentNormVal = ScrollController.verticalNormalizedPosition;
        ChildNormVal = worlddetailScrollContrl.verticalNormalizedPosition;
    }

    public void FakeBGHeightHandler()
    {
        // Get the parent RectTransform's height
        float posY = contentRectTransform.anchoredPosition.y;

        if (worlddetailScrollContrl.verticalNormalizedPosition < 1f)
        {
            if (firstTimePlay)
            {
                SetFakeBGHeight(430f);
            }
            else
            {
                SetFakeBGHeight(-100f);
            }
        }
        else if (worlddetailScrollContrl.verticalNormalizedPosition >= 1f)
        {
            if (posY > -300f)
            {
                SetFakeBGHeight(430f);
            }
            else if (posY > -600f)
            {
                SetFakeBGHeight(650f);
            }
            else if (posY > -800f)
            {
                SetFakeBGHeight(950f);
            }
            else if (posY > -1000f)
            {
                SetFakeBGHeight(1150f);
            }
        }
    }

    void SetFakeBGHeight(float _desrHeight)
    {
        // Get the current anchorMax and anchorMin values
        Vector2 anchorMax = fakeBGRectTransform.anchorMax;

        // Calculate the new anchor values based on desired top and bottom
        float parentHeight = fakeBGRectTransform.parent.GetComponent<RectTransform>().rect.height;
        anchorMax.y = 1f - (_desrHeight / parentHeight);

        //Required clamping due to fakebg showing outside
        anchorMax.y = Mathf.Clamp(anchorMax.y, 0, 1);

        // Apply the new anchor values to the RectTransform
        fakeBGRectTransform.anchorMax = anchorMax;
    }

    public void OnClickBackButton()
    {
        StartCoroutine(ExampleCoroutine());
    }

}