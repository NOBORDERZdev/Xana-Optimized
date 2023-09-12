using Metaverse;
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

    List<int> avatarCount = new List<int>();
    IEnumerator Start()
    {
        turnOffCameras();
        yield return new WaitForSeconds(3);
        
    }

    public void ButtonCall(){ 
        if (XanaConstants.xanaConstants.isCameraMan)
        {
          Cameras[checkCameras()].gameObject.SetActive(true);
        }
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


    /// <summary>
    /// To check how much player is in cameras
    /// </summary>
    int checkCameras(){
       
        avatarCount.Clear();
        int visibleCount;
        foreach (var cam in Cameras)
        { 
            visibleCount=0;
            cam.gameObject.SetActive(true);
            foreach (var avatar in Launcher.instance.playerobjects)
            {
                if (!avatar.GetComponent<PhotonView>().IsMine)
                {
                    print("!! DETECTING AVATAR "+avatar.name +"in cam "+cam.name );
                    if (avatar.GetComponent<AvatarController>().isVisibleOnCam)
                    {
                        print("~~~~~~ AVATAR "+avatar.name +"is visible in cam "+cam.name );
                        visibleCount++;
                    }
                }
            }
            cam.gameObject.SetActive(false);
            avatarCount.Add(visibleCount);
        }
       int crowdedCamIndex=  avatarCount.IndexOf(avatarCount.Max());
        return crowdedCamIndex;
    }

    //int CountInstanceInCam()
    //{
    //    instance.Clear();

    //    //for (int i = 0; i < length; i++)
    //    //{

    //    //}
    //}

}
