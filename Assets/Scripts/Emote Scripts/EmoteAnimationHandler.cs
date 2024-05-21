using ExitGames.Client.Photon;
using Metaverse;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static EmoteFilterManager;
using UnityEngine.InputSystem.OnScreen;
using System.IO;

public class EmoteAnimationHandler : MonoBehaviour, IInRoomCallbacks, IOnEventCallback
{
    public bool alreadyRuning = true;
    private int counter = 0;
    public GameObject AnimHighlight;
    public static EmoteAnimationHandler Instance;
    public GameObject spawnCharacterObject;
    public GameObject spawnCharacterObjectRemote;
    public GameObject popupPenal;
    public bool IsEmote;
    public Animator animator = null;
    public Animator animatorremote = null;
    public RuntimeAnimatorController controller;
    public AnimationDetails bean;
    public GameObject AnimObject;
    public List<AnimationList> emoteAnim = new List<AnimationList>();
    private GameObject AnimObjectHigh;
    Dictionary<object, object> AnimationUrl = new Dictionary<object, object>();
    private bool firsttimecall;
    private GameObject[] photonplayerObjects;
    private int remotePlayerId;
    public static string remoteUrlAnimation;
    public static string remoteUrlAnimationName;
    private bool callOnce;
    private bool iscashed = false;
    Dictionary<object, object> cashed_data = new Dictionary<object, object>();
    public bool isEmoteActive = false;
    public bool MyAnimLoader = false;
    internal bool isFetchingAnim = false;
    internal bool isAnimRunning = false;
    internal static event Action<string> AnimationStarted;
    internal static event Action<string> AnimationStopped;
    public delegate void ClearAnimation();
    public ClearAnimation clearAnimation;
    bool isInit = false;
    public string currentAnimationTab;
    public GameObject lastAnimClickButton;
    public bool waitForStandUp = false;
    public bool isPreviousBundleLoad = true;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnEnable()
    {
        firsttimecall = false;
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.StopEmoteAnimation -= StopAnimation;
    }

    void Init()
    {
        if (!isInit)
        {
            if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.StopEmoteAnimation += StopAnimation;
            isInit = true;
        }
        else if (GamePlayButtonEvents.inst == null) isInit = false;
    }

    void Update()
    {
        if (IsEmote)
        {
            IsEmote = false;
            animator.SetBool("IsEmote", true);
        }
    }
    public void Load(string url, GameObject prefabAnim)
    {
        Init();

        if (alreadyRuning)
        {
            alreadyRuning = false;
            AnimationUrl.Clear();
            isFetchingAnim = true;
            StartCoroutine(GetAssetBundleFromServerUrl(url, prefabAnim));
        }
    }
    public IEnumerator GetAssetBundleFromServerUrl(string BundleURL, GameObject prefabObject)
    {
        if (AnimObject != null && prefabObject != null)
        {
            if (AnimObject.GetInstanceID() != prefabObject.GetInstanceID())
            {
                ReferencesForGamePlay.instance.m_34player.GetComponent<RpcManager>().CheckIfDifferentAnimClicked(true);
            }
            else
                ReferencesForGamePlay.instance.m_34player.GetComponent<RpcManager>().CheckIfDifferentAnimClicked(false);
        }

        if (MyAnimLoader == false)
        {
            sendDataAnimationUrl(BundleURL);
            try
            {
                AnimObject = prefabObject;
            }
            catch (Exception e)
            {
                AnimObject = prefabObject;
            }
     
            if (AnimObject != null) 
                AnimObject.transform.GetChild(3).gameObject.SetActive(true);

            MyAnimLoader = true;
        }
        yield break;
    }

    IEnumerator GetAssetBundleFromServerRemotePlayerUrl(string BundleURL, int id)
    {
        string bundlePath = Path.Combine(ConstantsHolder.xanaConstants.r_EmoteStoragePersistentPath, BundleURL + ".unity3d");

#if UNITY_ANDROID
        BundleURL = bean.data.animationList.Find(x => x.name == BundleURL).android_file;
#elif UNITY_IOS
        BundleURL = bean.data.animationList.Find(x => x.name == BundleURL).ios_file;
#elif UNITY_EDITOR
        BundleURL = bean.data.animationList.Find(x => x.name == BundleURL).android_file;
#endif
        photonplayerObjects = null;
        photonplayerObjects = Photon.Pun.Demo.PunBasics.MutiplayerController.instance.playerobjects.ToArray();

        for (int i = 0; i < photonplayerObjects.Length; i++)
        {
            animatorremote = photonplayerObjects[i].gameObject.GetComponent<Animator>();
            if (photonplayerObjects[i] != null)
            {
                if (photonplayerObjects[i].GetComponent<PhotonView>().ViewID == id)
                {
                    if (CheckForIsAssetBundleAvailable(bundlePath))
                    {
                        StartCoroutine(LoadAssetBundleFromStorage(bundlePath, photonplayerObjects[i].gameObject));
                    }
                    else
                    {
                        using (WWW www = new WWW(BundleURL))
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
                                AssetBundle assetBundle = www.assetBundle;

                                if (assetBundle != null)
                                {
                                    GameObject[] animation = assetBundle.LoadAllAssets<GameObject>();
                                    var remotego = animation[0];
                                    {

                                        if (remotego.name.Equals("Animation"))
                                        {

                                            if (!PlayerSelfieController.Instance.selfiePanel.activeInHierarchy)
                                            {
                                                if (animatorremote.GetBool("EtcAnimStart"))   // Added by Ali Hamza
                                                {
                                                    animatorremote.SetBool("Stand", true);
                                                    animatorremote.SetBool("EtcAnimStart", false);
                                                    foreach (var clip in animatorremote.runtimeAnimatorController.animationClips)
                                                    {
                                                        if (clip.name == "Stand")
                                                        {
                                                            yield return new WaitForSeconds(clip.length);
                                                        }
                                                    }
                                                }

                                                spawnCharacterObjectRemote = remotego.transform.gameObject;

                                                var overrideController = new AnimatorOverrideController();
                                                overrideController.runtimeAnimatorController = controller;

                                                List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                                                foreach (var clip in overrideController.animationClips)
                                                {

                                                    if (clip.name == "emaotedefault")
                                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, spawnCharacterObjectRemote.transform.GetComponent<Animation>().clip));
                                                    else
                                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
                                                }
                                                overrideController.ApplyOverrides(keyValuePairs);

                                                animatorremote.runtimeAnimatorController = overrideController;

                                                animatorremote.SetBool("IsEmote", true);
                                                isPreviousBundleLoad = true;
                                                CheckSelfieOn();

                                            }

                                        }
                                        else // For Etc Tab
                                        {
                                            if (animatorremote != null)    //Added by Ali Hamza
                                            {
                                                animatorremote.SetBool("Stand", true);
                                                animatorremote.SetBool("EtcAnimStart", false);

                                                if (CheckSpecificAnimationPlaying("Sit") && animatorremote.GetComponent<RpcManager>().DifferentAnimClicked)
                                                {
                                                    animatorremote.GetComponent<RpcManager>().DifferentAnimClicked = false;
                                                    foreach (var clip in animatorremote.runtimeAnimatorController.animationClips)
                                                    {
                                                        if (clip.name == "Stand")
                                                        {
                                                            yield return new WaitForSeconds(clip.length);
                                                        }
                                                    }
                                                }
                                                else if (CheckSpecificAnimationPlaying("Sit"))
                                                {
                                                    isPreviousBundleLoad = true;
                                                    break;
                                                }
                                            }

                                            if (animatorremote != null)
                                                animatorremote.SetBool("Stand", false);

                                            animatorremote.SetBool("EtcAnimStart", true);

                                            var overrideController = new AnimatorOverrideController();
                                            overrideController.runtimeAnimatorController = controller;

                                            List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                                            foreach (var clip in overrideController.animationClips)
                                            {
                                                if (animatorremote.GetBool("EtcAnimStart"))
                                                {
                                                    if (clip.name == "crouchDefault" || clip.name == "standDefault")
                                                    {
                                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, remotego.transform.GetChild(0).GetComponent<Animation>().clip));
                                                    }
                                                    else
                                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));

                                                    overrideController.ApplyOverrides(keyValuePairs);

                                                    animatorremote.runtimeAnimatorController = overrideController;

                                                }
                                                else
                                                {
                                                    if (clip.name == "Sit")
                                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, remotego.transform.GetComponent<Animation>().GetClip("Stand")));
                                                    else
                                                        keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));

                                                    overrideController.ApplyOverrides(keyValuePairs);

                                                    animatorremote.runtimeAnimatorController = overrideController;
                                                }
                                                animatorremote.SetBool("IsEmote", false);
                                                isPreviousBundleLoad = true;
                                            }
                                        }
                                    }
                                    SaveAssetBundle(www.bytes, bundlePath, photonplayerObjects[i].gameObject);

                                    assetBundle.Unload(false);


                                }
                            }
                        }

                    }
                    if (photonplayerObjects[i].GetComponent<PhotonView>().IsMine)
                    {
                        MyAnimLoader = false;
                        isFetchingAnim = false;
                        isAnimRunning = true;

                        if (AnimObject != null)
                        {
                            AnimObject.transform.GetChild(3).gameObject.SetActive(false);
                        }
                        AnimationStarted?.Invoke(remoteUrlAnimationName);

                    }
                    alreadyRuning = true;
                    break;
                }
            }
        }
    }

    public void CheckSelfieOn()
    {
        if (PlayerSelfieController.Instance.selfiePanel.activeInHierarchy)
        {
            clearAnimation?.Invoke();
        }
    }

    public void StopAnimation()
    {
        GameObject player;
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();

        {
            if (AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer != null)
            {
                AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer.transform.localPosition = new Vector3(0f, -0.081f, 0);
            }

            player = ReferencesForGamePlay.instance.m_34player;
            if (player != null)
            {
                object[] viewMine = { player.GetComponent<PhotonView>().ViewID };
                RaiseEventOptions options = new RaiseEventOptions();
                options.CachingOption = EventCaching.DoNotCache;
                options.Receivers = ReceiverGroup.All;
                PhotonNetwork.RaiseEvent(1, viewMine as object, options,
                    SendOptions.SendReliable);
            }

            iscashed = false;

            PlayerPrefs.SetString(remoteUrlAnimation, "");

            AvatarSpawnerOnDisconnect.Instance.spawnPoint.GetComponent<PlayerController>().enabled = true;
            PlayerPrefsUtility.SetEncryptedString(ConstantsGod.SELECTED_ANIMATION_NAME, "");
            LoadEmoteAnimations.animClick = false;
        }
        isAnimRunning = false;
        MyAnimLoader = false;
        alreadyRuning = true;
        AnimationStopped?.Invoke(remoteUrlAnimationName);
        if (player.GetComponent<PhotonView>().IsMine)
        {
            if (AnimObject != null)
            {
                AnimObject.transform.GetChild(3).gameObject.SetActive(false);
            }
            StopAllCoroutines();
            isFetchingAnim = false;
            AnimObject = null;
        }
    }

    public void sendDataAnimationUrl(string url)
    {

        AnimHighlight.SetActive(true);

        Dictionary<object, object> clothsDic = new Dictionary<object, object>();
        clothsDic.Add(ReferencesForGamePlay.instance.m_34player.GetComponent<PhotonView>().ViewID.ToString(), remoteUrlAnimationName);

        RaiseEventOptions options = new RaiseEventOptions();
        options.CachingOption = EventCaching.DoNotCache;
        options.Receivers = ReceiverGroup.All;
        PhotonNetwork.RaiseEvent(12, clothsDic, options,
            SendOptions.SendReliable);
        cashed_data = clothsDic;
        iscashed = true;
    }

    private void NetworkingClient_EventReceived(EventData obj)
    {
        if (obj.Code == 0)
        {
            if (firsttimecall == false)
            {
                firsttimecall = true;
                remotePlayerId = (int)obj.CustomData;
            }
        }

        else if (obj.Code == 1)
        {

            object[] minePlayer = (object[])obj.CustomData;
            DisableAnim((int)minePlayer[0]);
        }
        else if (obj.Code == 12)
        {
            StartCoroutine(waittostart(obj));

        }
    }

    public IEnumerator LoadAssetBundleFromStorage(string bundlePath, GameObject PlayerAvatar)
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();

        AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return bundle;

        AssetBundle assetBundle = bundle.assetBundle;
        if (assetBundle == null)
        {
            yield break;
        }

        if (assetBundle != null)
        {
            AssetBundleRequest newRequest = assetBundle.LoadAllAssetsAsync<GameObject>();
            while (!newRequest.isDone)
            {
                yield return null;
            }
            if (newRequest.isDone)
            {

                var animation = newRequest.allAssets;
                foreach (var anim in animation)
                {
                    GameObject go = (GameObject)anim;
                    if (go.name.Equals("Animation") || go.name.Equals("animation"))
                    {
                        if (animatorremote.GetBool("EtcAnimStart"))   // Added by Ali Hamza
                        {
                            animatorremote.SetBool("Stand", true);
                            animatorremote.SetBool("EtcAnimStart", false);
                            foreach (var clip in animatorremote.runtimeAnimatorController.animationClips)
                            {
                                if (clip.name == "Stand")
                                {
                                    yield return new WaitForSeconds(clip.length);
                                }
                            }
                        }

                        var overrideController = new AnimatorOverrideController();
                        overrideController.runtimeAnimatorController = controller;

                        List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                        foreach (var clip in overrideController.animationClips)
                        {
                            if (clip.name == "emaotedefault")
                                keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetComponent<Animation>().clip));
                            else
                                keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));

                            overrideController.ApplyOverrides(keyValuePairs);

                            animatorremote.runtimeAnimatorController = overrideController;

                            animatorremote.SetBool("IsEmote", true);
                            isPreviousBundleLoad = true;
                        }
                    }
                    else
                    {
                        if (animatorremote != null)    //Added by Ali Hamza
                        {
                            animatorremote.SetBool("Stand", true);
                            animatorremote.SetBool("EtcAnimStart", false);

                            if (CheckSpecificAnimationPlaying("Sit") && animatorremote.GetComponent<RpcManager>().DifferentAnimClicked)
                            {
                                animatorremote.GetComponent<RpcManager>().DifferentAnimClicked = false;
                                foreach (var clip in animatorremote.runtimeAnimatorController.animationClips)
                                {
                                    if (clip.name == "Stand")
                                    {
                                        yield return new WaitForSeconds(clip.length);
                                    }
                                }
                            }
                            else if (CheckSpecificAnimationPlaying("Sit"))
                            {
                                isPreviousBundleLoad = true;
                                break;
                            }
                        }

                        if (animatorremote != null)
                            animatorremote.SetBool("Stand", false);

                        animatorremote.SetBool("EtcAnimStart", true);

                        var overrideController = new AnimatorOverrideController();
                        overrideController.runtimeAnimatorController = controller;

                        List<KeyValuePair<AnimationClip, AnimationClip>> keyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                        foreach (var clip in overrideController.animationClips)
                        {
                            if (animatorremote.GetBool("EtcAnimStart"))
                            {
                                if (clip.name == "crouchDefault" || clip.name == "standDefault")
                                {
                                    keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetChild(0).GetComponent<Animation>().clip));
                                    //currentEtcAnimName = go.name;
                                }
                                else
                                    keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));

                                overrideController.ApplyOverrides(keyValuePairs);

                                animatorremote.runtimeAnimatorController = overrideController;

                            }
                            else
                            {
                                if (clip.name == "Sit")
                                    keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, go.transform.GetComponent<Animation>().GetClip("Stand")));
                                else
                                    keyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));

                                overrideController.ApplyOverrides(keyValuePairs);

                                animatorremote.runtimeAnimatorController = overrideController;
                            }
                            animatorremote.SetBool("IsEmote", false);
                            isPreviousBundleLoad = true;
                        }
                    }
                }
            }
            if (assetBundle != null)
            {
                assetBundle.Unload(false);
            }
            alreadyRuning = true;

        }

    }

    bool CheckSpecificAnimationPlaying(string stateName) 
    {
        return animatorremote.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    public void SaveAssetBundle(byte[] data, string path, GameObject id)
    {
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            File.WriteAllBytes(path, data);
            if (id.GetComponent<PhotonView>().IsMine)
            {
                MyAnimLoader = false;
                isFetchingAnim = false;
                isAnimRunning = true;

                if (AnimObject != null)
                {
                    AnimObject.transform.GetChild(3).gameObject.SetActive(false);
                }
                AnimationStarted?.Invoke(remoteUrlAnimationName);
            }
        }
        catch (Exception e)
        {

        }
    }
    public bool CheckForIsAssetBundleAvailable(string path)
    {
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator waittostart(EventData obj)
    {
        List<EventData> data = new List<EventData>();
        data.Clear();
        if (data.Count > 0)
        {
            data.Add(obj);
            yield break;
        }
        data.Add(obj);


        while (data.Count > 0)
        {

            Dictionary<object, object> clothsDic2 = new Dictionary<object, object>();

            clothsDic2 = (Dictionary<object, object>)data[0].CustomData;

            foreach (KeyValuePair<object, object> keyValue in clothsDic2)
            {
                string s = keyValue.Key.ToString();
                remotePlayerId = int.Parse(s);

                if (!string.IsNullOrEmpty(clothsDic2[s].ToString()))
                    yield return StartCoroutine(GetAssetBundleFromServerRemotePlayerUrl(clothsDic2[s].ToString(), int.Parse(s)));

                // yield return null;
            }
            data.RemoveAt(0);

        }
        yield return null;
    }

    public void DisableAnim(int viewId)
    {
        photonplayerObjects = null;
        photonplayerObjects = Photon.Pun.Demo.PunBasics.MutiplayerController.instance.playerobjects.ToArray();

        for (int i = 0; i < photonplayerObjects.Length; i++)
        {
            if (photonplayerObjects[i] != null)
            {
                if (photonplayerObjects[i].GetComponent<PhotonView>().ViewID == viewId)
                {
                    animatorremote = photonplayerObjects[i].gameObject.GetComponent<Animator>();
                    animatorremote.runtimeAnimatorController = controller;
                    animatorremote.SetBool("IsEmote", false);

                }
            }
        }
    }

    public void animationClick()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();

        if (AnimHighlight.activeInHierarchy)
        {
            AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer.transform.localPosition = new Vector3(0f, -0.081f, 0);

            object[] viewMine = { ReferencesForGamePlay.instance.m_34player.GetComponent<PhotonView>().ViewID };
            RaiseEventOptions options = new RaiseEventOptions();
            options.CachingOption = EventCaching.DoNotCache;
            options.Receivers = ReceiverGroup.All;
            PhotonNetwork.RaiseEvent(1, viewMine as object, options,
                SendOptions.SendReliable);

            iscashed = false;
            PlayerPrefs.SetString(remoteUrlAnimation, "");

            AvatarSpawnerOnDisconnect.Instance.spawnPoint.GetComponent<PlayerController>().enabled = true;
            AnimHighlight.SetActive(false);
            LoadEmoteAnimations.animClick = false;


            try
            {
                GameplayEntityLoader.instance.leftJoyStick.transform.GetChild(0).GetComponent<OnScreenStick>().movementRange = GameplayEntityLoader.instance.joyStickMovementRange;

            }
            catch (Exception e)
            {

            }
        }
        else
        {
            isEmoteActive = true;
            try
            {
                GameplayEntityLoader.instance.joyStickMovementRange = GameplayEntityLoader.instance.leftJoyStick.transform.GetChild(0).GetComponent<OnScreenStick>().movementRange;
            }
            catch (Exception e)
            {

            }
        }



    }


    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (iscashed)
        {
            RaiseEventOptions options = new RaiseEventOptions();
            options.CachingOption = EventCaching.DoNotCache;
            options.TargetActors = new int[] { newPlayer.ActorNumber };
            PhotonNetwork.RaiseEvent(12, cashed_data, options,
                SendOptions.SendReliable);
        }
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
    }

    public IEnumerator getAllAnimations()
    {
        UnityWebRequest uwr = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.GetAllAnimatons + "/" + APIBasepointManager.instance.apiversionForAnimation);
        try
        {
            uwr.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        }
        catch (Exception e1)
        {
        }

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
           Debug.LogError("----->> getAllAnimations Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.LogError("----->> getAllAnimations Error While Sending: " + uwr.result);

            try
            {

                AssetBundle.UnloadAllAssetBundles(false);
                Resources.UnloadUnusedAssets();
                bean = JsonUtility.FromJson<AnimationDetails>(uwr.downloadHandler.text.ToString().Trim());
                if (!string.IsNullOrEmpty(bean.data.ToString()))
                {
                    emoteAnim.Clear();
                    emoteAnim.AddRange(bean.data.animationList);
                }
            }
            catch
            {

            }
        }
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }

    public void OnEvent(EventData photonEvent)
    {
        if (!string.IsNullOrEmpty(remoteUrlAnimation))
        {
            if (callOnce == false)
            {

            }

        }

    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
    }
}
