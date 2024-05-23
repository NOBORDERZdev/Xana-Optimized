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

    private SocketManager Manager;
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
            Manager = new SocketManager(new Uri(SocketUrl));
            Manager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
            Manager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnError);
        }
        DontDestroyOnLoad(this);

        while (ConstantsHolder.userId == null)
            yield return new WaitForSeconds(0.5f);

        myUserId = int.Parse(ConstantsHolder.userId);
    }
    private void OnDisable()
    {
        if (Manager != null)
        {
            Manager.Socket.Off();
            Manager.Close();
        }
    }

    private void OnConnected(ConnectResponse resp)
    {
        Manager.Socket.On<string>("xeny-rewarded", DailyRewardResponse);
    }

    private void OnError(CustomError args)
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