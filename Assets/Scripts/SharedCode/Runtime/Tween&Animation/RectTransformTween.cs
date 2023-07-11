using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectTransformTween : AnimationBase
{
    [System.Serializable]
    public class KeyPoint
    {
        public Vector2 valAbsolute;
        public RectTransform valReference;

        public Vector2 getValue()
        {
            if (valReference == null)
            {
                return valAbsolute;
            }
            else
            { 
                return valReference.sizeDelta;
            }
        }
    }

    public RectTransform target;
    public RectTransform targetTransform
    {
        get
        {
            if (target == null)
            {
                target = GetComponent<RectTransform>();
            }
            return target;
        }
    } 

    public AnimationCurve xCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve yCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public KeyPoint startPoint, endPoint;

    public float initialDleay = 0;
    public float duration = 1;

    float realTimeLastFrame, timeElapsed, x, y, z;

    void OnEnable()
    {
        if (startOnEnable) StartTween();
    }

    void OnDisable()
    {
        EndTween();
    }

    public void StartTween()
    {
        StartCoroutine("Tween");
    }

    public void EndTween()
    {
        StopCoroutine("Tween");
    }

    public void ResetTween()
    {
        EndTween();
        timeElapsed = 0;
        ApplyVal();
    }

    IEnumerator Tween()
    {
        onStart.Invoke();

        timeElapsed = 0;
        ApplyVal();
        if (initialDleay > 0) yield return new WaitForSeconds(initialDleay);
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            ApplyVal();
            yield return null;
        }

        ApplyVal();
        onEnd.Invoke();
        if (loop) StartCoroutine("Tween");
    }

    void ApplyVal()
    {
        x = Mathf.LerpUnclamped(startPoint.getValue().x, endPoint.getValue().x, xCurve.Evaluate(timeElapsed / duration));
        y = Mathf.LerpUnclamped(startPoint.getValue().y, endPoint.getValue().y, yCurve.Evaluate(timeElapsed / duration));
        target.sizeDelta = new Vector2(x,y);
    }

    #region AnimationBase implementation 
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
        StartTween();
    }
    public override void StopAnim()
    {
        EndTween();
    }
    public override void ResetAnim()
    {
        ResetTween();
    }
    #endregion
}
