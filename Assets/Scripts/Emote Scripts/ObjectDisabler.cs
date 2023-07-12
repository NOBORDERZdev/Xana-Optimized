using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ObjectDisabler : MonoBehaviour
{
    public float duration = .5f;
    public float delay = 1f;
    CanvasGroup canvasGroup;

    public void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnEnable()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", duration, "easetype", iTween.EaseType.easeOutExpo, "delay", delay, "onupdate",
            "OnUpdate", "oncomplete", "OnComplete"));
    }

    void OnUpdate(float val)
    {
        canvasGroup.alpha = val;
    }

    void OnComplete()
    {
        gameObject.SetActive(false);
        canvasGroup.alpha = 1f;
    }
}
