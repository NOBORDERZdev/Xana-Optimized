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
    private int _myUserId = 0;
    private bool _hasToShowDailyPopup = false;

    private string SocketUrl
    {
        get
        {
            if (APIBasepointManager.instance.IsXanaLive)
            {
                return null; //Mainet socket url will be added here
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

        if (SocketUrl != null)
        {
            _socketManager = new SocketManager(new Uri(SocketUrl));
            _socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnSocketConnected);
            _socketManager.Socket.On<CustomError>(SocketIOEventTypes.Error, OnSocketError);
            _socketManager.Socket.On<CustomError>(SocketIOEventTypes.Disconnect, OnSocketDisconnect);
        }
        else
        {
            Destroy(gameObject);
        }

        while (ConstantsHolder.userId == null)
            yield return new WaitForSeconds(0.5f);

        _myUserId = int.Parse(ConstantsHolder.userId);
        //Debug.LogError("Daily Reward User Id : " + _myUserId);
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
        Debug.LogError("Daily Reward Socket Connected : " + resp);
        _socketManager.Socket.On<string>("xeny-rewarded", DailyRewardResponse);
    }

    private void OnSocketError(CustomError args)
    {
        Debug.LogError("Daily Reward Socket Error : " + args);
    }

    private void OnSocketDisconnect(CustomError args)
    {
        Debug.LogError("Daily Reward Socket Disconnected : " + args);
    }
    private void DailyRewardResponse(string resp)
    {
        //Debug.LogError("Daily Reward Daily Reward Response : " + resp);
        UserDailyRewardData data = JsonConvert.DeserializeObject<UserDailyRewardData>(resp);

        if (data.userId == _myUserId)
        {
            _rewardedAmountText.text = data.coin.ToString();
            if (SceneManager.GetActiveScene().name == "Home")
            {
                ShowDailyRewardPopup();
            }
            else
            {
                _hasToShowDailyPopup = true;
            }
        }
    }
    private void ShowDailyRewardPopup()
    {
        _dailyRewardPopup.SetActive(true);
        _hasToShowDailyPopup = false;
        //InventoryManager.instance.UpdateUserXeny();
    }

    //Executes when Home Scene is loaded
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Home" && _hasToShowDailyPopup)
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

}

[Serializable]
public struct UserDailyRewardData
{
    public int userId;
    public int coin;
    public DateTime dateTime;
} 