using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLoader : MonoBehaviour
{
    public float speed;
    public bool isRotate = false;

    void OnDisable()
    {
        isRotate = false;
        Debug.Log("PrintOnDisable: script was disabled");
    }

    void OnEnable()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        Debug.Log("PrintOnEnable: script was enabled");
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotate)
        {
            transform.Rotate(new Vector3(0, 0, -10 * speed * Time.deltaTime));
        }
    }
}