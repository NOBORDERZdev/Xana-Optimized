using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpObjects : MonoBehaviour
{
    [SerializeField] GameObject[] gameObjs;
    public void OnEnable()
    {
        if(GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.UpdateHelpObject += UpdateHelpObject;
    }

    public void OnDisable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.UpdateHelpObject -= UpdateHelpObject;
    }

    private void UpdateHelpObject(bool isOn)
    {
        foreach (var item in gameObjs)
        {
            item.SetActive(isOn);
        }
    }
}
