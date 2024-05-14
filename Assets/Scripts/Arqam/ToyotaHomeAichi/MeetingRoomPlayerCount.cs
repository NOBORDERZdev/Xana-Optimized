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
using BestHTTP.JSON.LitJson;
using System.Security.Policy;

public class MeetingRoomPlayerCount : MonoBehaviour
{
    bool isCompanyMember = false;
    int RoomId = 4;

    void OnEnable()
    {
        StartCoroutine(MeetingRoomJoin());
    }
    void OnDisable()
    {
        StartCoroutine(MeetingRoomLeave());
    }
    IEnumerator MeetingRoomJoin()
    {

        //MyTestingClass myTestingClass = new MyTestingClass();// (RoomId, FB_Notification_Initilizer.Instance.toyotaUserEmail,false);
        //myTestingClass.worldId = RoomId;
        //myTestingClass.email = FB_Notification_Initilizer.Instance.toyotaUserEmail;
        //myTestingClass.isCompanyMember = isCompanyMember;

        //string json = JsonConvert.SerializeObject(myTestingClass);
        //using (UnityWebRequest request = new UnityWebRequest(ConstantsGod.API_BASEURL + ConstantsGod.joinmeetingroom, "POST"))
        //{
        //    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        //    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        //    request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        //    request.SetRequestHeader("Content-Type", "application/json");

        //    yield return request.SendWebRequest();

        //    if (request.isNetworkError || request.isHttpError)
        //    {
        //        Debug.LogError("Error: " + request.error);
        //    }
        //    else
        //    {
        //        Debug.Log("Response: " + request.downloadHandler.text);
        //    }
        //}

        string token = ConstantsGod.AUTH_TOKEN;
        WWWForm form = new WWWForm();
        form.AddField("worldId", RoomId);
        form.AddField("email", FB_Notification_Initilizer.Instance.toyotaUserEmail);
        form.AddField("isCompanyMember", isCompanyMember ? "1" : "0");
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
        form.AddField("worldId", RoomId);
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

public class MyTestingClass
{
    public int worldId;
    public string email;
    public bool isCompanyMember;

    //MyTestingClass(string abc, string xyz, bool klm)
    //{
    //    worldId = abc;
    //    email = xyz;
    //    isCompanyMember = klm;
    //}
}

