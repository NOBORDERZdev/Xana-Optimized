using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiTextCamRot : MonoBehaviour
{
     private Transform localTrans;
     private Transform mainCam;
     private Transform selfieCam;
     private Transform thirdPersonCam;
     private Transform firstPersonCam;
     public bool oneTimeCall = false;
     public Transform selfieCamOther;

    void Start()
    {
        localTrans = GetComponent<Transform>();
        mainCam = GameplayEntityLoader.instance.PlayerCamera.transform;
        thirdPersonCam = mainCam;
        firstPersonCam = GameplayEntityLoader.instance.firstPersonCamera.transform;
    }

    void Update()
    {
         if (firstPersonCam.gameObject.activeInHierarchy && mainCam!=firstPersonCam) 
            mainCam = firstPersonCam;
        else if(mainCam!=thirdPersonCam && !firstPersonCam.gameObject.activeInHierarchy)
            mainCam = thirdPersonCam;

         if (mainCam.gameObject.activeInHierarchy){
            localTrans.LookAt(2 * localTrans.position - mainCam.position);
            oneTimeCall = true;
         }
        else
        {
            if (oneTimeCall)
            {
                if (selfieCam== null)
                {
                    selfieCam = ReferencesForGamePlay.instance.m_34player.GetComponent<MyBeachSelfieCam>().Selfie.transform;
                }
                 localTrans.LookAt(2 * localTrans.position - selfieCam.position);
                //GameObject[] objects = Photon.Pun.Demo.PunBasics.MutiplayerController.instance.playerobjects.ToArray();

                //for (int i = 0; i < objects.Length; i++)
                //{
                //    if (!objects[i].GetComponent<PhotonView>().IsMine)
                //    {
                //        objects[i].gameObject.GetComponent<ArrowManager>().PhotonUserName.gameObject.GetComponent<FaceCameraUI>().selfieCamOther = selfieCamOther;
                //    }
                //}

                //localTrans.LookAt(2 * localTrans.position - selfieCamOther.position);
            }
        }
    }
}
