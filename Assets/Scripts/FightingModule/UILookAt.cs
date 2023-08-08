using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAt : MonoBehaviour
{
    GameObject cameraObj;

    // Start is called before the first frame update
    void Start()
    {
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + cameraObj.transform.rotation * Vector3.forward, 
            cameraObj.transform.rotation * Vector3.up);
    }
}
