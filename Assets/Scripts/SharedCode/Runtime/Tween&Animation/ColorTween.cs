using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class ColorTween : AnimationBase {
    public MaskableGraphic target;
    public MaskableGraphic[] additionalTargets;
    MaskableGraphic targetGraphic
    {
        get
        {
            if (target == null)
            {
                target = GetComponent<MaskableGraphic>();
            }
            return target;
        }
    }
    //public Color startColor = Color.white, endColor = Color.white;
    //public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });

    public Color[] keyColors = new Color[] { Color.white, Color.white };
    public bool alphaOnly = false;
    Color startColor, endColor, currentColor;

    public float initialDleay = 0;
    public float duration = 1;

    float realTimeLastFrame, timeElapsed, lotDuration;

    void OnEnable()
    {
       if(startOnEnable) StartTween();
    }

    void OnDisable()
    {
        EndTween();
    }

    public void StartTween() {
        StartCoroutine("Tween");
    }

    public void EndTween() {
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
        timeElapsed = 0;
        ApplyVal();
        if (initialDleay > 0) yield return new WaitForSeconds(initialDleay);
        if (duration>0)
        { 
            lotDuration = duration / (keyColors.Length-1);
            for (int i = 1; i < keyColors.Length; i++)
            {
                timeElapsed = 0;
                startColor = keyColors[i - 1];
                endColor = keyColors[i];
                while (timeElapsed < lotDuration)
                {
                    timeElapsed += Time.deltaTime;
                    currentColor = Color.LerpUnclamped(startColor, endColor, timeElapsed / lotDuration); 
                    ApplyVal();

                    yield return null;
                }
            }
        }

        currentColor = startColor = keyColors[keyColors.Length - 1];
        ApplyVal();

        onEnd.Invoke();
        if (duration > 0 && loop) StartCoroutine("Tween");
    }

    void ApplyVal()
    {
        if (alphaOnly)
        {
            currentColor.r = targetGraphic.color.r;
            currentColor.g = targetGraphic.color.g;
            currentColor.b = targetGraphic.color.b;
        }
        targetGraphic.color = Color.LerpUnclamped(startColor, endColor, timeElapsed / lotDuration);

        for (int a = 0; a < additionalTargets.Length; a++)
        {
            if (alphaOnly)
            {
                currentColor.r = additionalTargets[a].color.r;
                currentColor.g = additionalTargets[a].color.g;
                currentColor.b = additionalTargets[a].color.b;
            }
            additionalTargets[a].color = currentColor;
        }
    }


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
        EndTween();
    }
}
