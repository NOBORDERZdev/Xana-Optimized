using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DymanicMuseumUpdateObject : MonoBehaviour
{
    [SerializeField] GameObject[] gameObjs;

    public void OnEnable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnUpdateMuseumRoom += OnUpdateMuseumRoom;
    }

    public void OnDisable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.OnUpdateMuseumRoom -= OnUpdateMuseumRoom;
    }

    private void OnUpdateMuseumRoom(bool isOn)
    {
        foreach (var item in gameObjs)
        {
            item.SetActive(isOn);
        }
    }
}