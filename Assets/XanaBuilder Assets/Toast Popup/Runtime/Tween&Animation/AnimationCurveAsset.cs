using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationCurveAsset", menuName = "Animations/AnimationCurveAsset")]
public class AnimationCurveAsset : ScriptableObject
{
    public AnimationCurve curve;

    public float Evaluate(float time)
    {
        return curve.Evaluate(time);
    }
}
