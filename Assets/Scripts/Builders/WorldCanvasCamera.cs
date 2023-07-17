using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvasCamera : MonoBehaviour
{
    [SerializeField]
    GameObject[] uiCamerasforBuilder;

    void OnEnable()
    {
        BuilderEventManager.EnableWorldCanvasCamera += EnableCamera;
    }
    void OnDisable()
    {
        BuilderEventManager.EnableWorldCanvasCamera -= EnableCamera;
    }

    void EnableCamera()
    {
        foreach (GameObject camObj in uiCamerasforBuilder)
        {
            camObj.SetActive(true);
        }
    }
}
