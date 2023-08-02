using System.Collections;
using DG.Tweening;
using Models;
using UnityEngine;

public class TransformComponent : ItemComponent
{
    Ease AnimationCurveValueConvertor(int index)
    {
        switch (index)
        {
            case 0:
                return Ease.Linear;
                Debug.Log("Linear");
                break;
            case 1:
                return Ease.InOutBounce;
                Debug.Log("Spring");
                break;
            case 2:
                return Ease.InSine;
                Debug.Log("easeIn");
                break;
            case 3:
                return Ease.OutSine;
                Debug.Log("easeOut");
                break;
            case 4:
                return Ease.InOutSine;
                Debug.Log("easeInOut");
                break;
            case 5:
                return Ease.InOutBack;
            case 6:
                return Ease.InOutCirc;
            case 7:
                return Ease.InOutExpo;
            case 8:
                return Ease.OutBounce;
            case 9:
                return Ease.OutQuint;

            default:
                return Ease.Linear;
                break;
        }
    }

    #region Rotate Module
    RotateComponentData rotateComponentData;

    public void InitRotate(RotateComponentData rotateComponentData)
    {
        this.rotateComponentData = rotateComponentData;
        StartCoroutine(rotateModule());
    }

    IEnumerator rotateModule()
    {
        //StartComponent();
        while (true)
        {

            yield return transform.DORotate(rotateComponentData.maxValue, rotateComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(rotateComponentData.animationCurveIndex)).WaitForCompletion();
            yield return transform.DORotate(rotateComponentData.defaultValue, rotateComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(rotateComponentData.animationCurveIndex)).WaitForCompletion();
        }
    }


    #endregion


    #region ToAndFro Module
    ToFroComponentData toFroComponentData;
    public Ease toFroEaseType;
    public void InitToFro(ToFroComponentData toFroComponentData)
    {
        this.toFroComponentData = toFroComponentData;

        StartCoroutine(toFroModule());
    }

    IEnumerator toFroModule()
    {
        //StartComponent();
        while (true)
        {

            yield return transform.DOMove(toFroComponentData.maxValue, toFroComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(toFroComponentData.animationCurveIndex)).WaitForCompletion();
            yield return transform.DOMove(toFroComponentData.defaultValue, toFroComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(toFroComponentData.animationCurveIndex)).WaitForCompletion();
        }
    }

    #endregion


    #region Scale Module

    ScalerComponentData scalerComponentData;
    public Ease scalerEaseType;
    public void InitScale(ScalerComponentData scalerComponentData)
    {
        this.scalerComponentData = scalerComponentData;
        StartCoroutine(ScalingObject());
    }

    IEnumerator ScalingObject()
    {
        //StartComponent();
        while (true)
        {
            yield return transform.DOScale(scalerComponentData.maxScaleValue, scalerComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(scalerComponentData.animationCurveIndex)).WaitForCompletion();
            yield return transform.DOScale(scalerComponentData.defaultScaleValue, scalerComponentData.timeToAnimate).SetEase(AnimationCurveValueConvertor(scalerComponentData.animationCurveIndex)).WaitForCompletion();
        }
    }

    #endregion
}