using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using EnhancedUI;
using static RegisterAsCompanyEmails;
using System.Text;

public class MeetingRoomPlayerCount : MonoBehaviour
{
    bool isCompanyMember = false;
    int RoomId = 4;
    void OnEnable()
    {
       // StartCoroutine(MeetingRoomJoin());
        StartCoroutine(MeetingRoomLeave());
    }
    void OnDisable()
    {
    }
    IEnumerator MeetingRoomJoin()
    {
        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();
        form.AddField("worldId", RoomId);
        form.AddField("email", FB_Notification_Initilizer.Instance.toyotaUserEmail);
        form.AddField("isCompanyMember", 0);
        UnityWebRequest www;
        www = UnityWebRequest.Post(ConstantsGod.BASE_URL + ConstantsGod.joinmeetingroom, form);
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
        form.AddField("worldId", RoomId);
        form.AddField("email", FB_Notification_Initilizer.Instance.toyotaUserEmail);
        UnityWebRequest www;
        www = UnityWebRequest.Post(ConstantsGod.BASE_URL + ConstantsGod.leavemeetingroom, form);
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
   public async void CheckUsersCount()
    {
        StringBuilder ApiURL = new StringBuilder();
        ApiURL.Append(ConstantsGod.API_BASEURL + ConstantsGod.getmeetingroomcount + RoomId);
        Debug.Log("API URL is : " + ApiURL.ToString());
        using (UnityWebRequest request = UnityWebRequest.Get(ApiURL.ToString()))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Error is" + request.error);
            }
            else
            {
                StringBuilder data = new StringBuilder();
                data.Append(request.downloadHandler.text);
                Debug.Log("Meeting Room Player Count is : " + data.ToString());
            }
        }
    }
}

