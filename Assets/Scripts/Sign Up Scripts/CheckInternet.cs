using System;
using UnityEngine;
using UnityEngine.Events;

public class CheckInternet : MonoBehaviour
{
    public static CheckInternet instance;
    public GameObject PopUp, loader;
    private bool once;

    public UnityEvent onConnected;
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
        if (onConnected == null)
            onConnected = new UnityEvent();
        // onConnected = GameManager.Instance.ReloadMainScene();
        onConnected.AddListener(test);
    }

    void test()
    {
        print("CALL");
        GameManager.Instance.ReloadMainScene();
    }
    private void OnEnable()
    {
        // once = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        once = true;
        InvokeRepeating("checkConection", 1.0f, 3);
        //  Debug.Log(once + "i am still running");
    }

    void checkConection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && !LoadingHandler.Instance.gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
        {
            Debug.Log(once + "Error.Not going on");

            if (once == true)
            {
                {
                    if (UserRegisterationManager.instance)
                    {
                        if (PlayerPrefs.HasKey("TermsConditionAgreement") && PlayerPrefs.GetInt("shownWelcome") == 0)
                            UserRegisterationManager.instance.OpenUIPanal(1);
                        UserRegisterationManager.instance.FirstPanal.GetComponent<OnEnableDisable>().ClosePopUp();
                    }
                }
                once = false;
            }
            //UserRegisterationManager.instance.OpenUIPanal(1);
            //UserRegisterationManager.instance.FirstPanal.GetComponent<OnEnableDisable>().ClosePopUp();
            showPage();
            _hasInvokedConnection = false;
        }
        else
        {
            once = true;
            if (!_hasInvokedConnection)
            {
                onConnected?.Invoke();
            }
            _hasInvokedConnection = true;
            //print("inertnet availble");
        }
    }

    void showPage()
    {
        ispopUpClose = false;
        if (PopUp != null)
            PopUp.SetActive(true);
        onDisconnected?.Invoke();
    }

    public void cancel_PopUp()
    {
        ispopUpClose = true;
        PopUp.SetActive(false);
        checkConection();
    }
}
