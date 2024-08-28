using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using System.IO;
using System.Linq;
using TMPro;

namespace Metaverse
{
    /// <summary> this script is Handling Build in avatar models data and spawning  avatar buttons on  Ui.</summary>/// 
    public class AvatarManager : MonoBehaviourPunCallbacks
    {
        public GameObject InternetLost;
        public GameObject GameCanvas;
        public GameObject MenuCanvas;
        protected RuntimeAnimatorController animator;
        public RuntimeAnimatorController Defaultanimator;
        protected AnimatorOverrideController animatorOverrideController;
        GameObject spawnCharacterObject;

        // Start is called before the first frame update
        public GameObject PrefabObject;
        public GameObject contentPanel;
        public GameObject spawnPoint;
        public GameObject avatarPreview;
        public List<AvatarData> AvatarList;
        public static AvatarManager Instance;
        public static int avatarID = 0;
        public Button selectAvatarBtn;
        public GameObject currentselected;
        public List<GameObject> avatarRefs;
        public bool isMainScene = false;

        public Material clientMat;

        public GameObject arrow;

        public GameObject Currentplayer, parentobjPlayer, currentDummyPlayer, dummyPlayerParent;
        public RuntimeAnimatorController main_AnimatorController, fordummy;
        public int value;

        public GameObject PlayerPrefab;
        public bool reconnectAndRejoin;
        public GameObject clone_dummy, clonPlayer, temp1, temp2;
        float timer = 0f;
        float timerOvelapp = 0f;
        public static bool OnDisconnectedValue = false;
        private bool focusCheck;

        //public DefaultClothes _DClothes;
        public static bool timercall = false;
        public static bool sendDataValue = false;
        private bool internetdisconnect = false;
        public GameObject JoinCurrentRoomPanel;

        private DateTime lastMinimize;
        public TimeSpan minimizedSeconds;
        private CameraLook[] _cameraLooks;

        public static event Action OninternetDisconnect;
        public static event Action OninternetConnected;

        public Sprite FavouriteAnimationSprite;
        public Sprite NormalAnimationSprite;

        private void Awake()
        {
            print("AvatarManager " + "Awake");
            Instance = this;
            Scene scene = SceneManager.GetActiveScene();
            if (scene.buildIndex == 0)
            {
                isMainScene = true;
            }

            _cameraLooks = FindObjectsOfType<CameraLook>();
        }


        private void OnApplicationQuit()
        {
            PhotonNetwork.Destroy(currentDummyPlayer);
            SceneManage.callRemove = true;
            PhotonNetwork.LeaveRoom(false);
            PhotonNetwork.LeaveLobby();
            UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);

            Debug.Log(" ######     Quit    #########");
        }


        void Start()
        {
            // StartCoroutine(WaitforUsertoConnect());
        }
        IEnumerator WaitforUsertoConnect()
        {
            yield return new WaitForSeconds(2f);
            if (!(SceneManager.GetActiveScene().name == "AddressableScene") || !(SceneManager.GetActiveScene().name.Contains("Museum")))
            {
                InitCharacter();
            }
            else
            {
                Application.runInBackground = true;
            }
        }


        public void ShowJoinRoomPanel()
        {
            print("AvatarManager " + "ShowJoinRoomPanel");
            //InternetLost = null;
            if (InternetLost == null)
            {
                XanaConstants.xanaConstants.needToClearMemory = false;
                if (LoadingHandler.Instance)
                    LoadingHandler.Instance.HideLoading();
                GameObject go = Instantiate(JoinCurrentRoomPanel) as GameObject;
                InternetLost = go;
            }

            if (LoadingHandler.Instance != null && !LoadingHandler.Instance.gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
            {
                //CameraLook.instance.DisAllowControl();
                OffSelfie();
                TurnCameras(false);
                //Instantiate(JoinCurrentRoomPanel);
            }

        }
        public void InstantiatePlayerAgain()
        {
            print("AvatarManager " + "InstantiatePlayerAgain");
            StartCoroutine(MainReconnect());
        }

        public void InitCharacter()
        {
            print("AvatarManager " + "InitCharacter");
            avatarRefs.Clear();//clear ref before assigning
            if (Instance == null)
            {
                Instance = this;
            }

            //WaqasAhmad
            // LoadingHandler.Instance.UpdateLoadingSlider((LoadingHandler.Instance.loadingSlider.fillAmount + 0.1f), true);


            //AssignAvatarModel();
            //selectAvatarBtn.onClick.AddListener(ChangeButtonCommit);
            //   initStates();
        }

        private void OffSelfie()
        {
            SelfieController.Instance.SwitchFromSelfieControl();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                OffSelfie();
            }

            if (temp1 != null)
            {
                Destroy(temp1);
            }
            if (temp2 != null)
            {
                Destroy(temp2);
            }
            value = PlayerPrefs.GetInt("SelectedAvatarID");
            if (OnDisconnectedValue)
            {
                if (SceneManager.GetActiveScene().name != "Main")
                {
                    if (Application.internetReachability != NetworkReachability.NotReachable)
                    {
                        if (currentDummyPlayer == null)
                        {
                            //PhotonNetwork.ReconnectAndRejoin();
                            //PhotonNetwork.JoinRoom(PlayerPrefs.GetString("roomname"));
                            ////PhotonNetwork.RejoinRoom(PlayerPrefs.GetString("roomname"));

                            InternetLost = null;
                            //InternetLost.SetActive(false);
                            GameCanvas.SetActive(true);
                            if (MenuCanvas != null)
                            {
                                MenuCanvas.SetActive(true);
                                OnDisconnectedValue = false;
                            }
                        }
                    }
                }
            }
            if (currentDummyPlayer == null && SceneManager.GetActiveScene().name != "Main" && Application.internetReachability != NetworkReachability.NotReachable)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0f;
            }
        }

        IEnumerator checkPlayerInorOut(float waittime)
        {
            print("AvatarManager " + "checkPlayerInorOut");
            yield return new WaitForSeconds(waittime);
            LoadingManager.Instance.ShowLoading();
            StartCoroutine(LoadingManager.Instance.LoadAsncScene("Main"));
        }
        public void initStates()
        {
            print("AvatarManager " + "initStates");
            if (SceneManager.GetActiveScene().name.Equals("MuseumSceneLatest"))
            {
                avatarID = PlayerPrefs.GetInt("SelectedAvatarID", 0);
            }
            else
            {
                avatarID = 0;
            }
            if (SceneManager.GetActiveScene().name.Equals("Main"))
            {
                SelectedAvatarPreview(AvatarList[avatarID].prefab, avatarID);
            }
            HighLighter();
            if (PhotonNetwork.IsConnected)
                StartCoroutine(WaitForChangeButtonCommit());
        }

        IEnumerator WaitForChangeButtonCommit()
        {
            //LoadingHandler.Instance.UpdateLoadingSlider(0.95f, true);
            LoadingHandler.Instance.UpdateLoadingStatusText("Connected to Network");

            yield return new WaitForSeconds(1.0f);

            ChangeButtonCommit();
        }

        public void AssignAvatarModel()
        {
            print("AvatarManager " + "AssignAvatarModel");

            StartCoroutine(WaitForAssignModel());

        }

        IEnumerator WaitForAssignModel()
        {
            // LoadingHandler.Instance.UpdateLoadingSlider(0.85f, true);
            LoadingHandler.Instance.UpdateLoadingStatusText("Spawning Character");

            yield return new WaitForSeconds(1.0f);

            for (int i = 0; i < AvatarList.Count; i++)
            {
                if (AvatarList[i].prefab)
                {
                    GameObject SpawnedItem = Instantiate(PrefabObject, contentPanel.transform);
                    SpawnedItem.name = AvatarList[i].prefab.name;
                    SpawnedItem.GetComponent<AssignAvatar>().Init(AvatarList[i].prefab, AvatarList[i].ModelNumber, AvatarList[i].avatarImage);
                    avatarRefs.Add(SpawnedItem);


                }
            }
        }

        private void TurnCameras(bool active)
        {
            if (active)
            {
                CameraLook.instance.AllowControl();
            }
            else
            {
                CameraLook.instance.DisAllowControl();
            }
        }

        RoomOptions roomOptions;


        private IEnumerator MainReconnect()
        {
            print("AvatarManager " + "MainReconnect");
            while (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Disconnected)
            {
                //Debug.Log("Waiting for client to be fully disconnected..", this);

                yield return new WaitForSeconds(0.2f);
            }
            Debug.Log("Client is disconnected!", this);

            string lastRoomName = PlayerPrefs.GetString("roomname");
            if (!PhotonNetwork.ReconnectAndRejoin())
            {
                if (PhotonNetwork.RejoinRoom(lastRoomName))
                {
                    Debug.Log(" Successful reconnected!", this);
                }
            }
            else
            {
                Debug.Log("Successful reconnected and joined!", this);
                PhotonNetwork.AutomaticallySyncScene = true;
                roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = 20;
                roomOptions.IsOpen = true;
                roomOptions.IsVisible = true;
                roomOptions.PublishUserId = true;
                roomOptions.CleanupCacheOnLeave = true;
                PhotonNetwork.JoinOrCreateRoom(PlayerPrefs.GetString("roomname"), roomOptions, new TypedLobby(PlayerPrefs.GetString("lb"), LobbyType.Default), null);
                Invoke("ChangeButtonCommit", 2);
            }
        }


        public override void OnDisconnected(DisconnectCause cause)
        {
            //InternetLost = null;
            print("AvatarManager " + "OnDisconnected");
            if (OninternetDisconnect != null)
                OninternetDisconnect.Invoke();
            ShowJoinRoomPanel();
        }


        public override void OnConnected()
        {
            if (OninternetConnected != null)
                OninternetConnected.Invoke();
        }

        //public override void OnJoinRoomFailed(short returnCode, string message)
        //{
        //    print("AvatarManager " + "OnJoinRoomFailed");
        //    base.OnJoinRoomFailed(returnCode, message);
        //}

        private void LogFeedback(string v)
        {
            throw new NotImplementedException();
        }

        public IEnumerator SceneChange()
        {
            yield return new WaitForSeconds(2f);
            // SceneManager.LoadScene(1);
        }


        public void ChangeButtonCommit()
        {
            if (!isMainScene)
            {
                //Debug.Log("Local player===" + PlayerControllerPhoton.LocalPlayerInstance);
                Scene scene = SceneManager.GetActiveScene();
                if (scene.name != "AddressableScene" || !scene.name.Contains("Museum"))
                {
                    //if (PlayerControllerPhoton.LocalPlayerInstance == null)
                    //{

                    Quaternion rot = Quaternion.Euler(0, 180, 0);
                    if (currentDummyPlayer != null)
                    {
                        Destroy(currentDummyPlayer);
                    }

                    if (AvatarList[avatarID] != null && AvatarList[avatarID].prefab != null)
                        currentDummyPlayer = PhotonNetwork.Instantiate(AvatarList[avatarID].prefab.name, spawnPoint.transform.position, rot, 0);
                    else
                        return;


                    currentDummyPlayer.tag = "PhotonLocalPlayer";
                    currentDummyPlayer.transform.parent = spawnPoint.transform;
                    //Debug.Log("1");
                    if (WorldItemView.m_EnvName.Contains("AfterParty"))
                    {
                        //Debug.Log("2");
                        for (int i = 0; i < IdolVillaRooms.instance.villaRooms.Length; i++)
                        {
                            //Debug.Log("3" + IdolVillaRooms.instance.villaRooms[i].name + "-----" + ChracterPosition.currSpwanPos);
                            if (IdolVillaRooms.instance.villaRooms[i].name == ChracterPosition.currSpwanPos)
                            {
                                ReferrencesForDynamicMuseum.instance.MainPlayerParent.transform.localPosition = gameObject.GetComponent<ChracterPosition>().NewPos.transform.localPosition;
                                ReferrencesForDynamicMuseum.instance.PlayerParent.transform.SetParent(IdolVillaRooms.instance.villaRooms[i].gameObject.GetComponent<ChracterPosition>().NewPos.transform);
                                ReferrencesForDynamicMuseum.instance.PlayerParent.transform.localPosition = new Vector3(0, 0, 0);
                                break;
                            }
                        }
                    }
                    currentDummyPlayer.transform.localPosition = new Vector3(0, -0.081f, 0);
                    currentDummyPlayer.transform.localEulerAngles = new Vector3(0, 0, 0);

                    currentDummyPlayer.transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PhotonNetwork.NickName;
                    spawnPoint.GetComponent<PlayerControllerNew>().animator = currentDummyPlayer.GetComponent<Animator>();
                    // spawnPoint.GetComponent<EmoteAnimationPlay>().animator = currentDummyPlayer.GetComponent<Animator>();

                    currentDummyPlayer.GetComponent<IKMuseum>().Initialize();
                    //Defaultanimator  = currentDummyPlayer.transform.GetComponent<Animator>().runtimeAnimatorController;

                    if (SceneManager.GetActiveScene().name.Equals("MuseumSceneLatest"))
                    {
                        SelfieController.Instance.SetIkValues(0);
                    }
                    StartCoroutine(WaitAndDeactiveSelfie());
                    StartCoroutine(OverLapTime());
                    //}
                }

            }
            else
            {
            }

            PlayerPrefs.SetInt("SelectedAvatarID", avatarID);

        }
        IEnumerator WaitAndDeactiveSelfie()
        {
            print("AvatarManager " + "WaitAndDeactiveSelfie");
            yield return new WaitForSeconds(1.5f);
            SelfieController.Instance.SwitchFromSelfieControl();
        }
        IEnumerator OverLapTime()
        {
            print("AvatarManager " + "OverLapTime");
            yield return new WaitForSeconds(3f);
            if (MenuCanvas != null && GameCanvas != null)
            {
                MenuCanvas.SetActive(true);
                GameCanvas.SetActive(true);
            }
            else if (GameCanvas != null)
            {
                GameCanvas.SetActive(true);
            }
        }

        void OnApplicationPause(bool isGamePause)
        {
            print("AvatarManager " + "OnApplicationPause");
            if (isGamePause)
            {
                lastMinimize = DateTime.Now;
            }
        }
        void OnApplicationFocus(bool isGameFocus)
        {
            // Debug.Log("runtime controller=="+ currentDummyPlayer.transform.GetComponent<Animator>().runtimeAnimatorController);
            //currentDummyPlayer.transform.GetComponent<Animator>().runtimeAnimatorController = Defaultanimator as RuntimeAnimatorController;
            print("AvatarManager " + "OnApplicationFocus : " + isGameFocus);
            if (isGameFocus)
            {
                minimizedSeconds = DateTime.Now - lastMinimize;
            }


            // User Analatics 
            if (!SceneManager.GetActiveScene().name.Contains("Main"))
            {
                //UserAnalyticsHandler.onUserJoinedLeaved?.Invoke(isGameFocus);
                if (isGameFocus)
                {
                    UserAnalyticsHandler.onUpdateWorldStatCustom?.Invoke(true, false);
                }
                else
                {
                    UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
                }
            }

        }

        public void GetData()
        {
        }

        private void SetLayerRecursively(GameObject Parent, int Layer)
        {
            print("AvatarManager " + "SetLayerRecursively");
            Parent.layer = Layer;

            foreach (Transform child in Parent.transform)
            {
                SetLayerRecursively(child.gameObject, Layer);
            }
        }

        public void HighLighter()
        {
            print("AvatarManager " + "HighLighter");
            foreach (var item in avatarRefs)
            {
                if (item.GetComponent<AssignAvatar>()._characterIndex == avatarID)
                {
                    item.GetComponent<AssignAvatar>().avatarHighLighter.enabled = true;
                }
                else
                {
                    item.GetComponent<AssignAvatar>().avatarHighLighter.enabled = false;
                }
            }
        }

        public void SelectedAvatarPreview(GameObject _character, int _characterIndex)
        {
            print("AvatarManager " + "SelectedAvatarPreview");
            Debug.Log("Testing " + _characterIndex);
            if (currentDummyPlayer != null)
            {
                if (PhotonNetwork.IsConnected)
                    PhotonNetwork.Destroy(currentDummyPlayer);
                Destroy(currentDummyPlayer);

            }
            if (!SceneManager.GetActiveScene().name.Equals("Main"))
            {
                foreach (Transform child in avatarPreview.transform)
                {
                    Destroy(child.gameObject);
                }
                Instantiate(_character, avatarPreview.transform.position, avatarPreview.transform.rotation, avatarPreview.transform);
                currentDummyPlayer = PhotonNetwork.Instantiate(_character.name, spawnPoint.transform.position, spawnPoint.transform.rotation);
                currentDummyPlayer.tag = "PhotonLocalPlayer";
                Debug.Log("nick name 2 ==" + PhotonNetwork.NickName);
                currentDummyPlayer.transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PhotonNetwork.NickName;
                currentDummyPlayer.transform.parent = spawnPoint.transform;
                currentDummyPlayer.transform.localPosition = new Vector3(0, -0.081f, 0);
                // Defaultanimator = GameObject.FindGameObjectWithTag("PhotonLocalPlayer").transform.GetComponent<Animator>().runtimeAnimatorController;
                print("SpawningHere");
            }
            else
            {
                currentDummyPlayer = Instantiate(_character, spawnPoint.transform.position, spawnPoint.transform.rotation, spawnPoint.transform);
            }
            //InstantiateArrow(currentDummyPlayer.transform, false);
            spawnPoint.GetComponent<PlayerControllerNew>().animator = currentDummyPlayer.GetComponent<Animator>();
            spawnPoint.GetComponent<PlayerControllerNew>().playerRig = currentDummyPlayer.GetComponent<FirstPersonJump>().jumpRig;

            // spawnPoint.GetComponent<EmoteAnimationPlay>().animator = currentDummyPlayer.GetComponent<Animator>();
            avatarID = _characterIndex;
            HighLighter();
            PlayerPrefs.SetInt("SelectedAvatarID", avatarID);
        }


        // hardik work 12 jan2021





    }




}
[System.Serializable]
public class AvatarData
{
    public GameObject prefab;
    public RuntimeAnimatorController Controller;
    public int ModelNumber = 6;
    public Sprite avatarImage;
    public bool isHost;
}