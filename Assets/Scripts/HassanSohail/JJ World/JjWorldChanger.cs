using BestHTTP.SocketIO.Events;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class JjWorldChanger : MonoBehaviour
{
    public string WorldName;
    public bool isPMYWorld;
    public int WorldID=0;
    [SerializeField] bool HaveMultipleSpwanPoint;
    [SerializeField] JJMussuemEntry mussuemEntry;
    [Header("Xana Musuem")]
    public bool isMusuem;
    public int testNet;
    public int MainNet;
    [Header("Builder")]
    public bool isBuilderWorld;

    Collider collider;

    bool reSetCollider = false;

    private GameObject triggerObject;
    public bool isShowPopup=true;
    public bool isEnteringPopup;
    public UnityEvent performAction;

    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerObject = other.gameObject;
        if (triggerObject.CompareTag("PhotonLocalPlayer") && triggerObject.GetComponent<PhotonView>().IsMine)
        {
            if (WorldName.Contains("CommingSoon"))
            {
                SNSNotificationManager.Instance.ShowNotificationMsg("Coming Soon");
                return;
            }

            CanvasButtonsHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = false;
            if (ReferrencesForDynamicMuseum.instance.m_34player)
            {
                ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.PortalSound);
            }
            triggerObject = other.gameObject;
            if (isShowPopup)
            {
                if (isPMYWorld)
                {
                    if (WorldID == 1)
                    {
                        if (APIBaseUrlChange.instance.IsXanaLive)
                            XanaConstants.xanaConstants.pmy_classRoomID_Main = 8;
                        else
                            XanaConstants.xanaConstants.pmy_classRoomID_Test = 8;
                    }
                    else if(WorldID == 2)
                    {
                        if (APIBaseUrlChange.instance.IsXanaLive)
                            XanaConstants.xanaConstants.pmy_classRoomID_Main = 9;
                        else
                            XanaConstants.xanaConstants.pmy_classRoomID_Test = 9;
                    }
                    else if (WorldID == 3)
                    {
                        if (APIBaseUrlChange.instance.IsXanaLive)
                            XanaConstants.xanaConstants.pmy_classRoomID_Main = 10;
                        else
                            XanaConstants.xanaConstants.pmy_classRoomID_Test = 10;
                    }
                    else if (WorldID == 4)
                    {
                        if (APIBaseUrlChange.instance.IsXanaLive)
                            XanaConstants.xanaConstants.pmy_classRoomID_Main = 11;
                        else
                            XanaConstants.xanaConstants.pmy_classRoomID_Test = 11;
                    }
                    else if (WorldID == 5)
                    {
                        if (APIBaseUrlChange.instance.IsXanaLive)
                            XanaConstants.xanaConstants.pmy_classRoomID_Main = 12;
                        else
                            XanaConstants.xanaConstants.pmy_classRoomID_Test = 12;
                    }
                    else if (WorldID == 6)
                    {
                        if (APIBaseUrlChange.instance.IsXanaLive)
                            XanaConstants.xanaConstants.pmy_classRoomID_Main = 13;
                        else
                            XanaConstants.xanaConstants.pmy_classRoomID_Test = 13;
                    }
                }
                if (isEnteringPopup)
                    CanvasButtonsHandler.inst.EnableJJPortalPopup(this.gameObject, 0);
                else
                    CanvasButtonsHandler.inst.EnableJJPortalPopup(this.gameObject, 1);
            }
            else
            {
                RedirectToWorld();
            }
        }

    }
    public void RedirectToWorld()
    {
        if (triggerObject.CompareTag("PhotonLocalPlayer") && triggerObject.GetComponent<PhotonView>().IsMine)
        {
            //Gautam Commented below code due to shows pop up again and again.

            //collider.enabled = false;
            if (checkWorldComingSoon(WorldName) || isBuilderWorld)
            {

                performAction?.Invoke();
                this.StartCoroutine(swtichScene(WorldName));
            }
            //else
            //{
            //    this.StartCoroutine(ResetColider());
            //}
        }
    }

    IEnumerator ResetColider()
    {
        yield return new WaitForSeconds(1f);
        collider.enabled = true;
    }


    /// <summary>
    /// To Swtich Scene with JJ world Loading
    /// </summary>
    private IEnumerator swtichScene(string worldName)
    {
        if (worldName.Contains(" : "))
        {
            string name = worldName.Replace(" : ", string.Empty);
            worldName = name;
        }

        if (XanaConstants.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
            XanaConstants.xanaConstants.isFromXanaLobby = true;
        else if (XanaConstants.xanaConstants.EnviornmentName.Contains("PMY ACADEMY"))
            XanaConstants.xanaConstants.isFromPMYLobby = true;

            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));

        // LoadingHandler.Instance.UpdateLoadingSliderForJJ(Random.Range(0.1f, 0.19f), 1f, false);

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

        if (isMusuem)
        {
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
        else if (isBuilderWorld)
        {
            XanaConstants.xanaConstants.isBuilderScene = true;
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
            if (HaveMultipleSpwanPoint)
            {
                XanaConstants.xanaConstants.mussuemEntry = mussuemEntry;
            }
            else
            {
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
            Debug.Log("Upgrade Premium feature = false");
            return false;
        }
        else
        {
            Debug.Log("Upgrade Premium feature = true");
            return true;
        }
    }
}
