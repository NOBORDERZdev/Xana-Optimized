using UnityEngine;
using Photon.Pun;
using System.Collections;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class LoadFromFile : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    [Header("singleton object")]
    public static LoadFromFile instance;

    public GameObject mainPlayer;
    public GameObject mainController;
    private GameObject YoutubeStreamPlayer;

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
    public PopulationGenerator populationGenerator;

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

    public SceneManage _uiReferences;

    //string OrdinaryUTCdateOfSystem = "2023-08-10T14:45:00.000Z";
    //DateTime OrdinarySystemDateTime, localENDDateTime, univStartDateTime, univENDDateTime;

    //Bool for BuilderSpawn point available or not
    bool BuilderSpawnPoint = false;

    private void Awake()
    {
        instance = this;
        //    LoadFile();
        setLightOnce = false;
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
        ChangeOrientation_waqas._instance.EnableDefaultData();

        if (XanaEventDetails.eventDetails.DataIsInitialized)
        {
            StartEventTimer();
        }
        Input.multiTouchEnabled = true;
        for (int i = 0; i < SelfieController.Instance.OnFeatures.Length; i++)
        {
            if (SelfieController.Instance.OnFeatures[i] != null)
            {
                if (SelfieController.Instance.OnFeatures[i].name == "LeftJoyStick")
                {
                    leftJoyStick = SelfieController.Instance.OnFeatures[i];
                    break;
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
            if (XanaConstants.xanaConstants.isBuilderScene)
                SetupEnvirnmentForBuidlerScene();
            else
            {
                LoadEnvironment(XanaConstants.xanaConstants.EnviornmentName);
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

        SelfieController.Instance.DisableSelfieFromStart();



    }

    //void InstantiateYoutubePlayer()
    //{
    //    if (YoutubeStreamPlayer == null)
    //    {
    //        //Debug.Log("DJ Beach====" + WorldItemView.m_EnvName);
    //        if (WorldItemView.m_EnvName.Contains("DJ Event"))
    //        {
    //            YoutubeStreamPlayer = Instantiate(Resources.Load("DJEventData/YoutubeVideoPlayer") as GameObject);

    //            //#if UNITY_ANDROID || UNITY_EDITOR
    //            //                //YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
    //            //                //YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);

    //            //#else
    //            //YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
    //            //            YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);
    //            //#endif

    //            YoutubeStreamPlayer.transform.localPosition = new Vector3(0f, 0f, 10f);
    //            YoutubeStreamPlayer.transform.localScale = new Vector3(1f, 1f, 1f);

    //            YoutubeStreamPlayer.SetActive(false);
    //            if (YoutubeStreamPlayer)
    //            {
    //                YoutubeStreamPlayer.SetActive(true);
    //            }
    //        }
    //        if (WorldItemView.m_EnvName.Contains("XANA Festival Stage") && !WorldItemView.m_EnvName.Contains("Dubai"))
    //        {
    //            YoutubeStreamPlayer = Instantiate(Resources.Load("XANAFestivalStageData/YoutubeVideoPlayer1") as GameObject);

    //            //#if UNITY_ANDROID || UNITY_EDITOR
    //            //                YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
    //            //                YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);
    //            //#else
    //            //  YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
    //            //            YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);
    //            //#endif


    //            YoutubeStreamPlayer.transform.localPosition = new Vector3(0f, 0f, 10f);
    //            YoutubeStreamPlayer.transform.localScale = new Vector3(1f, 1f, 1f);

    //            YoutubeStreamPlayer.SetActive(false);
    //            if (YoutubeStreamPlayer)
    //            {
    //                YoutubeStreamPlayer.SetActive(true);
    //            }
    //        }

    //        if (WorldItemView.m_EnvName.Contains("Xana Festival") || WorldItemView.m_EnvName.Contains("NFTDuel Tournament"))
    //        {
    //            YoutubeStreamPlayer = Instantiate(Resources.Load("MyBeach/XanaFestivalPlayer") as GameObject);

    //            //#if UNITY_ANDROID || UNITY_EDITOR
    //            //                //YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
    //            //                //YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);

    //            //#else
    //            //YoutubeStreamPlayer.transform.localPosition = new Vector3(-0.44f, 0.82f, 14.7f);
    //            //            YoutubeStreamPlayer.transform.localScale = new Vector3(0.46f, 0.43f, 0.375f);
    //            //#endif

    //            YoutubeStreamPlayer.transform.localPosition = new Vector3(0f, 0f, 10f);
    //            YoutubeStreamPlayer.transform.localScale = new Vector3(1f, 1f, 1f);

    //            YoutubeStreamPlayer.SetActive(false);
    //            if (YoutubeStreamPlayer)
    //            {
    //                YoutubeStreamPlayer.SetActive(true);
    //            }
    //        }
    //        if (WorldItemView.m_EnvName.Contains("XANA Lobby"))
    //        {
    //            YoutubeStreamPlayer = Instantiate(Resources.Load("XanaLobby/XanaLobbyPlayer") as GameObject);
    //            YoutubeStreamPlayer.SetActive(false);
    //            if (YoutubeStreamPlayer)
    //            {
    //                YoutubeStreamPlayer.SetActive(true);
    //            }
    //        }
    //    }
    //}
    void CharacterLightCulling()
    {
        if ((!WorldItemView.m_EnvName.Contains("Xana Festival") || !WorldItemView.m_EnvName.Contains("NFTDuel Tournament")) && !XanaConstants.xanaConstants.isBuilderScene)
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
        return false;
    }

    public IEnumerator SpawnPlayer()
    {
        //if (XanaConstants.xanaConstants.isFromXanaLobby)
        //    LoadingHandler.Instance.UpdateLoadingSliderForJJ(.8f,0.1f);
        if (!XanaConstants.xanaConstants.isFromXanaLobby || !XanaConstants.xanaConstants.isFromPMYLobby)
        {
            // LoadingHandler.Instance.UpdateLoadingSlider(.8f);
            LoadingHandler.Instance.UpdateLoadingStatusText("Joining World...");
        }
        yield return new WaitForSeconds(.2f);
        if (!(SceneManager.GetActiveScene().name.Contains("Museum")))
        {
            //if (WorldItemView.m_EnvName.Contains("AfterParty"))
            //{
            //    if (XanaConstants.xanaConstants.setIdolVillaPosition)
            //    {
            //        spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + 2, spawnPoint.z);
            //        XanaConstants.xanaConstants.setIdolVillaPosition = false;
            //    }
            //    else
            //    {
            //        for (int i = 0; i < IdolVillaRooms.instance.villaRooms.Length; i++)
            //        {
            //            if (IdolVillaRooms.instance.villaRooms[i].name == ChracterPosition.currSpwanPos)
            //            {
            //                spawnPoint = IdolVillaRooms.instance.villaRooms[i].gameObject.GetComponent<ChracterPosition>().spawnPos;
            //                break;
            //            }
            //            else
            //            {
            //                spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + 2, spawnPoint.z);
            //            }
            //        }
            //    }
            //}
            //else
            //{
                  spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y + 2, spawnPoint.z);
            //}

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
            //if (WorldItemView.m_EnvName.Contains("XANALIA NFTART AWARD 2021"))
            //{
            //    mainPlayer.transform.rotation = Quaternion.Euler(0f, 230f, 0f);
            //}
            //else if (WorldItemView.m_EnvName.Contains("DJ Event") || WorldItemView.m_EnvName.Contains("XANA Festival Stage"))
            //{
            //    mainPlayer.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            //}
            //else if (WorldItemView.m_EnvName.Contains("Koto") || WorldItemView.m_EnvName.Contains("Tottori") || WorldItemView.m_EnvName.Contains("DEEMO") || WorldItemView.m_EnvName.Contains("XANA Lobby"))
            //{
            //    mainPlayer.transform.rotation = Quaternion.Euler(0f, 180f, 0);
            //    //Invoke(nameof(SetKotoAngle), 0.5f);
            //    if (WorldItemView.m_EnvName.Contains("XANA Lobby"))
            //    {
            //        StartCoroutine(setPlayerCamAngle(-0.830f, 0.5572f));
            //    }
            //    else
            //    {
            //        StartCoroutine(setPlayerCamAngle(0, 0.75f));

            //    }
            //}
            //else if (WorldItemView.m_EnvName.Contains("Genesis"))
            //{
            //    // No Need TO Rotate Player
            //    StartCoroutine(setPlayerCamAngle(0, 0.75f));
            //}
            //else if (WorldItemView.m_EnvName.Contains("ZONE X Musuem") || WorldItemView.m_EnvName.Contains("FIVE ELEMENTS"))
            //{
            //    StartCoroutine(setPlayerCamAngle(-30.0f, 0.5f));
            //}
            //else if (WorldItemView.m_EnvName.Contains("ZONE-X"))
            //{
            //    StartCoroutine(setPlayerCamAngle(0f, 00.5f));
            //}
            if (WorldItemView.m_EnvName.Contains("PMY ACADEMY"))
            {
                if (XanaConstants.xanaConstants.isFromPMYLobby)
                {
                    XanaConstants.xanaConstants.isFromPMYLobby = false;
                    // Set spawn pos when ReEnter into PMY lobby
                    spawnPoint = new Vector3(spawnPoint.x, spawnPoint.y, spawnPoint.z - 25.05f);
                    //Debug.LogError("Update spawnpoint for lobby::");
                }
                mainPlayer.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                //StartCoroutine(setPlayerCamAngle(0f, 0.5572f));
                //StartCoroutine(setPlayerCamAngle(1f, 0.32f));
                StartCoroutine(setPlayerCamAngle(0.38f, 0.8f)); // 0.38  //0.2275
            }
            else if (WorldItemView.m_EnvName.Contains("PMYRoomA"))
            {
                mainPlayer.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                StartCoroutine(setPlayerCamAngle(-1.6f, 0.5f));
            }
            else if (WorldItemView.m_EnvName.Contains("PMYGallery"))
            {
                //mainPlayer.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                StartCoroutine(setPlayerCamAngle(1.74f, 0.5f));
            }
        }

        mainPlayer.transform.position = new Vector3(0, 0, 0);
        mainController.transform.position = spawnPoint + new Vector3(0, 0.1f, 0);
        //Vector3 newPos = spawnPoint + new Vector3(500, 500f, 500);
        //mainController.transform.position = newPos;

        player = PhotonNetwork.Instantiate("34", spawnPoint, Quaternion.identity, 0);

        ReferrencesForDynamicMuseum.instance.m_34player = player;
        SetAxis();
        mainPlayer.SetActive(true);
        Metaverse.AvatarManager.Instance.InitCharacter();
        if (player.GetComponent<StepsManager>())
        {
            player.GetComponent<StepsManager>().isplayer = true;
        }
        GetComponent<ChecklPostProcessing>().SetPostProcessing();

        // LoadingHandler.Instance.UpdateLoadingSlider(0.98f, true);

        //change youtube player instantiation code because while env is in loading and youtube started playing video
        //InstantiateYoutubePlayer();

        SetAddressableSceneActive();
        CharacterLightCulling();
        if (!XanaConstants.xanaConstants.isCameraMan)
        {
            LoadingHandler.Instance.HideLoading();
            // LoadingHandler.Instance.UpdateLoadingSlider(0, true);
            LoadingHandler.Instance.UpdateLoadingStatusText("");
        }
        if ((WorldItemView.m_EnvName != "JJ MUSEUM") && player.GetComponent<PhotonView>().IsMine)
        {
            if (!XanaConstants.xanaConstants.isCameraMan)
                LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
        }
        else //if(XanaConstants.xanaConstants.isFromPMYLobby)
        {
            if (JjMusuem.Instance)
                JjMusuem.Instance.SetPlayerPos(XanaConstants.xanaConstants.mussuemEntry);
            else
            {
                if (!XanaConstants.xanaConstants.isCameraMan)
                    LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
            }
        }
        XanaConstants.xanaConstants.JjWorldSceneChange = false;

        updatedSpawnpoint.transform.localPosition = spawnPoint;
        //if (XanaConstants.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        //{
        //    XanaConstants.xanaConstants.isFromXanaLobby = false;
        //}
        //else if (XanaConstants.xanaConstants.EnviornmentName.Contains("PMY ACADEMY"))
        //{
        //    XanaConstants.xanaConstants.isFromPMYLobby = false;
        //}
        StartCoroutine(VoidCalculation());
        LightCullingScene();
        yield return new WaitForSeconds(.5f);

        if (XanaConstants.xanaConstants.isCameraMan)
        {
            ReferrencesForDynamicMuseum.instance.randerCamera.gameObject.SetActive(false);
            ReferrencesForDynamicMuseum.instance.FirstPersonCam.gameObject.SetActive(false);
            XanaConstants.xanaConstants.StopMic();
            XanaVoiceChat.instance.TurnOffMic();
            //ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<CharcterBodyParts>().HidePlayer();/*.gameObject.SetActive(false);*/
        }
        LoadingHandler.Instance.manualRoomController.HideRoomList();

        if (!XanaConstants.xanaConstants.isCameraMan)
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
                XanaConstants.xanaConstants.MuseumID = "" + XanaEventDetails.eventDetails.environmentId;
            }
            else
            {
                XanaConstants.xanaConstants.MuseumID = "" + XanaEventDetails.eventDetails.museumId;
            }
        }

        XanaChatSocket.onJoinRoom?.Invoke(XanaConstants.xanaConstants.MuseumID);
        if (XanaConstants.xanaConstants.isCameraMan)
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

        /// <summary>
        /// Load NPC fake chat system
        /// </summary>
        //ActivateNpcChat();
    }

    //public void SetPlayerPos()
    //{
    //    if (mainController != null)
    //    {
    //        mainController.transform.position = spawnPoint + new Vector3(0, 0.1f, 0);
    //        //Debug.LogError("SetPlayerPos" + spawnPoint);
    //    }
    //}

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
        XanaConstants.xanaConstants.JjWorldSceneChange = true;
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

        mainPlayer.transform.position = new Vector3(0, 0, 0);
        mainController.transform.position = spawnPoint + new Vector3(0, 0.1f, 0);
        player = PhotonNetwork.Instantiate("34", spawnPoint, Quaternion.identity, 0);
        if (XanaConstants.xanaConstants.isBuilderScene)
        {
            player.transform.localScale = Vector3.one * 1.153f;
            Rigidbody playerRB = player.AddComponent<Rigidbody>();
            playerRB.isKinematic = true;
            playerRB.useGravity = true;
            playerRB.constraints = RigidbodyConstraints.FreezeRotation;
            player.AddComponent<KeyValues>();
            GamificationComponentData.instance.spawnPointPosition = mainController.transform.position;
            GamificationComponentData.instance.buildingDetect = player.AddComponent<BuildingDetect>();
            //player.GetComponent<CapsuleCollider>().isTrigger = false;
            //player.GetComponent<CapsuleCollider>().enabled = false;
            TimeStats.playerCanvas = Instantiate(GamificationComponentData.instance.playerCanvas);
            GamificationComponentData.instance.playerControllerNew = mainPlayer.GetComponentInChildren<PlayerControllerNew>();

            if (GamificationComponentData.instance.raycast == null)
                GamificationComponentData.instance.raycast = new GameObject("Raycasst");
            GamificationComponentData.instance.raycast.transform.SetParent(GamificationComponentData.instance.playerControllerNew.transform);
            GamificationComponentData.instance.raycast.transform.localPosition = Vector3.up * 1.683f;
            GamificationComponentData.instance.raycast.transform.localScale = Vector3.one * 0.37f;
            if (GamificationComponentData.instance.worldCameraEnable)
                BuilderEventManager.EnableWorldCanvasCamera?.Invoke();
            GamificationComponentData.instance.avatarController = player.GetComponent<AvatarController>();
            GamificationComponentData.instance.charcterBodyParts = player.GetComponent<CharcterBodyParts>();
            GamificationComponentData.instance.ikMuseum = player.GetComponent<IKMuseum>();

            //Post Process enable for Builder Scene
            firstPersonCamera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            environmentCameraRender.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            Camera freeCam = this.GetComponent<ChecklPostProcessing>().freeCam;
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
                ReferrencesForDynamicMuseum.instance.playerControllerNew.StopBuilderComponent();
                SituationChangerSkyboxScript.instance.builderMapDownload.PlayerSetup();
                SituationChangerSkyboxScript.instance.builderMapDownload.UpdateScene();
                BuilderEventManager.ChangeCameraHeight?.Invoke(false);
            }
        }
        if ((WorldItemView.m_EnvName != "JJ MUSEUM") && player.GetComponent<PhotonView>().IsMine)
        {
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
        }
        ReferrencesForDynamicMuseum.instance.m_34player = player;
        SetAxis();
        mainPlayer.SetActive(true);
        Metaverse.AvatarManager.Instance.InitCharacter();
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


        if (!XanaConstants.xanaConstants.isCameraMan)
        {
            LoadingHandler.Instance.HideLoading();
            // LoadingHandler.Instance.UpdateLoadingSlider(0, true);
            LoadingHandler.Instance.UpdateLoadingStatusText("");
        }
        if ((WorldItemView.m_EnvName != "JJ MUSEUM") && player.GetComponent<PhotonView>().IsMine)
        {
            if (!XanaConstants.xanaConstants.isCameraMan)
                LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
        }
        else
        {
            JjMusuem.Instance.SetPlayerPos(XanaConstants.xanaConstants.mussuemEntry);
        }
        XanaConstants.xanaConstants.JjWorldSceneChange = false;

        while (!GamificationComponentData.instance.isSkyLoaded)
            yield return new WaitForSeconds(0.5f);
        BuilderEventManager.AfterPlayerInstantiated?.Invoke();


        isEnvLoaded = true;
        yield return new WaitForSeconds(1.75f);
        LoadingHandler.Instance.HideLoading();
        // LoadingHandler.Instance.UpdateLoadingSlider(0, true);
        //LoadingHandler.Instance.UpdateLoadingStatusText("");


        // Yes Join APi Call Here
        ////Debug.Log("Waqas : Room Joined.");
        //Debug.Log("<color=green> Analytics -- Joined </color>");
        UserAnalyticsHandler.onUpdateWorldRelatedStats?.Invoke(true, false, false, false);
        XanaChatSocket.onJoinRoom?.Invoke(XanaConstants.xanaConstants.builderMapID.ToString());

        ActivateNpcChat();
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
            if (XanaConstants.xanaConstants.EnviornmentName == "XANALIA NFTART AWARD 2021")
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

            if (XanaConstants.xanaConstants.EnviornmentName == "DJ Event" || XanaConstants.xanaConstants.EnviornmentName == "Xana Festival" || XanaConstants.xanaConstants.EnviornmentName == "NFTDuel Tournament")
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

            if (XanaConstants.xanaConstants.EnviornmentName == "XANALIA NFTART AWARD 2021")
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
            if (XanaConstants.xanaConstants.EnviornmentName == "DJ Event" || XanaConstants.xanaConstants.EnviornmentName == "Xana Festival" || XanaConstants.xanaConstants.EnviornmentName == "NFTDuel Tournament")
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

    void ResetPlayerPosition()
    {
        if (XanaConstants.xanaConstants.isBuilderScene)
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
            mainController.transform.localPosition = AvoidAvatarMergeInBuilderScene();

        }
        else
        {
            mainController.GetComponent<PlayerControllerNew>().gravityVector.y = 0;
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
        if (XanaConstants.xanaConstants.orientationchanged && XanaConstants.xanaConstants.JjWorldSceneChange)
        {
            ChangeOrientation_waqas._instance.MyOrientationChangeCode(DeviceOrientation.Portrait);
        }
        Transform tempSpawnPoint = null;
        LoadingHandler.Instance.UpdateLoadingStatusText("Getting World Ready....");
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
            while (!XanaConstants.isAddressableCatalogDownload)
            {
                yield return new WaitForSeconds(1f);
            }
            //yield return StartCoroutine(DownloadEnvoirnmentDependanceies(environmentLabel));
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(environmentLabel, LoadSceneMode.Additive, false);
            //if (XanaConstants.xanaConstants.isFromXanaLobby)
            //{
            //    LoadingHandler.Instance.UpdateLoadingSliderForJJ(UnityEngine.Random.Range(0.5f,0.7f), 0.1f);
            //}
            if (!XanaConstants.xanaConstants.isFromXanaLobby || !XanaConstants.xanaConstants.isFromPMYLobby)
            {
                LoadingHandler.Instance.UpdateLoadingStatusText("Loading World...");
                //LoadingHandler.Instance.UpdateLoadingSlider(.6f, true);
            }
            yield return handle;
            addressableSceneName = environmentLabel;
            //...

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
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("AddressableScene"));
        StartCoroutine(spwanPlayerWithWait());
    }


    void ResetPlayerAfterInstantiation()
    {
        if (BuilderAssetDownloader.isPostLoading)
        {
            //Debug.LogError("here resetting player .... ");
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

            mainController.transform.localPosition = AvoidAvatarMergeInBuilderScene();
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
                    spawnPoint = new Vector3(spawnPoint.x + UnityEngine.Random.Range(-1f, 1f), spawnPoint.y, spawnPoint.z + UnityEngine.Random.Range(-1f, 1f));
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

    public void SetAddressableSceneActive()
    {
        string temp = addressableSceneName;
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("AddressableScene"));
        if (temp.Contains(" Astroboy x Tottori Metaverse Museum"))
        {
            temp = "Astroboy x Tottori Metaverse Museum";
        }
        ////Debug.LogError("~~~~~~scene name to be activated :-  " + temp);
        if (!string.IsNullOrEmpty(temp))
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(temp));
        else if (XanaConstants.xanaConstants.isBuilderScene)
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Builder"));
        else
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(XanaConstants.xanaConstants.EnviornmentName));

    }

    void LightCullingScene()
    {
        // Forcfully resetting lights because on 

        if (GetComponent<ChecklPostProcessing>().CheckPostProcessEnable())
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
                    if (XanaConstants.xanaConstants.EnviornmentName.Contains("FIVE ELEMENTS"))
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