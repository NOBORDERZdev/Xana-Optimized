using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;
using Cinemachine;
using UnityEditor;
using WebSocketSharp;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Photon.Realtime;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Rendering.Universal;
using PhysicsCharacterController;
using UnityEngine.InputSystem;

public class GameplayEntityLoader : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [Header("singleton object")]
    public static GameplayEntityLoader instance;

    public GameObject mainPlayer;
    public GameObject mainController;
    private GameObject YoutubeStreamPlayer;
    public GameObject PenguinPlayer;

    public CinemachineFreeLook PlayerCamera;
    public CinemachineFreeLook playerCameraCharacterRender;
    public Camera environmentCameraRender;
    public Camera firstPersonCamera;
    [HideInInspector]
    private Transform updatedSpawnpoint;
    private Vector3 spawnPoint;
    private GameObject currentEnvironment;
    public bool isEnvLoaded = false;

    private float fallOffset = 10f;
    public bool setLightOnce = false;
    private GameObject player;

    System.DateTime eventUnivStartDateTime, eventLocalStartDateTime, eventlocalEndDateTime;

    [HideInInspector]
    public GameObject leftJoyStick;

    [HideInInspector]
    public float joyStickMovementRange;

    public LayerMask layerMask;

    public string addressableSceneName;

    [SerializeField] Button HomeBtn;

    public double eventRemainingTime;
    [SerializeField] int autoSwitchTime;
    public HomeSceneLoader _uiReferences;

    [Header("XANA Party")]
    public GameObject PositionResetButton;
    [SerializeField] GameObject XanaWorldController;
    [SerializeField] GameObject XanaPartyController;
    [SerializeField] public CameraManager XanaPartyCamera;
    [SerializeField] InputReader XanaPartyInput;
    [SerializeField] PenguinLookPointTracker penguinLook;

    //string OrdinaryUTCdateOfSystem = "2023-08-10T14:45:00.000Z";
    //DateTime OrdinarySystemDateTime, localENDDateTime, univStartDateTime, univENDDateTime;

    //Bool for BuilderSpawn point available or not
    bool BuilderSpawnPoint = false;



    private void Awake()
    {
        instance = this;
        //    LoadFile();
        setLightOnce = false;
        if (ConstantsHolder.xanaConstants.isJoinigXanaPartyGame && ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            PositionResetButton.SetActive(true);
            Invoke(nameof(LoadFile),1f);
        }
    }


    private void OnDestroy()
    {
        Physics.autoSimulation = true;
        Resources.UnloadUnusedAssets();
        GC.SuppressFinalize(this);
        GC.Collect(0);

        //    Caching.ClearCache();

    }

    private void Start()
    {
        if (XanaEventDetails.eventDetails.DataIsInitialized)
        {
            StartEventTimer();
        }
        Input.multiTouchEnabled = true;
        if (PlayerSelfieController.Instance)
        {
            for (int i = 0; i < PlayerSelfieController.Instance.OnFeatures.Length; i++)
            {
                if (PlayerSelfieController.Instance.OnFeatures[i] != null)
                {
                    if (PlayerSelfieController.Instance.OnFeatures[i].name == "LeftJoyStick")
                    {
                        leftJoyStick = PlayerSelfieController.Instance.OnFeatures[i];
                        break;
                    }
                }
            }
        }

        GameObject _updatedSpawnPoint = new GameObject();
        updatedSpawnpoint = _updatedSpawnPoint.transform;
        BuilderSpawnPoint = false;

    }

    void OnEnable()
    {
        BuilderEventManager.AfterWorldInstantiated += ResetPlayerAfterInstantiation;
    }


    private void OnDisable()
    {
        BuilderEventManager.AfterWorldInstantiated -= ResetPlayerAfterInstantiation;
        Resources.UnloadUnusedAssets();
        GC.SuppressFinalize(this);
        GC.Collect(0);
        //  Caching.ClearCache();
    }

    public void StartEventTimer()
    {
        eventUnivStartDateTime = DateTime.Parse(XanaEventDetails.eventDetails.startTime);
        eventLocalStartDateTime = eventUnivStartDateTime.ToLocalTime();
        eventlocalEndDateTime = eventLocalStartDateTime.Add(TimeSpan.FromSeconds(XanaEventDetails.eventDetails.duration));

        //eventRemainingTime = eventTimeInSeconds;
        InvokeRepeating("CalculateEventTime", 0, 1);
    }

    public void CalculateEventTime()
    {
        //univStartDateTime = DateTime.Parse(OrdinaryUTCdateOfSystem);
        //OrdinarySystemDateTime = univStartDateTime.ToLocalTime();
        int _eventEndSystemDateTimediff = (int)(eventlocalEndDateTime - System.DateTime.Now).TotalMinutes;

        //print("===================DIFFEND : " + _eventEndSystemDateTimediff);

        if (_eventEndSystemDateTimediff <= 0)
        {
            //print("Event Ended");
            _uiReferences.EventEndedPanel.SetActive(true);
            CancelInvoke("CalculateEventTime");
        }
    }

    public IEnumerator VoidCalculation()
    {
        while (true)
        {
            if (CheckVoid())
            {
                //Debug.Log("Resetting Position");
                ResetPlayerPosition();
            }
            yield return new WaitForSeconds(1f);
        }
    }


    public void LoadFile()
    {
        mainPlayer.SetActive(false);
        ////Debug.Log("Env Name : " + FeedEventPrefab.m_EnvName);
        //if (!setLightOnce)
        //{
        //    LoadLightSettings(FeedEventPrefab.m_EnvName);
        //    setLightOnce = true;
        //}
        //LoadEnvironment(FeedEventPrefab.m_EnvName);
        if (currentEnvironment == null)
        {
            if (ConstantsHolder.xanaConstants.isBuilderScene)
                SetupEnvirnmentForBuidlerScene();
            else
            {
                LoadEnvironment(ConstantsHolder.xanaConstants.EnviornmentName);
                //CharacterLightCulling();  //amit-05-05-2023 commented this line as it excuted before env instantidated and env light is not culled. called this method on another right place
            }
        }
        else
        {
            StartCoroutine(SpawnPlayer());
        }

        PlayerCamera.gameObject.SetActive(true);
        environmentCameraRender.gameObject.SetActive(true);
        //environmentCameraRender.transform.GetChild(0).gameObject.SetActive(true);

        if(PlayerSelfieController.Instance)
            PlayerSelfieController.Instance.DisableSelfieFromStart();



    }

    void InstantiateYoutubePlayer()
    {
        if (YoutubeStreamPlayer == null)
        {
            //Debug.Log("DJ Beach====" + WorldItemView.m_EnvName);
            if (WorldItemView.m_EnvName.Contains("DJ Event"))
            {
                YoutubeStreamPlayer = Instantiate(Resources.Load("DJEventData/YoutubeVideoPlayer") as GameObject);

                //#if UNITY_ANDROID || UNITY_EDITOR
                //                //YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
                //                //YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);

                //#else
                //YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
                //            YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);
                //#endif

                YoutubeStreamPlayer.transform.localPosition = new Vector3(0f, 0f, 10f);
                YoutubeStreamPlayer.transform.localScale = new Vector3(1f, 1f, 1f);

                YoutubeStreamPlayer.SetActive(false);
                if (YoutubeStreamPlayer)
                {
                    YoutubeStreamPlayer.SetActive(true);
                }
            }
            if (WorldItemView.m_EnvName.Contains("XANA Festival Stage") && !WorldItemView.m_EnvName.Contains("Dubai"))
            {
                YoutubeStreamPlayer = Instantiate(Resources.Load("XANAFestivalStageData/YoutubeVideoPlayer1") as GameObject);

                //#if UNITY_ANDROID || UNITY_EDITOR
                //                YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
                //                YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);
                //#else
                //  YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
                //            YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);
                //#endif


                YoutubeStreamPlayer.transform.localPosition = new Vector3(0f, 0f, 10f);
                YoutubeStreamPlayer.transform.localScale = new Vector3(1f, 1f, 1f);

                YoutubeStreamPlayer.SetActive(false);
                if (YoutubeStreamPlayer)
                {
                    YoutubeStreamPlayer.SetActive(true);
                }
            }

            if (WorldItemView.m_EnvName.Contains("Xana Festival") || WorldItemView.m_EnvName.Contains("NFTDuel Tournament"))
            {
                YoutubeStreamPlayer = Instantiate(Resources.Load("MyBeach/XanaFestivalPlayer") as GameObject);

                //#if UNITY_ANDROID || UNITY_EDITOR
                //                //YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
                //                //YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);

                //#else
                //YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
                //            YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);
                //#endif

                YoutubeStreamPlayer.transform.localPosition = new Vector3(0f, 0f, 10f);
                YoutubeStreamPlayer.transform.localScale = new Vector3(1f, 1f, 1f);

                YoutubeStreamPlayer.SetActive(false);
                if (YoutubeStreamPlayer)
                {
                    YoutubeStreamPlayer.SetActive(true);
                }
            }
            if (WorldItemView.m_EnvName.Contains("XANA Lobby"))
            {
                YoutubeStreamPlayer = Instantiate(Resources.Load("XanaLobby/XanaLobbyPlayer") as GameObject);

                //#if UNITY_ANDROID || UNITY_EDITOR
                //                //YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
                //                //YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);

                //#else
                //YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
                //            YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);
                //#endif

                //YoutubeStreamPlayer.transform.localPosition = new Vector3(0f, 0f, 10f);
                //YoutubeStreamPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
                //YoutubeStreamPlayer.transform.localPosition = new Vector3(-65.8f, 24.45f, -83.45f);
                //YoutubeStreamPlayer.transform.localScale = new Vector3(-0.54f, 0.53f, 0.53f);
                //YoutubeStreamPlayer.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);




                YoutubeStreamPlayer.SetActive(false);
                if (YoutubeStreamPlayer)
                {
                    YoutubeStreamPlayer.SetActive(true);
                }
            }
        }
    }
    void CharacterLightCulling()
    {
        if ((!WorldItemView.m_EnvName.Contains("Xana Festival") || !WorldItemView.m_EnvName.Contains("NFTDuel Tournament")) && !ConstantsHolder.xanaConstants.isBuilderScene)
        {
            //riken
            Light[] directionalLightList = FindObjectsOfType<Light>();
            for (int i = 0; i < directionalLightList.Length; i++)
            {
                if (directionalLightList[i].type == LightType.Directional && directionalLightList[i].gameObject.tag != "CharacterLight")
                {
                    directionalLightList[i].cullingMask = layerMask;
                }
            }
        }

        //.......
    }

    private void LoadLightSettings(string mEnvName)
    {
        string path = "Environment Data/" + mEnvName + " Data/LightingData/LightingData";
        if (!mEnvName.IsNullOrEmpty())
        {
            EnvironmentProperties EnvProp = Resources.Load<EnvironmentProperties>(path);
            if (EnvProp)
            {

                EnvProp.ApplyLightSettings();
            }
            else
            {
                //Debug.LogWarning("No Environment Light Properties Found");
            }
        }
        else
        {
            //Debug.LogWarning("No Environment Name Found");
        }
    }

    bool CheckVoid()
    {
        if (!ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            if (mainController.transform.position.y < (updatedSpawnpoint.transform.position.y - fallOffset))
            {
                RaycastHit hit;
                if (Physics.Raycast(mainController.transform.position, mainController.transform.TransformDirection(Vector3.down), out hit, 1000))
                {
                    updatedSpawnpoint.transform.localPosition = new Vector3(spawnPoint.x, hit.transform.localPosition.y, spawnPoint.z);
                    return false;
                }
                else
                {
                    updatedSpawnpoint.localPosition = spawnPoint;
                    return true;
                }
            }
        }
        else
        {
            if (XanaPartyCamera.characterManager != null)
            {
                if (XanaPartyCamera.characterManager.transform.position.y < (updatedSpawnpoint.transform.position.y - fallOffset))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(XanaPartyCamera.characterManager.transform.position, XanaPartyCamera.characterManager.transform.TransformDirection(Vector3.down), out hit, 1000))
                    {
                        updatedSpawnpoint.transform.localPosition = new Vector3(spawnPoint.x, hit.transform.localPosition.y, spawnPoint.z);
                        return false;
                    }
                    else
                    {
                        updatedSpawnpoint.localPosition = spawnPoint;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public IEnumerator SpawnPlayer()
    {
        //if (ConstantsHolder.xanaConstants.isFromXanaLobby)
        //    LoadingHandler.Instance.UpdateLoadingSliderForJJ(.8f,0.1f);
        if (!ConstantsHolder.xanaConstants.isFromXanaLobby)
        {
            // LoadingHandler.Instance.UpdateLoadingSlider(.8f);
            LoadingHandler.Instance.UpdateLoadingStatusText("Joining World...");
        }
        yield return new WaitForSeconds(.2f);
        if (!(SceneManager.GetActiveScene().name.Contains("Museum")))
        {
            if (WorldItemView.m_EnvName.Contains("AfterParty"))
            {
                if (ConstantsHolder.xanaConstants.setIdolVillaPosition)
                {
                    spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + 2, spawnPoint.z);
                    ConstantsHolder.xanaConstants.setIdolVillaPosition = false;
                }
                else
                {
                    for (int i = 0; i < IdolVillaRooms.instance.villaRooms.Length; i++)
                    {
                        if (IdolVillaRooms.instance.villaRooms[i].name == ChracterPosition.currSpwanPos)
                        {
                            spawnPoint = IdolVillaRooms.instance.villaRooms[i].gameObject.GetComponent<ChracterPosition>().spawnPos;
                            break;
                        }
                        else
                        {
                            spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + 2, spawnPoint.z);
                        }
                    }
                }
            }
            else
            {
                spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + 2, spawnPoint.z);
            }
            RaycastHit hit;
        CheckAgain:
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(spawnPoint, -transform.up, out hit, 2000))
            {
                if (hit.collider.gameObject.tag == "PhotonLocalPlayer" || hit.collider.gameObject.layer == LayerMask.NameToLayer("NoPostProcessing"))
                {
                    spawnPoint = new Vector3(spawnPoint.x + UnityEngine.Random.Range(-1f, 1f), spawnPoint.y, spawnPoint.z + UnityEngine.Random.Range(-1f, 1f));
                    goto CheckAgain;
                } //else if()

                else if (hit.collider.gameObject.GetComponent<NPCRandomMovement>())
                {
                    spawnPoint = new Vector3(spawnPoint.x + UnityEngine.Random.Range(-2, 2), spawnPoint.y, spawnPoint.z + UnityEngine.Random.Range(-2, 2));
                    goto CheckAgain;
                }

                spawnPoint = new Vector3(spawnPoint.x, hit.point.y, spawnPoint.z);
            }
            if (WorldItemView.m_EnvName.Contains("XANALIA NFTART AWARD 2021"))
            {
                mainPlayer.transform.rotation = Quaternion.Euler(0f, 230f, 0f);
            }
            else if (WorldItemView.m_EnvName.Contains("DJ Event") || WorldItemView.m_EnvName.Contains("XANA Festival Stage"))
            {
                mainPlayer.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (WorldItemView.m_EnvName.Contains("Koto") || WorldItemView.m_EnvName.Contains("Tottori") || WorldItemView.m_EnvName.Contains("DEEMO") || WorldItemView.m_EnvName.Contains("XANA Lobby"))
            {
                mainPlayer.transform.rotation = Quaternion.Euler(0f, 180f, 0);
                //Invoke(nameof(SetKotoAngle), 0.5f);
                if (WorldItemView.m_EnvName.Contains("XANA Lobby"))
                {
                    StartCoroutine(setPlayerCamAngle(-0.830f, 0.5572f));
                }
                else
                {
                    StartCoroutine(setPlayerCamAngle(0, 0.75f));

                }
            }
            else if (WorldItemView.m_EnvName.Contains("Genesis"))
            {
                // No Need TO Rotate Player
                StartCoroutine(setPlayerCamAngle(0, 0.75f));
            }
            else if (WorldItemView.m_EnvName.Contains("ZONE X Musuem") || WorldItemView.m_EnvName.Contains("FIVE ELEMENTS"))
            {
                StartCoroutine(setPlayerCamAngle(-30.0f, 0.5f));
            }
            else if (WorldItemView.m_EnvName.Contains("ZONE-X"))
            {
                StartCoroutine(setPlayerCamAngle(0f, 00.5f));
            }
            if (WorldItemView.m_EnvName.Contains("JJ MUSEUM") || WorldItemView.m_EnvName.Contains("FIVE ELEMENTS"))
            {
                PlayerCamera.m_Lens.NearClipPlane = 0.05f;
            }
            if (WorldItemView.m_EnvName.Contains("D_Infinity_Labo"))     // D +  Infinity Labo
            {              // added by AR for ToyotaHome world
                mainPlayer.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                StartCoroutine(setPlayerCamAngle(0f, 00.5f));
            }
            //else
            //{
            //    StartCoroutine(setPlayerCamAngle(0f, 00.5f));
            //}
        }
        mainPlayer.transform.position = new Vector3(0, 0, 0);
        mainController.transform.position = spawnPoint + new Vector3(0, 0.1f, 0);

        InstantiatePlayerAvatar();

        ReferencesForGamePlay.instance.m_34player = player;
        SetAxis();
        mainPlayer.SetActive(true);
        Metaverse.AvatarSpawnerOnDisconnect.Instance.InitCharacter();
        if (player!=null && player.GetComponent<StepsManager>())
        {
            player.GetComponent<StepsManager>().isplayer = true;
        }
        GetComponent<PostProcessManager>().SetPostProcessing();

        // LoadingHandler.Instance.UpdateLoadingSlider(0.98f, true);

        //change youtube player instantiation code because while env is in loading and youtube started playing video
        InstantiateYoutubePlayer();

        SetAddressableSceneActive();
        CharacterLightCulling();
        if (!ConstantsHolder.xanaConstants.isCameraMan)
        {
            LoadingHandler.Instance.HideLoading();
            // LoadingHandler.Instance.UpdateLoadingSlider(0, true);
            LoadingHandler.Instance.UpdateLoadingStatusText("");
        }
        if ((WorldItemView.m_EnvName != "JJ MUSEUM") && player.GetComponent<PhotonView>().IsMine)
        {
            if (!ConstantsHolder.xanaConstants.isCameraMan)
                LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
        }
        else
        {
            if (JjMusuem.Instance)
                JjMusuem.Instance.SetPlayerPos(ConstantsHolder.xanaConstants.mussuemEntry);
            else
            {
                if (!ConstantsHolder.xanaConstants.isCameraMan)
                    LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
            }
        }
        ConstantsHolder.xanaConstants.JjWorldSceneChange = false;

        updatedSpawnpoint.transform.localPosition = spawnPoint;
        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            ConstantsHolder.xanaConstants.isFromXanaLobby = false;
        }
        StartCoroutine(VoidCalculation());
        LightCullingScene();

        yield return new WaitForSeconds(.5f);

        if (ConstantsHolder.xanaConstants.isCameraMan)
        {
            ReferencesForGamePlay.instance.randerCamera.gameObject.SetActive(false);
            ReferencesForGamePlay.instance.FirstPersonCam.gameObject.SetActive(false);
            ConstantsHolder.xanaConstants.StopMic();
            XanaVoiceChat.instance.TurnOffMic();
            //ReferencesForGamePlay.instance.m_34player.GetComponent<CharcterBodyParts>().HidePlayer();/*.gameObject.SetActive(false);*/
        }
        LoadingHandler.Instance.manualRoomController.HideRoomList();

        if (!ConstantsHolder.xanaConstants.isCameraMan)
            LoadingHandler.Instance.HideLoading();
        //TurnOnPostCam();
        // Commented By WaqasAhmad
        {
            //try
            //{
            //    LoadingHandler.Instance.Loading_WhiteScreen.SetActive(false);
            //}
            //catch (System.Exception e)
            //{
            //    //Debug.Log("<color = red>Exception here..............</color>");
            //}
        }
        // Yes Join APi Call Here
        ////Debug.Log("Waqas : Room Joined.");
        //Debug.Log("<color=green> Analytics -- Joined </color>");
        UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(true, false, false, false);

        // Join Room Activate Chat
        ////Debug.Log("<color=blue> XanaChat -- Joined </color>");
        if (XanaEventDetails.eventDetails.DataIsInitialized)
        {
            string worldId = 0.ToString();
            if (XanaEventDetails.eventDetails.environmentId != 0)
            {
                ConstantsHolder.xanaConstants.MuseumID = "" + XanaEventDetails.eventDetails.environmentId;
            }
            else
            {
                ConstantsHolder.xanaConstants.MuseumID = "" + XanaEventDetails.eventDetails.museumId;
            }
        }

        ChatSocketManager.onJoinRoom?.Invoke(ConstantsHolder.xanaConstants.MuseumID);
        if (ConstantsHolder.xanaConstants.isCameraMan)
        {
            if (StreamingCamera.instance)
            {
                StreamingCamera.instance.TriggerStreamCam();
            }
            else // sterming cam's not found so switching to main menu 
            {
                _uiReferences.LoadMain(false);
            }
        }

        XanaWorldDownloader.initialPlayerPos = mainController.transform.localPosition;
        BuilderEventManager.AfterPlayerInstantiated?.Invoke();


        // Firebase Event for Join World
        Debug.Log("Player Spawn Completed --  Join World");
        GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Join_World.ToString());

        /// <summary>
        /// Load NPC fake chat system
        /// </summary>
        //ActivateNpcChat();
    }

    void InstantiatePlayerAvatar()
    {
        if (!ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
               XanaWorldController.SetActive(true);
               XanaPartyController.SetActive(false);
            if (SaveCharacterProperties.instance?.SaveItemList.gender == AvatarGender.Male.ToString())
            {
                player = PhotonNetwork.Instantiate("XanaAvatar2.0_Male", spawnPoint, Quaternion.identity, 0);    // Instantiate Male Avatar
                player.GetComponent<AvatarController>().SetAvatarClothDefault(player.gameObject, "Male");        // Set Default Cloth to avoid naked avatar
            }
            else
            {
                player = PhotonNetwork.Instantiate("XanaAvatar2.0_Female", spawnPoint, Quaternion.identity, 0);  // Instantiate Female Avatar
                player.GetComponent<AvatarController>().SetAvatarClothDefault(player.gameObject, "Female");      // Set Default Cloth to avoid naked avatar
            }
        }
        else
        {
            XanaWorldController.SetActive(false);
            XanaPartyController.SetActive(true);
            spawnPoint.y += 1;
            player = PhotonNetwork.Instantiate("XanaPenguin", spawnPoint, Quaternion.identity, 0);    // Instantiate Penguin
            PenguinPlayer = player;
            if (player != null)
            {
                if (SceneManager.GetActiveScene().name == "Builder")
                {
                    SituationChangerSkyboxScript.instance.builderMapDownload.XANAPartyLoading.SetActive(false);
                }
                StartCoroutine(SetXanaPartyControllers(player));
            }
             
        }
    }

    IEnumerator SetXanaPartyControllers(GameObject player){
        ScreenOrientationManager tempRef = ScreenOrientationManager._instance;
        CharacterManager characterManager = player.GetComponent<CharacterManager>();
        XanaPartyCamera.characterManager = characterManager;
        characterManager.input= XanaPartyInput;
        characterManager.characterCamera = XanaPartyCamera.GetComponentInChildren<Camera>().gameObject;
        //penguinLook.referenceObject = characterManager.headPoint.gameObject;
       // penguinLook.characterManager = characterManager;
        XanaPartyCamera.thirdPersonCamera.Follow = player.transform;// characterManager.headPoint;
        XanaPartyCamera.thirdPersonCamera.LookAt = player.transform;// characterManager.headPoint;
        characterManager.enabled =true;
        XanaPartyCamera.SetCamera();
        XanaPartyCamera.SetDebug();
        XanaPartyCamera.thirdPersonCamera.GetComponent<XANAPartyCameraController>().SetReference(player,characterManager.headPoint.gameObject);
        yield return new WaitForSeconds(0.1f);
        if(GamificationComponentData.instance != null){
            GamificationComponentData.instance.PlayerRigidBody = player.GetComponent<Rigidbody>();
            GamificationComponentData.instance.PlayerRigidBody.constraints = RigidbodyConstraints.None;
            GamificationComponentData.instance.PlayerRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        // Landscape
        tempRef.XanaFeaturesLandsacape.SetActive(false);
        tempRef.XanaChatCanvasLandsacape.SetActive(false);
        tempRef.XanaJumpLandsacape.SetActive(false);
        tempRef.EmoteFavLandsacape.SetActive(false);
        tempRef.PartyChatCanvasLandsacape.SetActive(true);
        tempRef.PartJumpLandsacape.SetActive(true);
        // Potrait
        tempRef.XanaFeaturesPotraite.SetActive(false);
        tempRef.XanaChatCanvasPotraite.SetActive(false);
        tempRef.XanaJumpPotraite.SetActive(false);
        //tempRef.EmoteFavPotraite.SetActive(false);
        tempRef.PartyChatCanvasPotraite.SetActive(true);
        tempRef.PartJumpPotraite.SetActive(true);
        PositionResetButton.SetActive(false);
        if (ConstantsHolder.xanaConstants.isXanaPartyWorld && !ConstantsHolder.xanaConstants.isJoinigXanaPartyGame)
        {
            ReferencesForGamePlay.instance.XANAPartyWaitingText.SetActive(true);

        }
        else
        {
            ReferencesForGamePlay.instance.XANAPartyWaitingText.SetActive(false);
        }
        if (ConstantsHolder.xanaConstants.isXanaPartyWorld && ConstantsHolder.xanaConstants.isJoinigXanaPartyGame && GamificationComponentData.instance!= null && !GamificationComponentData.instance.isRaceStarted  &&  ReferencesForGamePlay.instance != null)
        {
            ReferencesForGamePlay.instance.IsLevelPropertyUpdatedOnlevelLoad = false;
            ReferencesForGamePlay.instance.CheckActivePlayerInCurrentLevel();
        }  
    }

    void UpdateCanvasGroup(CanvasGroup canvasGroup , bool state){
        if (state)
        {   
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.gameObject.SetActive(false);
        }
        else
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.gameObject.SetActive(true);
        }
        print("Disabling "+ canvasGroup.name + ": State: "+state );
    }

    void ActivateNpcChat()
    {
        GameObject npcChatSystem = Resources.Load("NpcChatSystem") as GameObject;
        Instantiate(npcChatSystem);
        //Debug.Log("<color=red> NPC Chat Object Loaded </color>");
    }

    public IEnumerator BackToMainmenuforAutoSwtiching()
    {
        print("AUTO BACK CALL");
        yield return new WaitForSecondsRealtime(30);
        LoadingHandler.Instance.streamingLoading.UpdateLoadingText(false);
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        ConstantsHolder.xanaConstants.JjWorldSceneChange = true;
        _uiReferences.LoadMain(false);
    }


    public IEnumerator SpawnPlayerForBuilderScene()
    {
        LoadingHandler.Instance.UpdateLoadingStatusText("Joining World...");
        yield return new WaitForSeconds(0.2f);
        spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + 2, spawnPoint.z);

        RaycastHit hit;
    CheckAgain:
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(spawnPoint, -transform.up, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "PhotonLocalPlayer" || hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.layer == LayerMask.NameToLayer("NoPostProcessing"))
            {
                if (BuilderData.StartFinishPoints.Count > 1 && BuilderData.mapData.data.worldType == 1)
                {
                    StartFinishPointData startFinishPoint = BuilderData.StartFinishPoints.Find(x => x.IsStartPoint);
                    StartPoint sp = startFinishPoint.SpawnObject.GetComponent<StartPoint>();
                    BuilderData.StartPointID = startFinishPoint.ItemID;
                    spawnPoint = sp.SpawnPoints[UnityEngine.Random.Range(0, sp.SpawnPoints.Count)].transform.position;
                }
                else
                {
                    spawnPoint = new Vector3(spawnPoint.x + UnityEngine.Random.Range(-1f, 1f), spawnPoint.y, spawnPoint.z + UnityEngine.Random.Range(-1f, 1f));
                }
                goto CheckAgain;
            } //else if()

            else if (hit.collider.gameObject.GetComponent<NPCRandomMovement>())
            {
                spawnPoint = new Vector3(spawnPoint.x + UnityEngine.Random.Range(-2, 2), spawnPoint.y, spawnPoint.z + UnityEngine.Random.Range(-2, 2));
                goto CheckAgain;
            }

            spawnPoint = new Vector3(spawnPoint.x, hit.point.y, spawnPoint.z);
        }

        mainPlayer.transform.position = new Vector3(0, 0, 0);
        mainController.transform.position = spawnPoint + new Vector3(0, 0.1f, 0);

        InstantiatePlayerAvatar();
        while (player == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (ConstantsHolder.xanaConstants.isBuilderScene && !ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            player.transform.localScale = Vector3.one * 1.153f;
            Rigidbody playerRB = player.AddComponent<Rigidbody>();
            playerRB.mass = 70;
            playerRB.isKinematic = true;
            playerRB.useGravity = true;
            playerRB.constraints = RigidbodyConstraints.FreezeRotation;
            GamificationComponentData.instance.PlayerRigidBody = playerRB;
            player.AddComponent<KeyValues>();
            GamificationComponentData.instance.spawnPointPosition = mainController.transform.position;
            GamificationComponentData.instance.buildingDetect = player.AddComponent<BuildingDetect>();
            //player.GetComponent<CapsuleCollider>().isTrigger = false;
            //player.GetComponent<CapsuleCollider>().enabled = false;
            TimeStats.playerCanvas = Instantiate(GamificationComponentData.instance.playerCanvas);
            GamificationComponentData.instance.playerControllerNew = mainPlayer.GetComponentInChildren<PlayerController>();
            player.AddComponent<EnvironmentChecker>();
            if (GamificationComponentData.instance.raycast == null)
                GamificationComponentData.instance.raycast = new GameObject("Raycasst");
            GamificationComponentData.instance.raycast.transform.SetParent(GamificationComponentData.instance.playerControllerNew.transform);
            GamificationComponentData.instance.raycast.transform.localPosition = Vector3.up * 1.683f;
            GamificationComponentData.instance.raycast.transform.localScale = Vector3.one * 0.37f;
            if (GamificationComponentData.instance.worldCameraEnable)
                BuilderEventManager.EnableWorldCanvasCamera?.Invoke();
            GamificationComponentData.instance.avatarController = player.GetComponent<AvatarController>();
            GamificationComponentData.instance.charcterBodyParts = player.GetComponent<CharacterBodyParts>();
            GamificationComponentData.instance.ikMuseum = player.GetComponent<IKMuseum>();

            //Post Process enable for Builder Scene
            firstPersonCamera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            environmentCameraRender.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            Camera freeCam = this.GetComponent<PostProcessManager>().freeCam;
            freeCam.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            if (player.GetComponent<StepsManager>())
            {
                player.GetComponent<StepsManager>().isplayer = true;
            }
            //set Far & Near value same as builder for flickering assets testing
            firstPersonCamera.nearClipPlane = 0.03f;
            environmentCameraRender.nearClipPlane = 0.03f;
            freeCam.nearClipPlane = 0.03f;
            firstPersonCamera.farClipPlane = 1000;
            environmentCameraRender.farClipPlane = 1000;
            freeCam.farClipPlane = 1000;
            BuilderEventManager.ApplySkyoxSettings?.Invoke();
            //Rejoin world after internet connection stable
            if (GamificationComponentData.instance.isBuilderWorldPlayerSetup)
            {
                ReferencesForGamePlay.instance.playerControllerNew.StopBuilderComponent();
                SituationChangerSkyboxScript.instance.builderMapDownload.PlayerSetup();
                SituationChangerSkyboxScript.instance.builderMapDownload.UpdateScene();
                BuilderEventManager.ChangeCameraHeight?.Invoke(false);
            }
        }
        else
        {
             BuilderEventManager.ApplySkyoxSettings?.Invoke();
        }
        if ((WorldItemView.m_EnvName != "JJ MUSEUM") && player.GetComponent<PhotonView>().IsMine)
        {
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
        }
        ReferencesForGamePlay.instance.m_34player = player;
        SetAxis();
        mainPlayer.SetActive(true);
        Metaverse.AvatarSpawnerOnDisconnect.Instance.InitCharacter();
    End:
        //LoadingHandler.Instance.UpdateLoadingSlider(0.98f, true);
        yield return new WaitForSeconds(1);

        // Commented By WaqasAhmad
        {
            //try
            //{
            //    LoadingHandler.Instance.Loading_WhiteScreen.SetActive(false);
            //}
            //catch (System.Exception e)
            //{
            //    //Debug.Log("<color = red> Exception here..............</color>");
            //}
        }

        SetAddressableSceneActive();
        updatedSpawnpoint.localPosition = spawnPoint;
        StartCoroutine(VoidCalculation());
        LightCullingScene();


        if (!ConstantsHolder.xanaConstants.isCameraMan)
        {
            LoadingHandler.Instance.HideLoading();
            // LoadingHandler.Instance.UpdateLoadingSlider(0, true);
            LoadingHandler.Instance.UpdateLoadingStatusText("");
        }
        if ((WorldItemView.m_EnvName != "JJ MUSEUM") && player.GetComponent<PhotonView>().IsMine)
        {
            if (!ConstantsHolder.xanaConstants.isCameraMan)
                LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
        }
        else
        {
            JjMusuem.Instance.SetPlayerPos(ConstantsHolder.xanaConstants.mussuemEntry);
        }
        ConstantsHolder.xanaConstants.JjWorldSceneChange = false;

        //if ( GamificationComponentData.instance.MultiplayerComponentData.Count > 0)
        //{
        //    foreach (var _itemData in GamificationComponentData.instance.MultiplayerComponentData)
        //    {
        //        var multiplayerObject = Instantiate(GamificationComponentData.instance.MultiplayerComponente, _itemData.Position, _itemData.Rotation);
        //        MultiplayerComponentData multiplayerComponentData = new();
        //        multiplayerComponentData.RuntimeItemID = _itemData.RuntimeItemID;
        //        //multiplayerComponentData.viewID = multiplayerObject.GetPhotonView().ViewID;
        //        GamificationComponentData.instance.SetMultiplayerComponentData(multiplayerComponentData);
        //    }
        //}

        while (!GamificationComponentData.instance.isSkyLoaded)
            yield return new WaitForSeconds(0.5f);
        BuilderEventManager.AfterPlayerInstantiated?.Invoke();


        isEnvLoaded = true;
        yield return new WaitForSeconds(1.75f);
        if (!ConstantsHolder.xanaConstants.isBackFromWorld)
        {
            LoadingHandler.Instance.HideLoading();

        }
        // LoadingHandler.Instance.UpdateLoadingSlider(0, true);
        //LoadingHandler.Instance.UpdateLoadingStatusText("");


        // Yes Join APi Call Here
        ////Debug.Log("Waqas : Room Joined.");
        //Debug.Log("<color=green> Analytics -- Joined </color>");
        UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(true, false, false, false);
        ChatSocketManager.onJoinRoom?.Invoke(ConstantsHolder.xanaConstants.builderMapID.ToString());

        Debug.Log("Player Spawn Completed --  Join World");
        GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Join_World.ToString());
        //yield return new WaitForSeconds(5f);
        //if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
        //{
        //    XanaPartyCamera.thirdPersonCamera.m_XAxis.Value = 0;
        //    XanaPartyCamera.thirdPersonCamera.m_YAxis.Value = 0.66f;
        //}
        //ActivateNpcChat();
    }

    public IEnumerator setPlayerCamAngle(float xValue, float yValue)
    {
        yield return new WaitForSeconds(0.1f);
        PlayerCamera.m_XAxis.Value = xValue;
        PlayerCamera.m_YAxis.Value = yValue;
    }

    //void SetKotoAngle()
    //{
    //    PlayerCamera.m_XAxis.Value = 0f;
    //    PlayerCamera.m_YAxis.Value = 0.75f;
    //}
    public void SetAxis()
    {
        CinemachineFreeLook cam = PlayerCamera.GetComponent<CinemachineFreeLook>();
        if (cam)
        {
            if (ConstantsHolder.xanaConstants.EnviornmentName == "XANALIA NFTART AWARD 2021")
            {
                cam.Follow = mainController.transform;
                cam.m_XAxis.Value = 0;
                cam.m_YAxis.Value = 0.5f;
            }
            else
            {

                cam.Follow = mainController.transform;
                cam.m_XAxis.Value = 180;
                cam.m_YAxis.Value = 0.5f;
            }

            if (ConstantsHolder.xanaConstants.EnviornmentName == "DJ Event" || ConstantsHolder.xanaConstants.EnviornmentName == "Xana Festival" || ConstantsHolder.xanaConstants.EnviornmentName == "NFTDuel Tournament")
            {
                cam.Follow = mainController.transform;
                cam.m_XAxis.Value = 0;
                cam.m_YAxis.Value = 0.5f;
            }
            else
            {

                cam.Follow = mainController.transform;
                cam.m_XAxis.Value = 173;
                cam.m_YAxis.Value = 0.5f;
            }


        }

        CinemachineFreeLook cam2 = playerCameraCharacterRender.GetComponent<CinemachineFreeLook>();
        if (cam2)
        {

            if (ConstantsHolder.xanaConstants.EnviornmentName == "XANALIA NFTART AWARD 2021")
            {
                cam2.Follow = mainController.transform;
                cam2.m_XAxis.Value = 0;
                cam2.m_YAxis.Value = 0.5f;
            }

            else
            {

                cam2.Follow = mainController.transform;
                cam2.m_XAxis.Value = 180;
                cam2.m_YAxis.Value = 0.5f;
            }
            if (ConstantsHolder.xanaConstants.EnviornmentName == "DJ Event" || ConstantsHolder.xanaConstants.EnviornmentName == "Xana Festival" || ConstantsHolder.xanaConstants.EnviornmentName == "NFTDuel Tournament")
            {
                cam2.Follow = mainController.transform;
                cam2.m_XAxis.Value = 0;
                cam2.m_YAxis.Value = 0.5f;
            }
            else
            {

                cam2.Follow = mainController.transform;
                cam2.m_XAxis.Value = 173;
                cam2.m_YAxis.Value = 0.5f;
            }

        }

       
    }

    public void ResetPlayerPosition()
    {
        if (ConstantsHolder.xanaConstants.isBuilderScene)
        {
            //RaycastHit hit;
            //if (Physics.Raycast(new Vector3(spawnPoint.x, spawnPoint.y + 1000, spawnPoint.z), Vector3.down, out hit, 3000))
            //{
            //    mainController.transform.localPosition = new Vector3(spawnPoint.x, hit.point.y, spawnPoint.z);
            //}
            //else
            //{
            //    mainController.transform.localPosition = new Vector3(spawnPoint.x, 100, spawnPoint.z);
            //}
            //Player respawn at spawn point after jump down from world
            if (!ConstantsHolder.xanaConstants.isXanaPartyWorld)
                mainController.transform.localPosition = AvoidAvatarMergeInBuilderScene();
            else if (XanaPartyCamera.characterManager != null)
                XanaPartyCamera.characterManager.transform.localPosition = AvoidAvatarMergeInBuilderScene();
        }
        else
        {
            mainController.GetComponent<PlayerController>().gravityVector.y = 0;
            mainController.transform.localPosition = spawnPoint;
        }
        if (IdolVillaRooms.instance != null)
        {
            IdolVillaRooms.instance.ResetVilla();
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(false, false, false, true);
    }


    public override void OnLeftRoom()
    {

    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //Debug.Log("Instantiating Photon Complete");

        ResetPlayerPosition();
    }
    /*******************************************************************new code */

    string environmentLabel;
    public void LoadEnvironment(string label)
    {
        environmentLabel = label;
        StartCoroutine(DownloadAssets());
    }



    void SetupEnvirnmentForBuidlerScene()
    {
        if (ConstantsHolder.xanaConstants.orientationchanged && ConstantsHolder.xanaConstants.JjWorldSceneChange)
        {
            ScreenOrientationManager._instance.MyOrientationChangeCode(DeviceOrientation.Portrait);
        }
        Transform tempSpawnPoint = null;
        LoadingHandler.Instance.UpdateLoadingStatusText("Getting World Ready....");

        if (BuilderData.StartFinishPoints.Count > 1 && BuilderData.mapData.data.worldType == 1)
        {
            StartFinishPointData startFinishPoint = BuilderData.StartFinishPoints.Find(x => x.IsStartPoint);
            StartPoint sp = startFinishPoint.SpawnObject.GetComponent<StartPoint>();
            BuilderData.StartPointID = startFinishPoint.ItemID;
            tempSpawnPoint = sp.SpawnPoints[UnityEngine.Random.Range(0, sp.SpawnPoints.Count)].transform;
        }
        else
        {
            if (BuilderData.spawnPoint.Count == 1)
            {
                tempSpawnPoint = BuilderData.spawnPoint[0].spawnObject.transform;
                BuilderSpawnPoint = true;
            }
            else if (BuilderData.spawnPoint.Count > 1)
            {
                foreach (SpawnPointData g in BuilderData.spawnPoint)
                {
                    if (g.IsActive)
                    {
                        BuilderSpawnPoint = true;
                        tempSpawnPoint = g.spawnObject.transform;
                        break;
                    }
                }
            }
        }
        if (tempSpawnPoint != null)
            spawnPoint = tempSpawnPoint.position;
        if (tempSpawnPoint == null)
        {
            GameObject newobject = new GameObject("SpawningPoint");
            newobject.transform.position = new Vector3(0, 2500, 0);
            tempSpawnPoint = newobject.transform;
            RaycastHit hit;
            if (Physics.Raycast(newobject.transform.position, newobject.transform.TransformDirection(Vector3.down), out hit, 3000))
            {
                newobject.transform.position = new Vector3(0, hit.point.y, 0);
            }
            else
            {
                newobject.transform.position = new Vector3(0, 100, 0);
            }
            spawnPoint = newobject.transform.position;
        }
        BuilderAssetDownloader.initialPlayerPos = tempSpawnPoint.localPosition;
        if (tempSpawnPoint)
        {
            if (XanaEventDetails.eventDetails.DataIsInitialized)
            {
                StartCoroutine(SpawnPlayer());
            }
            else
            {
                StartCoroutine(SpawnPlayerForBuilderScene());
            }
        }


    }

    IEnumerator DownloadAssets()
    {
        if (!isEnvLoaded)
        {
            if (environmentLabel.Contains(" : "))
            {
                string name = environmentLabel.Replace(" : ", string.Empty);
                environmentLabel = name;
            }
            while (!ConstantsHolder.isAddressableCatalogDownload)
            {
                yield return new WaitForSeconds(1f);
            }
            //yield return StartCoroutine(DownloadEnvoirnmentDependanceies(environmentLabel));
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(environmentLabel, LoadSceneMode.Additive, false);
            //if (ConstantsHolder.xanaConstants.isFromXanaLobby)
            //{
            //    LoadingHandler.Instance.UpdateLoadingSliderForJJ(UnityEngine.Random.Range(0.5f,0.7f), 0.1f);
            //}
            if (!ConstantsHolder.xanaConstants.isFromXanaLobby)
            {
                LoadingHandler.Instance.UpdateLoadingStatusText("Loading World...");
                //LoadingHandler.Instance.UpdateLoadingSlider(.6f, true);
            }
            yield return handle;
            addressableSceneName = environmentLabel;
            //...
            if (environmentLabel=="RoofTopParty")
            {
                GameObject obj = new GameObject();
                obj.name = "Sky Fix";
                obj.AddComponent<ApplySkyBoxShader>();
                obj.GetComponent<ApplySkyBoxShader>().skyBoxShader ="Skybox/Cubemap";
            }
            //One way to handle manual scene activation.
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                AddressableDownloader.Instance.MemoryManager.AddToReferenceList(handle, environmentLabel);

                yield return handle.Result.ActivateAsync();
                DownloadCompleted();
            }
            else // error occur 
            {
                AssetBundle.UnloadAllAssetBundles(false);
                Resources.UnloadUnusedAssets();

                HomeBtn.onClick.Invoke();
            }
            // Addressables.Release(handle);
        }
        else
        {
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();

            RespawnPlayer();
        }
    }
    private void DownloadCompleted()
    {
        isEnvLoaded = true;
        StartCoroutine(spwanPlayerWithWait());
    }

    IEnumerator spwanPlayerWithWait()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
    CheckAgain:
        Transform temp = null;
        if (GameObject.FindGameObjectWithTag("SpawnPoint"))
            temp = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        else
            temp = new GameObject("SpawnPoint").transform;
        if (temp)
        {
            spawnPoint = temp.position;
        }
        else
            goto CheckAgain;
        if (temp)
        {
            StartCoroutine(SpawnPlayer());
        }
        yield return null;
    }


    void RespawnPlayer()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GamePlayScene"));
        StartCoroutine(spwanPlayerWithWait());
    }


    void ResetPlayerAfterInstantiation()
    {
        if (BuilderAssetDownloader.isPostLoading)
        {
            //Debug.LogError("here resetting player .... ");
            if (BuilderData.StartFinishPoints.Count > 1 && BuilderData.mapData.data.worldType == 1 )
            {
                if (!IsPlayerOnStartPoint())
                {
                    BuilderSpawnPoint = true;
                    StartFinishPointData startFinishPoint = BuilderData.StartFinishPoints.Find(x => x.IsStartPoint);
                    StartPoint sp = startFinishPoint.SpawnObject.GetComponent<StartPoint>();
                    BuilderData.StartPointID = startFinishPoint.ItemID;
                    spawnPoint = sp.SpawnPoints[UnityEngine.Random.Range(0, sp.SpawnPoints.Count)].transform.position;
                }
               
            }
            else
            {
                if (BuilderData.spawnPoint.Count == 1)
                {
                    BuilderSpawnPoint = true;
                    spawnPoint = BuilderData.spawnPoint[0].spawnObject.transform.localPosition;
                }
                else if (BuilderData.spawnPoint.Count > 1)
                {
                    foreach (SpawnPointData g in BuilderData.spawnPoint)
                    {
                        if (g.IsActive)
                        {
                            BuilderSpawnPoint = true;
                            spawnPoint = g.spawnObject.transform.localPosition;
                            break;
                        }
                    }
                }
            }

            if (!ConstantsHolder.xanaConstants.isXanaPartyWorld)
                mainController.transform.localPosition = AvoidAvatarMergeInBuilderScene();
            else if (XanaPartyCamera.characterManager != null && !IsPlayerOnStartPoint())
                XanaPartyCamera.characterManager.transform.localPosition = AvoidAvatarMergeInBuilderScene();
        }
    }

    Vector3 AvoidAvatarMergeInBuilderScene()
    {
        Vector3 spawnPoint = this.spawnPoint;
        spawnPoint.y += BuilderSpawnPoint ? 1 : 1000;

        RaycastHit hit;
    CheckAgain:
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(spawnPoint, -transform.up, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "PhotonLocalPlayer" || hit.collider.gameObject.tag == "Player" || hit.collider.gameObject.layer == LayerMask.NameToLayer("NoPostProcessing"))
            {
                PhotonView pv = hit.collider.GetComponent<PhotonView>();
                if (pv == null || !pv.IsMine)
                {
                    if (BuilderData.StartFinishPoints.Count > 1 && BuilderData.mapData.data.worldType == 1 )
                    {
                        if (!IsPlayerOnStartPoint())
                        {
                            StartFinishPointData startFinishPoint = BuilderData.StartFinishPoints.Find(x => x.IsStartPoint);
                            StartPoint sp = startFinishPoint.SpawnObject.GetComponent<StartPoint>();
                            BuilderData.StartPointID = startFinishPoint.ItemID;
                            spawnPoint = sp.SpawnPoints[UnityEngine.Random.Range(0, sp.SpawnPoints.Count)].transform.position;
                        }
                       
                    }
                    else
                    {

                        spawnPoint = new Vector3(spawnPoint.x + UnityEngine.Random.Range(-1f, 1f), spawnPoint.y, spawnPoint.z + UnityEngine.Random.Range(-1f, 1f));
                    }
                    goto CheckAgain;
                }
            } //else if()

            else if (hit.collider.gameObject.GetComponent<NPCRandomMovement>())
            {
                spawnPoint = new Vector3(spawnPoint.x + UnityEngine.Random.Range(-2, 2), spawnPoint.y, spawnPoint.z + UnityEngine.Random.Range(-2, 2));
                goto CheckAgain;
            }
            spawnPoint = new Vector3(spawnPoint.x, hit.point.y, spawnPoint.z);
            this.spawnPoint = spawnPoint;
        }
        return spawnPoint;
    }

    bool IsPlayerOnStartPoint()
    {
        Transform playerTransform = PenguinPlayer.transform;
        float raycastDistance = 2f; // Adjust as needed

        // Cast a ray from the player's position downward
        if (Physics.Raycast(playerTransform.position, -playerTransform.up, out RaycastHit hit, raycastDistance))
        {
            // Draw a debug ray to visualize the hit point
            Debug.DrawRay(playerTransform.position, -playerTransform.up * raycastDistance, Color.green);

            // Check if the hit object has the StartPoint script
            StartPoint startPoint = hit.collider.GetComponentInParent<StartPoint>();
            if (startPoint != null)
            {
                Debug.Log("Player is on a StartPoint object.");
                return true;
            }
            else
            {
                Debug.Log("Player is not on a StartPoint object.");
                return false;
            }
        }
        else
        {
            Debug.Log("Player is not on a StartPoint object.");
            return false;
        }
    }

    public void SetAddressableSceneActive()
    {
        string temp = addressableSceneName;
        if (temp.Contains(" Astroboy x Tottori Metaverse Museum"))
        {
            temp = "Astroboy x Tottori Metaverse Museum";
        }
        ////Debug.LogError("~~~~~~scene name to be activated :-  " + temp);
        if (!string.IsNullOrEmpty(temp))
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(temp));
        else if (ConstantsHolder.xanaConstants.isBuilderScene)
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Builder"));
        else
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(ConstantsHolder.xanaConstants.EnviornmentName));

    }

    void LightCullingScene()
    {
        // Forcfully resetting lights because on 

        if (GetComponent<PostProcessManager>().CheckPostProcessEnable())
        {
            Light[] sceneLight;
            sceneLight = GameObject.FindObjectsOfType<Light>();
            for (int i = 0; i < sceneLight.Length; i++)
            {
                if (sceneLight[i].name.Contains("Character"))
                {
                    // sceneLight[i].cullingMask = LayerMask.GetMask("Nothing");
                    sceneLight[i].cullingMask = LayerMask.GetMask("NoPostProcessing");
                }
                else if (sceneLight[i].name.Contains("Directional Light"))
                {
                    if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("FIVE ELEMENTS"))
                    {
                        sceneLight[i].cullingMask = LayerMask.GetMask("Water");
                    }
                    else
                    {
                        sceneLight[i].cullingMask = LayerMask.GetMask("Default", "TransparentFX", "RenderTexture", "Character", "Head", "Body", "Plane", "Room", "AvaterSelection", "MiniMap", "ZoomUI", "Arrow", "CameraColliderIgnore", "PostProcessing", "PictureInteractable", "Particles", "NFTDisplayPanel", "NoRenderOnFPS", "Hair_Light");
                    }
                }
            }
        }
        else
        {
            CharacterLightCulling();
        }
    }


}