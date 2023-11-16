using Photon.Pun;
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
    private void Start()
    {
        localTrans = GetComponent<Transform>();
        mainCam = LoadFromFile.instance.PlayerCamera.transform;
        thirdPersonCam = mainCam;
        firstPersonCam = LoadFromFile.instance.firstPersonCamera.transform;
    }

    private void Update()
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
                    GameObject[] objects = Photon.Pun.Demo.PunBasics.Launcher.instance.playerobjects.ToArray();

                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (!objects[i].GetComponent<PhotonView>().IsMine)
                        {
                            objects[i].gameObject.GetComponent<ArrowManager>().PhotonUserName.gameObject
                                .GetComponent<FaceCameraUI>().selfieCam = selfieCam;
                            objects[i].gameObject.GetComponent<ArrowManager>().PhotonUserName.gameObject
                                .GetComponent<FaceCameraUI>().selfieCamOther = selfieCamOther;
                        }
                    }
                }
                localTrans.LookAt(2 * localTrans.position - selfieCam.position);
                localTrans.LookAt(2 * localTrans.position - selfieCamOther.position);
            }
        }
    }
}
