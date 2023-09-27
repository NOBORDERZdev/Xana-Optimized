using Metaverse;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
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
        if (XanaConstants.xanaConstants.isCameraMan)
        {
           
            if (Cameras.Count>0)
            {
                StartCoroutine(checkCameras());
            }
            else // there is no any streaming camera in scene so back to main menu
            {
                  LoadFromFile.instance._uiReferences.LoadMain(false);
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
            yield return new WaitForSeconds(1f);
            foreach (var avatar in Launcher.instance.playerobjects)
            {
                if (!avatar.GetComponent<PhotonView>().IsMine)
                {
                    // print("!! DETECTING AVATAR "+avatar.name +"in cam "+cam.name );
                    if (avatar.GetComponent<CharcterBodyParts>().Body.GetComponent<SkinnedMeshRenderer>().isVisible)
                    {
                        // print("~~~~~~ AVATAR "+avatar.name +"is visible in cam "+cam.name );
                        visibleCount++;
                    }
                }
            }
            cam.gameObject.SetActive(false);
            avatarCount.Add(visibleCount);
        }
        List<StreamCam> camSortList = new List<StreamCam>();
       // List<int>avatarCountSort = avatarCount.OrderBy(x); 
        for (int i = 0; i < Cameras.Count; i++)
        {
            camSortList.Add(new StreamCam(avatarCount[i] , Cameras[i].gameObject));
        }

        List<StreamCam> temp = new List<StreamCam>(camSortList.OrderBy(x =>x.avatarCount).ToList());
       yield return StartCoroutine(EnableCam(temp));
           
        
    }
       
     IEnumerator EnableCam(List<StreamCam> list){
        int index =0;
        while (true)
        {
            turnOffCameras();
            bool canCamOn = false;
            if (list[index].avatarCount>0)
            {
                canCamOn=true;
            }
            else
            {
                canCamOn =false;
            }

            if (canCamOn){
                list[index].cam.SetActive(true);
                if (!list[index].cam.GetComponent<StreamingCameraPaining>())
                {
                    list[index].cam.AddComponent<StreamingCameraPaining>();
                }
                //if (!list[index].cam.GetComponent<CinemachineCollider>())
                //{
                //    list[index].cam.AddComponent<CinemachineCollider>();
                //}
                //if (!list[index].cam.GetComponent<CinemachineFreeLook>())
                //{
                //    list[index].cam.AddComponent<CinemachineFreeLook>();
                //}
                //// Random player look;
                //list[index].cam.GetComponent<CinemachineFreeLook>().LookAt = ReferrencesForDynamicMuseum.instance.m_34player.transform;
                //list[index].cam.GetComponent<CinemachineFreeLook>().Follow = ReferrencesForDynamicMuseum.instance.m_34player.transform;
                if (list[index].cam.GetComponent<StreamingCameraPaining>().lookObj == null)
                {
                    if (list[index].cam.GetComponent<StreamingCameraPaining>().focusOnScreen)
                    {
                        if (XanaConstants.xanaConstants.EnviornmentName== "Xana Festival")
                        {
                            GameObject screen = GameObject.Find("XanaFestivalPlayer(Clone)");
                            if (screen.GetComponent<YoutubeStreamController>().LiveStreamPlayer.activeInHierarchy)
                            {
                                list[index].cam.GetComponent<StreamingCameraPaining>().lookObj = screen.GetComponent<YoutubeStreamController>().LiveStreamPlayer;
                            }
                            else
                            {
                                list[index].cam.GetComponent<StreamingCameraPaining>().lookObj = screen.GetComponent<YoutubeStreamController>().NormalPlayer;

                            }
                            }
                            else if (XanaConstants.xanaConstants.EnviornmentName== "BreakingDown Arena")
                            {

                            }
                            else
                            {
                                list[index].cam.GetComponent<StreamingCameraPaining>().lookObj=ReferrencesForDynamicMuseum.instance.m_34player ;
                            }
                        }
                        else
                        {
                            list[index].cam.GetComponent<StreamingCameraPaining>().lookObj=ReferrencesForDynamicMuseum.instance.m_34player ;
                        }
                }
                //else
                //{
                //        list[index].cam.GetComponent<StreamingCameraPaining>().lookObj=ReferrencesForDynamicMuseum.instance.m_34player ;
                //}

                if (XanaConstants.xanaConstants.newStreamEntery)
                {
                    XanaConstants.xanaConstants.newStreamEntery =false;
                    ReferrencesForDynamicMuseum.instance.workingCanvas.SetActive(false);
                    ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<CharcterBodyParts>().HidePlayer();
                    //LoadingHandler.Instance.streamingLoading.FullFillBar();
                    LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
                    LoadFromFile.instance.StartCoroutine(LoadFromFile.instance.BackToMainmenuforAutoSwtiching());
                }
               
                if (index< list.Count-1)
                {
                    index++;
                }
                else
                {
                    index=0;
                }
                yield return new WaitForSecondsRealtime(10);
            }
            else{
                yield return null;
                break;
            }
            //int crowdedCamIndex = avatarCount.IndexOf(avatarCount.Max());
            //Cameras[crowdedCamIndex].gameObject.SetActive(true);
          
        }
    }



}


class StreamCam{ 
    public int avatarCount;
    public GameObject cam;

    public  StreamCam(int _id, GameObject _cam){ 
        avatarCount= _id;
        cam =_cam ;
    }
}
   
  
