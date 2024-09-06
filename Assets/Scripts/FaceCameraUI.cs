using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraUI : MonoBehaviour
{
    public static Transform selficamOtherAssign;
    private Transform mainCam;
    private Transform thirdPersonCam;
    private Transform firstPersonCam;
    public GameObject selfieStick;
    public Transform selfieCam;
    public Transform selfieCamOther;

    private Transform localTrans;
    public bool oneTimeCall = false;
    // Start is called before the first frame update
    void Start()
    {
        localTrans = GetComponent<Transform>();
        mainCam = GameplayEntityLoader.instance.PlayerCamera.transform;
        thirdPersonCam = mainCam;
        firstPersonCam = GameplayEntityLoader.instance.firstPersonCamera.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (firstPersonCam.gameObject.activeInHierarchy && mainCam!=firstPersonCam) 
            mainCam = firstPersonCam;
        else if(mainCam!=thirdPersonCam && !firstPersonCam.gameObject.activeInHierarchy)
            mainCam = thirdPersonCam;
        

        if (mainCam.gameObject.activeInHierarchy)
        {
            localTrans.LookAt(2 * localTrans.position - mainCam.position);
            oneTimeCall = true;
        }
        else
        {
            if (selfieStick.activeInHierarchy)
            {
                if (oneTimeCall)
                {
                    selficamOtherAssign = selfieCamOther;
                    oneTimeCall = false;
                    //GameObject[] objects = GameObject.FindGameObjectsWithTag("PhotonLocalPlayer");
                    GameObject[] objects = Photon.Pun.Demo.PunBasics.MutiplayerController.instance.playerobjects.ToArray();

                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (!objects[i].GetComponent<PhotonView>().IsMine)
                        {
                            objects[i].gameObject.GetComponent<ArrowManager>().PhotonUserName.gameObject.GetComponent<FaceCameraUI>().selfieCam = selfieCam;
                            objects[i].gameObject.GetComponent<ArrowManager>().PhotonUserName.gameObject.GetComponent<FaceCameraUI>().selfieCamOther = selfieCamOther;
                        }
                    }
                }
                localTrans.LookAt(2 * localTrans.position - selfieCam.position);
                localTrans.LookAt(2 * localTrans.position - selfieCamOther.position);
            }
        }
    }
}
