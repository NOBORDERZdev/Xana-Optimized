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
    private static UserDailyRewardHandler _instance;

    [SerializeField]
    private GameObject _dailyRewardPopup;

    [SerializeField]
    private TextMeshProUGUI _rewardedAmountText;

    private SocketManager _socketManager;

    private string SocketUrl
    {
        get
        {
            if (APIBasepointManager.instance.IsXanaLive)
            {
                return "https://mslog.xana.net";
            }
            else
            {
                return "https://mslog-test.xana.net";
            }
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;

    }
    private IEnumerator Start()
    {
        while (ConstantsHolder.userId == null)
        {
            yield return new WaitForSeconds(1f);
        }

        if (SocketUrl != null)
        {
            _socketManager = new SocketManager(new Uri(SocketUrl));
            _socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnSocketConnected);
            _socketManager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnSocketError);
            _socketManager.Socket.On<CustomError>(SocketIOEventTypes.Disconnect, OnSocketDisconnect);
        }
        CheckToShowDailyReward();
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
        Debug.Log("<color=green>Daily Reward Socket Connected : " + resp + "</color>");
        _socketManager.Socket.On<string>("xeny-rewarded", DailyRewardResponse);
    }

    private void OnSocketError(CustomError args)
    {
        Debug.Log("<color=red>Daily Reward Socket Error : " + args + "</color>");
    }

    private void OnSocketDisconnect(CustomError args)
    {
        Debug.Log("<color=yellow>Daily Reward Socket Disconnected : " + args + "</color>");
    }
    private void DailyRewardResponse(string resp)
    {
        Debug.Log("Daily Reward Daily Reward Response : " + resp);
        //Debug.Log("<color=green>Daily Reward ConstantsHolder.userId received : " + ConstantsHolder.userId + "</color>");

        if (ConstantsHolder.xanaConstants.LoginasGustprofile)
            return;

        UserDailyRewardData data = JsonConvert.DeserializeObject<UserDailyRewardData>(resp);

        if (data.userId == int.Parse(ConstantsHolder.userId))
        {
            Debug.Log("Daily Reward Daily Reward Response Id matched : " + resp);
            _rewardedAmountText.text = data.coin.ToString();
            ConstantsHolder.xanaConstants.hasToShowDailyPopup = true;
            //if (SceneManager.GetActiveScene().name == "Home")
            //{
            //    ShowDailyRewardPopup();
            //}
            //else
            //{
            //    _hasToShowDailyPopup = true;
            //}
        }
    }
    private void ShowDailyRewardPopup()
    {
        _dailyRewardPopup.SetActive(true);
        InventoryManager.instance.UpdateUserXeny();
    }

    //Executes when Scene is loaded
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Home" && ConstantsHolder.xanaConstants.hasToShowDailyPopup && ConstantsHolder.xanaConstants.isGoingForHomeScene)
        {
            //Debug.LogError("Home Scene Loaded");
            StartCoroutine(ShowDailyRewardRoutine());
        }
    }

    private IEnumerator ShowDailyRewardRoutine()
    {
        //Adding delay to avoid showing the popup on the lobby worlds loading throuth Home scene
        yield return new WaitForSecondsRealtime(3f);
        if (SceneManager.GetActiveScene().name == "Home")
        {
            ShowDailyRewardPopup();
        }
        StopCoroutine(ShowDailyRewardRoutine());
    }

    private void CheckToShowDailyReward()
    {
        if (ConstantsHolder.xanaConstants.hasToShowDailyPopup)
        {
            ShowDailyRewardPopup();
        }
    }

    public void DailyRewardPopupOkBtn()
    {
        _dailyRewardPopup.SetActive(false);
        ConstantsHolder.xanaConstants.hasToShowDailyPopup = false;
    }
}

[Serializable]
public struct UserDailyRewardData
{
    public int userId;
    public int coin;
    public DateTime dateTime;
} 