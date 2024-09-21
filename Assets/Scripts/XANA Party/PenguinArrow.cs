using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinArrow : MonoBehaviour
{
     [SerializeField] GameObject arrow;

    private void OnEnable()
    {
        if (!GetComponentInParent<PhotonView>().IsMine)
        {
            arrow.SetActive(false);
        }
    }

    void Update()
    {
        // Check if there is a main camera in the scene
        if (Camera.main != null)
        {
            // Make the object face the camera
            transform.LookAt(Camera.main.transform);
            
            // Optional: Make the object only rotate around the Y axis
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}
