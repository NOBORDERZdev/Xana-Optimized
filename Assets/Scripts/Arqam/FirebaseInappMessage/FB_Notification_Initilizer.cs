using Firebase.Extensions;
using Firebase.Messaging;
using Firebase;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Android;
using System.Collections;


public class FB_Notification_Initilizer : MonoBehaviour
{
    public bool isShowLogs = false;
    public enum ActorType { User, CompanyUser };
    public ActorType actorType;
    [Space(5)]
    public string toyotaUserEmail;
    public List<string> companyEmails = new List<string>();
    public List<string> fbTokens = new List<string>();
    [Tooltip("Action invoke when device token received for push notification")]
    public Action<string> onReceiveToken;
    public static FB_Notification_Initilizer Instance;

    [SerializeField]
    private bool isFirebaseInitialized = false;
    private string topic = "TestTopic";
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;  

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If an instance already exists, destroy this instance
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            // assign the instance to this instance
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDisable()
    {
        UserLoginSignupManager.instance.logoutAction -= DeleteToken;
    }
    public void InitPushNotification(string mail)
    {
        UserLoginSignupManager.instance.logoutAction += DeleteToken;
        toyotaUserEmail = mail;
        actorType = CheckEmailStatus() ? ActorType.CompanyUser : ActorType.User;
        if (actorType.Equals(ActorType.CompanyUser))
            StartCoroutine(InitilizeFirebase());
    }

    private bool CheckEmailStatus()
    {
        if (companyEmails.Contains(toyotaUserEmail))
            return true;
        else
            return false;
    }


    IEnumerator InitilizeFirebase()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");

        FirebaseMessaging.RequestPermissionAsync();
        FirebaseMessaging.TokenRegistrationOnInitEnabled = true;  // for reRegister Token 
        yield return new WaitForEndOfFrame();

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
                InitializeFirebase();
            else
                DebugMsg("Could not resolve all Firebase dependencies: ");
        });
    }

    // Setup message event handlers.
    void InitializeFirebase()
    {
        FirebaseMessaging.MessageReceived += OnMessageReceived;
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.SubscribeAsync(topic).ContinueWithOnMainThread(task =>
        {
            LogTaskCompletion(task, "SubscribeAsync");
        });

        DebugMsg("Firebase Messaging Initialized");

        // On iOS, this will display the prompt to request permission to receive
        // notifications if the prompt has not already been displayed before. (If
        // the user already responded to the prompt, thier decision is cached by
        // the OS and can be changed in the OS settings).
        // On Android, this will return successfully immediately, as there is no
        // equivalent system logic to run.
        FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(
          task =>
          {
              LogTaskCompletion(task, "RequestPermissionAsync");
          }
        );
        isFirebaseInitialized = true;
    }

    protected bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            DebugMsg(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            DebugMsg(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string errorCode = "";
                FirebaseException firebaseEx = exception as FirebaseException;
                if (firebaseEx != null)
                {
                    errorCode = String.Format("Error.{0}: ",
                      ((Error)firebaseEx.ErrorCode).ToString());
                }
                DebugMsg(errorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            DebugMsg(operation + " completed");
            complete = true;
        }
        return complete;
    }

    public virtual void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message");
        var notification = e.Message.Notification;
        if (notification != null)
        {
            Debug.Log("title: " + notification.Title);
            Debug.Log("body: " + notification.Body);
            var android = notification.Android;
            if (android != null)
            {
                DebugMsg("android channel_id: " + android.ChannelId);
            }
        }
        if (e.Message.From.Length > 0)
            DebugMsg("from: " + e.Message.From);
        if (e.Message.Link != null)
        {
            DebugMsg("link: " + e.Message.Link.ToString());
        }
        if (e.Message.Data.Count > 0)
        {
            DebugMsg("data:");
            foreach (KeyValuePair<string, string> iter in
                     e.Message.Data)
            {
                DebugMsg("  " + iter.Key + ": " + iter.Value);
            }
        }
        Handheld.Vibrate();
    }

    public virtual void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        onReceiveToken?.Invoke(token.Token);
        Debug.LogError("Token Generated: " + token.Token);
    }

    private void DebugMsg(string msg)
    {
        if (isShowLogs)
            Debug.LogError(msg);
    }

    public void DeleteToken()
    {
        FirebaseMessaging.DeleteTokenAsync();
        toyotaUserEmail = "";
        companyEmails.Clear();
        fbTokens.Clear();

        Debug.LogError("Token Deleted");
    }

}
