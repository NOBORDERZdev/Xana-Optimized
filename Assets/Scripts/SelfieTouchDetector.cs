using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfieTouchDetector : MonoBehaviour
{
    PlayerSelfieController controller;

    private void Start()
    {
        controller = GetComponent<PlayerSelfieController>();
    }

    private void Update()
    {
        if (Input.touchCount<=1)
        {
            controller.allowRotate = true;
        }
        else
        {
            controller.allowRotate = false;
        }
    }

}
