using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMeRandom : MonoBehaviour
{
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 50), "Rotate!"))
        {
            transform.rotation = Random.rotation;
        }
    }
}
