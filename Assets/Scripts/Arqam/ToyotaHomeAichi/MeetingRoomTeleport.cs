using Photon.Pun;
using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.Networking;

public class MeetingRoomTeleport : MonoBehaviour
{
    [SerializeField] bool isLocked;
    [SerializeField] Transform destinationPoint;
    public enum PortalType { Enter, Exit }
    public PortalType currentPortal;
    public float Cam_XValue = -50f;

    private PlayerController ref_PlayerControllerNew;
    private ReferencesForGamePlay referrencesForDynamicMuseum;
    private GameObject triggerObject;
    private string currentRoomId;
    private string userId;
    private string _thaRoomId;
    private int _thaPageNumber = 1;
    private int _thaPageSize = 100;
    private void Start()
    {
        referrencesForDynamicMuseum = ReferencesForGamePlay.instance;
        ref_PlayerControllerNew = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>();
        currentRoomId = ConstantsHolder.xanaConstants.MuseumID;
        userId = ConstantsHolder.userId;
        //GameplayEntityLoader.instance.HomeBtn.onClick.AddListener(LeaveMeetingOnExit);
        if (APIBasepointManager.instance.IsXanaLive)
        {
            _thaRoomId = "2";
        }
        else
        {
            _thaRoomId = "4";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>() != null)
        {
            if (destinationPoint != null && other.GetComponent<PhotonView>().IsMine)
            {
                triggerObject = other.gameObject;
                CheckMeetingStatus();
            }
        }
    }

    private void CheckMeetingStatus()
    {
        if (currentPortal == PortalType.Enter)
        {
            if (NFT_Holder_Manager.instance.meetingStatus.ThaMeetingStatus.Equals(ThaMeetingStatusUpdate.MeetingStatus.HouseFull))
                return;
            if (FB_Notification_Initilizer.Instance.actorType != FB_Notification_Initilizer.ActorType.CompanyUser &&
                   NFT_Holder_Manager.instance.meetingStatus.ThaMeetingStatus.Equals(ThaMeetingStatusUpdate.MeetingStatus.Inprogress))
            {
                return;
            }
            else if (FB_Notification_Initilizer.Instance.actorType == FB_Notification_Initilizer.ActorType.CompanyUser &&
                                   NFT_Holder_Manager.instance.meetingStatus.ThaMeetingStatus.Equals(ThaMeetingStatusUpdate.MeetingStatus.End))
            {
                return;
            }
            GetUpdatedToken();
            GamePlayUIHandler.inst.EnableJJPortalPopup(this.gameObject, 2);
        }
        else if (currentPortal == PortalType.Exit)
            GamePlayUIHandler.inst.EnableJJPortalPopup(this.gameObject, 3);

        GamePlayUIHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = false;
    }

    public void RedirectToWorld()    // call on popup button click
    {
        if (triggerObject.GetComponent<PhotonView>().IsMine)
        {
            this.StartCoroutine(Teleport());
        }
    }
    IEnumerator Teleport()
    {
        if (!isLocked)
        {
            referrencesForDynamicMuseum.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = false;
            LoadingHandler.Instance.JJLoadingSlider.fillAmount = 0;
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
            StartCoroutine(LoadingHandler.Instance.IncrementSliderValue(Random.Range(2f, 3f)));

            yield return new WaitForSeconds(.4f);

            referrencesForDynamicMuseum.MainPlayerParent.transform.eulerAngles = destinationPoint.eulerAngles;
            referrencesForDynamicMuseum.MainPlayerParent.transform.position = destinationPoint.position;
            yield return new WaitForSeconds(.8f);
            referrencesForDynamicMuseum.MainPlayerParent.transform.position = destinationPoint.position;
            referrencesForDynamicMuseum.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = true;

            GameplayEntityLoader.instance.StartCoroutine(GameplayEntityLoader.instance.setPlayerCamAngle(Cam_XValue, 0.5f));
            yield return new WaitForSeconds(.15f);
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));

            if (currentPortal.Equals(PortalType.Enter))
                EnterInMeeting();
            else if (currentPortal.Equals(PortalType.Exit))
                StartCoroutine(ExitFromMeeting());
        }
        else
        {
            if (SNSNotificationHandler.Instance != null)
                SNSNotificationHandler.Instance.ShowNotificationMsg("Coming soon");
            yield return null;
        }
    }

    private void EnterInMeeting()
    {
        if (!APIBasepointManager.instance.IsXanaLive)
            ConstantsHolder.xanaConstants.MuseumID = "2399";   // meeting room testnet id
        else if (APIBasepointManager.instance.IsXanaLive)
            ConstantsHolder.xanaConstants.MuseumID = "5616";       // meeting room mainnet id
        ConstantsHolder.userId = userId + ChatSocketManager.instance.socketId;
        ChatSocketManager.onJoinRoom?.Invoke(ConstantsHolder.xanaConstants.MuseumID);
        XanaChatSystem.instance.ClearChatTxtForMeeting();

        NFT_Holder_Manager.instance.voiceManager.SetVoiceGroup(5);

        NFT_Holder_Manager.instance.meetingStatus.GetActorNum(
        triggerObject.GetComponent<PhotonView>().Controller.ActorNumber, (int)FB_Notification_Initilizer.Instance.actorType);
        if (FB_Notification_Initilizer.Instance.actorType.Equals(FB_Notification_Initilizer.ActorType.User))
        {
            NFT_Holder_Manager.instance.pushNotification.SendNotification();
        }
        int temp = FB_Notification_Initilizer.Instance.userInMeeting + 1;
        NFT_Holder_Manager.instance.meetingStatus.UpdateUserCounter(temp);
        if (NFT_Holder_Manager.instance.meetingStatus.ThaMeetingStatus.Equals(ThaMeetingStatusUpdate.MeetingStatus.End))
        {// for customer
            NFT_Holder_Manager.instance.meetingStatus.UpdateMeetingParams((int)ThaMeetingStatusUpdate.MeetingStatus.Inprogress);
            triggerObject.GetComponent<ArrowManager>().UpdateMeetingTxt("Waiting For Interviewer");
        }
        else if (NFT_Holder_Manager.instance.meetingStatus.ThaMeetingStatus.Equals(ThaMeetingStatusUpdate.MeetingStatus.Inprogress))
        { // for interviewer
            NFT_Holder_Manager.instance.meetingStatus.UpdateMeetingParams((int)ThaMeetingStatusUpdate.MeetingStatus.HouseFull);
            triggerObject.GetComponent<ArrowManager>().UpdateMeetingTxt("Meeting Is In Progress");
        }
    }

    private IEnumerator ExitFromMeeting()
    {
        ConstantsHolder.xanaConstants.MuseumID = currentRoomId;
        ConstantsHolder.userId = userId;
        ChatSocketManager.onJoinRoom?.Invoke(ConstantsHolder.xanaConstants.MuseumID);
        StartCoroutine(ChatSocketManager.instance.FetchOldMessages());

        NFT_Holder_Manager.instance.voiceManager.SetVoiceGroup(0);

        int temp = FB_Notification_Initilizer.Instance.userInMeeting - 1;
        NFT_Holder_Manager.instance.meetingStatus.UpdateUserCounter(temp);
        yield return new WaitForSeconds(1f);
        if (FB_Notification_Initilizer.Instance.userInMeeting <= 0)
        {
            NFT_Holder_Manager.instance.meetingStatus.UpdateMeetingParams((int)ThaMeetingStatusUpdate.MeetingStatus.End);
            triggerObject.GetComponent<ArrowManager>().UpdateMeetingTxt("Join Meeting Now!");
        }
        NFT_Holder_Manager.instance.meetingTxtUpdate.MeetingRoomText.text = "";
    }
    private async void GetUpdatedToken()
    {
        StringBuilder ApiURL = new StringBuilder();
        ApiURL.Append(ConstantsGod.API_BASEURL + ConstantsGod.toyotaEmailApi + _thaRoomId + "/" + _thaPageNumber + "/" + _thaPageSize);
        Debug.LogError("API URL is : " + ApiURL.ToString());
        using (UnityWebRequest request = UnityWebRequest.Get(ApiURL.ToString()))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Error is" + request.error);
            }
            else
            {
                StringBuilder data = new StringBuilder();
                data.Append(request.downloadHandler.text);
                RegisterAsCompanyEmails.THAEmailDataResponse json = JsonConvert.DeserializeObject<RegisterAsCompanyEmails.THAEmailDataResponse>(data.ToString());
                for (int i = 0; i < json.data.rows.Count; i++)
                {
                    if (i < FB_Notification_Initilizer.Instance.companyEmails.Count)
                    {
                        FB_Notification_Initilizer.Instance.companyEmails[i] = json.data.rows[i].email;
                        FB_Notification_Initilizer.Instance.fbTokens[i] = json.data.rows[i].userToken;
                    }
                    else
                    {
                        FB_Notification_Initilizer.Instance.companyEmails.Add(json.data.rows[i].email);
                        FB_Notification_Initilizer.Instance.fbTokens.Add(json.data.rows[i].userToken);
                    }

                }
            }
            request.Dispose();
        }
    }
}

