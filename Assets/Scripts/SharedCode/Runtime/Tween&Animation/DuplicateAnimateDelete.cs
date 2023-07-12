using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateAnimateDelete : AnimationBase
{
    public Transform targetObject;
    public AnimationBase targetAnimation;

    Transform duplicatedObject;
    bool cbAdded;

    void OnEnable()
    {
        if (startOnEnable) StartAnim();
    }

    void OnDisable()
    {
        ResetAnim();
    }

    public override void StartAnim()
    {
        ResetAnim();
        duplicatedObject = targetObject.Duplicate();
        targetAnimation.StartAnim();
        targetAnimation.AddCallbackOnEnd(OnTargetAnimEnd);
        cbAdded = true;
    }

    void OnTargetAnimEnd()
    {
        ResetAnim();
    }

    public override void ResetAnim()
    {
        CleanUp();
        targetAnimation.ResetAnim();
    }

    public override void StopAnim()
    {
        CleanUp();
        targetAnimation.ResetAnim();
    }

    void CleanUp()
    {
        if (cbAdded)
        {
            targetAnimation.RemoveCallbackFromEnd(OnTargetAnimEnd);
            cbAdded = false;
        }
        if (duplicatedObject!=null)
        {
            Destroy(duplicatedObject.gameObject);
        }
    }
}
