using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StreamingCamera : MonoBehaviour
{
    [SerializeField]
    List<Camera> Cameras;
   static public StreamingCamera instance;
    List<int> avatarCount = new List<int>();
    private void Awake()
    {
        if (instance == null)
	    {
		    instance = this;
	    }
	    else
	    {
		    Destroy(gameObject);
		    return;
	    } 
    }

    void Start()
    {
        turnOffCameras();
        //yield return new WaitForSeconds(2);
    }


    /// <summary>
    /// To turn of all streaming cameras 
    /// </summary>
    void turnOffCameras(){
       
        foreach (var item in Cameras)
        {
            item.gameObject.SetActive(false);
        }    
    }

    public void TriggerStreamCam(){ 
        if (ConstantsHolder.xanaConstants.isCameraMan)
        {
           
            if (Cameras.Count>0)
            {
                StartCoroutine(checkCameras());
            }
            else // there is no any streaming camera in scene so back to main menu
            {
                  GameplayEntityLoader.instance._uiReferences.LoadMain(false);
            }
        }    
    }

    /// <summary>
    /// To check how much player is in cameras
    /// </summary>
    IEnumerator checkCameras(){
       
        avatarCount.Clear();
        int visibleCount;
        foreach (var cam in Cameras)
        { 
            visibleCount=0;
            cam.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            foreach (var avatar in MutiplayerController.instance.playerobjects)
            {
                if (!avatar.GetComponent<PhotonView>().IsMine)
                {
                   // print("!! DETECTING AVATAR "+avatar.name +"in cam "+cam.name );
                    if (avatar.GetComponent<CharacterBodyParts>().body.isVisible)
                    {
                       // print("~~~~~~ AVATAR "+avatar.name +"is visible in cam "+cam.name );
                        visibleCount++;
                    }
                }
            }
            cam.gameObject.SetActive(false);
            avatarCount.Add(visibleCount);
        }
       int crowdedCamIndex=  avatarCount.IndexOf(avatarCount.Max());
       Cameras[crowdedCamIndex].gameObject.SetActive(true);
      // LoadingHandler.Instance.HideLoading();
       ReferencesForGamePlay.instance.workingCanvas.SetActive(false);
       ReferencesForGamePlay.instance.m_34player.GetComponent<CharacterBodyParts>().HidePlayer();
       LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
       GameplayEntityLoader.instance.StartCoroutine(GameplayEntityLoader.instance.BackToMainmenuforAutoSwtiching());
    }

   
}
