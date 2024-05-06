using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RioPresetHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> presetButtons;
    string presetGetApi = "/hot/items/get-admin/";
    void Start()
    {
        turnAllPresetOff();
    }

    /// <summary>
    /// To disable all specials presets.
    /// </summary>
    public void turnAllPresetOff() {
        foreach (var preset in presetButtons)
        {
            preset.gameObject.SetActive(false);
        }
    }
}