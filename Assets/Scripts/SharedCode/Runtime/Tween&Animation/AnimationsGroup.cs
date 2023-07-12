using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsGroup : AnimationBase
{
    public List<AnimationBase> animations = new List<AnimationBase>();
    int runningAnimations;

    void OnEnable()
    {
        for (int i = 0; i < animations.Count; i++)
        {
            animations[i].startOnEnable = false;
            animations[i].loop = false;
            animations[i].AddCallbackOnEnd(OnSingleAnimationEnded);
        }
        StopAnimations();
        if (startOnEnable)
        {
            StartAnimations();
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < animations.Count; i++)
        {
            try { animations[i].RemoveCallbackFromEnd(OnSingleAnimationEnded); }
            catch { }
        }
    }

    void StartAnimations()
    {
        //Debug.Log("StartAnimations");
        StopAnimations();
        runningAnimations = 0;
        onStart.Invoke();
        for (int i = 0; i < animations.Count; i++)
        { 
            animations[i].StartAnim();
            runningAnimations++;
        }
    }

    void StopAnimations()
    {
        //Debug.Log("StopAnimations");
        for (int i = 0; i < animations.Count; i++)
        { 
            animations[i].StopAnim();
        }
        runningAnimations = 0;
    }

    void ResetAnimations()
    {
        //Debug.Log("ResetAnimations");
        for (int i = 0; i < animations.Count; i++)
        {
            animations[i].ResetAnim();
        }
        runningAnimations = 0;
    }

    void OnSingleAnimationEnded()
    {
        //Debug.Log("OnSingleAnimationEnded");
        runningAnimations--;
        if (runningAnimations == 0)
        {
            onEnd.Invoke(); 
            if (loop)
            {
                StartAnimations();
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
