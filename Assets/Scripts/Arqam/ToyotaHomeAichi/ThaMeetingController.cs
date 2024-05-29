using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class ThaMeetingController : MonoBehaviour
{
    public int roomID = 4;
    //string address = "https://api-test.xana.net/";

    void Start()
    {
        if (ConstantsHolder.xanaConstants.meetingStatus == ConstantsHolder.MeetingStatus.Inprogress)
            GetComponent<FB_PushNotificationSender>().SendNotification();

        ConstantsHolder.xanaConstants.MuseumID = "2399";                 // Toyota_Meeting_Room id

        //only user can back to toyota world when press on exit btn
        if (ConstantsHolder.xanaConstants.meetingStatus == ConstantsHolder.MeetingStatus.Inprogress)
        {
            ConstantsHolder.xanaConstants.parentSceneName = "D_Infinity_Labo";
            ConstantsHolder.xanaConstants.backFromMeeting = true;
        }
        else
            ConstantsHolder.xanaConstants.isBackToParentScane = false;
    }


    private void OnEnable()
    {
        JoinMeeting();
        GameplayEntityLoader.instance.HomeBtn.onClick.AddListener(LeaveMeeting);
    }

    private void JoinMeeting()
    {
        StartCoroutine(MeetingRoomJoin());
    }
    IEnumerator MeetingRoomJoin()
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();
        form.AddField("worldId", roomID);
        form.AddField("email", FB_Notification_Initilizer.Instance.toyotaUserEmail);

        UnityWebRequest www;
        www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.joinmeetingroom, form);
        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }

        if (!www.isHttpError && !www.isNetworkError)
            Debug.Log("Meeting Room Player  on join : " + www.downloadHandler.text);
        else
            Debug.Log("Error is" + www.error);
    }

    private void LeaveMeeting()
    {
        StartCoroutine(FB_Notification_Initilizer.Instance.MeetingRoomLeave());
        FB_Notification_Initilizer.Instance.MeetingRoomLeaveSocket();
    }




}

