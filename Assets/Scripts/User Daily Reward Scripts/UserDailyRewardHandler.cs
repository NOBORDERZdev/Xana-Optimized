using System;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class UserDailyRewardHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject dailyRewardPopup;

    [SerializeField]
    private TextMeshProUGUI rewardedAmountText;

    public UserDailyRewardData userDailyRewardData;
    public SocketManager Manager;
    public int userID = 0;
    public int rewardedUserID;
    public bool dailyRewardReceived = false;


    public string msg;

    private string SocketUrl
    {
        get
        {
            if (APIBasepointManager.instance.IsXanaLive)
            {
                return "LiveURL";
            }
            else
            {
                return "https://mslog-test.xana.net";
            }
        }
    }
    
    private IEnumerator Start()
    {
        if (SocketUrl != null)
        {
            Manager = new SocketManager(new Uri(SocketUrl));
            Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
            Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);
        }
        DontDestroyOnLoad(this);

        while (ConstantsHolder.userId == null)
            yield return new WaitForSeconds(0.5f);

        userID = int.Parse(ConstantsHolder.userId);
    }

    private void OnConnected(ConnectResponse resp)
    {
        Manager.Socket.On<string>("xeny-rewarded", DailyRewardResponse);
    }

    private void OnError(CustomError args)
    {
        Debug.LogError("Socket Error : " + args);
    }

    public void DailyRewardResponse(string resp)
    {
        msg = resp;
        UserDailyRewardData data = JsonConvert.DeserializeObject<UserDailyRewardData>(resp);
        rewardedUserID = data.userId;
        if (data.userId == userID)
        {
            dailyRewardReceived = true;
            rewardedAmountText.text = data.amount.ToString();   
            StartCoroutine(ShowDailyRewardRoutine());
        }
    }

    private IEnumerator ShowDailyRewardRoutine()
    {
        while (SceneManager.GetActiveScene().name != "Home")
            yield return new WaitForSeconds(5f);

        dailyRewardPopup.SetActive(true);
        dailyRewardReceived = false;
        StopCoroutine(ShowDailyRewardRoutine());
    }
}

[Serializable]
public struct UserDailyRewardData
{
    public int userId;
    public int amount;
    public DateTime datetime;
}