using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Metaverse;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Voice.PUN;

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
    public delegate void UserNameToggleDeligate(int userNameToggleConstant);
    public static event UserNameToggleDeligate userNameToggleDelegate;
    public static int viewID;
    public static string parentTransform;

    public static ArrowManager Instance;
    Coroutine temp = null;
    Coroutine chatco = null;

    public PhotonVoiceView VoiceView;

    private void Awake()
    {

        Instance = this;

    }
    void Start()
    {

        //if (XanaConstants.xanaConstants.userName == 1)
        //{
        //    PhotonUserName.enabled = true;
        //}
        //else {

        //    PhotonUserName.enabled = false;
        //}

        if (XanaChatSystem.instance.UserName.Length > 12)
        {
            PhotonNetwork.NickName = XanaChatSystem.instance.UserName.Substring(0, 12) + "...";
        }
        else
        {
            PhotonNetwork.NickName = XanaChatSystem.instance.UserName;
        }

        arrow = Resources.Load<GameObject>("Arrow");
        clientMat = Resources.Load<Material>("Material #27");
        playerMat = Resources.Load<Material>("Material #25");
        mainPlayerParent = AvatarManager.Instance.spawnPoint.transform;
        print("nick name 3 4==" + XanaChatSystem.instance.UserName);
        if (this.GetComponent<PhotonView>().IsMine)
        {
            if (AvatarManager.Instance.currentDummyPlayer == null)
            {
                this.transform.parent = mainPlayerParent;
                this.transform.localPosition = new Vector3(0, -0.081f, 0);
                this.transform.localEulerAngles = new Vector3(0, 0, 0);
                AvatarManager.Instance.currentDummyPlayer = this.gameObject;
                print("nick name 3==" + XanaChatSystem.instance.UserName);
                PhotonUserName.text = PhotonNetwork.NickName;

                //if ((!string.IsNullOrEmpty(PlayerPrefs.GetString(ConstantsGod.ReactionThumb)))
                //    && !PlayerPrefs.GetString(ConstantsGod.ReactionThumb).Equals(ConstantsGod.ReactionThumb))
                //{
                //    //StartCoroutine(LoadSpriteEnv(PlayerPrefs.GetString(ConstantsGod.ReactionThumb),reactionUi));
                //    sendDataReactionUrl(PlayerPrefs.GetString(ConstantsGod.ReactionThumb));
                //}



                AvatarManager.Instance.spawnPoint.GetComponent<PlayerControllerNew>().animator = this.GetComponent<Animator>();
                //AvatarManager.Instance.spawnPoint.GetComponent<EmoteAnimationPlay>().animatorremote = this.GetComponent<Animator>();
                AvatarManager.Instance.spawnPoint.GetComponent<PlayerControllerNew>().playerRig = GetComponent<FirstPersonJump>().jumpRig;

                AvatarManager.Instance.Defaultanimator = AvatarManager.Instance.currentDummyPlayer.transform.GetComponent<Animator>().runtimeAnimatorController;
            }
        }
        StartCoroutine(WaitForArrowIntanstiate(this.transform, !this.GetComponent<PhotonView>().IsMine));
        Debug.Log("call arrow");
        //GameObject myobj = GameObject.FindGameObjectWithTag("PhotonLocalPlayer");
        //Debug.Log("Arrow manager for is mine " + myobj.GetComponent<PhotonView>().IsMine + "view id object==" + myobj.GetComponent<PhotonView>().ViewID);
        //myobj.GetComponent<RPCCallforBufferPlayers>().sendData();

        try
        {
            AvatarManager.Instance.currentDummyPlayer.GetComponent<IKMuseum>().Initialize();
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
                //Debug.Log("Speaker in use true");
                VoiceImage.gameObject.SetActive(true);
                IsSpeak = true;
            }
            else
            {
               // Debug.Log("Speaker in use false");
                VoiceImage.gameObject.SetActive(false);
                IsSpeak = false;
            }
        }
    }
    public static void OnInvokeReactionButtonClickEvent(string url)
    {
        ReactionDelegateButtonClickEvent?.Invoke(url);
    }
    public static void OnInvokeUsername(int userNameToggle)
    {
        userNameToggleDelegate?.Invoke(userNameToggle);
    }
    public static void OnInvokeCommentButtonClickEvent(string text)
    {

        CommentDelegateButtonClickEvent?.Invoke(text);
    }


    private void OnEnable()
    {
        ReactionDelegateButtonClickEvent += OnChangeReactionIcon;
        CommentDelegateButtonClickEvent += OnChangeText;
        userNameToggleDelegate += OnChangeUsernameToggle;

    }

    private void OnDisable()
    {
        ReactionDelegateButtonClickEvent -= OnChangeReactionIcon;
        CommentDelegateButtonClickEvent -= OnChangeText;
        userNameToggleDelegate -= OnChangeUsernameToggle;

    }


    private void OnChangeReactionIcon(string url)
    {

        Debug.Log($"sendDataReactionUrl {url}");
        if ((!string.IsNullOrEmpty(url))
                  /*  && !PlayerPrefs.GetString(url).Equals(url)*/)
        {

            //sendDataReactionUrl(url);

            gameObject.GetComponent<PhotonView>().RPC("sendDataReactionUrl", RpcTarget.All, url, ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<PhotonView>().ViewID);
        }
    }
    private void OnChangeUsernameToggle(int userNameToggleConstant)
    {


        //sendDataReactionUrl(url);
       
            gameObject.GetComponent<PhotonView>().RPC("sendDataUserNAmeToggle", RpcTarget.All, userNameToggleConstant, ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<PhotonView>().ViewID);
       

    }
    private void OnChangeText(string text)
    {
        Debug.Log($"sendDatatext {text}");

        if (!string.IsNullOrEmpty(text)
                 )
        {
            gameObject.GetComponent<PhotonView>().RPC("sendDataChatMsg", RpcTarget.All, text, ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<PhotonView>().ViewID);
            text = string.Empty;
        }
        //if ((!string.IsNullOrEmpty(text))
        //          /*  && !PlayerPrefs.GetString(url).Equals(url)*/)
        //{
        //    sendDataReactionUrl(text);
        //}
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

                Debug.Log("photon objects reaction====" + ImageUrl);
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
        Debug.Log("ThisidToggle:" +UserNameContantToggle);
        Debug.Log("ThisitOGGLEid:" +viewId);
        NameToggle(UserNameContantToggle,viewId);


    }
    public void NameToggle(int ToggleConstant,int id) {
        if (gameObject.GetComponent<PhotonView>().ViewID == id) {
            Debug.Log("USERNAME VALUE:"+ XanaConstants.xanaConstants.userName);
            if (ToggleConstant == 0)
            {
                Debug.Log("Onbtn:"+ReferrencesForDynamicMuseum.instance.onBtnUsername);
                PhotonUserName.enabled = true;

            }
                else 
             {
                Debug.Log("Offbtn:"+ReferrencesForDynamicMuseum.instance.onBtnUsername);
                PhotonUserName.enabled = false;
            }
        }
        
  }


    [PunRPC]
    public void sendDataChatMsg(string chat, int viewId)
    {
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
        if (XanaChatSystem.instance.UserName.Length > 12)
        {
            PhotonNetwork.NickName = XanaChatSystem.instance.UserName.Substring(0, 12) + "...";
        }
        else
        {
            PhotonNetwork.NickName = XanaChatSystem.instance.UserName;
        }

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
        else
        {

            go.transform.localPosition = new Vector3(-0.74f, 0.1f, -26f);
            go.transform.localEulerAngles = new Vector3(-85, -113.1f, -65);
            go.transform.localScale = new Vector3(6.0f, 5.25f, 1);

            //EmoteAnimationPlay.Instance.controller = (AnimatorController)EmoteAnimationPlay.Instance.animator.runtimeAnimatorController;
            //// var state = controller.layers[0].stateMachine.defaultState;
            //var state = EmoteAnimationPlay.Instance.controller.layers[0].stateMachine.states.FirstOrDefault(s => s.state.name.Equals("Animation")).state;
            //Debug.Log("states===" + state.name);
            //if (state == null)
            //{
            //    Debug.LogError("Couldn't get the state!");

            //}
            //try
            //{

            //}catch()
            //EmoteAnimationPlay.Instance.controller.SetStateEffectiveMotion(state, EmoteAnimationPlay.Instance.spawnCharacterObject.transform.GetComponent<Animation>().clip);
        }

        if (isOtherPlayer)
        {
            go.GetComponent<MeshRenderer>().material = clientMat;

            //go.AddComponent<ChangeGear>();
            // go.AddComponent<Equipment>();
            //  GameObject.FindGameObjectWithTag("DCloth").GetComponent<DefaultClothes>()._DefaultInitializer();
        }
        else
        {
            go.GetComponent<MeshRenderer>().material = playerMat;
        }

        if (isBear)
        {
            go.SetActive(false);
        }

        //LoadingManager.Instance.HideLoading();
        //LoadingHandler.Instance.HideLoading();

        if (XanaConstants.xanaConstants.IsMuseum && FeedEventPrefab.m_EnvName.Contains("J & J WORLD_5"))
            go.SetActive(false);
        if (SoundManager.Instance)
        {

            SoundManager.Instance.PlayBGM();
        }
    }






}
