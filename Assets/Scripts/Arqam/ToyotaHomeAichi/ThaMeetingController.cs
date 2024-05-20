using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ThaMeetingController : MonoBehaviour
{
    public int roomID = 4;
    void Start()
    {
        GetComponent<FB_PushNotificationSender>().SendNotification();
        //only user can back to toyota world when press on exit btn
        if (ConstantsHolder.xanaConstants.meetingStatus == ConstantsHolder.MeetingStatus.Inprogress)
            ConstantsHolder.xanaConstants.parentSceneName = "D_Infinity_Labo";
        else
            ConstantsHolder.xanaConstants.isBackToParentScane = false;
    }

    private void OnEnable()
    {
        JoinMeeting();
        GamePlayButtonEvents.inst.OnExitButton += LeaveMeeting;
    }
    private void JoinMeeting()
    {
        StartCoroutine(MeetingRoomJoin());
    }
    private void LeaveMeeting()
    {
        StartCoroutine(MeetingRoomLeave());
    }
    IEnumerator MeetingRoomJoin()
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();
        form.AddField("worldId", roomID);
        form.AddField("email", FB_Notification_Initilizer.Instance.toyotaUserEmail);
        //form.AddField("isCompanyMember", isCompanyMember.ToString());
        UnityWebRequest www;
        www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.joinmeetingroom, form);
        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }
        string str = www.downloadHandler.text;
        if (!www.isHttpError && !www.isNetworkError)
        {
            Debug.Log("Meeting Room Player  on join : " + www.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error is" + www.error);
        }

    }
    IEnumerator MeetingRoomLeave()
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();
        form.AddField("worldId", roomID);
        form.AddField("email", FB_Notification_Initilizer.Instance.toyotaUserEmail);
        UnityWebRequest www;
        www = UnityWebRequest.Post(ConstantsGod.API_BASEURL + ConstantsGod.leavemeetingroom, form);
        www.SetRequestHeader("Authorization", token);
        www.SendWebRequest();
        while (!www.isDone)
        {
            yield return null;
        }
        if (!www.isHttpError && www.isNetworkError)
        {
            Debug.Log("Error is" + www.error);
        }
        else
        {
            Debug.Log("Meeting Room Player  on leave : " + www.downloadHandler.text);
        }
    }
}

