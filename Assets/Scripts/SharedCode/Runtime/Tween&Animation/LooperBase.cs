using UnityEngine;
using System.Collections;  

public abstract class LooperBase : MonoBehaviour 
{  
    protected int startVal, endVal, incrementVal;
    public float timeGap;
    public Vector2 timeGapRange;
    public bool startOnEnable; 
    public bool restartOnEnd; 

    void OnEnable()
    {
        if (startOnEnable) StartLoop();
    }

    Coroutine cr;
    public virtual void StartLoop()
    {
        if (cr != null) CommonMono.instance.StopCoroutine(cr);
        cr = CommonMono.instance.StartCoroutine(Loop_c()); 
    }
     
    public virtual void StopLoop()
    {
        if (cr != null) StopCoroutine(cr);
    }

    public abstract void InvokeEvent(int i); 
    public virtual void EndEvent() { }

    IEnumerator Loop_c()
    { 
        int i = startVal;
        while(true)
        { 
            InvokeEvent(i);
            if (timeGap > 0) yield return new WaitForSeconds(timeGap);
            else if (timeGapRange.y > timeGapRange.x) yield return new WaitForSeconds(Random.Range(timeGapRange.x, timeGapRange.y));
            if (i == endVal)
            { 
                if (restartOnEnd)
                {
                    StartLoop();
                }
                print("looper end");
                EndEvent();
                yield break;
            }   
            i += incrementVal;  
        }
    }
}
