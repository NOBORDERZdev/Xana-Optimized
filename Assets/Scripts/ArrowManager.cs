using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Voice.PUN;
using UnityEngine.Animations.Rigging;

public class ArrowManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    GameObject arrow;
    Material clientMat;
    Material playerMat;
    Transform mainPlayerParent;
    private bool iscashed = false;
    Dictionary<object, object> cashed_data = new Dictionary<object, object>();

    public bool isBear;
    public TMPro.TextMeshProUGUI PhotonUserName;
    public Image VoiceImage;
    public bool IsSpeak;
    public GameObject ChatShow;
    //public GameObject ChatShowSecond;
    public GameObject reactionUi;
    private List<EventData> data = new List<EventData>();
    private List<EventData> chatData = new List<EventData>();
    private PhotonView[] photonplayerObjects;

    public delegate void ReactionDelegate(string iconUrl);
    public delegate void CommentDelegate(string iconUrl);
    public static event ReactionDelegate ReactionDelegateButtonClickEvent;
    public static event CommentDelegate CommentDelegateButtonClickEvent;


    public static int viewID;
    public static string parentTransform;

    public static ArrowManager Instance;
    Coroutine temp = null;
    Coroutine chatco = null;

    public PhotonVoiceView VoiceView;

    //Gautam added for changing the position of the name canvas for avatar changer
    internal Canvas nameCanvas;

    private void Awake()
    {

        Instance = this;

    }
    void Start()
    {
        nameCanvas = PhotonUserName.GetComponentInParent<Canvas>();
        try
        {
            if (ConstantsHolder.userName.Length > 12)
            {
                PhotonNetwork.NickName = ConstantsHolder.userName.Substring(0, 12) + "...";
            }
            else
            {
                PhotonNetwork.NickName = ConstantsHolder.userName;
            }
        }
        catch(Exception e)
        {
            PhotonNetwork.NickName = ConstantsHolder.userName;
        }
       
        arrow = Resources.Load<GameObject>("Arrow");
        Material _mat = Resources.Load<Material>("Material #25");
        clientMat = playerMat = _mat;
        //clientMat = Resources.Load<Material>("Material #27");
        //playerMat = Resources.Load<Material>("Material #25");
        if (this.GetComponent<PhotonView>().IsMine)
        {
            if (ConstantsHolder.xanaConstants.isBuilderScene)
                GamificationComponentData.instance.nameCanvas = PhotonUserName.GetComponentInParent<Canvas>();
            if(SMBCManager.Instance)
                SMBCManager.Instance.NameCanvas= PhotonUserName.GetComponentInParent<Canvas>();
            if (AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer == null)
            {
                AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer = this.gameObject;
                PhotonUserName.text = PhotonNetwork.NickName;

                if(!ConstantsHolder.isPenguin)
                {
                    AvatarSpawnerOnDisconnect.Instance.spawnPoint.GetComponent<PlayerController>().animator = this.GetComponent<Animator>();
                    AvatarSpawnerOnDisconnect.Instance.spawnPoint.GetComponent<PlayerController>().playerRig = GetComponent<FirstPersonJump>().jumpRig;
                }
            }
        }
        StartCoroutine(WaitForArrowIntanstiate(this.transform, !this.GetComponent<PhotonView>().IsMine));
        try
        {
            if(AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer)
                AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer.GetComponent<IKMuseum>().Initialize();
        }
        catch (Exception e)
        {
            print(e.Message);
        }
        VoiceView = GetComponent<PhotonVoiceView>();
    }

    public void Update()
    {
        if (VoiceView != null)
        {
            if (VoiceView.IsSpeaking)
            {
                VoiceImage.gameObject.SetActive(true);
                IsSpeak = true;
            }
            else
            {
                VoiceImage.gameObject.SetActive(false);
                IsSpeak = false;
            }
        }
    }
    public static void OnInvokeReactionButtonClickEvent(string url)
    {
        ReactionDelegateButtonClickEvent?.Invoke(url);
    }
   
    public static void OnInvokeCommentButtonClickEvent(string text)
    {

        CommentDelegateButtonClickEvent?.Invoke(text);
    }


    public override void OnEnable()
    {
        ReactionDelegateButtonClickEvent += OnChangeReactionIcon;
        CommentDelegateButtonClickEvent += OnChangeText;
        ConstantsHolder.userNameToggleDelegate += OnChangeUsernameToggle;
        OnChangeUsernameToggle(ConstantsHolder.xanaConstants.userNameVisibilty);
        base.OnEnable();

    }

    public override void OnDisable ()
    {
        ReactionDelegateButtonClickEvent -= OnChangeReactionIcon;
        CommentDelegateButtonClickEvent -= OnChangeText;
        ConstantsHolder.userNameToggleDelegate -= OnChangeUsernameToggle;
        base.OnDisable();

    }


    private void OnChangeReactionIcon(string url)
    {
        if ((!string.IsNullOrEmpty(url)))
        {
            gameObject.GetComponent<PhotonView>().RPC("sendDataReactionUrl", RpcTarget.All, url, ReferencesForGamePlay.instance.m_34player.GetComponent<PhotonView>().ViewID);
        }
    }
    private void OnChangeUsernameToggle(int userNameToggleConstant)
    {
        if (userNameToggleConstant == 1)
        {
            Debug.Log("Onbtn:" + ReferencesForGamePlay.instance.onBtnUsername);
            PhotonUserName.enabled = true;

        }
        else
        {
            Debug.Log("Offbtn:" + ReferencesForGamePlay.instance.onBtnUsername);
            PhotonUserName.enabled = false;
        }

        //  gameObject.GetComponent<PhotonView>().RPC("sendDataUserNAmeToggle", RpcTarget.All, userNameToggleConstant, ReferencesForGamePlay.instance.m_34player.GetComponent<PhotonView>().ViewID);  Zeel Commented We have to disable for our side oly why we are disabling for all other players as well
    }
    private void OnChangeText(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            gameObject.GetComponent<PhotonView>().RPC("sendDataChatMsg", RpcTarget.All, text, ReferencesForGamePlay.instance.m_34player.GetComponent<PhotonView>().ViewID);
            text = string.Empty;
        }
    }

    IEnumerator LoadSpriteEnv(string ImageUrl, int id)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
        }
        else
        {
            if (gameObject.GetComponent<PhotonView>().ViewID == id)
            {
                using (WWW www = new WWW(ImageUrl))
                {

                    while (!www.isDone)
                    {
                        yield return null;
                    }

                    yield return www;
                    if (www.error != null)
                    {

                        throw new Exception("WWW download had an error:" + www.error);
                    }
                    else
                    {

                        if (!string.IsNullOrEmpty(ImageUrl))
                        {
                            UnityWebRequest www1 = UnityWebRequestTexture.GetTexture(ImageUrl);
                            www1.SendWebRequest();
                            while (!www1.isDone)
                            {
                                yield return null;
                            }


                            Texture2D thumbnailTexture = DownloadHandlerTexture.GetContent(www1);
                            //thumbnailTexture.Compress(true);
                            if (Application.internetReachability == NetworkReachability.NotReachable)
                            {

                            }
                            else
                            {
                                Sprite sprite = Sprite.Create(thumbnailTexture, new Rect(0, 0, thumbnailTexture.width, thumbnailTexture.height), new Vector2(0, 0));
                                if (reactionUi != null)
                                {
                                    reactionUi.SetActive(true);
                                    reactionUi.GetComponent<Image>().sprite = sprite;

                                    yield return new WaitForSeconds(5f);
                                    PlayerPrefs.SetString(ConstantsGod.ReactionThumb, "");
                                    reactionUi.SetActive(false);
                                }
                                else
                                {

                                }
                            }

                            www.Dispose();
                        }
                    }

                }
            }
        }
    }
    IEnumerator ChatShowData(string chatData, int id)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
        }
        else
        {
            if (gameObject.GetComponent<PhotonView>().ViewID == id)
            {
                if (chatData.Length <= 20)
                {
                    if (ChatShow != null)
                    {
                        ChatShow.SetActive(true);
                        ChatShow.transform.GetChild(0).GetComponent<LayoutElement>().enabled = false;

                        ChatShow.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = chatData;

                        yield return new WaitForSeconds(5f);
                        PlayerPrefs.SetString(ConstantsGod.SENDMESSAGETEXT, "");
                        ChatShow.SetActive(false);
                    }
                }
                else
                {
                    if (ChatShow != null)
                    {
                        ChatShow.SetActive(true);
                        ChatShow.transform.GetChild(0).GetComponent<LayoutElement>().enabled = true;
                        ChatShow.transform.GetChild(0).GetComponent<LayoutElement>().preferredWidth = 18;

                        ChatShow.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = chatData;

                        yield return new WaitForSeconds(5f);
                        PlayerPrefs.SetString(ConstantsGod.SENDMESSAGETEXT, "");
                        ChatShow.SetActive(false);
                    }
                }
            }
        }
    }


    [PunRPC]
    public void sendDataReactionUrl(string url, int viewId)
    {
        PlayerPrefs.SetString(ConstantsGod.SENDMESSAGETEXT, "");
        ChatShow.SetActive(false);
        if (temp != null)
        {
            StopCoroutine(temp);
        }
        temp = StartCoroutine(LoadSpriteEnv(url, viewId));
    }
    [PunRPC]
    public void sendDataUserNAmeToggle(int UserNameContantToggle, int viewId)
    {
        Debug.Log("ThisidToggle:" + UserNameContantToggle);
        Debug.Log("ThisitOGGLEid:" + viewId);
        NameToggle(UserNameContantToggle, viewId);


    }
    public void NameToggle(int ToggleConstant, int id)
    {
        if (gameObject.GetComponent<PhotonView>().ViewID == id)
        {
            //Debug.Log("USERNAME VALUE:" + ConstantsHolder.xanaConstants.userName);
            if (ToggleConstant == 1)
            {
                Debug.Log("Onbtn:" + ReferencesForGamePlay.instance.onBtnUsername);
                PhotonUserName.enabled = true;

            }
            else
            {
                Debug.Log("Offbtn:" + ReferencesForGamePlay.instance.onBtnUsername);
                PhotonUserName.enabled = false;
            }
        }

    }


    [PunRPC]
    public void sendDataChatMsg(string chat, int viewId)
    {
        Debug.LogError("RPC chat :- "+chat+"--"+viewId);
        PlayerPrefs.SetString(ConstantsGod.ReactionThumb, "");
        reactionUi.SetActive(false);
        if (chatco != null)
        {
            StopCoroutine(chatco);
        }
        chatco = StartCoroutine(ChatShowData(chat, viewId));
    }

    //Dictionary<object, object> reactDic = new Dictionary<object, object>();
    //reactDic.Add(GameObject.FindGameObjectWithTag("Player").transform.GetChild(18).GetComponent<PhotonView>().ViewID.ToString(), chat);


    //RaiseEventOptions options = new RaiseEventOptions();
    //options.CachingOption = EventCaching.DoNotCache;
    //options.Receivers = ReceiverGroup.All;
    //PhotonNetwork.RaiseEvent(44, reactDic, options,
    //    SendOptions.SendReliable);


    //cashed_data = reactDic;
    //iscashed = true;
    //Debug.Log("data send sucessfully==" + reactDic.Count);





    IEnumerator WaitForArrowIntanstiate(Transform parent, bool isOtherPlayer)
    {
        yield return new WaitForSeconds(1.0f);
        InstantiateArrow(this.transform, !this.GetComponent<PhotonView>().IsMine);
    }

    //riken created
    public void CallFirstPersonRPC(bool isFirstPerson)
    {
        this.GetComponent<PhotonView>().RPC("SendDataIfPlayerSetFirstPersonView", RpcTarget.Others, isFirstPerson, this.GetComponent<PhotonView>().ViewID);
    }
    [PunRPC]
    public void SendDataIfPlayerSetFirstPersonView(bool isFirstPerson, int viewId)
    {

    }

    public void InstantiateArrow(Transform parent, bool isOtherPlayer)
    {

        GameObject go = Instantiate(arrow, parent);
        go.layer = 17;
        if (isOtherPlayer)
        {
            PhotonUserName.text = gameObject.GetComponent<PhotonView>().Owner.NickName;
            Debug.Log("nick name 4==" + gameObject.GetComponent<PhotonView>().Owner.NickName);
            if ((!string.IsNullOrEmpty(PlayerPrefs.GetString(ConstantsGod.ReactionThumb)))
                   && !PlayerPrefs.GetString(ConstantsGod.ReactionThumb).Equals(ConstantsGod.ReactionThumb))
            {
                // StartCoroutine(LoadSpriteEnv(PlayerPrefs.GetString(ConstantsGod.ReactionThumb), reactionUi));
            }


            go.transform.localPosition = new Vector3(-0.27f, 0.37f, -10.03f);
            go.transform.localEulerAngles = new Vector3(-85, -113.1f, -65);
            go.transform.localScale = new Vector3(2.35f, 2f, 1);

            //go.AddComponent<ChangeGear>();
            // go.AddComponent<Equipment>();
            //  GameObject.FindGameObjectWithTag("DCloth").GetComponent<DefaultClothes>()._DefaultInitializer();
        }
        //else
        {
            if(ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Summit"))
            {
                go.transform.localPosition = new Vector3(-1.6f, -1.46f, -50f);
                go.transform.localEulerAngles = new Vector3(-85, -113.1f, -65);
                go.transform.localScale = new Vector3(10.0f, 10f, 1);
            }
            else
            {
                // Old Default Position = Vector3(-0.74f, 0.1f, -26f);
                // Old Default Scale = Vector3(6.0f, 5.25f, 1);

                go.transform.localPosition = new Vector3(-.98f, 0.43f, -18.73f);
                go.transform.localEulerAngles = new Vector3(-85, -113.1f, -65);
                go.transform.localScale = new Vector3(4.0f, 3.8f, 1); 
            }
            

            //EmoteAnimationHandler.Instance.controller = (AnimatorController)EmoteAnimationHandler.Instance.animator.runtimeAnimatorController;
            //// var state = controller.layers[0].stateMachine.defaultState;
            //var state = EmoteAnimationHandler.Instance.controller.layers[0].stateMachine.states.FirstOrDefault(s => s.state.name.Equals("Animation")).state;
            //Debug.Log("states===" + state.name);
            //if (state == null)
            //{
            //    Debug.LogError("Couldn't get the state!");

            //}
            //try
            //{

            //}catch()
            //EmoteAnimationHandler.Instance.controller.SetStateEffectiveMotion(state, EmoteAnimationHandler.Instance.spawnCharacterObject.transform.GetComponent<Animation>().clip);
        }
        go.GetComponent<MeshRenderer>().material = playerMat;


        //if (isOtherPlayer)
        //{
        //    go.GetComponent<MeshRenderer>().material = clientMat;

        //    //go.AddComponent<ChangeGear>();
        //    // go.AddComponent<Equipment>();
        //    //  GameObject.FindGameObjectWithTag("DCloth").GetComponent<DefaultClothes>()._DefaultInitializer();
        //}
        //else
        //{
        //    go.GetComponent<MeshRenderer>().material = playerMat;
        //}

        if (isBear)
        {
            go.SetActive(false);
        }

        //LoadingManager.Instance.HideLoading();
        //LoadingHandler.Instance.HideLoading();

        if (ConstantsHolder.xanaConstants.IsMuseum && WorldItemView.m_EnvName.Contains("J & J WORLD_5"))
            go.SetActive(false);
        if (SoundController.Instance)
        {

            SoundController.Instance.PlayBGM();
        }
    }


    #region ToyotaMeetingArea
    public void UpdateMeetingTxt(string message)
    {
        this.GetComponent<PhotonView>().RPC("RemoteUpdateTxt", RpcTarget.AllBuffered, message); //ReferencesForGamePlay.instance.m_34player.GetComponent<PhotonView>().ViewID
    }

    [PunRPC]
    public void RemoteUpdateTxt(string message) //, int ViewID
    {
        if (NFT_Holder_Manager.instance && NFT_Holder_Manager.instance.meetingTxtUpdate != null)
            NFT_Holder_Manager.instance.meetingTxtUpdate.UpdateMeetingTxt(message);
    }


    public void ChangeVoiceGroup(int ViewID, byte newGroup)
    {
        this.GetComponent<PhotonView>().RPC("RemoteChangeVoice", RpcTarget.All, ViewID, newGroup);
    }
    [PunRPC]
    public void RemoteChangeVoice(int ViewID, byte newGroup) //, int ViewID
    {
        if (gameObject.GetComponent<PhotonView>().ViewID == ViewID)
            FindObjectOfType<VoiceManager>().SetVoiceGroup(newGroup);
    }
    #endregion

}
