using System;
using UnityEngine;
using UnityEngine.Events;

public class InternetChecker : MonoBehaviour
{
    public static InternetChecker instance;
    public GameObject PopUp,loader;
    //private bool once;

    public UnityEvent onConnected ;
    public UnityEvent onDisconnected;
    private bool _hasInvokedConnection = true;
    public bool ispopUpClose = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        onConnected ??= new UnityEvent();
        //onConnected = GameManager.Instance.ReloadMainScene();
        onConnected.AddListener(OnConnected);
    }

    void OnConnected()
    {
        //print("CALL");
        GameManager.Instance.ReloadMainScene();
    }

    //private void OnEnable()
    //{
    //   // once = true;
    //}
    
    // Start is called before the first frame update
    void Start()
    {
        //once = true;
        InvokeRepeating(nameof(CheckConnection), 1.0f, 3);
        //Debug.Log(once + "i am still running");
    }
     
    void CheckConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && !LoadingHandler.Instance.gameObject.transform.GetChild(0).gameObject.activeInHierarchy) // tf is this?
        {
            ShowPage();
            _hasInvokedConnection = false;
        }
        else
        {
            //once = true;
            if (!_hasInvokedConnection)
            {
                onConnected.Invoke();
            }
            _hasInvokedConnection = true;
            //print("inertnet availble");
        }
    }

    void ShowPage()
    {
        ispopUpClose = false;
        PopUp.SetActive(true);
        onDisconnected.Invoke();
    }

    public void cancel_PopUp()
    {
        ispopUpClose = true;
        PopUp.SetActive(false);
        CheckConnection();
    }
}
