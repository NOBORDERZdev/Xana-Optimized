// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH"/>
// <summary>Demo code for Photon Chat in Unity.</summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Collections;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
using TMPro;
using SimpleJSON;
using Crosstales.BWF;

#endif


/// <summary>
/// This simple Chat UI demonstrate basics usages of the Chat Api
/// </summary>
/// <remarks>
/// The ChatClient basically lets you create any number of channels.
///
/// some friends are already set in the Chat demo "DemoChat-Scene", 'Joe', 'Jane' and 'Bob', simply log with them so that you can see the status changes in the Interface
///
/// Workflow:
/// Create ChatClient, Connect to a server with your AppID, Authenticate the user (apply a unique name,)
/// and subscribe to some channels.
/// Subscribe a channel before you publish to that channel!
///
///
/// Note:
/// Don't forget to call ChatClient.Service() on Update to keep the Chatclient operational.
/// </remarks>
public class XanaChatSystem : MonoBehaviour
{
    public static XanaChatSystem instance;
    public GameObject chatOutPutPenal;

    private const string UsernamePrefs = "UsernamePref";
    [SerializeField]
    public string UserName { get; set; }

    [Header("UI Elements")]
    public GameObject chatButton;
    public GameObject chatCloseButton;
    public GameObject chatNotificationIcon;

    public GameObject chatDialogBox;
    public GameObject chatConfirmationPanel;
    bool isPanelConfirmationRequire = false;

    public RectTransform ChatPanel;     // set in inspector (to enable/disable panel)
    public GameObject UserIdFormPanel;
    public InputField InputFieldChat;   // set in inspector

    public TextMeshProUGUI CurrentChannelText;     // set in inspector
    public TextMeshProUGUI PotriatCurrentChannelText;     // set in inspector

    public Toggle ChannelToggleToInstantiate; // set in inspector
    public GameObject XanaChatLand; // set in inspector
    public GameObject XanaChatPotrait; // set in inspector
    public RectTransform outline;

    public ScrollRect ChatScrollRect;

    public Action<string> npcAlert;

    #region Not Required

    //public bool helpChecked = false;
    //public GameObject HelpScreenObject;
    //public string ChannelsToJoinOnConnect; // set in inspector. Demo channels to join automatically.
    //public string[] FriendsList;
    //public int HistoryLengthToFetch; // set in inspector. Up to a certain degree, previously sent messages can be fetched for context
    //private string selectedChannelName; // mainly used for GUI/input
    //public GameObject FriendListUiItemtoInstantiate;
    //private readonly Dictionary<string, Toggle> channelToggles = new Dictionary<string, Toggle>();
    //public bool ShowState = true;
    //public bool sayHiOnJoiningChannel;
    //private bool isQuitGame;

    #endregion


    private void Awake()
    {
        if (instance != null && instance != this)
            this.isPanelConfirmationRequire = instance.isPanelConfirmationRequire;
        //instance = this;
    }

    private void OnEnable()
    {
        instance = this;
        if (!ConstantsHolder.xanaConstants.IsChatUseByOther)
        {
            this.InputFieldChat.onSubmit.AddListener(OnEnterSend);
        }
    }
    private void OnDisable()
    {
        if (!ConstantsHolder.xanaConstants.IsChatUseByOther)
        {
            this.InputFieldChat.onSubmit.RemoveAllListeners();
        }
    }

    public void Start()
    {
        //CheckIfDeviceHasNotch();
        CheckPlayerPrefItems();

        if (!string.IsNullOrEmpty(CurrentChannelText.text.ToString()))
        {
            PotriatCurrentChannelText.text = CurrentChannelText.text;
        }
        if (!string.IsNullOrEmpty(PotriatCurrentChannelText.text.ToString()))
        {
            CurrentChannelText.text = PotriatCurrentChannelText.text;
        }

        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
            this.UserName = PlayerPrefs.GetString("UserName");
        else if (string.IsNullOrEmpty(PlayerPrefs.GetString(ConstantsGod.GUSTEUSERNAME)))
        {
            if (string.IsNullOrEmpty(this.UserName))
            {
                this.UserName = "user" + Environment.TickCount % 99; //made-up username
            }
        }
        else
        {
            this.UserName = PlayerPrefs.GetString(ConstantsGod.GUSTEUSERNAME);
        }

        Connect();
    }


    public void DisplayMsg_FromSocket(string _userName, string _msg)
    {
        //Debug.Log("<color=red> XanaOldChat: " + _userName + " : " + _userName.Length + " : " + _msg +"</color>");

        if (_userName.Length > 12)
        {
            this.CurrentChannelText.text = "<b>" + _userName.Substring(0, 12) + "...</b>" + " : " + _msg + "\n" + this.CurrentChannelText.text;
            this.PotriatCurrentChannelText.text = "<b>" + _userName.Substring(0, 12) + "...</b>" + " : " + _msg + "\n" + this.PotriatCurrentChannelText.text;
        }
        else
        {
            this.CurrentChannelText.text = "<b>" + _userName + "</b>" + " : " + _msg + "\n" + this.CurrentChannelText.text;
            this.PotriatCurrentChannelText.text = "<b>" + _userName + "</b>" + " : " + _msg + "\n" + this.PotriatCurrentChannelText.text;
        }

        if (!chatDialogBox.activeSelf && _userName != UserName)
        {
            chatNotificationIcon.SetActive(true);
        }

        StartCoroutine(Delay());

        //this.CurrentChannelText.text = _userName + " : " + _msg + "\n" + this.CurrentChannelText.text;
    }
    public void DisplayMsg_FromSocket(string _userName, string _msg, TextMeshProUGUI MsgTextBox)
    {
        //Debug.Log("<color=red> XanaOldChat: " + _userName + " : " + _userName.Length + " : " + _msg +"</color>");

        if (_userName.Length > 12)
        {
            MsgTextBox.text = "<b>" + _userName.Substring(0, 12) + "...</b>" + " : " + _msg;
        }
        else
        {
            MsgTextBox.text = "<b>" + _userName + "</b>" + " : " + _msg;
        }

        if (!chatDialogBox.activeSelf && _userName != UserName)
        {
            chatNotificationIcon.SetActive(true);
        }

        StartCoroutine(Delay());

        //this.CurrentChannelText.text = _userName + " : " + _msg + "\n" + this.CurrentChannelText.text;
    }
    public void ClearChatTxtForMeeting()
    {
        this.CurrentChannelText.text = "";
        this.PotriatCurrentChannelText.text = "";
    }

    public void DisplayErrorMsg_FromSocket(string _msg, string errorType)
    {

        if (errorType.Contains("Error"))
        {
            this.CurrentChannelText.text = "<color=red>" + _msg + "...</color>" + "\n" + this.CurrentChannelText.text; ;
            this.PotriatCurrentChannelText.text = "<color=red>" + _msg + "...</color>" + "\n" + this.PotriatCurrentChannelText.text;
        }
        else
        {
            this.CurrentChannelText.text = "<color=green>" + _msg + "</color>" + "\n" + this.CurrentChannelText.text; ;
            this.PotriatCurrentChannelText.text = "<color=green>" + _msg + "</color>" + "\n" + this.PotriatCurrentChannelText.text;
        }

        //StartCoroutine(Delay());

        //this.CurrentChannelText.text = _userName + " : " + _msg + "\n" + this.CurrentChannelText.text;
    }

    public void LoadOldChat()
    {
        ChatSocketManager.callApi?.Invoke();
        //chatConfirmationPanel.SetActive(false);
    }
    void CheckIfDeviceHasNotch()
    {
        if (!ScreenOrientationManager._instance.isPotrait)
        {
            outline.offsetMin = new Vector2((Screen.safeArea.xMin / (float)(Screen.width / 800f)), outline.offsetMin.y);
        }
    }

    void CheckPlayerPrefItems()
    {
        if (PlayerPrefs.HasKey(UsernamePrefs))
        {
            UserName = PlayerPrefs.GetString(UsernamePrefs);
            if (UserName.Contains("Guest") || UserName.Contains("ゲスト"))
            {
                if (GameManager.currentLanguage == "ja")
                {
                    UserName = "ゲスト" + UserName.Substring(UserName.Length - 4);
                }
                else if (GameManager.currentLanguage == "en")
                {
                    UserName = "Guest" + UserName.Substring(UserName.Length - 4);
                }
            }
        }
    }
    public void Connect()
    {
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1)
            this.UserName = PlayerPrefs.GetString("UserName");

        PlayerPrefs.SetString(UsernamePrefs, this.UserName);
        PlayerPrefs.SetString(ConstantsGod.PLAYERNAME, this.UserName);
    }

    System.Collections.IEnumerator Delay()
    {
        yield return new WaitForSeconds(.3f);
        ChatScrollRect.verticalNormalizedPosition = 1f;
    }

    public bool isChatOpen;

    public void OpenCloseChatDialog()
    {
        isChatOpen = !isChatOpen;

        if (isChatOpen)
        {
            chatDialogBox.SetActive(true);
            chatNotificationIcon.SetActive(false);
            chatButton.GetComponent<Image>().enabled = true;


            // Due to Overlapping of Minimap and Chat, Disable Minimap
            ReferencesForGamePlay.instance.minimap.SetActive(false);
            ReferencesForGamePlay.instance.SumitMapStatus(false);

            if (!oneTime)
            {
                //oneTime = true;
                StartCoroutine(ChatOpenDelay());
            }
            
            // Confirmation Panel Not Require
            //if (!isPanelConfirmationRequire)
            //{
            //    if (!string.IsNullOrEmpty(ChatSocketManager.instance.oldChatResponse))
            //    {
            //        JSONNode jsonNode = JSON.Parse(ChatSocketManager.instance.oldChatResponse);
            //        int countValue = jsonNode["count"].AsInt;

            //        if (countValue > 0)
            //        {
            //            isPanelConfirmationRequire = true;
            //            chatConfirmationPanel.SetActive(true);
            //        }
            //    }
            //}
        }
        else
        {
            chatDialogBox.SetActive(false);
            chatNotificationIcon.SetActive(false);
            chatButton.GetComponent<Image>().enabled = false;

            if (PlayerPrefs.GetInt("minimap") == 1)
            {
                if (ConstantsHolder.xanaConstants.IsMetabuzzEnvironment)
                    return;
                ReferencesForGamePlay.instance.minimap.SetActive(true);
                ReferencesForGamePlay.instance.SumitMapStatus(true);
            }
        }
        Debug.Log("ChatOpenDelay");
        
    }
    bool oneTime = false;
    VerticalLayoutGroup verticalLayoutGroup;
    IEnumerator ChatOpenDelay()
    {
        if (verticalLayoutGroup == null)
            verticalLayoutGroup = ChatSocketManager.instance.MsgParentObj.GetComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.enabled = false;
        yield return new WaitForSeconds(0.05f);
        verticalLayoutGroup.enabled = true;
        if (ChatSocketManager.instance.MsgParentObjScrollRect)
            ChatSocketManager.instance.MsgParentObjScrollRect.verticalNormalizedPosition = 1;

    }
    public void OpenCloseChatDialog(bool _state)
    {
        isChatOpen = _state;

        if (isChatOpen)
        {
            chatDialogBox.SetActive(true);
            chatNotificationIcon.SetActive(false);
            chatButton.GetComponent<Image>().enabled = true;
        }
        else
        {
            chatDialogBox.SetActive(false);
            chatNotificationIcon.SetActive(false);
            chatButton.GetComponent<Image>().enabled = false;
        }
    }
    public void OnEnterSend()
    {
        string removeBadWords = "";
        if (!string.IsNullOrEmpty(InputFieldChat.text))
        {
            removeBadWords = BWFManager.Instance.ReplaceAll(InputFieldChat.text);
        }
        Debug.LogError("removeBadWords_" + removeBadWords);
        if (!UserPassManager.Instance.CheckSpecificItem("Message Option/Chat option"))
        {
            //UserPassManager.Instance.PremiumUserUI.SetActive(true);
            print("Please Upgrade to Premium account");
            this.InputFieldChat.text = "";
            removeBadWords = "";
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        PlayerPrefs.SetString(ConstantsGod.SENDMESSAGETEXT, removeBadWords);

        ChatSocketManager.onSendMsg?.Invoke(ConstantsHolder.xanaConstants.MuseumID, removeBadWords, CallBy.User, "");
        ArrowManager.OnInvokeCommentButtonClickEvent(PlayerPrefs.GetString(ConstantsGod.SENDMESSAGETEXT));

        //  npcAlert?.Invoke(removeBadWords);  // call npc's to start chat //

        this.InputFieldChat.text = "";
    }

    public void OnEnterSend(string s)
    {
        OnEnterSend();
    }

    public void OnClickSend()
    {
        if (this.InputFieldChat != null)
        {
            this.SendChatMessage(this.InputFieldChat.text);

            this.InputFieldChat.text = "";
        }
    }
    private void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine))
        {
            return;
        }
        //if ("test".Equals(inputLine))
        //{
        //    if (this.TestLength != this.testBytes.Length)
        //    {
        //        this.testBytes = new byte[this.TestLength];
        //    }

        //}
        ChatSocketManager.onSendMsg.Invoke(ConstantsHolder.xanaConstants.MuseumID, inputLine, CallBy.User, "");
    }

    #region Photon Chat Region

    //public void helpScreenOnOff()
    //{
    //    //if (chatOutPutPenal.activeInHierarchy)
    //    //{
    //    //    if (HelpScreenObject.activeInHierarchy)
    //    //    {
    //    //        helpChecked = true;
    //    //        chatOutPutPenal.SetActive(false);
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    if (helpChecked && !HelpScreenObject.activeInHierarchy)
    //    //    {
    //    //        helpChecked = false;
    //    //        chatOutPutPenal.SetActive(true);
    //    //        HelpScreenObject.SetActive(false);
    //    //    }
    //    //}
    //}
    //public void OnEnterSend()
    //{
    //    if (!UserPassManager.Instance.CheckSpecificItem("Message Option/Chat option"))
    //    {
    //        //UserPassManager.Instance.PremiumUserUI.SetActive(true);
    //        print("Please Upgrade to Premium account");
    //        return;
    //    }
    //    else
    //    {
    //        print("Horayyy you have Access");
    //    }

    //    PlayerPrefs.SetString(ConstantsGod.SENDMESSAGETEXT, this.InputFieldChat.text);
    //    Debug.Log("text msg====" + PlayerPrefs.GetString(ConstantsGod.SENDMESSAGETEXT));

    //    ChatSocketManager.onSendMsg?.Invoke(ConstantsHolder.xanaConstants.MuseumID, this.InputFieldChat.text);
    //    ArrowManager.OnInvokeCommentButtonClickEvent(PlayerPrefs.GetString(ConstantsGod.SENDMESSAGETEXT));
    //    this.InputFieldChat.text = "";
    //}
    //public void OnClickSend()
    //{
    //    if (this.InputFieldChat != null)
    //    {
    //        this.SendChatMessage(this.InputFieldChat.text);

    //        this.InputFieldChat.text = "";
    //    }
    //}
    //public int TestLength = 2048;
    //private byte[] testBytes = new byte[2048];
    //private void SendChatMessage(string inputLine)
    //{
    //    if (string.IsNullOrEmpty(inputLine))
    //    {
    //        return;
    //    }
    //    if ("test".Equals(inputLine))
    //    {
    //        if (this.TestLength != this.testBytes.Length)
    //        {
    //            this.testBytes = new byte[this.TestLength];
    //        }

    //        ChatSocketManager.onSendMsg.Invoke(ConstantsHolder.xanaConstants.MuseumID,inputLine);
    //    }
    //}
    //public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
    //{
    //    if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
    //    {
    //        Debug.Log(message);
    //    }
    //    else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
    //    {
    //        Debug.LogWarning(message);
    //    }
    //    else
    //    {
    //        Debug.Log(message);
    //    }
    //}
    //public void OnConnected()
    //{
    //    CheckPlayerPrefItems();

    //    //if (this.ChannelsToJoinOnConnect != null)
    //    //{
    //    //    if (!ConstantsHolder.xanaConstants.isBuilderScene)
    //    //    {
    //    //        if (SceneManager.GetActiveScene().name == "AddressableScene")
    //    //        {
    //    //            this.ChannelsToJoinOnConnect = FeedEventPrefab.m_EnvName;// FeedEventPrefab
    //    //        }
    //    //        else
    //    //        {
    //    //            this.ChannelsToJoinOnConnect = SceneManager.GetActiveScene().name;// FeedEventPrefab
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        this.ChannelsToJoinOnConnect = ConstantsHolder.xanaConstants.builderMapID+ FeedEventPrefab.m_EnvName;
    //    //    }
    //    //  //  this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
    //    //    //Debug.Log(this.ChannelsToJoinOnConnect);
    //    //}

    //    //this.ConnectingLabel.SetActive(false);

    //    //this.UserIdText.text = "Connected as " + this.UserName;

    //    this.ChatPanel.gameObject.SetActive(true);

    //    #region Friend List - Might be useful in a later update
    //    //if (this.FriendsList != null && this.FriendsList.Length > 0)
    //    //{
    //    //    this.chatClient.AddFriends(this.FriendsList); // Add some users to the server-list to get their status updates

    //    //    // add to the UI as well
    //    //    foreach (string _friend in this.FriendsList)
    //    //    {
    //    //        if (this.FriendListUiItemtoInstantiate != null && _friend != this.UserName)
    //    //        {
    //    //            this.InstantiateFriendButton(_friend);
    //    //        }

    //    //    }

    //    //}

    //    //if (this.FriendListUiItemtoInstantiate != null)
    //    //{
    //    //    this.FriendListUiItemtoInstantiate.SetActive(false);
    //    //}
    //    #endregion

    //    //this.chatClient.SetOnlineStatus(ChatUserStatus.Online); // You can set your online state (without a mesage).
    //}
    //public void OnDisconnected()
    //{
    //    //this.ConnectingLabel.SetActive(false);
    //    if (!isQuitGame)
    //        Connect();
    //}
    //public void OnSubscribed(string[] channels, bool[] results)
    //{
    //    // in this demo, we simply send a message into each channel. This is NOT a must have!
    //    foreach (string channel in channels)
    //    {
    //        if (sayHiOnJoiningChannel)
    //        {
    //            Debug.Log("Currently Joined Channel: " + channel);

    //            //if (Application.systemLanguage == SystemLanguage.English)
    //            //    this.chatClient.PublishMessage(channel, " has joined."); // you don't HAVE to send a msg on join but you could.
    //            //else
    //            //    this.chatClient.PublishMessage(channel, " 参加しました");
    //        }

    //        if (this.ChannelToggleToInstantiate != null)
    //        {
    //            this.InstantiateChannelButton(channel);

    //        }
    //    }

    //    Debug.Log("OnSubscribed: " + string.Join(", ", channels));

    //    /*
    //    // select first subscribed channel in alphabetical order
    //    if (this.chatClient.PublicChannels.Count > 0)
    //    {
    //        var l = new List<string>(this.chatClient.PublicChannels.Keys);
    //        l.Sort();
    //        string selected = l[0];
    //        if (this.channelToggles.ContainsKey(selected))
    //        {
    //            ShowChannel(selected);
    //            foreach (var c in this.channelToggles)
    //            {
    //                c.Value.isOn = false;
    //            }
    //            this.channelToggles[selected].isOn = true;
    //            AddMessageToSelectedChannel(WelcomeText);
    //        }
    //    }
    //    */

    //    // Switch to the first newly created channel
    //    this.ShowChannel(channels[0]);
    //}

    ///// <inheritdoc />
    //public void OnSubscribed(string channel, string[] users, Dictionary<object, object> properties)
    //{
    //    Debug.LogFormat("OnSubscribed: {0}, users.Count: {1} Channel-props: {2}.", channel, users.Length, properties.ToStringFull());
    //}
    //private void InstantiateChannelButton(string channelName)
    //{
    //    if (this.channelToggles.ContainsKey(channelName))
    //    {
    //        Debug.Log("Skipping creation for an existing channel toggle.");
    //        return;
    //    }

    //    Toggle cbtn = (Toggle)Instantiate(this.ChannelToggleToInstantiate);
    //    cbtn.gameObject.SetActive(true);
    //  //  cbtn.GetComponentInChildren<ChannelSelector>().SetChannel(channelName);
    //    cbtn.transform.SetParent(this.ChannelToggleToInstantiate.transform.parent, false);

    //    this.channelToggles.Add(channelName, cbtn);
    //}
    //// Might be useful in a later update. If we want to add friends to chat with.
    //private void InstantiateFriendButton(string friendId)
    //{
    //    GameObject fbtn = (GameObject)Instantiate(this.FriendListUiItemtoInstantiate);
    //    fbtn.gameObject.SetActive(true);
    //   // FriendItem _friendItem = fbtn.GetComponent<FriendItem>();

    //   // _friendItem.FriendId = friendId;

    //    fbtn.transform.SetParent(this.FriendListUiItemtoInstantiate.transform.parent, false);

    //   // this.friendListItemLUT[friendId] = _friendItem;
    //}
    //public void OnUnsubscribed(string[] channels)
    //{
    //    foreach (string channelName in channels)
    //    {
    //        if (this.channelToggles.ContainsKey(channelName))
    //        {
    //            Toggle t = this.channelToggles[channelName];
    //            Destroy(t.gameObject);

    //            this.channelToggles.Remove(channelName);

    //            Debug.Log("Unsubscribed from channel '" + channelName + "'.");

    //            // Showing another channel if the active channel is the one we unsubscribed from before
    //            if (channelName == this.selectedChannelName && this.channelToggles.Count > 0)
    //            {
    //                IEnumerator<KeyValuePair<string, Toggle>> firstEntry = this.channelToggles.GetEnumerator();
    //                firstEntry.MoveNext();

    //                this.ShowChannel(firstEntry.Current.Key);

    //                firstEntry.Current.Value.isOn = true;
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("Can't unsubscribe from channel '" + channelName + "' because you are currently not subscribed to it.");
    //        }
    //    }
    //}
    //public void OnGetMessages(string channelName, string[] senders, object[] messages)
    //{
    //    if (XanaChatPotrait.activeInHierarchy)
    //    {

    //        if (channelName.Equals(this.selectedChannelName))
    //        {
    //            // update text
    //            this.ShowChannel(this.selectedChannelName);

    //            if (GameManager.currentLanguage == "en")
    //            {
    //                if (senders[senders.Length - 1].Length > 12)
    //                {
    //                    this.PotriatCurrentChannelText.text =
    //                    "<b>" + senders[senders.Length - 1].Substring(0, 12) + "...</b>" + " : " + (string)messages[messages.Length - 1] + "\n" + this.PotriatCurrentChannelText.text;
    //                }
    //                else
    //                {
    //                    this.PotriatCurrentChannelText.text =
    //                   "<b>" + senders[senders.Length - 1].Substring(0, senders[senders.Length - 1].Length) + "</b>" + "<space=0.3em>:<space=0.3em>" + (string)messages[messages.Length - 1] + "\n" + this.PotriatCurrentChannelText.text;
    //                }

    //            }
    //            else
    //            if (GameManager.currentLanguage == "ja")
    //            {
    //                //this.CurrentChannelText.text +=
    //                //    senders[senders.Length - 1] + " : " + (string)messages[messages.Length - 1] + "\n";
    //                // commented by Usman Aslam
    //                string lastMessage = (string)messages[messages.Length - 1];
    //                if (lastMessage.Contains("has joined"))
    //                {
    //                    print(senders[senders.Length - 1]);
    //                    if (senders[senders.Length - 1].Length > 12)
    //                    {
    //                        SetJapaneseTextPotriat("<b>" + senders[senders.Length - 1].Substring(0, 12) + "...</b>", true, lastMessage);
    //                    }
    //                    else
    //                    {
    //                        SetJapaneseTextPotriat("<b>" + senders[senders.Length - 1] + "</b>", true, lastMessage);
    //                    }


    //                }
    //                else
    //                {
    //                    if (senders[senders.Length - 1].Length > 12)
    //                    {
    //                        SetJapaneseTextPotriat("<b>" + senders[senders.Length - 1].Substring(0, 12) + "</b>", false, lastMessage);
    //                    }
    //                    else
    //                    {
    //                        SetJapaneseTextPotriat("<b>" + senders[senders.Length - 1] + "</b>", false, lastMessage);
    //                    }

    //                }

    //            }

    //            if (!chatDialogBox.activeSelf && senders[senders.Length - 1] != UserName)
    //            {
    //                chatNotificationIcon.SetActive(true);
    //            }
    //        }
    //    }
    //    else if(XanaChatLand.activeInHierarchy)
    //    {
    //        if (channelName.Equals(this.selectedChannelName))
    //        {
    //            // update text
    //            this.ShowChannel(this.selectedChannelName);

    //            if (GameManager.currentLanguage == "en")
    //            {
    //                if (senders[senders.Length - 1].Length > 12)
    //                {
    //                    this.CurrentChannelText.text =
    //                    "<b>" + senders[senders.Length - 1].Substring(0, 12) + "...</b>" + " : " + (string)messages[messages.Length - 1] + "\n" + this.CurrentChannelText.text;
    //                }
    //                else
    //                {
    //                    this.CurrentChannelText.text =
    //                   "<b>" + senders[senders.Length - 1].Substring(0, senders[senders.Length - 1].Length) + "</b>" + "<space=0.3em>:<space=0.3em>" + (string)messages[messages.Length - 1] + "\n" + this.CurrentChannelText.text;
    //                }

    //            }
    //            else
    //            if (GameManager.currentLanguage == "ja")
    //            {
    //                //this.CurrentChannelText.text +=
    //                //    senders[senders.Length - 1] + " : " + (string)messages[messages.Length - 1] + "\n";
    //                // commented by Usman Aslam
    //                string lastMessage = (string)messages[messages.Length - 1];
    //                if (lastMessage.Contains("has joined"))
    //                {
    //                    print(senders[senders.Length - 1]);
    //                    if (senders[senders.Length - 1].Length > 12)
    //                    {
    //                        SetJapaneseText("<b>" + senders[senders.Length - 1].Substring(0, 12) + "...</b>", true, lastMessage);
    //                    }
    //                    else
    //                    {
    //                        SetJapaneseText("<b>" + senders[senders.Length - 1] + "</b>", true, lastMessage);
    //                    }


    //                }
    //                else
    //                {
    //                    if (senders[senders.Length - 1].Length > 12)
    //                    {
    //                        SetJapaneseText("<b>" + senders[senders.Length - 1].Substring(0, 12) + "</b>", false, lastMessage);
    //                    }
    //                    else
    //                    {
    //                        SetJapaneseText("<b>" + senders[senders.Length - 1] + "</b>", false, lastMessage);
    //                    }

    //                }

    //            }

    //            if (!chatDialogBox.activeSelf && senders[senders.Length - 1] != UserName)
    //            {
    //                chatNotificationIcon.SetActive(true);
    //            }
    //        }
    //    }



    //    StartCoroutine(Delay());
    //}
    //public void OnGetMessagesPotrait(string channelName, string[] senders, object[] messages)
    //{



    //    StartCoroutine(Delay());
    //}
    //public void OnPrivateMessage(string sender, object message, string channelName)
    //{
    //    // as the ChatClient is buffering the messages for you, this GUI doesn't need to do anything here
    //    // you also get messages that you sent yourself. in that case, the channelName is determinded by the target of your msg
    //    this.InstantiateChannelButton(channelName);

    //    byte[] msgBytes = message as byte[];
    //    if (msgBytes != null)
    //    {
    //        Debug.Log("Message with byte[].Length: " + msgBytes.Length);
    //    }
    //    if (this.selectedChannelName.Equals(channelName))
    //    {
    //        this.ShowChannel(channelName);
    //    }
    //}

    ///// <summary>
    ///// New status of another user (you get updates for users set in your friends list).
    ///// </summary>
    ///// <param name="user">Name of the user.</param>
    ///// <param name="status">New status of that user.</param>
    ///// <param name="gotMessage">True if the status contains a message you should cache locally. False: This status update does not include a
    ///// message (keep any you have).</param>
    ///// <param name="message">Message that user set.</param>
    //public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    //{

    //    Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));

    //    //if (this.friendListItemLUT.ContainsKey(user))
    //    //{
    //    //    FriendItem _friendItem = this.friendListItemLUT[user];
    //    //    if (_friendItem != null) _friendItem.OnFriendStatusUpdate(status, gotMessage, message);
    //    //}
    //}
    //public void OnUserSubscribed(string channel, string user)
    //{
    //    Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    //}
    //public void OnUserUnsubscribed(string channel, string user)
    //{
    //    Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    //}
    ///// <inheritdoc />
    //public void OnChannelPropertiesChanged(string channel, string userId, Dictionary<object, object> properties)
    //{
    //    //Debug.LogFormat("OnChannelPropertiesChanged: {0} by {1}. Props: {2}.", channel, userId, Extensions.ToStringFull(properties));
    //}
    //public void OnUserPropertiesChanged(string channel, string targetUserId, string senderUserId, Dictionary<object, object> properties)
    //{
    //    //Debug.LogFormat("OnUserPropertiesChanged: (channel:{0} user:{1}) by {2}. Props: {3}.", channel, targetUserId, senderUserId, Extensions.ToStringFull(properties));
    //}
    ///// <inheritdoc />
    //public void OnErrorInfo(string channel, string error, object data)
    //{
    //    Debug.LogFormat("OnErrorInfo for channel {0}. Error: {1} Data: {2}", channel, error, data);
    //}
    //public void AddMessageToSelectedChannel(string msg)
    //{
    //    //ChatChannel channel = null;
    //    //bool found = this.chatClient.TryGetChannel(this.selectedChannelName, out channel);
    //    //if (!found)
    //    //{
    //    //    Debug.Log("AddMessageToSelectedChannel failed to find channel: " + this.selectedChannelName);
    //    //    return;
    //    //}

    //    //if (channel != null)
    //    //{
    //    //    channel.Add("Bot", msg, 0); //TODO: how to use msgID?
    //    //}
    //}
    //private void SetJapaneseText(string UserID, bool IsInfoMessage, string message)
    //{
    //    string temp = null;
    //    if (IsInfoMessage)
    //    {
    //        if (!UserID.IsNullOrEmpty())
    //        {
    //            if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
    //            {
    //                temp = "ゲスト<space=0.3em>:<space=0.3em>参加しました";
    //                string[] GuestIDs = Regex.Split(UserID, @"\D+");

    //                if (!GuestIDs[1].IsNullOrEmpty())
    //                {
    //                    string final = temp.Insert(3, GuestIDs[1]);

    //                    CurrentChannelText.text = final + '\n' + CurrentChannelText.text;
    //                }
    //            }
    //            else
    //            {
    //                temp = UserID + "<space=0.3em>:<space=0.3em>参加しました";
    //                CurrentChannelText.text = temp + '\n' + CurrentChannelText.text;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        temp = "ゲスト";
    //        string[] GuestIDs = Regex.Split(UserID, @"\D+");
    //        Debug.Log(GuestIDs.Length);
    //        if (!GuestIDs[0].IsNullOrEmpty())
    //        {
    //            temp += GuestIDs[0] + " : ";
    //            string final = temp + message;
    //            print(final);
    //            CurrentChannelText.text = final + '\n' + CurrentChannelText.text;
    //        }
    //        else
    //        {
    //            temp = UserID;
    //            temp += " : ";
    //            string final = temp + message;
    //            CurrentChannelText.text = final + '\n' + CurrentChannelText.text;
    //        }
    //    }
    //}
    //private void SetJapaneseTextPotriat(string UserID, bool IsInfoMessage, string message)
    //{
    //    string temp = null;
    //    if (IsInfoMessage)
    //    {
    //        if (!UserID.IsNullOrEmpty())
    //        {
    //            if (PlayerPrefs.GetInt("IsLoggedIn") == 0)
    //            {
    //                temp = "ゲスト<space=0.3em>:<space=0.3em>参加しました";
    //                string[] GuestIDs = Regex.Split(UserID, @"\D+");

    //                if (!GuestIDs[1].IsNullOrEmpty())
    //                {
    //                    string final = temp.Insert(3, GuestIDs[1]);

    //                    PotriatCurrentChannelText.text = final + '\n' + PotriatCurrentChannelText.text;
    //                }
    //            }

    //            else
    //            {

    //                temp = UserID + "<space=0.3em>:<space=0.3em>参加しました";
    //                PotriatCurrentChannelText.text = temp + '\n' + PotriatCurrentChannelText.text;
    //            }
    //        }
    //        else
    //        {

    //        }
    //    }
    //    else
    //    {

    //        temp = "ゲスト";
    //        string[] GuestIDs = Regex.Split(UserID, @"\D+");
    //        Debug.Log(GuestIDs.Length);
    //        if (!GuestIDs[0].IsNullOrEmpty())
    //        {
    //            temp += GuestIDs[0] + " : ";
    //            string final = temp + message;
    //            print(final);
    //            PotriatCurrentChannelText.text = final + '\n' + PotriatCurrentChannelText.text;
    //        }
    //        else
    //        {
    //            temp = UserID;
    //            temp += " : ";
    //            string final = temp + message;
    //            PotriatCurrentChannelText.text = final + '\n' + PotriatCurrentChannelText.text;
    //        }
    //    }

    //}
    //public void ShowChannel(string channelName)
    //{


    //    if (string.IsNullOrEmpty(channelName))
    //    {
    //        return;
    //    }

    //    //ChatChannel channel = null;
    //    //bool found = this.chatClient.TryGetChannel(channelName, out channel);
    //    //if (!found)
    //    //{
    //    //    Debug.Log("ShowChannel failed to find channel: " + channelName);
    //    //    return;
    //    //}

    //    this.selectedChannelName = channelName;


    //    Debug.Log("ShowChannel: " + this.selectedChannelName);


    //    foreach (KeyValuePair<string, Toggle> pair in this.channelToggles)
    //    {
    //        pair.Value.isOn = pair.Key == channelName ? true : false;
    //    }
    //}

    //public void OpenDashboard()
    //{
    //    Application.OpenURL("https://dashboard.photonengine.com");
    //}

    #endregion


}
