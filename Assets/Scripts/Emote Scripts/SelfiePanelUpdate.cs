using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelfiePanelUpdate : MonoBehaviour
{
    [SerializeField] GameObject[] gameObjs;
    public void OnEnable()
    {
        if(GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.SelfiePanleUpdateObjects += SelfiePanleUpdateObjects;
    }

    public void OnDisable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.SelfiePanleUpdateObjects -= SelfiePanleUpdateObjects;
    }

    private void SelfiePanleUpdateObjects(bool isOn)
    {
        foreach (var item in gameObjs)
        {
            item.SetActive(isOn);
        }
    }
}
