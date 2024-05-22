using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
//using BestHTTP.SocketIO3;
//using BestHTTP.SocketIO3.Events;
using System;
using BestHTTP.JSON;
using Newtonsoft.Json;

public class ThaMeetingController : MonoBehaviour
{
    public int roomID = 4;
    //public SocketManager Manager;
    //string address = "https://api-test.xana.net/";

    void Start()
    {
        //address = ConstantsGod.API_BASEURL;

        //if (!address.EndsWith("/"))
        //{
        //    address = address + "/";
        //}
        //Manager = new SocketManager(new Uri(address));

        GetComponent<FB_PushNotificationSender>().SendNotification();
        //only user can back to toyota world when press on exit btn
        if (ConstantsHolder.xanaConstants.meetingStatus == ConstantsHolder.MeetingStatus.Inprogress)
            ConstantsHolder.xanaConstants.parentSceneName = "D_Infinity_Labo";
        else
            ConstantsHolder.xanaConstants.isBackToParentScane = false;
    }
    public void JoinMeetingRoom(string msg)
    {
        Debug.Log("Join Meeting Room" + msg);
    }
   public  void LeaveMeetingRoom(string msg)
    {
        Debug.Log("Leave Meeting Room" + msg);

    }
    private void OnEnable()
    {
        JoinMeeting();
        GamePlayButtonEvents.inst.OnExitButton += LeaveMeeting;
        GamePlayButtonEvents.inst.OnExitButton += MeetingRoomLeaveSocket;
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
            Debug.Log("Error is" + www.error);
        else
            Debug.Log("Meeting Room Player  on leave : " + www.downloadHandler.text);
    }

    void MeetingRoomLeaveSocket()
    {
        THALeaveRoom tHALeaveRoom = new THALeaveRoom();
        tHALeaveRoom.userType = FB_Notification_Initilizer.Instance.actorType.ToString();
        tHALeaveRoom.userId = ConstantsHolder.userId.ParseToInt();
        tHALeaveRoom.world_id = ConstantsHolder.xanaConstants.builderMapID;
        string jsonData = JsonConvert.SerializeObject(tHALeaveRoom);
        HomeScoketHandler.instance.GetCallFromMeetingRoom(jsonData);

        Debug.Log("Actor Type : " + FB_Notification_Initilizer.Instance.actorType + "userid : " + ConstantsHolder.userId);
    }

}

