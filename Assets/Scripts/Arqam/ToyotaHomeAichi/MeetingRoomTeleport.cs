using Photon.Pun;
using UnityEngine;
using System.Collections;
using static Unity.Burst.Intrinsics.X86.Avx;

public class MeetingRoomTeleport : MonoBehaviour
{
    [SerializeField] bool isLocked;
    [SerializeField] Transform destinationPoint;
    public enum PortalType { Enter, Exit }
    public PortalType currentPortal;
    public float cam_XValue = -50f;

    private PlayerController ref_PlayerControllerNew;
    private ReferencesForGamePlay referrencesForDynamicMuseum;
    private GameObject triggerObject;
    private string currentRoomId;
    private string userId;

    private void Start()
    {
        referrencesForDynamicMuseum = ReferencesForGamePlay.instance;
        ref_PlayerControllerNew = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>();
        currentRoomId = ConstantsHolder.xanaConstants.MuseumID;
        userId = ConstantsHolder.userId;
        //GameplayEntityLoader.instance.HomeBtn.onClick.AddListener(LeaveMeetingOnExit);
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
            if (NFT_Holder_Manager.instance.meetingStatus.tms.Equals(ThaMeetingStatusUpdate.MeetingStatus.HouseFull))
                return;
            if (FB_Notification_Initilizer.Instance.actorType != FB_Notification_Initilizer.ActorType.CompanyUser &&
                   NFT_Holder_Manager.instance.meetingStatus.tms.Equals(ThaMeetingStatusUpdate.MeetingStatus.Inprogress))
            {
                return;
            }
            else if (FB_Notification_Initilizer.Instance.actorType == FB_Notification_Initilizer.ActorType.CompanyUser &&
                                   NFT_Holder_Manager.instance.meetingStatus.tms.Equals(ThaMeetingStatusUpdate.MeetingStatus.End))
            {
                return;
            }
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

            GameplayEntityLoader.instance.StartCoroutine(GameplayEntityLoader.instance.setPlayerCamAngle(cam_XValue, 0.5f));
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
            ConstantsHolder.xanaConstants.MuseumID = "";       // meeting room mainnet id
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
        NFT_Holder_Manager.instance.meetingTxtUpdate.tmp.text = "";
    }
}

