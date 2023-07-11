using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ToastElementAnimation : MonoBehaviour
{
    public ImageFillTween fillTween;
    public CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    float moveOffset = 100;
    public void PlayOpenAnimation(float time)
    {

        StartCoroutine(PlayAnimation(time));

    }

    IEnumerator PlayAnimation(float time) 
    {
        OpenToast();

        canvasGroup.alpha = 1;
        if (fillTween != null)
        {
            fillTween.duration = time;
            fillTween.ReplayAnim();

        }
        while (!fillTween.isProgressbarComplete) yield return new WaitForEndOfFrame();

        CloseToast();
    }

    public void OpenToast() 
    {
        iTween.ValueTo(this.gameObject, iTween.Hash("from", GetStartFloat(rectTransform), "to", GetEndFloat(rectTransform), "time", .6f, "onupdate", "UpdateRectTransform", "onupdatetarget", this.gameObject));
    }
    public void CloseToast() 
    {
        iTween.ValueTo(this.gameObject, iTween.Hash("from", GetEndFloat(rectTransform), "to", GetStartFloat(rectTransform), "time", .2f, "onupdate", "UpdateRectTransform", "onupdatetarget", this.gameObject));

        Destroy(gameObject, 0.3f);

    }

    void UpdateRectTransform(float val)
    {
        Vector3 pos = new Vector3(val, rectTransform.anchoredPosition.y, 0);
        rectTransform.anchoredPosition = pos;
    }

    float GetStartFloat(RectTransform r)
    {
        float pos = r.anchoredPosition3D.x;
        pos += moveOffset;
        //print("Start " + pos);
        return pos;
    }

    float GetEndFloat(RectTransform r)
    {
        float pos = r.anchoredPosition3D.x;
        //print("End " + pos);
        return pos;
    }
}
