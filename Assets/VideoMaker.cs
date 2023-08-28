using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using RFM;

public class VideoMaker : MonoBehaviour
{
    public GameObject[] cameras;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            SwtichCamera(0);

        if (Input.GetKeyDown(KeyCode.I))
            SwtichCamera(1);

        if (Input.GetKeyDown(KeyCode.O))
            SwtichCamera(2);

        if (Input.GetKeyDown(KeyCode.P))
            SwtichCamera(3);

        if (Input.GetKeyDown(KeyCode.J))
            SwtichCamera(4);

        if (Input.GetKeyDown(KeyCode.K))
            SwtichCamera(5);

        if (Input.GetKeyDown(KeyCode.L))
            SwtichCamera(6);

        if (Input.GetKeyDown(KeyCode.M))
        {
            cameras[2].GetComponent<CinemachineVirtualCamera>().m_LookAt = GameObject.FindGameObjectWithTag(Globals.LOCAL_PLAYER_TAG).transform;//o
            cameras[3].GetComponent<CinemachineVirtualCamera>().m_LookAt = GameObject.FindGameObjectWithTag(Globals.LOCAL_PLAYER_TAG).transform;//p
            cameras[4].GetComponent<CinemachineVirtualCamera>().m_LookAt = GameObject.FindGameObjectWithTag(Globals.LOCAL_PLAYER_TAG).transform;//j
        }

    }

    void SwtichCamera(int i) 
    {
        for (int k=0; k < cameras.Length; k++) 
        {
            cameras[k].gameObject.SetActive(false);
            if(k==i)
                cameras[k].gameObject.SetActive(true);
        }
    }


}
