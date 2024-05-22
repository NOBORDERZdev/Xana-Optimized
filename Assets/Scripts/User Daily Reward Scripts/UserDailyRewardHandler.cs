using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;

public enum DailyRewardTypes
{
    XENYCoins
}
public class UserDailyRewardHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject dailyRewardPopup;
    public string socketId;
    //[SerializeField, Header("All daily rewards")]
    //private List<DailyRewardItem> dailyRewards;
    private UserDailyRewardData userDailyRewardData;


    public SocketManager Manager;

    private void Start()
    {
        //Manager = new SocketManager(new Uri("http://localhost:3000"));
        //Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        //Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);

    }


    private void OnConnected(ConnectResponse resp)
    {
        Debug.Log("Connected to server");
        Debug.Log("Response : " + resp);
        socketId = resp.sid;
        //RequestUserDailyRewardData();
        //Manager.Socket.On<string>("send_xana_text_post_info", ReceivePost);
        //Manager.Socket.On<string>("likeTextPost", FeedLikeUpdate);
        //Manager.Socket.On<string>("user_enter_world", FriendJoinedSpace);
        //Manager.Socket.On<string>("user_exit_world", FriendExitSpace);
    }

    private void OnError(CustomError args)
    {
        Debug.Log("Socket Error : " + args);
    }

    public void RequestUserDailyRewardData()
    {
        //get data from API
        UserDailyRewardData userDailyRewardData = new()
        {
            day = 1,
            rewardAmount = 1,
            currentServerTime = DateTime.Now,
            lastRewardedtime = DateTime.Now,
            isClaimed = false
        };
        this.userDailyRewardData = userDailyRewardData; 
    }
}

[System.Serializable]
public struct DailyRewardItem
{
    public DailyRewardTypes rewardType;
    public int rewardAmount;
}

public struct UserDailyRewardData
{
    public int day; //or time
    public int rewardAmount;
    public DateTime currentServerTime;
    public DateTime lastRewardedtime;
    public bool isClaimed;
    public int totalXenyCoins;
}