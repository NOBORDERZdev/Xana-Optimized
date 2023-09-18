using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JjWorldChanger : MonoBehaviour
{
    public string WorldName;
    [SerializeField] bool HaveMultipleSpwanPoint;
    [SerializeField] JJMussuemEntry mussuemEntry;
    [Header("Xana Musuem")]
    [SerializeField] bool isMusuem;
    public int testNet; 
    public int MainNet;
    [Header("Builder")]
    [SerializeField] bool isBuilderWorld; 

    Collider collider;

    bool reSetCollider=false;
    private GameObject triggerObject;
    private void Start()
    {
        collider = GetComponent<Collider>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerObject = other.gameObject;
        CanvasButtonsHandler.inst.EnableJJPortalPopup(this.gameObject);
       
    }
    public void RedirectToWorld()
    {
        if (triggerObject.CompareTag("PhotonLocalPlayer") && triggerObject.GetComponent<PhotonView>().IsMine)
        {
            collider.enabled = false;
            if (checkWorldComingSoon(WorldName) || isBuilderWorld)
            {
                this.StartCoroutine(swtichScene(WorldName));
            }
            else
            {
                this.StartCoroutine(ResetColider());
            }
        }
    }
    IEnumerator ResetColider(){ 
         yield return new WaitForSeconds(1f);
        collider.enabled = true;
    }


    /// <summary>
    /// To Swtich Scene with JJ world Loading
    /// </summary>
    private IEnumerator swtichScene(string worldName)
    {
        

        if (XanaConstants.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            XanaConstants.xanaConstants.isFromXanaLobby =true;
        }
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        if (!XanaConstants.xanaConstants.JjWorldSceneChange && !XanaConstants.xanaConstants.orientationchanged)
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        //XanaConstants.xanaConstants.EnviornmentName = worldName;
        //FeedEventPrefab.m_EnvName = worldName;
        //Launcher.sceneName = worldName;

        // Added by WaqasAhmad
        // For Live User Count
        if (APIBaseUrlChange.instance.IsXanaLive)
        {
            XanaConstants.xanaConstants.customWorldId = MainNet;
        }
        else
        {
            XanaConstants.xanaConstants.customWorldId = testNet;
        }
        //

        if (isMusuem){
            XanaConstants.xanaConstants.IsMuseum = true;
            if (APIBaseUrlChange.instance.IsXanaLive)
            {
                XanaConstants.xanaConstants.MuseumID = MainNet.ToString();
            }
            else
            {
                 XanaConstants.xanaConstants.MuseumID = testNet.ToString();
            }
        }
        else if (isBuilderWorld){
            XanaConstants.xanaConstants.isBuilderScene=true;
            if (APIBaseUrlChange.instance.IsXanaLive)
            {
               XanaConstants.xanaConstants.builderMapID = MainNet;
            }
            else
            {
                  XanaConstants.xanaConstants.builderMapID = testNet;
            }
        }
        else // FOR JJ WORLD
        {
            if (HaveMultipleSpwanPoint){
                XanaConstants.xanaConstants.mussuemEntry = mussuemEntry;
            }
            else{
                XanaConstants.xanaConstants.mussuemEntry = JJMussuemEntry.Null;
            }
        }
        yield return new WaitForSeconds(1f);
        XanaConstants.xanaConstants.JjWorldSceneChange = true;
        XanaConstants.xanaConstants.JjWorldTeleportSceneName = worldName;
        LoadFromFile.instance._uiReferences.LoadMain(false);
       
       
    }


    private bool checkWorldComingSoon(string worldName)
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem(worldName, true))
        {
       
             return false;
        }
        else
        {
            return true;
        }
    }
}
