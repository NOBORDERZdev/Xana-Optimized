using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScrollActivity : MonoBehaviour
{
    [Header("For World Icons Scroll")]
    public ScrollRect ScrollController;
    public GameObject btnback;
    public float normalized;
    private int lastindex = 1;

    private void Awake()
    {
        ScrollController.verticalNormalizedPosition = 3.5f;
    }

    //Worked by Abdullah & Riken
    private void OnDisable()
    {
        ScrollController.verticalNormalizedPosition = 3.5f;
        ScrollController.movementType = ScrollRect.MovementType.Elastic;
        lastindex = 1;
    }
    private void OnEnable()
    {
        ScrollController.movementType = ScrollRect.MovementType.Elastic;
        lastindex = 1;
    }


    private void Update()
    {
        if (ScrollController.verticalNormalizedPosition > 1f)
        {
            ScrollController.movementType = ScrollRect.MovementType.Unrestricted;

            if (Input.touchCount > 0)
            {

            }
            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended))
            {
                ScrollController.movementType = ScrollRect.MovementType.Unrestricted;
                StartCoroutine(ExampleCoroutine());
                lastindex = 2;
            }
        }
        else if (ScrollController.verticalNormalizedPosition < 1f)
        {
            ScrollController.movementType = ScrollRect.MovementType.Elastic;
            if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)) && lastindex == 0 && ScrollController.verticalNormalizedPosition < 0.99f && ScrollController.verticalNormalizedPosition > 0)
            {
                DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 1, 0.1f).SetEase(Ease.Linear);
                lastindex = 1;
            }
            else if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)) && lastindex == 1 && ScrollController.verticalNormalizedPosition < 0.99f && ScrollController.verticalNormalizedPosition > 0)
            {
                DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 0, 0.1f).SetEase(Ease.Linear);
                lastindex = 0;
            }
        }
    }
    public void Closer()
    {
        if (ScrollController.verticalNormalizedPosition < 0.001f)
        {
            btnback.SetActive(true);
        }
        else
        {
            btnback.SetActive(false);
        }
        normalized = ScrollController.verticalNormalizedPosition;
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
        yield return new WaitForSeconds(0.2f);
        DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 1, 0.2f).SetEase(Ease.Linear).OnComplete(WaitForOpenWorldPage);
        IEBottomToTopCoroutine = null;
    }
    IEnumerator ExampleCoroutine()
    {
        DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 3.5f, 0.2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.2f);
        this.gameObject.SetActive(false);
        UIManager.Instance.ShowFooter(true);
    }
    public void WaitForOpenWorldPage()
    {
        ScrollController.transform.parent.GetComponent<ScrollActivity>().enabled = true;
    }
}