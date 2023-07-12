using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
//using UnityEditor.Build.Pipeline;

//Rigidbody is required as force is added to the Rigidbody component
//[RequireComponent(typeof(Rigidbody))]
public class TransformComponent : ItemComponent
{
    //Rigidbody itemRigidbody;

    //private void Awake()
    //{
    //    itemRigidbody = GetComponent<Rigidbody>();
    //    itemRigidbody.isKinematic = true;
    //    itemRigidbody.useGravity = false;
    //}

    //private void OnValidate()
    //{
    //    itemRigidbody = GetComponent<Rigidbody>();
    //}

    #region Rotate Module
    RotateComponentData rotateComponentData;

    public void InitRotate(RotateComponentData rotateComponentData)
    {
        this.rotateComponentData = rotateComponentData;
        StartCoroutine(rotateModule());
    }

    Vector3 currentRotation;
    float rotateTime, x_Rotate, y_Rotate, z_Rotate;

    IEnumerator rotateModule()
    {
        do
        {
            rotateTime = 0;
            ApplyRotateVal();
            while (rotateTime < rotateComponentData.timeToAnimate)
            {
                yield return new WaitForEndOfFrame();
                ApplyRotateVal();
                rotateTime += Time.deltaTime;
            }
            yield return null;
            ApplyRotateVal();
        } while (rotateComponentData.shallLoop);
    }

    void ApplyRotateVal()
    {
        //currentRotation = itemRigidbody.rotation.eulerAngles;
        currentRotation = gameObject.transform.eulerAngles;
        x_Rotate = Mathf.LerpUnclamped(rotateComponentData.defaultValue.x, rotateComponentData.maxValue.x, rotateComponentData.animationCurve.Evaluate(rotateTime / rotateComponentData.timeToAnimate));
        y_Rotate = Mathf.LerpUnclamped(rotateComponentData.defaultValue.y, rotateComponentData.maxValue.y, rotateComponentData.animationCurve.Evaluate(rotateTime / rotateComponentData.timeToAnimate));
        z_Rotate = Mathf.LerpUnclamped(rotateComponentData.defaultValue.z, rotateComponentData.maxValue.z, rotateComponentData.animationCurve.Evaluate(rotateTime / rotateComponentData.timeToAnimate));
        //y_Rotate = Mathf.LerpUnclamped(0, 360, rotateComponentData.animationCurve.Evaluate(rotateTime / rotateComponentData.timeToAnimate));
        //currentRotation = new Vector3(0f, y_Rotate, 0f);
        currentRotation = new Vector3(x_Rotate, y_Rotate, z_Rotate);
        gameObject.transform.rotation = Quaternion.Euler(currentRotation);
        //itemRigidbody.rotation = Quaternion.Euler(currentRotation);
    }
    #endregion

    #region ToAndFro Module
    ToFroComponentData toFroComponentData;
    float toFro_X, toFro_Y, toFro_Z;

    public void InitToFro(ToFroComponentData toFroComponentData)
    {
        this.toFroComponentData = toFroComponentData;

        StartCoroutine(toFroModule());
    }

    float toFroTime;
    IEnumerator toFroModule()
    {
        do
        {
            toFroTime = 0;
            ApplyToFroVal();
            while (toFroTime < toFroComponentData.timeToAnimate)
            {
                yield return new WaitForEndOfFrame();
                ApplyToFroVal();
                toFroTime += Time.deltaTime;
            }
            yield return null;
            ApplyToFroVal();
        } while (toFroComponentData.shallLoop);
    }

    void ApplyToFroVal()
    {
        //Vector3 v = startingPosition;
        //switch (toFroComponentData.xyz_Axis)
        //{
        //    case ToFroComponentData.axis.x_Axis:
        //        v.x += Mathf.LerpUnclamped(0, toFroComponentData.distanceToCover, toFroComponentData.animationCurve.Evaluate(toFroTime / toFroComponentData.timeToAnimate));
        //        break;
        //    case ToFroComponentData.axis.y_Axis:
        //        v.y += Mathf.Sin(Mathf.LerpUnclamped(0, toFroComponentData.distanceToCover, toFroComponentData.animationCurve.Evaluate(toFroTime / toFroComponentData.timeToAnimate)));
        //        break;
        //    case ToFroComponentData.axis.z_Axis:
        //        v.z += Mathf.Sin(Mathf.LerpUnclamped(0, toFroComponentData.distanceToCover, toFroComponentData.animationCurve.Evaluate(toFroTime / toFroComponentData.timeToAnimate)));
        //        break;
        //}
        //transform.position = v;
        toFro_X = Mathf.LerpUnclamped(toFroComponentData.defaultValue.x, toFroComponentData.maxValue.x, toFroComponentData.animationCurve.Evaluate(toFroTime / toFroComponentData.timeToAnimate));
        toFro_Y = Mathf.LerpUnclamped(toFroComponentData.defaultValue.y, toFroComponentData.maxValue.y, toFroComponentData.animationCurve.Evaluate(toFroTime / toFroComponentData.timeToAnimate));
        toFro_Z = Mathf.LerpUnclamped(toFroComponentData.defaultValue.z, toFroComponentData.maxValue.z, toFroComponentData.animationCurve.Evaluate(toFroTime / toFroComponentData.timeToAnimate));
        transform.localPosition = new Vector3(toFro_X, toFro_Y, toFro_Z);
    }
    #endregion

    #region Scale Module
    ScalerComponentData scalerComponentData;
    float x, y, z, timeElapsed;

    public void InitScale(ScalerComponentData scalerComponentData)
    {
        this.scalerComponentData = scalerComponentData;
        StartCoroutine(ScalingObject());
    }

    IEnumerator ScalingObject()
    {
        do
        {
            timeElapsed = 0;
            ApplyVal();
            yield return new WaitForEndOfFrame();
            while (timeElapsed < scalerComponentData.timeToAnimate)
            {
                timeElapsed += Time.deltaTime;
                ApplyVal();
                yield return null;
            }
            ApplyVal();
        } while (scalerComponentData.shallLoop);
    }

    void ApplyVal()
    {
        x = Mathf.LerpUnclamped(scalerComponentData.defaultScaleValue.x, scalerComponentData.maxScaleValue.x, scalerComponentData.animationCurve.Evaluate(timeElapsed / scalerComponentData.timeToAnimate));
        y = Mathf.LerpUnclamped(scalerComponentData.defaultScaleValue.y, scalerComponentData.maxScaleValue.y, scalerComponentData.animationCurve.Evaluate(timeElapsed / scalerComponentData.timeToAnimate));
        z = Mathf.LerpUnclamped(scalerComponentData.defaultScaleValue.z, scalerComponentData.maxScaleValue.z, scalerComponentData.animationCurve.Evaluate(timeElapsed / scalerComponentData.timeToAnimate));
        transform.localScale = new Vector3(x, y, z);
        //        Debug.Log("Animation Curve : " + scalerComponentData.animationCurve.Evaluate(timeElapsed / scalerComponentData.timeToAnimate));
    }
    #endregion
}
