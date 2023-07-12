using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class UnityEventTransform : UnityEvent<Transform>{}

public class LooperTransformChildren : LooperBase {
    public Transform parentTransform;
    public UnityEventTransform ev;

    public override void StartLoop()
    {
        //print("start loop " + transform.childCount);
        base.startVal = 0;
        if(parentTransform != null)  base.endVal = parentTransform.childCount - 1;
        base.incrementVal = 1;
        base.StartLoop();
    }

    public override void InvokeEvent(int i)
    { 
        if(parentTransform != null && i<parentTransform.childCount)
        {
            ev.Invoke(parentTransform.GetChild(i));
        } 
    }
}
