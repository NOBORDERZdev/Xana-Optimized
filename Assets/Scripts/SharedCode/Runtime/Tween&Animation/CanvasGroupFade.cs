using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class CanvasGroupFade : AnimationBase 
{ 
    public enum OnEnableBehaviour { DoNothing, FadeIn, FadeOut }
    public CanvasGroup canvasGroup;
    public float initalDelay = 0;
    public float duration = 1;
    public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1); 
    public OnEnableBehaviour onEnable; 

    public void OnEnable()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            switch (onEnable)
            { 
                case OnEnableBehaviour.FadeIn:
                    FadeIn();
                    break;

                case OnEnableBehaviour.FadeOut:
                    FadeOut();
                    break;

                default:
                    break;
            }
        }
    }

    public void FadeIn(float _duration = -1)
    {  
        if (!gameObject.activeSelf || canvasGroup == null) return;
        v1 = 0;
        v2 = 1;
        d = _duration == -1 ? duration : _duration;
        StopCoroutine("Tween");
        StartCoroutine("Tween");
    }

    public void FadeOut(float _duration = -1)
    {
        if (!gameObject.activeSelf || canvasGroup == null) return;
        v1 = 1;
        v2 = 0;
        d = _duration == -1 ? duration : _duration;
        StopCoroutine("Tween");
        StartCoroutine("Tween");
    }

    float v1, v2, d;
    IEnumerator Tween()
    {
        onStart.Invoke();
        canvasGroup.alpha = v1;
        if (initalDelay > 0) yield return new WaitForSeconds(initalDelay);

        float timeElapsed = 0; 
        while (timeElapsed<duration)
        {
            timeElapsed += Time.deltaTime; 
            canvasGroup.alpha = Mathf.Lerp(v1, v2, curve.Evaluate(timeElapsed/duration));
            yield return null;
        } 
        canvasGroup.alpha = v2;
        onEnd.Invoke();
    }

    public void StopFade()
    {
        StopCoroutine("Tween"); 
    } 

    #region implemented abstract members of AnimationBase
    public enum Anim { FadeIn, FadeOut }

    [Header("AnimationBase Options")] 
    public Anim defaultAnim = Anim.FadeIn;

    public override float Duration
    {
        get
        {
            return duration;
        }

        set
        {
            duration = value;
        }
    }

    public override void StartAnim()
    {
        if (defaultAnim == Anim.FadeIn)
        {
            FadeIn();
        }
        else
        {
            FadeOut();
        }
    }

    public override void StopAnim()
    {
        StopFade();
    }

    public override void ResetAnim()
    {
        StopFade();
        if (defaultAnim == Anim.FadeIn)
        {
            canvasGroup.alpha = 0;
        }
        else
        {
            canvasGroup.alpha = 1;
        }
    }

    public override void AddCallbackOnEnd(UnityEngine.Events.UnityAction act)
    {
        onEnd.AddListener(act);
    }

    public override void RemoveCallbackFromEnd(UnityEngine.Events.UnityAction act)
    {
        onEnd.RemoveListener(act);
    }

    #endregion
}
