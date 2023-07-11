using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationsStack : AnimationBase {
    public List<AnimationBase> animations = new List<AnimationBase>(); 
    int runningAnimation;

    void OnEnable()
    {
        for (int i = 0; i < animations.Count; i++)
        {
            animations[i].startOnEnable = false;
            animations[i].loop = false;
        }
        if (startOnEnable)
        {
            StartAnimations();
        }
    }

    void StartAnimations()
    {
        //Debug.Log("StartAnimations");
        StopAnimations();
        runningAnimation = 0;
        onStart.Invoke();
        StartSingleAnim();
    }

    void StopAnimations()
    {
        //Debug.Log("StopAnimations");
        for (int i = 0; i < animations.Count; i++)
        {
            animations[i].StopAnim();
            try
            {
                animations[i].RemoveCallbackFromEnd(OnSingleAnimationEnded);
            }
            catch {}
        }
    }

    void ResetAnimations()
    {
        //Debug.Log("ResetAnimations");
        onEnd.Invoke();
        for (int i = 0; i < animations.Count; i++)
        {
            animations[i].ResetAnim();
        }
        runningAnimation = 0;
    }

    void StartSingleAnim()
    {
        //Debug.Log("StartSingleAnim");
        animations[runningAnimation].AddCallbackOnEnd(OnSingleAnimationEnded);
        animations[runningAnimation].StartAnim();
    }

    void OnSingleAnimationEnded()
    {
        //Debug.Log("OnSingleAnimationEnded");
        try
        {
            animations[runningAnimation].RemoveCallbackFromEnd(OnSingleAnimationEnded);
        }
        catch { }
        runningAnimation++;
        if (runningAnimation < animations.Count) StartSingleAnim();
        else
        {
            if (loop)
            {
                runningAnimation = 0;
                StartSingleAnim();
            }
            else
            {
                onEnd.Invoke();
            }
        }
    }

    #region AnimationBase implementation 
    public override void StartAnim()
    {
        StartAnimations();
    }

    public override void StopAnim()
    {
        StopAnimations();
    }

    public override void ResetAnim()
    {
        ResetAnimations();
    }
    #endregion
}
