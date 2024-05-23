using System;
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

    private SocketManager socketManager;
    private int myUserId = 0;

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
            socketManager = new SocketManager(new Uri(SocketUrl));
            socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnSocketConnected);
            socketManager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnSocketError);
        }
        DontDestroyOnLoad(this);

        while (ConstantsHolder.userId == null)
            yield return new WaitForSeconds(0.5f);

        myUserId = int.Parse(ConstantsHolder.userId);
    }
    private void OnDisable()
    {
        if (socketManager != null)
        {
            socketManager.Socket.Off();
            socketManager.Close();
        }
    }
    private void OnSocketConnected(ConnectResponse resp)
    {
        socketManager.Socket.On<string>("xeny-rewarded", DailyRewardResponse);
    }

    private void OnSocketError(CustomError args)
    {
        Debug.LogError("Socket Error : " + args);
    }

    private void DailyRewardResponse(string resp)
    {
        UserDailyRewardData data = JsonConvert.DeserializeObject<UserDailyRewardData>(resp);
        if (data.userId == myUserId)
        {
            rewardedAmountText.text = data.amount.ToString();   
            StartCoroutine(ShowDailyRewardRoutine());
        }
    }

    private IEnumerator ShowDailyRewardRoutine()
    {
        while (SceneManager.GetActiveScene().name != "Home")
            yield return new WaitForSecondsRealtime(5f);

        dailyRewardPopup.SetActive(true);
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