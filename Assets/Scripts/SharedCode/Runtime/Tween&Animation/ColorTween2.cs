using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class ColorTween2 : AnimationBase {
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

	public Gradient gradient;
    public bool alphaOnly = false; 

    public float initialDleay = 0;
    public float duration = 1;

    float timeElapsed;

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
        ApplyColor(gradient.Evaluate(0));
    }

    IEnumerator Tween()
    {
        ApplyColor(gradient.Evaluate (0));
        if (initialDleay > 0) yield return new WaitForSeconds(initialDleay);
		if (duration > 0) { 
			timeElapsed = 0;
			while (timeElapsed < duration) {
				ApplyColor (gradient.Evaluate (timeElapsed / duration));
				timeElapsed += Time.deltaTime;
				yield return null;
			}
		}
		ApplyColor (gradient.Evaluate (1));
        onEnd.Invoke();
        if (duration > 0 && loop) StartCoroutine("Tween");
    }

	void ApplyColor(Color col){
		if (alphaOnly) col = new Color (targetGraphic.color.r, targetGraphic.color.g, targetGraphic.color.b, col.a);
		targetGraphic.color = col;
		for (int i = 0; i < additionalTargets.Length; i++) {
			if (alphaOnly) col = new Color (additionalTargets [i].color.r, additionalTargets [i].color.g, additionalTargets [i].color.b, col.a);
			additionalTargets [i].color = col;
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
        ResetTween();
    }
}
