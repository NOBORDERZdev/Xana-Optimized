using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    public Transform refObject;
    public IntRange count;
    public FloatRange timeGap;
    public bool instantiteOnEnable;

    int c = 0;
    Delayed.Action loopedAct = null;

	void OnEnable ()
    {
        if(instantiteOnEnable) Instantiate();
    }

    void OnDisable()
    {
        if (loopedAct != null) loopedAct.CancelAction();
    }
	 
	void Instantiate ()
    {
        if (loopedAct != null) loopedAct.CancelAction();
        c = 0;
        LoopedAct();
    }

    void LoopedAct()
    {
        if (c < count.randomValue)
        {
            refObject.Duplicate();
            c++;
            loopedAct = new Delayed.Action(LoopedAct, timeGap.randomValue);
        }
    }
}
