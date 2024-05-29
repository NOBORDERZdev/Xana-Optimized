using Photon.Pun;
using System.Collections;
using UnityEngine;

public class JjWorldChanger : MonoBehaviour
{
    public string WorldName;
    [SerializeField] bool HaveMultipleSpwanPoint;
    [SerializeField] JJMussuemEntry mussuemEntry;
    [Header("Xana Musuem")]
    public bool isMusuem;
    public int testNet;
    public int MainNet;
    [Header("Builder")]
    public bool isBuilderWorld;

    Collider collider;
    private GameObject triggerObject;
    public bool isEnteringPopup;
  
    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {

        triggerObject = other.gameObject;
        if (triggerObject.CompareTag("PhotonLocalPlayer") && triggerObject.GetComponent<PhotonView>().IsMine)
        {
            // For toyota bussiness meeting world only
            if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("D_Infinity_Labo") && ConstantsHolder.xanaConstants)
            {
                triggerObject.GetComponent<ArrowManager>().ChangeVoiceGroup(triggerObject.GetComponent<PhotonView>().ViewID,
                    0);

                if (NFT_Holder_Manager.instance.meetingStatus.tms.Equals(ThaMeetingStatusUpdate.MeetingStatus.HouseFull))
                    return;
                if(FB_Notification_Initilizer.Instance.actorType != FB_Notification_Initilizer.ActorType.CompanyUser &&
                       NFT_Holder_Manager.instance.meetingStatus.tms.Equals(ThaMeetingStatusUpdate.MeetingStatus.Inprogress))
                {
                    return;
                }
                else if (FB_Notification_Initilizer.Instance.actorType == FB_Notification_Initilizer.ActorType.CompanyUser &&
                                       NFT_Holder_Manager.instance.meetingStatus.tms.Equals(ThaMeetingStatusUpdate.MeetingStatus.End))
                {
                    return;
                }
            }

            GamePlayUIHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = false;
            if (ReferencesForGamePlay.instance.m_34player)
            {
                ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.PortalSound);
            }
            triggerObject = other.gameObject;
            if (isEnteringPopup)
                GamePlayUIHandler.inst.EnableJJPortalPopup(this.gameObject, 0);
            else
                GamePlayUIHandler.inst.EnableJJPortalPopup(this.gameObject, 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        triggerObject.GetComponent<ArrowManager>().ChangeVoiceGroup(triggerObject.GetComponent<PhotonView>().ViewID,
                    1);
    }

    public void RedirectToWorld()
    {
        if (triggerObject.CompareTag("PhotonLocalPlayer") && triggerObject.GetComponent<PhotonView>().IsMine)
        {
            if (checkWorldComingSoon(WorldName) || isBuilderWorld)
            {

                // For toyota bussiness meeting world only
                if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("D_Infinity_Labo"))
                {
                    if (NFT_Holder_Manager.instance.meetingStatus.tms.Equals(ThaMeetingStatusUpdate.MeetingStatus.End))
                    {// for customer
                        NFT_Holder_Manager.instance.meetingStatus.UpdateMeetingParams((int)ThaMeetingStatusUpdate.MeetingStatus.Inprogress);
                        triggerObject.GetComponent<ArrowManager>().UpdateMeetingTxt("Waiting For Interviewer");
                    }
                    else if (NFT_Holder_Manager.instance.meetingStatus.tms.Equals(ThaMeetingStatusUpdate.MeetingStatus.Inprogress))
                    { // for interviewer
                        NFT_Holder_Manager.instance.meetingStatus.UpdateMeetingParams((int)ThaMeetingStatusUpdate.MeetingStatus.HouseFull);
                        triggerObject.GetComponent<ArrowManager>().UpdateMeetingTxt("Meeting Is In Progress");
                    }

                    ConstantsHolder.xanaConstants.isBackToParentScane = true;
                }

                this.StartCoroutine(swtichScene(WorldName));
            }
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

        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            ConstantsHolder.xanaConstants.isFromXanaLobby = true;
        }

        // LoadingHandler.Instance.UpdateLoadingSliderForJJ(Random.Range(0.1f, 0.19f), 1f, false);
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        if (!ConstantsHolder.xanaConstants.JjWorldSceneChange && !ConstantsHolder.xanaConstants.orientationchanged)
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        //ConstantsHolder.xanaConstants.EnviornmentName = worldName;
        //FeedEventPrefab.m_EnvName = worldName;
        //MutiplayerController.sceneName = worldName;

        // Added by WaqasAhmad
        // For Live User Count
        if (APIBasepointManager.instance.IsXanaLive)
        {
            ConstantsHolder.xanaConstants.customWorldId = MainNet;
            ConstantsHolder.xanaConstants.MuseumID = MainNet.ToString();
        }
        else
        {
            ConstantsHolder.xanaConstants.customWorldId = testNet;
            ConstantsHolder.xanaConstants.MuseumID = testNet.ToString();
        }
        //

        if (isMusuem)
        {
            ConstantsHolder.xanaConstants.IsMuseum = true;
            //if (APIBasepointManager.instance.IsXanaLive)
            //{
            //    ConstantsHolder.xanaConstants.MuseumID = MainNet.ToString();
            //}
            //else
            //{
            //    ConstantsHolder.xanaConstants.MuseumID = testNet.ToString();
            //}
        }
        else if (isBuilderWorld)
        {
            ConstantsHolder.xanaConstants.isBuilderScene = true;
            if (APIBasepointManager.instance.IsXanaLive)
            {
                ConstantsHolder.xanaConstants.builderMapID = MainNet;
            }
            else
            {
                ConstantsHolder.xanaConstants.builderMapID = testNet;
            }
        }
        else // FOR JJ WORLD
        {
            if (HaveMultipleSpwanPoint)
            {
                ConstantsHolder.xanaConstants.mussuemEntry = mussuemEntry;
            }
            else
            {
                ConstantsHolder.xanaConstants.mussuemEntry = JJMussuemEntry.Null;
            }
        }
        yield return new WaitForSeconds(1f);
        ConstantsHolder.xanaConstants.JjWorldSceneChange = true;
        ConstantsHolder.xanaConstants.JjWorldTeleportSceneName = worldName;
        GameplayEntityLoader.instance._uiReferences.LoadMain(false);


    }


    private bool checkWorldComingSoon(string worldName)
    {
        if (!UserPassManager.Instance.CheckSpecificItem(worldName, true))
        {

            return false;
        }
        else
        {
            return true;
        }
    }
}
