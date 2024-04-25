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
    public bool isMusuem;
    public int testNet;
    public int MainNet;
    [Header("Builder")]
    public bool isBuilderWorld;

    Collider collider;

    bool reSetCollider = false;

    private GameObject triggerObject;
    public bool isEnteringPopup;
    public GameObject portalObject;
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
                if (ConstantsHolder.xanaConstants.toyotaMeetingStatus.Equals(ConstantsHolder.MeetingStatus.HouseFull))
                    return;
                else if (!ConstantsHolder.xanaConstants.toyotaEmail.Contains("ktoyota@yopmail.com") &&
                    ConstantsHolder.xanaConstants.toyotaMeetingStatus.Equals(ConstantsHolder.MeetingStatus.Inprogress))
                    return;
                else if (ConstantsHolder.xanaConstants.toyotaEmail.Contains("ktoyota@yopmail.com") &&
                    ConstantsHolder.xanaConstants.toyotaMeetingStatus.Equals(ConstantsHolder.MeetingStatus.End))
                    return;
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
    public void RedirectToWorld()
    {
        if (triggerObject.CompareTag("PhotonLocalPlayer") && triggerObject.GetComponent<PhotonView>().IsMine)
        {
            if (checkWorldComingSoon(WorldName) || isBuilderWorld)
            {
                this.StartCoroutine(swtichScene(WorldName));

                // For toyota bussiness meeting world only
                if (ConstantsHolder.xanaConstants.toyotaMeetingStatus.Equals(ConstantsHolder.MeetingStatus.End))
                {// for customer
                    triggerObject.GetComponent<ArrowManager>().UpdateMeetingPrams(ConstantsHolder.MeetingStatus.Inprogress);
                    GetComponent<ThaMeetingTxtUpdate>().UpdateMeetingTxt("Waiting For Interviewer");
                }
                else if (ConstantsHolder.xanaConstants.toyotaMeetingStatus.Equals(ConstantsHolder.MeetingStatus.Inprogress))
                { // for interviewer
                    triggerObject.GetComponent<ArrowManager>().UpdateMeetingPrams(ConstantsHolder.MeetingStatus.HouseFull);
                    GetComponent<ThaMeetingTxtUpdate>().UpdateMeetingTxt("Meeting is starting", Color.red);
                }
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
