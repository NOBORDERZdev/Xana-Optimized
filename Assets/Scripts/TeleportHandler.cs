using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraTransition
{
    public GameObject Des_Cam;
    public GameObject main_Cam;
    public GameObject last_activeCam;
    public Transform LookAtTransfrom;
    public Vector3 Eu_angles;

    public void CamActive(bool n)
    {
        Des_Cam.SetActive(n);
        main_Cam.SetActive(!n);
        if (last_activeCam != null)
        {
            last_activeCam.SetActive(false);
        }
    }
}


public class TeleportHandler : MonoBehaviour
{
    public Vector3 TelePos;

    public bool Check1, check2;

    public GameObject testcine, Cam2;

    public CameraTransition[] cameraTransitionobj;

    public GameObject Player;

    public AudioClip teleportSFX;

    public int indexC;
    public Vector3 EV3;



    


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TeleportO2L"))
        {
           
            indexC = 0;
            cameraTransitionobj[0].CamActive(true);
            EV3 = new Vector3(cameraTransitionobj[indexC].Eu_angles.x, cameraTransitionobj[indexC].Eu_angles.y, cameraTransitionobj[indexC].Eu_angles.z);

            transform.localEulerAngles = EV3;

            //CanvusHandler.canvusHandlerInstance._VcamClear.gameObject.SetActive(true);

            TelePos = Gamemanager._InstanceGM.teleportPoints[1].transform.position;
            transform.GetComponent<PlayerController>().enabled = false;
            transform.position = new Vector3(TelePos.x, TelePos.y, TelePos.z);
            
            Gamemanager._InstanceGM.env_Subparts[0].SetActive(false);
            Gamemanager._InstanceGM.env_Subparts[1].SetActive(true);
            Gamemanager._InstanceGM.minimapParent.SetActive(false);


            Invoke("Test", 0.5f);
           

            Gamemanager._InstanceGM.UnMute(true);

        }
        else if (other.CompareTag("TeleportL2O"))
        {
            cameraTransitionobj[1].CamActive(true);
            indexC = 1;
            EV3 = new Vector3(cameraTransitionobj[indexC].Eu_angles.x, cameraTransitionobj[indexC].Eu_angles.y, cameraTransitionobj[indexC].Eu_angles.z);
            transform.localEulerAngles = EV3;
            
            TelePos = Gamemanager._InstanceGM.teleportPoints[0].transform.position;
            transform.GetComponent<PlayerController>().enabled = false;
            transform.position = new Vector3(TelePos.x, TelePos.y, TelePos.z);
            //DialogueManager.Instance.AndroidNonXRcanvas.gameObject.SetActive(false);
            Gamemanager._InstanceGM.env_Subparts[0].SetActive(true);
            Gamemanager._InstanceGM.env_Subparts[1].SetActive(false);
            Gamemanager._InstanceGM.minimapParent.SetActive(true);
            Gamemanager._InstanceGM.count = 0;
            Invoke("Test", 0.5f);

            Gamemanager._InstanceGM.UnMute(true);


        }


        else if (other.CompareTag("reseption"))
        {
            
        }
        else if (other.CompareTag("TeleportL2I"))
        {
            cameraTransitionobj[2].CamActive(true);
            indexC = 2;
            EV3 = new Vector3(cameraTransitionobj[indexC].Eu_angles.x, cameraTransitionobj[indexC].Eu_angles.y, cameraTransitionobj[indexC].Eu_angles.z);
            transform.localEulerAngles = EV3;
            
            TelePos = Gamemanager._InstanceGM.teleportPoints[2].transform.position;
            transform.GetComponent<PlayerController>().enabled = false;
            transform.position = new Vector3(TelePos.x, TelePos.y, TelePos.z);
            Gamemanager._InstanceGM.UnMute(true);
            Gamemanager._InstanceGM.env_Subparts[2].SetActive(true);
            Gamemanager._InstanceGM.env_Subparts[1].SetActive(false);
            Gamemanager._InstanceGM.minimapParent.SetActive(true);

            Gamemanager._InstanceGM.mediaPlayer.AudioVolume = 0;
            Gamemanager._InstanceGM.mediaPlayer.AudioMuted = false;

            //SoundController.Instance.MusicSource.volume = 0.2f;
            SoundController.Instance.MusicSource.mute = false;

            Invoke("Test", 0.5f);

        }
        else if (other.CompareTag("TeleportI2L"))
        {
            Gamemanager._InstanceGM.UnMute(false);

            cameraTransitionobj[3].CamActive(true);
            indexC = 3;
            EV3 = new Vector3(cameraTransitionobj[indexC].Eu_angles.x, cameraTransitionobj[indexC].Eu_angles.y, cameraTransitionobj[indexC].Eu_angles.z);
            transform.localEulerAngles = EV3;
            
            TelePos = Gamemanager._InstanceGM.teleportPoints[1].transform.position;
            transform.GetComponent<PlayerController>().enabled = false;
            transform.position = new Vector3(TelePos.x, TelePos.y, TelePos.z);

            Gamemanager._InstanceGM.env_Subparts[1].SetActive(true);
            Gamemanager._InstanceGM.env_Subparts[2].SetActive(false);
            Gamemanager._InstanceGM.minimapParent.SetActive(false);

            Gamemanager._InstanceGM.UnMute(true);

            Invoke("Test", 0.5f);

        }
        else if (other.CompareTag("TeleportSFX2L"))
        {
            cameraTransitionobj[6].CamActive(true);
            indexC = 6;
            EV3 = new Vector3(cameraTransitionobj[indexC].Eu_angles.x, cameraTransitionobj[indexC].Eu_angles.y, cameraTransitionobj[indexC].Eu_angles.z);
            transform.localEulerAngles = EV3;
            
            TelePos = Gamemanager._InstanceGM.teleportPoints[5].transform.position;
            transform.GetComponent<PlayerController>().enabled = false;
            transform.position = new Vector3(TelePos.x, TelePos.y, TelePos.z);

            Gamemanager._InstanceGM.env_Subparts[2].SetActive(true);
            Gamemanager._InstanceGM.env_Subparts[1].SetActive(false);
            Gamemanager._InstanceGM.minimapParent.SetActive(true);
            
            Gamemanager._InstanceGM.UnMute(false);
            SoundController.Instance.videoPlayerSource.gameObject.SetActive(false);

            Invoke("Test", 0.5f);

        }

        else if (other.CompareTag("TeleportI2PO"))
        {
            cameraTransitionobj[4].CamActive(true);
            
            indexC = 4;
            EV3 = new Vector3(cameraTransitionobj[indexC].Eu_angles.x, cameraTransitionobj[indexC].Eu_angles.y, cameraTransitionobj[indexC].Eu_angles.z);
            transform.localEulerAngles = EV3;
            
            TelePos = Gamemanager._InstanceGM.teleportPoints[3].transform.position;
            transform.GetComponent<PlayerController>().enabled = false;
            transform.position = new Vector3(TelePos.x, TelePos.y, TelePos.z);

            Gamemanager._InstanceGM.env_Subparts[3].SetActive(true);
            Gamemanager._InstanceGM.minimapParent.SetActive(false);
            Gamemanager._InstanceGM.env_Subparts[2].SetActive(false);
            Gamemanager._InstanceGM.UnMute(true);

            SoundController.Instance.videoPlayerSource.gameObject.SetActive(false);
            Invoke("Test", 0.5f);
        }

        else if (other.CompareTag("TeleportPO2I"))
        {
            Gamemanager._InstanceGM.UnMute(true);

            cameraTransitionobj[5].CamActive(true);

            TelePos = Gamemanager._InstanceGM.teleportPoints[4].transform.position;
            transform.GetComponent<PlayerController>().enabled = false;
            transform.position = new Vector3(TelePos.x, TelePos.y, TelePos.z);

            Gamemanager._InstanceGM.env_Subparts[2].SetActive(true);
            Gamemanager._InstanceGM.env_Subparts[3].SetActive(false);
            Gamemanager._InstanceGM.minimapParent.SetActive(true);
            //SoundController.Instance.MusicSource.volume = 0.2f;

            Invoke("Test", 0.5f);
        }
        else if (other.CompareTag("ExitCollider"))
        {
            TopMenuButtonController.Instance.SetPress(3);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TeleportO2L"))
        {
            check2 = false;
        }
        if (other.CompareTag("reseption"))
        {
            Gamemanager._InstanceGM.animationController.StatBow(false);
            SoundController.Instance.EffectsSource.Stop();
            CanvusHandler.canvusHandlerInstance.andoridCanvus.SetActive(true);

        }
    }

    void Test()
    {
        Gamemanager._InstanceGM.mainPlayer.GetComponent<PlayerController>().enabled = true;
        transform.GetComponent<PlayerController>().enabled = true;
    }

    void HeadSet()
    {
        Gamemanager._InstanceGM.animationController.StatBow(false);
        Gamemanager._InstanceGM.animationController.HeadSet(true);
        Gamemanager._InstanceGM.animationController.headSetInReseptionistHand.SetActive(true);
    }

    void check()
    {
        Debug.Log("DetachChildren now");
        Gamemanager._InstanceGM.animationController.DC();
    }

    public void StopPlay()
    {
        Gamemanager._InstanceGM.animationController.PlayerAnimation(false);
    }

    public void DCOnHead()
    {

    }
}
