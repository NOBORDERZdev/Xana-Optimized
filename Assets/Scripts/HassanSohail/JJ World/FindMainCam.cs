using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMainCam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
