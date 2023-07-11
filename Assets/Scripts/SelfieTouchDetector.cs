using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfieTouchDetector : MonoBehaviour
{
    SelfieController controller;

    private void Start()
    {
        controller = GetComponent<SelfieController>();
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
