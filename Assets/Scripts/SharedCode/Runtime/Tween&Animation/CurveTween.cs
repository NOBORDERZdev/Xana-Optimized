using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CurveTween : AnimationBase
{
    public enum Scope { Position, Eulers, Scale }
    public enum Space { Local, Global }

    [System.Serializable]
    public class KeyPoint
    {
        public Vector3 valAbsolute;
        public Transform valReference;

        public Vector3 getValue(Scope sc, Space sp)
        {
            if (valReference == null)
            {
                return valAbsolute;
            }
            else
            {
                switch (sc)
                {
                    case Scope.Position:
                        if (sp==Space.Local)
                            return valReference.localPosition;
                        else 
                            return valReference.position;

                    case Scope.Eulers:
                        if (sp == Space.Local)
                            return valReference.localEulerAngles;
                        else
                            return valReference.eulerAngles; 

                    case Scope.Scale:
                        return valReference.localScale; 

                    default:
                        return valAbsolute;
                }
            }
        } 
    }


    public Transform target;
    public Transform[] additionalTargets;
    public Transform targetTransform {
        get {
            if (target==null)
            {
                target = transform;
            }
            return target;
        }
    }

    public Scope scope;
    public Space space;

    public AnimationCurve xCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve yCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve zCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve timeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public KeyPoint startPoint, endPoint;

    public float initialDleay = 0;
    public float duration = 1;

    [HideInInspector]
    public float timeElapsed;
    float realTimeLastFrame, timeBended, x, y, z;

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
        timeBended = 0;
        ApplyVal();
        onEnd.Invoke();
    }

    IEnumerator Tween() {
        onStart.Invoke();

        timeElapsed = 0;
        timeBended = 0;
        ApplyVal();
        if(initialDleay>0)yield return new WaitForSeconds(initialDleay);
        while (timeElapsed<duration)
        {
            timeElapsed += Time.deltaTime;
            timeBended = timeCurve.Evaluate(timeElapsed/duration)*duration;
            ApplyVal ();
            yield return null;
        }

        timeElapsed = duration;
        timeBended = timeCurve.Evaluate(1) * duration;
        ApplyVal ();
        onEnd.Invoke();
        if (loop) StartCoroutine("Tween");
    }

    void ApplyVal()
    {
        if (scope == Scope.Eulers)
        {
            x = Mathf.LerpAngle(startPoint.getValue(scope, space).x, endPoint.getValue(scope, space).x, xCurve.Evaluate(timeBended / duration));
            y = Mathf.LerpAngle(startPoint.getValue(scope, space).y, endPoint.getValue(scope, space).y, yCurve.Evaluate(timeBended / duration));
            z = Mathf.LerpAngle(startPoint.getValue(scope, space).z, endPoint.getValue(scope, space).z, zCurve.Evaluate(timeBended / duration));
        }
        else
        {
            x = Mathf.LerpUnclamped(startPoint.getValue(scope, space).x, endPoint.getValue(scope, space).x, xCurve.Evaluate(timeBended / duration));
            y = Mathf.LerpUnclamped(startPoint.getValue(scope, space).y, endPoint.getValue(scope, space).y, yCurve.Evaluate(timeBended / duration));
            z = Mathf.LerpUnclamped(startPoint.getValue(scope, space).z, endPoint.getValue(scope, space).z, zCurve.Evaluate(timeBended / duration));
        }
        switch (scope)
        {
            case Scope.Position:
                if (space == Space.Local)
                {
                    targetTransform.localPosition = new Vector3(x, y, z);
                    if (additionalTargets != null)
                    {
                        for (int i = 0; i < additionalTargets.Length; i++)
                        {
                            if (additionalTargets[i]!=null)
                            {
                                additionalTargets[i].localPosition = targetTransform.localPosition;
                            }
                        }
                    }
                }
                else
                {
                    targetTransform.position = new Vector3(x, y, z); 
                    if (additionalTargets != null)
                    {
                        for (int i = 0; i < additionalTargets.Length; i++)
                        {
                            if (additionalTargets[i] != null)
                            {
                                additionalTargets[i].position = targetTransform.position;
                            }
                        }
                    }
                }
                break;
            case Scope.Eulers:
                if (space == Space.Local)
                {
                    targetTransform.localEulerAngles = new Vector3(x, y, z);
                    if (additionalTargets != null)
                    {
                        for (int i = 0; i < additionalTargets.Length; i++)
                        {
                            if (additionalTargets[i] != null)
                            {
                                additionalTargets[i].localEulerAngles = targetTransform.localEulerAngles;
                            }
                        }
                    }
                }
                else
                {
                    targetTransform.eulerAngles = new Vector3(x, y, z);
                    if (additionalTargets != null)
                    {
                        for (int i = 0; i < additionalTargets.Length; i++)
                        {
                            if (additionalTargets[i] != null)
                            {
                                additionalTargets[i].eulerAngles = targetTransform.eulerAngles;
                            }
                        }
                    }
                }
                break;
            case Scope.Scale:
                targetTransform.localScale = new Vector3(x, y, z);
                if (additionalTargets != null)
                {
                    for (int i = 0; i < additionalTargets.Length; i++)
                    {
                        if (additionalTargets[i] != null)
                        {
                            additionalTargets[i].localScale = targetTransform.localScale;
                        }
                    }
                }
                break;
            default:
                break;
        } 
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
