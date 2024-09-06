using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLookAt : MonoBehaviour
{
    private GameObject mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = ReferencesForGamePlay.instance.randerCamera.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera != null)
        {
            if (this.gameObject.activeSelf)
            {
                this.transform.LookAt(mainCamera.transform);
                this.transform.Rotate(0, 180, 0);
            }
        }
    }
}
