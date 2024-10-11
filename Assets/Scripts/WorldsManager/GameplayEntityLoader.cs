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
using Photon.Pun.Demo.PunBasics;
using Photon.Voice.PUN;
using PhysicsCharacterController;
using System.Threading.Tasks;
#if UNITY_IOS
using UnityEngine.iOS;
#endif


public class GameplayEntityLoader : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public StayTimeTrackerForSummit StayTimeTrackerForSummit;
    public bool isAlreadySpawned;
    public Camera MiniMapCamera;
    [Header("singleton object")]
    public static GameplayEntityLoader instance;
    public bool IsJoinSummitWorld = false;
    public GameObject mainPlayer;
    public GameObject mainController;
    private GameObject mainControllerRefHolder;
    private GameObject YoutubeStreamPlayer;
    public GameObject PenguinPlayer;
    public GameObject DashButton;
    public ActionManager ActionEmoteSystem;

    public CinemachineFreeLook PlayerCamera;
    public CinemachineFreeLook playerCameraCharacterRender;
    public Camera environmentCameraRender;
    public Camera firstPersonCamera;
    [HideInInspector]
    private Transform updatedSpawnpoint;
    private Transform _spawnTransform;
    [HideInInspector]
    public Vector3 spawnPoint;
    public GameObject currentEnvironment;
    public bool isEnvLoaded = false;

    private float fallOffset = 10f;
    public bool setLightOnce = false;

    [HideInInspector]
    public GameObject player;

    System.DateTime eventUnivStartDateTime, eventLocalStartDateTime, eventlocalEndDateTime;

    [HideInInspector]
    public GameObject leftJoyStick;

    [HideInInspector]
    public float joyStickMovementRange;

    public LayerMask layerMask;

    public string addressableSceneName;

    [SerializeField] Button HomeBtn;

    public double eventRemainingTime;

    public HomeSceneLoader _uiReferences;
    public bool ClothsLoaded = false;
    //string OrdinaryUTCdateOfSystem = "2023-08-10T14:45:00.000Z";
    //DateTime OrdinarySystemDateTime, localENDDateTime, univStartDateTime, univENDDateTime;

    //Bool for BuilderSpawn point available or not
    bool BuilderSpawnPoint = false;

    #region XANA PARTY WORLD
    [Header("XANA Party")]
    public GameObject PositionResetButton;
    [SerializeField] GameObject XanaWorldController;
    [SerializeField] GameObject XanaPartyController;
    [SerializeField] public CameraManager XanaPartyCamera;
    [SerializeField] InputReader XanaPartyInput;
    [SerializeField] PenguinLookPointTracker penguinLook;
    public ReferenceForPenguinAvatar referenceForPenguin;
    #endregion
    [SerializeField] RaffleTicketHandler _raffleTickets;

    [Header("XANA Summit Performer AI")]
    public GameObject[] AIAvatarPrefab;

    public XANASummitDataContainer XanaSummitDataContainerObject;
    public DownloadPopupHandler DownloadPopupHandlerInstance;

    public GameObject OldPlayer;
    private void Awake()
    {
        instance = this;
        setLightOnce = false;
        mainControllerRefHolder = mainController;
        if (ConstantsHolder.xanaConstants.isJoinigXanaPartyGame && ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            PositionResetButton.SetActive(true);
            Invoke(nameof(LoadFile), 1f);
        }
    }


    private void OnDestroy()
    {
        Physics.autoSimulation = true;
        Resources.UnloadUnusedAssets();
        GC.SuppressFinalize(this);
        GC.Collect(0);
    }

    private void Start()
    {
        if (XanaEventDetails.eventDetails.DataIsInitialized)
        {
            StartEventTimer();
        }
        Input.multiTouchEnabled = true;
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

        GameObject _updatedSpawnPoint = new GameObject();
        updatedSpawnpoint = _updatedSpawnPoint.transform;
        SceneManager.MoveGameObjectToScene(updatedSpawnpoint.gameObject, SceneManager.GetSceneByName("GamePlayScene"));
        BuilderSpawnPoint = false;

        // Reset Camera 
         MiniMapCamera.orthographicSize = 30;
        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Summit"))
        {
            // Zoom Out map Camera
            MiniMapCamera.orthographicSize = 30;
        }
        ConstantsHolder.xanaConstants.isGoingForHomeScene = false;

        //ForcedMapOpenForSummitScene();
    }

    void OnEnable()
    {
        BuilderEventManager.AfterWorldInstantiated += ResetPlayerAfterInstantiation;
        GamePlayButtonEvents.OnExitButtonXANASummit += ResetOnBackFromSummit;
    }


    private void OnDisable()
    {
        BuilderEventManager.AfterWorldInstantiated -= ResetPlayerAfterInstantiation;
        GamePlayButtonEvents.OnExitButtonXANASummit -= ResetOnBackFromSummit;
    }
    SummitPlayerRPC _SummitPlayerRPC;
    public void ForcedMapOpenForSummitScene()
    {
        if (ReferencesForGamePlay.instance.m_34player == null && !ConstantsHolder.xanaConstants)
            return;
        
        try
        {
            _SummitPlayerRPC = ReferencesForGamePlay.instance.m_34player.GetComponent<SummitPlayerRPC>();
        }
        catch (Exception ex)
        {
            Debug.LogError("Player not found");
        }


        if (ConstantsHolder.xanaConstants.EnviornmentName == "XANA Summit" && !_SummitPlayerRPC.isInsideCAr && !_SummitPlayerRPC.isInsideWheel)
        {
            ReferencesForGamePlay.instance.minimap.SetActive(true);
            PlayerPrefs.SetInt("minimap", 1);
            ConstantsHolder.xanaConstants.minimap = PlayerPrefs.GetInt("minimap");
            ReferencesForGamePlay.instance.SumitMapStatus(true);

            XanaChatSystem.instance.chatDialogBox.SetActive(false);
        }
        else
        { 
            ReferencesForGamePlay.instance.minimap.SetActive(false);
            PlayerPrefs.SetInt("minimap", 0);
            ConstantsHolder.xanaConstants.minimap = PlayerPrefs.GetInt("minimap");
            ReferencesForGamePlay.instance.SumitMapStatus(false);
        }
    }
    public void ForcedMapCloseForSummitScene()
    {
        if (ConstantsHolder.xanaConstants.EnviornmentName == "XANA Summit")
        {
            ReferencesForGamePlay.instance.minimap.SetActive(false);
            PlayerPrefs.SetInt("minimap", 0);
            ConstantsHolder.xanaConstants.minimap = 0;
            ReferencesForGamePlay.instance.SumitMapStatus(false);
        }
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
        if (currentEnvironment == null)
        {
            if (ConstantsHolder.xanaConstants.isBuilderScene)
                StartCoroutine(WaitForMapDownload());

            else
            {
                LoadEnvironment(ConstantsHolder.xanaConstants.EnviornmentName);
                //CharacterLightCulling();  //amit-05-05-2023 commented this line as it excuted before env instantidated and env light is not culled. called this method on another right place
            }
        }
        else
        {
            LoadingHandler.CompleteSlider?.Invoke();
            StartCoroutine(SpawnPlayer());
        }

        PlayerCamera.gameObject.SetActive(true);
        environmentCameraRender.gameObject.SetActive(true);
        PlayerSelfieController.Instance.DisableSelfieFromStart();
    }

    void InstantiateYoutubePlayer()
    {
        if (YoutubeStreamPlayer == null)
        {
            if (WorldItemView.m_EnvName.Contains("DJ Event"))
            {
                YoutubeStreamPlayer = Instantiate(Resources.Load("DJEventData/YoutubeVideoPlayer") as GameObject);
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
            if (mainController == null)
                return false;
            if (mainController?.transform.position.y < (updatedSpawnpoint.transform.position.y - fallOffset))
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
    public void SetSpawnPosition()
    {


    }

    public void SetPlayer()
    {


        AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer = null;
        SpawnPlayerSection();

    }
    public async void SpawnPlayerSection()  // Created this for summit
    {
        spawnPoint = player.transform.position;
   ;
        OldPlayer = player;
        ClothsLoaded = false;
        Debug.Log("player shoud be destroyed");
        InstantiatePlayerAvatarSector(new Vector3 (0,-1000,0));  // instantiate player below ground to avoid glitter;
        
        while(ClothsLoaded == false)
        {
            await Task.Delay(1000);
        }
        Quaternion rotation = OldPlayer.transform.localRotation;
       
        Destroy(OldPlayer);
        MutiplayerController.instance.DestroyPlayerDelay();
        player.transform.parent = mainController.transform;
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = rotation;
   
        ReferencesForGamePlay.instance.m_34player = player;
        if (player.GetComponent<SummitAnalyticsTrigger>() == null)
            player.AddComponent<SummitAnalyticsTrigger>();
        //  SetAxis();
        mainPlayer.SetActive(true);
        if (player.GetComponent<StepsManager>())
        {
            player.GetComponent<StepsManager>().isplayer = true;
        }
        //GetComponent<PostProcessManager>().SetPostProcessing();

        //change youtube player instantiation code because while env is in loading and youtube started playing video




        XanaWorldDownloader.initialPlayerPos = mainController.transform.localPosition;



        // Firebase Event for Join World
        /* Debug.Log("Player Spawn Completed --  Join World");
         GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Join_World.ToString());
         UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(true, false, false, false);*/
        /// <summary>
        /// Load NPC fake chat system
        /// </summary>
        //ActivateNpcChat();

        await new WaitForSeconds(1);
        var controller = GameplayEntityLoader.instance.mainController.GetComponent<PlayerController>();
        if (controller.isFirstPerson)
        {
            controller.DisablePlayerOnFPS();
        }

    }

    public IEnumerator SpawnPlayer()
    {
        if (!ConstantsHolder.xanaConstants.isFromXanaLobby)
        {
            LoadingHandler.Instance.UpdateLoadingStatusText("Joining World...");
        }
        //code by hardik 9aug2024
        if (!(SceneManager.GetActiveScene().name.Contains("Museum")))
        {
            spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + .5f, spawnPoint.z);
            RaycastHit hit;

            // Loop to check for valid spawn point
            bool validSpawnPointFound = false;
            int maxAttempts = 10; // Limit the number of attempts to prevent an infinite loop
            int attempts = 0;

            while (!validSpawnPointFound && attempts < maxAttempts)
            {
                attempts++;

                // Cast a ray downwards from the spawn point to detect collisions
                if (Physics.Raycast(spawnPoint, -transform.up, out hit, 2000))
                {
                    // Check if the hit object is a player or other non-walkable surface
                    if (hit.collider.gameObject.CompareTag("PhotonLocalPlayer") ||
                        hit.collider.gameObject.layer == LayerMask.NameToLayer("NoPostProcessing"))
                    {
                        // Adjust spawn point slightly if occupied
                        spawnPoint = new Vector3(
                            spawnPoint.x + UnityEngine.Random.Range(-3f, 3f),
                            spawnPoint.y,
                            spawnPoint.z + UnityEngine.Random.Range(-3f, 3f)
                        );
                    }
                    else
                    {
                        // Valid spawn point found
                        spawnPoint = new Vector3(spawnPoint.x, hit.point.y, spawnPoint.z);
                        validSpawnPointFound = true;
                    }
                }
            }

            if (!validSpawnPointFound)
            {
                Debug.LogWarning("Failed to find a valid spawn point after multiple attempts.");
            }

            SetPlayerCameraAngle();
        }

        // Set main player and controller positions
        mainPlayer.transform.position = new Vector3(0, 0, 0);

        if (mainController == null)
            mainController = mainControllerRefHolder;

        mainController.transform.position = spawnPoint + new Vector3(0, 0.1f, 0);

        // Optional: Adjust camera position based on new player position
        Vector3 newPos = spawnPoint + new Vector3(500, 500f, 500);



        //if (!(SceneManager.GetActiveScene().name.Contains("Museum")))
        //{
        //    spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + 2, spawnPoint.z);
        //    RaycastHit hit;
        //CheckAgain:
        //    // Does the ray intersect any objects excluding the player layer
        //    if (Physics.Raycast(spawnPoint, -transform.up, out hit, 2000))
        //    {
        //        if (hit.collider.gameObject.tag == "PhotonLocalPlayer" || hit.collider.gameObject.layer == LayerMask.NameToLayer("NoPostProcessing"))
        //        {
        //            spawnPoint = new Vector3(spawnPoint.x + UnityEngine.Random.Range(-1f, 1f), spawnPoint.y, spawnPoint.z + UnityEngine.Random.Range(-1f, 1f));
        //            goto CheckAgain;
        //        }
        //        spawnPoint = new Vector3(spawnPoint.x, hit.point.y, spawnPoint.z);
        //    }
        //    SetPlayerCameraAngle();
        //}


        //mainPlayer.transform.position = new Vector3(0, 0, 0);
        //if (mainController == null)
        //    mainController = mainControllerRefHolder;
        //mainController.transform.position = spawnPoint + new Vector3(0, 0.1f, 0);

        //Vector3 newPos = spawnPoint + new Vector3(500, 500f, 500);
        InstantiatePlayerAvatar(newPos);

        ReferencesForGamePlay.instance.m_34player = player;
        SetAxis();
        mainPlayer.SetActive(true);
        if (player.GetComponent<StepsManager>())
        {
            player.GetComponent<StepsManager>().isplayer = true;
        }
        GetComponent<PostProcessManager>().SetPostProcessing();

        //change youtube player instantiation code because while env is in loading and youtube started playing video
        InstantiateYoutubePlayer();

        SetAddressableSceneActive();
        CharacterLightCulling();
        if (!ConstantsHolder.xanaConstants.isCameraMan && LoadingHandler.Instance.isFirstTime)  // Added due to slider not going to 100
        {
            LoadingHandler.Instance.HideLoading();
            // LoadingHandler.Instance.UpdateLoadingSlider(0, true);
            //// LoadingHandler.Instance.UpdateLoadingStatusText("");
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

        if (WorldItemView.m_EnvName.Contains("XANA_DUNE"))
        {
            ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<XanaDuneControllerHandler>().AddComponentOn34();
        }
        if (ConstantsHolder.xanaConstants.JjWorldSceneChange)
        {
            ConstantsHolder.xanaConstants.hasWorldTransitionedInternally = true;
        }
        ConstantsHolder.xanaConstants.JjWorldSceneChange = false;

        updatedSpawnpoint.transform.localPosition = spawnPoint;
        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            ConstantsHolder.xanaConstants.isFromXanaLobby = false;
        }

        ConstantsHolder.xanaConstants.isFromTottoriWorld = false;

        StartCoroutine(VoidCalculation());
        LightCullingScene();

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

        if (ConstantsHolder.isPenguin)
            XanaWorldDownloader.initialPlayerPos = player.transform.localPosition;
        else
            XanaWorldDownloader.initialPlayerPos = mainController.transform.localPosition;
        BuilderEventManager.AfterPlayerInstantiated?.Invoke();


        // Firebase Event for Join World
        GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Join_World.ToString());
        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Summit"))
        {
            ReferencesForGamePlay.instance.m_34player.AddComponent<SummitAnalyticsTrigger>();
            if (StayTimeTrackerForSummit != null)
            {
                string eventName = "XS_TV_" + StayTimeTrackerForSummit.SummitAreaName;
                GlobalConstants.SendFirebaseEventForSummit(eventName);
                StayTimeTrackerForSummit.IsTrackingTimeForExteriorArea = true;
                StayTimeTrackerForSummit.StartTrackingTime();
            }
        }
        UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(true, false, false, false);
        yield return null;
        /// <summary>
        /// Load NPC fake chat system
        /// </summary>
        //ActivateNpcChat();
        //Debug.Log("this is called..........................");
        //yield return new WaitForSeconds(1);
        //var controller = GameplayEntityLoader.instance.mainController.GetComponent<PlayerController>();
        //if(controller.isFirstPerson)
        //{
        //   controller.DisablePlayerOnFPS();
        //}
    }

    void SetPlayerCameraAngle()
    {
        if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            StartCoroutine(setPlayerCamAngle(-0.830f, 0.5572f));
            return;
        }
        if (WorldItemView.m_EnvName.Contains("DJ Event") || WorldItemView.m_EnvName.Contains("XANA Festival Stage"))
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
            else if (WorldItemView.m_EnvName.Contains("Daisen"))
            {
                StartCoroutine(setPlayerCamAngle(0, 0.5f));
            }
            else
            {
                StartCoroutine(setPlayerCamAngle(0, 0.75f));
            }
        }
        else if (WorldItemView.m_EnvName.Contains("Genesis"))
        {
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
        if (WorldItemView.m_EnvName.Contains("XANA_DUNE"))
        {
            mainPlayer.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            StartCoroutine(setPlayerCamAngle(0f, 0.5f));
        }
        if (WorldItemView.m_EnvName == "TOTTORI METAVERSE")
        {
            mainPlayer.transform.rotation = _spawnTransform.rotation;
            StartCoroutine(setPlayerCamAngle(180f, 0.5f));
        }
        if (WorldItemView.m_EnvName.Contains("JJTest"))
        {
            mainPlayer.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            StartCoroutine(setPlayerCamAngle(0f, 0.5f));
        }
    }

    public void SetPlayerPos()
    {
        mainController.transform.position = spawnPoint + new Vector3(0, 0.1f, 0);
    }

    void InstantiatePlayerAvatar(Vector3 pos)
    {
        if (ScreenOrientationManager._instance != null && ScreenOrientationManager._instance.isPotrait)
        {
            ScreenOrientationManager._instance.MyOrientationChangeCode(DeviceOrientation.LandscapeLeft);
        }
        if (ConstantsHolder.isPenguin || ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            DashButton.SetActive(false);
            XanaWorldController.SetActive(false);
            XanaPartyController.SetActive(true);
            Debug.LogError("Xana Player Penguin.......");
            player = PhotonNetwork.Instantiate("XanaPenguin", spawnPoint, Quaternion.identity, 0);
            PenguinPlayer = player;
            mainController = player;
            if (player != null)
            {
                if (SceneManager.GetActiveScene().name == "Builder" && ConstantsHolder.xanaConstants.isXanaPartyWorld)
                {
                    SituationChangerSkyboxScript.instance.builderMapDownload.XANAPartyLoading.SetActive(false);
                }
                StartCoroutine(SetXanaPartyControllers(player));
            }
            return;
        }
        else 
        {
            DashButton.SetActive(true);
        }
        XanaPartyController.SetActive(false);
        XanaWorldController.SetActive(true);
        mainController = mainControllerRefHolder;
        if (ConstantsHolder.isFixedHumanoid)
        {
            InstantiatePlayerForFixedHumanoid();
            return;
        }

        if (SaveCharacterProperties.instance?.SaveItemList.gender == AvatarGender.Male.ToString())
        {
            player = PhotonNetwork.Instantiate("XanaAvatar2.0_Male", spawnPoint, Quaternion.identity, 0);    // Instantiate Male Avatar
            player.transform.parent = mainController.transform;
            player.transform.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.identity;
            player.GetComponent<AvatarController>().SetAvatarClothDefault(player.gameObject, "Male");        // Set Default Cloth to avoid naked avatar
        }
        else
        {
            player = PhotonNetwork.Instantiate("XanaAvatar2.0_Female", spawnPoint, Quaternion.identity, 0);  // Instantiate Female Avatar
            player.transform.parent = mainController.transform;
            player.transform.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.identity;
            player.GetComponent<AvatarController>().SetAvatarClothDefault(player.gameObject, "Female");      // Set Default Cloth to avoid naked avatar
        }
    }

    void InstantiatePlayerAvatarSector(Vector3 pos)
    {
        if (ConstantsHolder.isPenguin || ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            DashButton.SetActive(false);
            XanaWorldController.SetActive(false);
            XanaPartyController.SetActive(true);
            player = PhotonNetwork.Instantiate("XanaPenguin", spawnPoint, Quaternion.identity, 0);
            PenguinPlayer = player;
            mainController = player;
            if (player != null)
            {
                if (SceneManager.GetActiveScene().name == "Builder" && ConstantsHolder.xanaConstants.isXanaPartyWorld)
                {
                    SituationChangerSkyboxScript.instance.builderMapDownload.XANAPartyLoading.SetActive(false);
                }
                StartCoroutine(SetXanaPartyControllers(player));
            }
            return;
        }
        else
        {
            DashButton.SetActive(true);
        }
        XanaPartyController.SetActive(false);
        XanaWorldController.SetActive(true);
        mainController = mainControllerRefHolder;
        if (ConstantsHolder.isFixedHumanoid)
        {
            InstantiatePlayerForFixedHumanoid();
            return;
        }

        if (SaveCharacterProperties.instance?.SaveItemList.gender == AvatarGender.Male.ToString())
        {
            player = PhotonNetwork.Instantiate("XanaAvatar2.0_Male", pos, Quaternion.identity, 0);    // Instantiate Male Avatar
           
    
           
        }
        else
        {
            player = PhotonNetwork.Instantiate("XanaAvatar2.0_Female", pos, Quaternion.identity, 0);  // Instantiate Female Avatar
           
                   }
    }

    void InstantiatePlayerForFixedHumanoid()
    {
        if (ConstantsHolder.AvatarIndex < 10)
        {
            player = PhotonNetwork.Instantiate("XanaAvatar2.0_Male", spawnPoint, Quaternion.identity, 0);    // Instantiate Male Avatar
            player.transform.parent = mainController.transform;
            player.transform.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.identity;
            player.GetComponent<AvatarController>().SetAvatarClothDefault(player.gameObject, "Male");        // Set Default Cloth to avoid naked avatar
        }
        else
        {
            player = PhotonNetwork.Instantiate("XanaAvatar2.0_Female", spawnPoint, Quaternion.identity, 0);  // Instantiate Female Avatar
            player.transform.parent = mainController.transform;
            player.transform.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.identity;
            player.GetComponent<AvatarController>().SetAvatarClothDefault(player.gameObject, "Female");      // Set Default Cloth to avoid naked avatar
        }

    }

    void ActivateNpcChat()
    {
        GameObject npcChatSystem = Resources.Load("NpcChatSystem") as GameObject;
        Instantiate(npcChatSystem);
        //Debug.Log("<color=red> NPC Chat Object Loaded </color>");
    }

    [SerializeField] int autoSwitchTime;
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
        yield return new WaitForSeconds(0.1f);
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
        if (mainController)
        {
            mainController.transform.position = spawnPoint + new Vector3(0, 0.1f, 0);
        }
       
        Vector3 newPos = spawnPoint + new Vector3(500, 500f, 500);

        InstantiatePlayerAvatar(newPos);
        while (player == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (ConstantsHolder.xanaConstants.isBuilderScene && !ConstantsHolder.xanaConstants.isXanaPartyWorld && !ConstantsHolder.xanaConstants.isSoftBankGame )
        {
            player.transform.localScale = Vector3.one * 1.153f;
            Rigidbody playerRB = player.AddComponent<Rigidbody>();
            playerRB.mass = 60;
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
                SituationChangerSkyboxScript.instance.builderMapDownload.UpdateScene();
                BuilderEventManager.ChangeCameraHeight?.Invoke(false);
            }

            SituationChangerSkyboxScript.instance.builderMapDownload.PlayerSetup();
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

        //while (!GamificationComponentData.instance.isSkyLoaded)
        //    yield return new WaitForSeconds(0.5f);
        BuilderEventManager.AfterPlayerInstantiated?.Invoke();


        isEnvLoaded = true;
        if (!ConstantsHolder.xanaConstants.isBackFromWorld)
        {
            LoadingHandler.Instance.HideLoading();

        }

        UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(true, false, false, false);
        ChatSocketManager.onJoinRoom?.Invoke(ConstantsHolder.xanaConstants.builderMapID.ToString());

        Debug.Log("Player Spawn Completed --  Join World");
        GlobalConstants.SendFirebaseEvent(GlobalConstants.FirebaseTrigger.Join_World.ToString());

        //ActivateNpcChat();
    }

    public IEnumerator setPlayerCamAngle(float xValue, float yValue)
    {
        yield return new WaitForSeconds(0.1f);
        if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            CinemachineFreeLook cam = XanaPartyCamera.GetComponentInChildren<CinemachineFreeLook>();
            cam.m_XAxis.Value = xValue;
            cam.m_YAxis.Value = yValue;
        }
        else
        {
            PlayerCamera.m_XAxis.Value = xValue;
            PlayerCamera.m_YAxis.Value = yValue;
        }
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
        //Stop selfi functionality when respawn after fall down
        if (!ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            PlayerSelfieController.Instance.DisableSelfieFeature();
        }
        if (ConstantsHolder.xanaConstants.isBuilderScene)
        {
            //Player respawn at spawn point after jump down from world
            if (!ConstantsHolder.xanaConstants.isXanaPartyWorld)
                mainController.transform.localPosition = AvoidAvatarMergeInBuilderScene();
            else if (XanaPartyCamera.characterManager != null)
                XanaPartyCamera.characterManager.transform.localPosition = AvoidAvatarMergeInBuilderScene();
        }
        else
        {
            if (!ConstantsHolder.isPenguin)
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

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //Debug.Log("Instantiating Photon Complete");

        ResetPlayerPosition();
    }
    /*******************************************************************new code */

    string environmentLabel;
    public async void LoadEnvironment(string label)
    {
        environmentLabel = label;
        if (label == "XANA Summit")
        {
            XanaWorldDownloader.downloadSize = Addressables.GetDownloadSizeAsync(environmentLabel).WaitForCompletion();
            XanaWorldDownloader.downloadSize += (71 * 1024 * 1024);
            if (!DownloadPopupHandler.AlwaysAllowDownload && !XanaWorldDownloader.CheckForVisitedWorlds(ConstantsHolder.xanaConstants.EnviornmentName))
            {
                LoadingHandler.StopLoader = true;
                if (!XanaWorldDownloader.DownloadedWorldNames.Contains(ConstantsHolder.xanaConstants.EnviornmentName))
                    XanaWorldDownloader.DownloadedWorldNames.Add(ConstantsHolder.xanaConstants.EnviornmentName);
                bool permission = await DownloadPopupHandlerInstance.ShowDialogAsync();
                LoadingHandler.StopLoader = false;
                LoadingHandler.CompleteSlider?.Invoke();
                if (!permission)
                {
                    return;
                }
            }
            else
            {
                LoadingHandler.StopLoader = false;

                LoadingHandler.CompleteSlider?.Invoke();
            }
        }
        else
        {
            LoadingHandler.CompleteSlider?.Invoke();
        }
        StartCoroutine(DownloadAssets());
    }

    IEnumerator WaitForMapDownload()
    {
        while (!BuilderAssetDownloader.isSpawnDownloaded)
        {
            yield return new WaitForSeconds(0.1f);
        }

        SetupEnvirnmentForBuidlerScene();
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
        LoadingHandler.CompleteSlider?.Invoke();
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
            /*  while (!ConstantsHolder.isAddressableCatalogDownload) //Zeel Replaced loop with waituntil
              {
                  yield return new WaitForSeconds(1f);
              }*/

            yield return new WaitUntil(() => ConstantsHolder.isAddressableCatalogDownload);

            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(environmentLabel, LoadSceneMode.Additive, false);
            if (!ConstantsHolder.xanaConstants.isFromXanaLobby)
            {
                LoadingHandler.Instance.UpdateLoadingStatusText("Loading World");
            }
            while (!handle.IsDone)
            {
              
                yield return null;
            }
            addressableSceneName = environmentLabel;

            //One way to handle manual scene activation.
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                //AddressableDownloader.Instance.MemoryManager.AddToReferenceList(handle, environmentLabel);
                AddressableDownloader.bundleAsyncOperationHandle.Add(handle);
                yield return handle.Result.ActivateAsync();
                DownloadCompleted();
            }
            else // error occur 
            {
                AssetBundle.UnloadAllAssetBundles(false);
                Resources.UnloadUnusedAssets();

                HomeBtn.onClick.Invoke();
            }
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

        Debug.Log("not coming from else");
        if (GameObject.FindGameObjectWithTag("SpawnPoint"))
        {
            Debug.Log("not coming from else2");

            temp = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        }
        else
            temp = new GameObject("SpawnPoint").transform;
        if (temp)
        {
            _spawnTransform = temp;
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
        if (ConstantsHolder.xanaConstants.isSoftBankGame)
        {
            return;
        }
        if (BuilderAssetDownloader.isPostLoading  )
        {
            //Debug.LogError("here resetting player .... ");
            if (BuilderData.StartFinishPoints.Count > 1 && BuilderData.mapData.data.worldType == 1)
            {
                if (!IsPlayerOnStartPoint()  )
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
        spawnPoint.y += BuilderSpawnPoint ? 2 : 1000;

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
                    if (BuilderData.StartFinishPoints.Count > 1 && BuilderData.mapData.data.worldType == 1)
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
        if (!string.IsNullOrEmpty(temp) && temp.Contains(" Astroboy x Tottori Metaverse Museum"))
        {
            temp = "Astroboy x Tottori Metaverse Museum";
        }
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


    //penguin mehtods 
    IEnumerator SetXanaPartyControllers(GameObject player)
    {
        CharacterManager characterManager = player.GetComponent<CharacterManager>();
        XanaPartyCamera.characterManager = characterManager;
        characterManager.input = XanaPartyInput;
        characterManager.characterCamera = XanaPartyCamera.GetComponentInChildren<Camera>().gameObject;

        XanaPartyCamera.thirdPersonCamera.Follow = player.transform;// characterManager.headPoint;
        XanaPartyCamera.thirdPersonCamera.LookAt = player.transform;// characterManager.headPoint;
        characterManager.enabled = true;
        XanaPartyCamera.SetCamera();
        XanaPartyCamera.SetDebug();
        XanaPartyCamera.thirdPersonCamera.GetComponent<XANAPartyCameraController>().SetReference(player, characterManager.headPoint.gameObject);
        yield return new WaitForSeconds(0.1f);
        if (GamificationComponentData.instance != null)
        {
            GamificationComponentData.instance.PlayerRigidBody = player.GetComponent<Rigidbody>();
            GamificationComponentData.instance.PlayerRigidBody.constraints = RigidbodyConstraints.None;
            GamificationComponentData.instance.PlayerRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        // Landscape
        referenceForPenguin.ActiveXanaUIData(false);
        if (ConstantsHolder.xanaConstants.isXanaPartyWorld )
        {
            ReferencesForGamePlay.instance.SetGameplayForPenpenz(false);
            if (!ConstantsHolder.xanaConstants.isJoinigXanaPartyGame) // For Spwaning in PENPENZ Lobby
            {
                ReferencesForGamePlay.instance.XANAPartyWaitingPanel.SetActive(true);
            }
            else // For Spwaning in PENPENZ GAME
            {
                ReferencesForGamePlay.instance.XANAPartyWaitingPanel.SetActive(false);
            }
            player.GetComponent<PartyTimerManager>().enabled = true;
            player.GetComponent<XANAPartyMulitplayer>().enabled = true;
        }
        else
        {
            ReferencesForGamePlay.instance.XANAPartyWaitingPanel.SetActive(false);
            player.GetComponent<PartyTimerManager>().enabled = false;
            player.GetComponent<XANAPartyMulitplayer>().enabled = false;
        }
        if (ConstantsHolder.xanaConstants.isXanaPartyWorld && ConstantsHolder.xanaConstants.isJoinigXanaPartyGame && GamificationComponentData.instance != null && !GamificationComponentData.instance.isRaceStarted && ReferencesForGamePlay.instance != null)
        {
            ReferencesForGamePlay.instance.IsLevelPropertyUpdatedOnlevelLoad = false;
           
            ReferencesForGamePlay.instance.CheckActivePlayerInCurrentLevel();
        }
    }

    public void ResetOnBackFromSummit()
    {
        if (YoutubeStreamPlayer)
            Destroy(YoutubeStreamPlayer);
        mainController = mainControllerRefHolder;

        ReferencesForGamePlay.instance.XANAPartyCounterPanel.SetActive(false);
        ReferencesForGamePlay.instance.XANAPartyWaitingPanel.SetActive(false);

        ConstantsHolder.isFixedHumanoid = false;
        ConstantsHolder.isPenguin = false;
        ConstantsHolder.xanaConstants.isXanaPartyWorld = false;
        ConstantsHolder.xanaConstants.isSoftBankGame = false;
        ConstantsHolder.xanaConstants.isJoinigXanaPartyGame = false;
    }

    public void AssignRaffleTickets(int domeID)
    {
        _raffleTickets.UpdateData(domeID);
    }
}