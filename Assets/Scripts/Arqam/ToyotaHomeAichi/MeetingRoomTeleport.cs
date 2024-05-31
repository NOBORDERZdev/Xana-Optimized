using Photon.Pun;
using UnityEngine;
using System.Collections;

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

    private void Start()
    {
        referrencesForDynamicMuseum = ReferencesForGamePlay.instance;
        ref_PlayerControllerNew = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>();
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

        GamePlayUIHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = false;
        if (currentPortal == PortalType.Enter)
            GamePlayUIHandler.inst.EnableJJPortalPopup(this.gameObject, 2);
        else if (currentPortal == PortalType.Exit)
            GamePlayUIHandler.inst.EnableJJPortalPopup(this.gameObject, 3);
    }

    public void RedirectToWorld()
    {
        if (triggerObject.GetComponent<PhotonView>().IsMine)
        {
            this.StartCoroutine(Teleport());

            if (currentPortal.Equals(PortalType.Enter))
                EnterInMeeting();
            else if (currentPortal.Equals(PortalType.Exit))
                ExitFromMeeting();
        }
    }

    private void EnterInMeeting()
    {
        FindObjectOfType<VoiceManager>().SetVoiceGroup(5);

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

    private void ExitFromMeeting()
    {
        FindObjectOfType<VoiceManager>().SetVoiceGroup(0);
    }

    IEnumerator Teleport()
    {
        if (!isLocked)
        {
            referrencesForDynamicMuseum.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = false;
            LoadingHandler.Instance.JJLoadingSlider.fillAmount = 0;
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
            StartCoroutine(LoadingHandler.Instance.IncrementSliderValue(Random.Range(2f, 3f)));
            //yield return new WaitForSeconds(.5f);
            //RaycastHit hit;
        //CheckAgain:
        //    if (Physics.Raycast(destinationPoint.position, destinationPoint.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        //    {
        //        if ((hit.collider.GetComponent<PhotonView>() != null) && hit.collider.GetComponent<PhotonView>().IsMine)
        //        {
        //            destinationPoint.position = new Vector3(destinationPoint.position.x + Random.Range(-2, 2), destinationPoint.position.y, destinationPoint.position.z + Random.Range(-2, 2));
        //            goto CheckAgain;
        //        }
        //        else if (hit.collider.gameObject.tag != "GroundFloor")
        //        {
        //            destinationPoint.position = new Vector3(destinationPoint.position.x + Random.Range(-2, 2), destinationPoint.position.y, destinationPoint.position.z + Random.Range(-2, 2));

        //            goto CheckAgain;
        //        }
        //    }
        //    else
        //    {
        //        destinationPoint.position = new Vector3(destinationPoint.position.x + Random.Range(-2, 2), destinationPoint.position.y, destinationPoint.position.z + Random.Range(-2, 2));
        //        goto CheckAgain;
        //    }
            yield return new WaitForSeconds(.4f);

            referrencesForDynamicMuseum.MainPlayerParent.transform.eulerAngles = destinationPoint.eulerAngles;
            referrencesForDynamicMuseum.MainPlayerParent.transform.position = destinationPoint.position;
            yield return new WaitForSeconds(.8f);
            referrencesForDynamicMuseum.MainPlayerParent.transform.position = destinationPoint.position;
            referrencesForDynamicMuseum.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = true;

            GameplayEntityLoader.instance.StartCoroutine(GameplayEntityLoader.instance.setPlayerCamAngle(cam_XValue, 0.5f));
            yield return new WaitForSeconds(.15f);
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
        }
        else
        {
            if (SNSNotificationHandler.Instance != null)
                SNSNotificationHandler.Instance.ShowNotificationMsg("Coming soon");
            yield return null;
        }
    }

}

