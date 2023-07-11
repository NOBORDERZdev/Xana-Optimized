using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LayoutElementTween : AnimationBase
{
    public enum AnimVar { Preferred, Flexible }

    public LayoutElement layoutElement;
    public float duration = 1;

    public bool animateWidth;
    public AnimVar widthVar;
    public float widthFrom = 0, widthTo = 0;
    public bool animateHeight;
    public AnimVar heightVar;
    public float heightFrom = 0, heightTo = 0;
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public bool changeActive = false;

    void OnEnable()
    {
        if (layoutElement == null) layoutElement = GetComponent<LayoutElement>();
        if (startOnEnable) StartTween();
    }

    public void StartTween()
    {
        if (duration == 0)
        {
            PostTween();
        }
        else
        {
            ChangeActive(true);
            StopCoroutine("Tween");
            StartCoroutine("Tween");
        }
    }

    public void StopTween()
    {
        StopCoroutine("Tween");
    }

    public void ResetTween()
    {
        StopCoroutine("Tween");
        timeElapsed = 0;
        PostTween();
    }

    float timeElapsed, lf;
    IEnumerator Tween() 
    {  
        timeElapsed = 0;  
        while (timeElapsed < duration)
        {
            lf = curve.Evaluate(timeElapsed / duration);
            if (animateWidth)
            {
                switch (widthVar)
                {
                    case AnimVar.Preferred:
                        layoutElement.preferredWidth = Mathf.Lerp(widthFrom, widthTo, lf);
                        break;
                    case AnimVar.Flexible:
                        layoutElement.flexibleWidth = Mathf.Lerp(widthFrom, widthTo, lf);
                        break;
                    default:
                        break;
                }
            }

            if (animateHeight)
            {
                switch (heightVar)
                {
                    case AnimVar.Preferred:
                        layoutElement.preferredHeight = Mathf.Lerp(heightFrom, heightTo, lf);
                        break;
                    case AnimVar.Flexible:
                        layoutElement.flexibleHeight = Mathf.Lerp(heightFrom, heightTo, lf);
                        break;
                    default:
                        break;
                }
            }

            yield return null;
            timeElapsed += Time.deltaTime; 
        }  
        PostTween();
        onEnd.Invoke();
        if (loop)
        {
            StartTween();
        }
    }

    void PostTween()
    {
        if (animateWidth)
        {
            switch (widthVar)
            {
                case AnimVar.Preferred:
                    layoutElement.preferredWidth = widthTo;
                    ChangeActive(layoutElement.preferredWidth > 0);
                    break;
                case AnimVar.Flexible:
                    layoutElement.flexibleWidth = widthTo;
                    ChangeActive(layoutElement.flexibleWidth > 0);
                    break;
                default:
                    break;
            }
        }

        if (animateHeight)
        {
            switch (heightVar)
            {
                case AnimVar.Preferred:
                    layoutElement.preferredHeight = heightTo;
                    ChangeActive(layoutElement.preferredHeight > 0);
                    break;
                case AnimVar.Flexible: 
                    layoutElement.flexibleHeight = heightTo;
                    ChangeActive(layoutElement.flexibleHeight > 0);
                    break;
                default:
                    break;
            }
        } 
    }

    void ChangeActive(bool active)
    {
        if (changeActive)
        {
            if (active)
            {
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
//                    print(gameObject.name + " SetActive: true");
                }
            } 
            else
            {
                if (gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
//                    print(gameObject.name + " SetActive: false");
                }
            }
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
        StopTween();
    }

    public override void ResetAnim()
    {
        ResetTween();
    }
}

