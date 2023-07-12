using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointScroll : MonoBehaviour 
{
    public Transform[] targets; 
    public AnimationCurve curve;
    public float speed;

    Interpolate.Position anim = null;


    public void ScrollTo(Transform point)
    {
        float tempFloat = 0;
        ScrollTo(point, ref tempFloat);
    }
    public void ScrollTo(Transform point, ref float duration)
    { 
//        if (point.parent != target) return;
        if (anim != null) anim.Stop();
        if (gameObject.activeInHierarchy && Application.isPlaying)
        {
            //s = d/t
            float d = Vector3.Distance(targets[0].localPosition, -point.localPosition) / speed;
            for (int i = 0; i < targets.Length; i++)
            {
                anim = new Interpolate.Position(targets[i], targets[i].localPosition, -point.localPosition, d, true, curve);
            }
            duration = d;
        }
        else
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].localPosition = -point.localPosition; 
            } 
        }  
    }

    public Transform testPoint;
    public void Test()
    {
        ScrollTo(testPoint);
    }
}
