using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomActivateDeactivate : MonoBehaviour
{
    public FloatRange activeTime;
    public FloatRange inactiveTime;

    public Vector3 positionMin;
    public Vector3 positionMax;

    public Vector3 eulerAnglesMin;
    public Vector3 eulerAnglesMax;

    Delayed.Action act;

    void Awake ()
    {
        if (activeTime.min <= 0 || activeTime.max <= 0) return;
        if (inactiveTime.min <= 0 || inactiveTime.max <= 0) return;
        gameObject.SetActive(false);
        ToggleState();
    }

    void OnDestroy()
    {
        if (act != null) act.CancelAction();
    }

    void ToggleState()
    {
        if (act != null) act.CancelAction();
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            act = new Delayed.Action(ToggleState, inactiveTime.randomValue);
        }
        else
        {
            transform.localPosition = positionMax + (positionMax - positionMin) * Random.Range(0f, 1f); 
            transform.localEulerAngles = eulerAnglesMin + (eulerAnglesMax - eulerAnglesMin) * Random.Range(0f, 1f);
            gameObject.SetActive(true);
            act = new Delayed.Action(ToggleState, activeTime.randomValue);
        }
    }
}
