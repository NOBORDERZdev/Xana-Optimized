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
    private GameObject _dailyRewardPopup;

    [SerializeField]
    private TextMeshProUGUI _rewardedAmountText;

    private SocketManager _socketManager;
    private int _myUserId = 0;

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
            _socketManager = new SocketManager(new Uri(SocketUrl));
            _socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnSocketConnected);
            _socketManager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnSocketError);
        }
        DontDestroyOnLoad(this);

        while (ConstantsHolder.userId == null)
            yield return new WaitForSeconds(0.5f);

        _myUserId = int.Parse(ConstantsHolder.userId);
    }
    private void OnDisable()
    {
        if (_socketManager != null)
        {
            _socketManager.Socket.Off();
            _socketManager.Close();
        }
    }
    private void OnSocketConnected(ConnectResponse resp)
    {
        _socketManager.Socket.On<string>("xeny-rewarded", DailyRewardResponse);
    }

    private void OnSocketError(CustomError args)
    {
        Debug.LogError("Socket Error : " + args);
    }

    private void DailyRewardResponse(string resp)
    {
        UserDailyRewardData data = JsonConvert.DeserializeObject<UserDailyRewardData>(resp);
        if (data.userId == _myUserId)
        {
            _rewardedAmountText.text = data.amount.ToString();   
            StartCoroutine(ShowDailyRewardRoutine());
        }
    }

    private IEnumerator ShowDailyRewardRoutine()
    {
        while (SceneManager.GetActiveScene().name != "Home")
            yield return new WaitForSecondsRealtime(5f);

        _dailyRewardPopup.SetActive(true);
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