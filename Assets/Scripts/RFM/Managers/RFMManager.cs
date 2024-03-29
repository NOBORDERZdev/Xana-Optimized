using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using ExitGames.Client.Photon;
using MoreMountains.Feedbacks;
using Photon.Pun;
using Photon.Realtime;
using RFM.Character;
using RFM.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace RFM.Managers
{
    public class RFMManager : MonoBehaviourPunCallbacks
    {
        #region Serialized Fields
        [SerializeField] public Transform playersSpawnArea;
        [SerializeField] private GameObject huntersCage;
        [SerializeField] private GameObject huntersCageDoor;

        [SerializeField] private FollowNPC npcCameraPrefab;
        [SerializeField] public Transform huntersSpawnArea;

        [SerializeField] private TextMeshProUGUI countDownText;
        [SerializeField] private TextMeshProUGUI gameplayTimeText;
        [SerializeField] private TextMeshProUGUI statusTMP;
        [SerializeField] private GameObject statusBG;

        //MM effects
        [SerializeField] private MMScaleShaker timerTextScaleShaker;
        [SerializeField] private MMScaleShaker countdownTimerTextScaleShaker;
        [SerializeField] private MMF_Player statusMMFPlayer;

        //Camera Manager
        [SerializeField] private RFMCameraManager rfmCameraManager;
        [SerializeField] public GameObject _mainCam;

        //VFX
        [SerializeField] private GameObject playerSpawnVFX, hunterSpawnVFX;

        //list of references of NPCS
        public List<RFM.Character.NPCRunner> runnerNPCList = new List<RFM.Character.NPCRunner>();
        public List<RFM.Character.NPCHunter> hunterNPCList = new List<RFM.Character.NPCHunter>();
        public Dictionary<RFM.Character.NPCHunter, Transform> hunterTargetsDictionary = new Dictionary<NPCHunter, Transform>();

        //RFM Water light 
        public Light rfmWaterLight;
        public LayerMask rfmWaterLightMask;
        #endregion

        #region Fields

        public static RFMManager Instance;

        [HideInInspector] public bool isPlayerHunter;
        private GameObject _gameCanvas;
        private FollowNPC _npcCamera;
        [HideInInspector] public static GameConfiguration CurrentGameConfiguration;
        private float spawnOffset = 0;


        #endregion

        #region Unity Callback Methods

        private void Awake()
        {
            //RFM.Globals.IsRFMWorld = true; // TODO: Do this in main menu
            Instance = this;
            ChangeOrientation_waqas._instance.MyOrientationChangeCode(DeviceOrientation.LandscapeLeft);
            InvokeRepeating(nameof(OrientationChange), 1, 1);
            EventsManager.OnHideCanvasElements();
            //StartCoroutine(CheckandFixLights());
        }
        int checkNum;
        public void OrientationChange()
        {
            checkNum++;
            if (checkNum < 5)
            {
                if (Screen.orientation == ScreenOrientation.Portrait)
                {
                    ChangeOrientation_waqas._instance.MyOrientationChangeCode(DeviceOrientation.LandscapeLeft);
                }
            }
            else
            {
                CancelInvoke(nameof(OrientationChange));
            }
        }

        /*private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (PhotonNetwork.PlayerListOthers.Length > 0)
                    {
                        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer.GetNext());

                        PhotonNetwork.SendAllOutgoingCommands();
                    }
                }
            }
        }*/


        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.NetworkingClient.EventReceived += ReceivePhotonEvents;
            //StartCoroutine(CheckandFixLights());

        }


        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.EventReceived -= ReceivePhotonEvents;
        }

        public Timer timer;

        private IEnumerator Start()
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            // RFMUIManager.Instance.ShowXanaStonePopup();
            //StartCoroutine(CheckandFixLights());
            Application.runInBackground = true;
            yield return StartCoroutine(FetchConfigDataFromServer());

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.MaxPlayers = (byte)CurrentGameConfiguration.MaxPlayersInRoom;
                PhotonNetwork.CurrentRoom.PlayerTtl = 5000;
            }

            Globals.gameState = Globals.GameState.InLobby;
            //_mainCam = GameObject.FindGameObjectWithTag(Globals.MAIN_CAMERA_TAG);
            _gameCanvas = GameObject.FindGameObjectWithTag(Globals.CANVAS_TAG);
            //RFMButtonsLayoutManager.instance.LoadLayout();
            gameplayTimeText.transform.parent.gameObject.SetActive(true);

            StartMatchMaking();

            _gameCanvas.SetActive(true);
            CanvasButtonsHandler.inst.ShowRFMButtons(true);
            CanvasButtonsHandler.inst.RFMResetSprintButton(); // TODO: Call this function in the above function

            //this is to turn post processing on
            var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = true;


            StartCoroutine(CheckandFixLights());

        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (PhotonNetwork.PlayerListOthers.Length > 0)
                    {
                        Debug.LogError("Sent Time");
                        LoadFromFile.instance.myRfmCharacter.GetComponent<PhotonView>().RPC("TimerRPC", RpcTarget.AllBuffered, timer._elapsedSeconds);
                        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer.GetNext());

                        PhotonNetwork.SendAllOutgoingCommands();
                    }
                }
            }
        }


        #endregion

        #region Private Methods

        public void StartMatchMaking() // Need to be called from UI if XanaStone is required to play
        {
            gameplayTimeText.transform.parent.gameObject.SetActive(true);

            Timer.SetDurationAndRun(CurrentGameConfiguration.MatchMakingTime - 10, () =>
            { // Close the room before the last 10 seconds
                if (RFM.Globals.gameState == Globals.GameState.InLobby)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.CurrentRoom.IsOpen = false;
                    }
                }
            });

            Timer.SetDurationAndRun(CurrentGameConfiguration.MatchMakingTime, () =>
             {
                 if (Globals.gameState == Globals.GameState.InLobby)
                 {
                     if (PhotonNetwork.IsMasterClient)
                     {
                         PhotonNetwork.RaiseEvent(PhotonEventCodes.StartRFMEventCode, null,
                             new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                     }
                     //CancelInvoke(nameof(CheckForGameStartCondition));
                 }

             }, gameplayTimeText, true, true);

            //photonView.RPC(nameof(PlayerJoined), RpcTarget.AllBuffered);
            //Debug.Log("RFM PlayerJoined() RPC Requested by " + PhotonNetwork.NickName);

            //InvokeRepeating(nameof(CheckForGameStartCondition), 1, 1);

            statusTMP.text = "Waiting for other players to join:";
            statusBG.SetActive(true);
            statusMMFPlayer.PlayFeedbacks();
        }

        public void RFMStartInterrupted()
        {
            Globals.gameState = Globals.GameState.InLobby;
            countDownText.transform.parent.gameObject.SetActive(false);

            statusTMP.text = "Waiting for other players to join:";
            statusBG.SetActive(true);

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
            }

            StopAllCoroutines();
        }
        public void RestartRFM()
        {
            if (_npcCamera != null)
            {
                Destroy(_npcCamera.gameObject);
            }


            PhotonNetwork.CurrentRoom.CustomProperties.Clear();
            // clear all custom properties of all players
            foreach (var player in PhotonNetwork.PlayerList)
            {
                player.CustomProperties.Clear();
            }
            //huntersCage.GetComponent<Animator>().Play("RFMCloseDoor");
            //huntersCage.GetComponent<Animator>().Play("Cage Door Close");
            huntersCageDoor.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));



            var spawnPosition = playersSpawnArea.position;
            spawnPosition = new Vector3(
                spawnPosition.x + Random.Range(-1.0f, 1.0f),
                spawnPosition.y,
                spawnPosition.z + Random.Range(-1.0f, 1.0f));

            //RFM.Globals.player.transform.root.gameObject.SetActive(true);
            var newPlayer = PhotonNetwork.Instantiate("XANA Player", spawnPosition, Quaternion.identity, 0);

            _mainCam.SetActive(true);
            _gameCanvas.SetActive(true);

            RFM.Globals.player = newPlayer.transform.GetChild(0).gameObject; // Player is the 1st obj. TODO Muneeb

            CanvasButtonsHandler.inst.ShowRFMButtons(true);
            CanvasButtonsHandler.inst.RFMResetSprintButton(); // TODO: Call this function in the above function

            StartCoroutine(Start());
        }

        IEnumerator CheckandFixLights()
        {
            print("RFM KUSH CHECKING LIGHTS STARTED");
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);
                if (rfmWaterLight.cullingMask != rfmWaterLightMask)
                    rfmWaterLight.cullingMask = rfmWaterLightMask;
            }
        }
        public Coroutine SpawnNPCsCoroutine;
        private IEnumerator StartRFM()
        {
            EventsManager.StartCountdown();
            Globals.gameState = Globals.GameState.Countdown;
            //CancelInvoke(nameof(CheckForGameStartCondition));
            //StartCoroutine(CheckandFixLights());
            countDownText.transform.parent.gameObject.SetActive(true);
            statusTMP.text = "Resetting position in:";
            statusBG.SetActive(true);
            statusMMFPlayer.PlayFeedbacks();

            //huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Up"); // ?? There is no such animation
            //huntersCage.GetComponent<Animator>().Play("Cage Door Close");
            huntersCageDoor.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            Debug.LogError("StartRFM Cage Door Close");


            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                int roomLimit = PhotonNetwork.CurrentRoom.MaxPlayers;
                var numberOfPlayers = PhotonNetwork.PlayerList.Length;

                var roles = CalculateRoles(roomLimit, numberOfPlayers,
                    CurrentGameConfiguration.RunnersToHuntersRatio);

                Debug.Log($"RFM roles: {roles}");

                runnerNPCList.Clear();
                hunterNPCList.Clear();
                SpawnNPCsCoroutine = StartCoroutine(SpawnNPCs(roles.Item4, roles.Item3));
                Debug.LogError("SpawnNPCs");
                var numberOfPlayerHunters = roles.Item2;
                foreach (var roomPlayer in PhotonNetwork.CurrentRoom.Players)
                {
                    if (numberOfPlayerHunters > 0)
                    {
                        roomPlayer.Value.SetCustomProperties(new Hashtable
                            { { "isHunter", true } });
                        numberOfPlayerHunters--;
                    }
                    else
                    {
                        roomPlayer.Value.SetCustomProperties(new Hashtable
                            { { "isHunter", false } });
                    }
                }

            }

            gameplayTimeText.gameObject.SetActive(false);

            yield return StartCoroutine(Timer.SetDurationAndRunEnumerator(10, null,
                countDownText, false, AfterEachSecondCountdownTimer));

            SetRunnerOrHunterStatusOfPlayer();

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.RaiseEvent(PhotonEventCodes.ResetPositionEventCode, null,
                    new RaiseEventOptions { Receivers = ReceiverGroup.All },
                    SendOptions.SendReliable);
            }

        }

        private void SetRunnerOrHunterStatusOfPlayer()
        {
            bool isHunter = false;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isHunter", out object _isHunter))
            {
                isPlayerHunter = isHunter = (bool)_isHunter;
            }


            if (isHunter) // Player spawning as Hunter
            {
                Debug.Log($"RFM {PhotonNetwork.NickName} Spawning as Hunter.");

                Globals.IsLocalPlayerHunter = true;
            }

            else // Player spawning as Runner
            {
                Debug.Log($"RFM {PhotonNetwork.NickName} Spawning as Runner.");

                Globals.IsLocalPlayerHunter = false;
            }
        }

        private void ResetPosition()
        {
            EventsManager.TakePositionTime();
            Globals.gameState = Globals.GameState.TakePosition;


            if (RFM.Globals.IsLocalPlayerHunter) // Player spawning as Hunter
            {
                statusTMP.text = "CATCH THE <#FF36D3>RUNNERS!</color>";
                statusBG.SetActive(true);
                statusMMFPlayer.PlayFeedbacks();

                Timer.SetDurationAndRun(CurrentGameConfiguration.TakePositionTime,
                    StartGameplay,
                    countDownText, false, false,
                    AfterEachSecondCountdownTimer);

                var hunterPosition = huntersSpawnArea.position;
                var randomHunterPos = new Vector3(
                    hunterPosition.x + Random.Range(-1.0f, 1.0f),
                    hunterPosition.y,
                    hunterPosition.z + Random.Range(-1.0f, 1.0f));

                //play VFX
                //hunterSpawnVFX.SetActive(true);
                //Destroy(hunterSpawnVFX, 10f);

                Globals.player.transform.SetPositionAndRotation(randomHunterPos, Quaternion.identity);

            }

            else // Player spawning as Runner
            {
                statusTMP.text = "<#FF36D3>HUNTERS</color> RELEASING IN:";
                statusBG.SetActive(true);
                statusMMFPlayer.PlayFeedbacks();

                Timer.SetDurationAndRun(CurrentGameConfiguration.TakePositionTime,
                    StartGameplay,
                    countDownText, false, false,
                    AfterEachSecondCountdownTimer);

                var position = playersSpawnArea.position;
                var randomPos = new Vector3(
                    position.x + Random.Range(-2.0f, 2.0f),
                    position.y,
                    position.z + spawnOffset);
                spawnOffset += 1;

                //play VFX
                //playerSpawnVFX.SetActive(true);
                //Destroy(playerSpawnVFX, 10f); // Causes a null reference on game restart.
                // Should be instantiated or disabled.

                Globals.player.transform.SetPositionAndRotation(randomPos, Quaternion.identity);

            }
        }


        private IEnumerator SpawnNPCs(int numOfHunters, int numOfRunners)
        {
            var delay = (numOfHunters + numOfRunners) / 11; // 10 seconds countdown.
                                                            // Need to spawn all NPCs before last second


            Debug.Log("SpawnNPCs RFM numOfAIRunners: " + numOfRunners);
            GameObject hunterNPC, runnerNPC;
            for (int i = 0; i < numOfRunners; i++)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    runnerNPC = PhotonNetwork.InstantiateRoomObject("RFM/RunnerNPC",
                        playersSpawnArea.position + new Vector3(
                            Random.Range(-1.0f, 1.0f), 0,
                          i),
                        playersSpawnArea.rotation);
                    runnerNPCList.Add(runnerNPC.GetComponent<NPCRunner>());
                    runnerNPC.GetComponent<NavMeshAgent>().speed = Random.Range(3, 3.6f); // TODO: Set speed in NPCRunner.cs
                }
                else
                {
                    GetComponent<PhotonView>().RPC(nameof(SpawnNPCsRPC), RpcTarget.MasterClient, numOfHunters, (numOfRunners - i));
                    StopCoroutine(SpawnNPCsCoroutine);
                }
                yield return new WaitForSeconds(delay);
            }


            Debug.Log("RFM numOfAIHunters: " + numOfHunters);
            for (int i = 0; i < numOfHunters; i++)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    hunterNPC = PhotonNetwork.InstantiateRoomObject("RFM/HunterNPC",
                        huntersSpawnArea.position + new Vector3(Random.Range(-1.0f, 1.0f), 0,
                            Random.Range(-1.0f, 1.0f)),
                        huntersSpawnArea.rotation);
                    hunterNPCList.Add(hunterNPC.GetComponent<NPCHunter>());
                    hunterNPC.GetComponent<NavMeshAgent>().speed = Random.Range(3, 3.6f); // TODO: Set speed in NPCHunter.cs
                }
                else
                {
                    GetComponent<PhotonView>().RPC(nameof(SpawnNPCsRPC), RpcTarget.MasterClient, (numOfHunters - (i)), 0);
                    StopCoroutine(SpawnNPCsCoroutine);
                }
                yield return new WaitForSeconds(delay);
            }
        }

        [PunRPC]
        public void SpawnNPCsRPC(int numOfHunters, int numOfRunners)
        {
            SpawnNPCsCoroutine = StartCoroutine(SpawnNPCs(numOfHunters, numOfRunners));
        }

        //[PunRPC]
        private void StartGameplay()
        {
            EventsManager.StartGame();
            Debug.LogError("RFM StartGameplay");
            rfmCameraManager.SwitchOffAllCameras();
            Globals.gameState = Globals.GameState.Gameplay;
            gameplayTimeText.transform.parent.gameObject.SetActive(true);
            gameplayTimeText.gameObject.SetActive(true);
            countDownText.transform.parent.gameObject.SetActive(false);
            statusBG.SetActive(false);
            statusMMFPlayer.PlayFeedbacks();

            Timer.SetDurationAndRun(CurrentGameConfiguration.GameplayTime, () =>
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC(nameof(SetGameOver), RpcTarget.AllBuffered, "RUNNERS WIN");
                    CancelInvoke(nameof(CheckForGameOverCondition));
                };
                GameplayTimeOver(); 
            },
                gameplayTimeText, true, false, AfterEachSecondGameplayTimer);

            InvokeRepeating(nameof(CheckForGameOverCondition), 10, 3);
        }

        private void AfterEachSecondGameplayTimer(float time)
        {
            if (timerTextScaleShaker) timerTextScaleShaker.Play();
        }

        bool doorOpening = false;
        private void AfterEachSecondCountdownTimer(float time)
        {
            if (countdownTimerTextScaleShaker) countdownTimerTextScaleShaker.Play();

            //camera logic
            if (Globals.gameState == Globals.GameState.TakePosition)
            {
                if (time < 4)
                {
                    //huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Down");
                    //huntersCage.GetComponent<Animator>().Play("Cage Door Open");
                    if (!doorOpening)
                    {
                        huntersCageDoor.transform.DORotate(new Vector3(140, 0, 0), 6f).OnComplete(delegate { doorOpening = false; });
                        doorOpening = true;
                    }

                    rfmCameraManager.SwtichCamera(1);
                    Debug.LogError("Switch  to Hunter Camera");
                }
                if (time < 1)
                    rfmCameraManager.SwitchOffAllCameras();
                Debug.LogError("Switch  to Player Camera");
            }
        }

        private void CheckForGameOverCondition()
        {
            if (Globals.gameState != Globals.GameState.Gameplay) return;
            if (!PhotonNetwork.IsMasterClient) return;


            var hunters = FindObjectsOfType<RFM.Character.Hunter>(false);
            var huntersCount = 0;

            for (int i = 0; i < hunters.Length; i++)
            {
                if (hunters[i].enabled)
                {
                    huntersCount++;
                }
            }

            if (huntersCount == 0)
            {
                //if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC(nameof(SetGameOver), RpcTarget.AllBuffered, "RUNNERS WIN");
                    CancelInvoke(nameof(CheckForGameOverCondition));
                    return;
                }

                //Timer.StopAllTimers();
                //GameplayTimeOver();
            }

            var runners = FindObjectsOfType<RFM.Character.Runner>(false);
            var runnersCount = 0;

            for (int i = 0; i < runners.Length; i++)
            {
                if (runners[i].enabled)
                {
                    runnersCount++;
                }
            }

            Debug.LogError("CheckForGameOverCondition runners count: " + runnersCount);
            if (runnersCount == 0)
            {
                //if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC(nameof(SetGameOver), RpcTarget.AllBuffered, "HUNTERS WIN");
                    CancelInvoke(nameof(CheckForGameOverCondition));
                    return;
                }

                //Timer.StopAllTimers();
                //GameplayTimeOver();
            }


            // I did this fix in my branch and you did it in your branch. So I have commented out mine.

            /*var hunters = FindObjectsOfType<RFM.Character.Hunter>(false);
            var huntersCount = 0;

            for (int i = 0; i < hunters.Length; i++)
            {
                if (hunters[i].enabled)
                {
                    huntersCount++;
                }
            }

            Debug.LogError("CheckForGameOverCondition hunters count: " + huntersCount);
            if (huntersCount == 0)
            {
                Debug.Log($"All hunters have left the game. Triggering winning condition for runners.");
                Timer timerToSet = UnityEngine.Object.FindObjectOfType<Timer>();
                timerToSet.FinishGameOnHuntersLeft();
            }*/
        }


        [PunRPC]
        private void SetGameOver(string text)
        {
            Globals.gameOverText = text;

            Timer.StopAllTimers();
            GameplayTimeOver();
        }


        private async void GameplayTimeOver()
        {
            CancelInvoke(nameof(CheckForGameOverCondition));

            gameplayTimeText.transform.parent.gameObject.SetActive(false);

            EventsManager.CalculateScores();
            CancelInvoke(nameof(CheckHuntersForSpectating));
            EventsManager.GameOver();
            Globals.gameState = Globals.GameState.GameOver;
            statusTMP.text = "Time's Up!";
            statusBG.SetActive(false);

            //gameOverPanel.SetActive(true);

            if (_npcCamera != null)
            {
                Destroy(_npcCamera.gameObject);
            }

            await Task.Delay(1000);
            EventsManager.ShowScores();
            await Task.Delay(1000);
            EventsManager.DestroyAllNPCHunters();
        }

        public void PlayerCaught(int hunterViewID = -1)
        {
            //Debug.LogError("RFM I was caught by " + hunterViewID + " Globals.gameState: " + Globals.gameState);
            if (Globals.gameState != Globals.GameState.Gameplay) return;

            _mainCam.SetActive(false);
            //_gameCanvas.SetActive(false);
            CanvasButtonsHandler.inst.ShowRFMButtons(false);
            RFM.RFMAudioManager.Instance.PlayRunnerCatchSFX();

            statusTMP.text = "Player caught! Spectating...";
            statusBG.SetActive(true);
            statusMMFPlayer.PlayFeedbacks();

            if (_npcCamera == null)
            {
                _npcCamera = Instantiate(npcCameraPrefab);
            }


            if (hunterViewID != -1)
            {
                var hunter = PhotonView.Find(hunterViewID).GetComponent<RFM.Character.Hunter>();
                if (hunter != null)
                {
                    hunterForSpectating = hunter;
                    _npcCamera.Init(hunter.cameraTarget);
                    InvokeRepeating(nameof(CheckHuntersForSpectating), 1, 2);
                    return;
                }
            }

            List<RFM.Character.Hunter> hunterList = new List<Hunter>(FindObjectsOfType<RFM.Character.Hunter>().ToList());
            var randomHunter = hunterList.Find(o => o.enabled == true);
            if (randomHunter != null)
            {
                hunterForSpectating = randomHunter;
                if (_npcCamera == null)
                {
                    _npcCamera = Instantiate(npcCameraPrefab);
                }
                _npcCamera.Init(randomHunter.cameraTarget);
                InvokeRepeating(nameof(CheckHuntersForSpectating), 1, 2);
            }
        }
        public Hunter hunterForSpectating;
        public void CheckHuntersForSpectating()
        {
            Debug.LogError("CheckHuntersForSpectating: " + hunterForSpectating);
            if (hunterForSpectating == null)
            {
                List<RFM.Character.Hunter> hunterList = new List<Hunter>(FindObjectsOfType<RFM.Character.Hunter>().ToList());
                var randomHunter = hunterList.Find(o => o.enabled == true);

                if (randomHunter != null)
                {
                    hunterForSpectating = randomHunter;
                    if (_npcCamera == null)
                    {
                        _npcCamera = Instantiate(npcCameraPrefab);
                    }
                    _npcCamera.Init(randomHunter.cameraTarget);
                }
            }
        }

        private void ReceivePhotonEvents(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case PhotonEventCodes.ResetPositionEventCode:
                    {
                        ResetPosition();
                        break;
                    }
                case PhotonEventCodes.StartRFMEventCode:
                    {
                        StartCoroutine(StartRFM());
                        break;
                    }
                    //case PhotonEventCodes.PlayerRunnerCaught:
                    //    {
                    //        // PhotonView.Find(id)
                    //        //var viewId = (int)photonEvent.CustomData;
                    //        int runnerViewID = (int)((object[])photonEvent.CustomData)[0];
                    //        int hunterViewID = (int)((object[])photonEvent.CustomData)[1];

                    //        if (runnerViewID == RFM.Globals.player.GetComponent<PhotonView>().ViewID)
                    //        {
                    //            PlayerCaught(hunterViewID);
                    //        }

                    //        // Game should be over if all runners are caught

                    //        break;
                    //    }
            }
        }


        #endregion

        #region Photon Callbacks

        public override void OnJoinedRoom()
        {
            Debug.Log("RFM OnJoinedRoom() Called by RFMManager");
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log("RFM OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log($"RFM OnPlayerLeftRoom() {other.NickName}"); // seen when other disconnects
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Loads game configuration data from API
        /// </summary>
        private static IEnumerator FetchConfigDataFromServer()
        {
            //the api is set we just have to get the map
            var url = "https://api.npoint.io/2b73c02e13403750bcb0"; // This JSON file can be edited
                                                                    // at https://www.npoint.io/docs/2b73c02e13403750bcb0

            if (RFM.Globals.DevMode)
            {
                url = "https://api.npoint.io/d348af22a8d5fe5167f4"; // Use a different JSON file for dev mode.
                                                                    // https://www.npoint.io/docs/d348af22a8d5fe5167f4
            }


            using UnityWebRequest www = UnityWebRequest.Get(url);
            // www.SetRequestHeader("Authorization", userToken);
            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            if (www.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("RFM Failed to load configuration from API. Using default values...");

                CurrentGameConfiguration = new GameConfiguration
                {
                    MatchMakingTime = 10,
                    TakePositionTime = 10,
                    GameplayTime = 60,
                    //GameRestartWaitTime = 3000,
                    MaxPlayersInRoom = 10,
                    RunnersToHuntersRatio = Vector2.one,
                    GainingMoneyTimeInterval = 1,
                    MoneyPerInterval = 15,
                };
            }
            else
            {
                CurrentGameConfiguration = JsonUtility.FromJson<GameConfiguration>(www.downloadHandler.text);
            }
        }

        /// <summary>
        /// Returns the number of Runners, Hunters, NPCRunners, and NPCHunters
        /// </summary>
        /// <param name="roomLimit">Max number of players allowed</param>
        /// <param name="numberOfPlayers">Current number of connected players</param>
        /// <param name="ratioVector">Runner to Hunter ratio</param>
        /// <author>Muneeb</author>
        private static (int, int, int, int) CalculateRoles(int roomLimit, int numberOfPlayers, Vector2 ratioVector)
        {
            // Calculate the total ratio
            int totalRatio = (int)(ratioVector.x + ratioVector.y);

            // Calculate the number of players for each role
            int runnerCount = (int)(roomLimit * ratioVector[0] / totalRatio);
            int hunterCount = (int)(roomLimit * ratioVector[1] / totalRatio);

            // Adjust the counts to ensure they sum up to RoomLimit
            int totalPlayers = runnerCount + hunterCount;
            if (totalPlayers < roomLimit)
            {
                runnerCount += roomLimit - totalPlayers;
            }
            else if (totalPlayers > roomLimit)
            {
                // This may happen due to rounding errors, so we decrease one of the counts
                runnerCount -= totalPlayers - roomLimit;
            }



            int numberOfRunners = runnerCount;
            int numberOfHunters = hunterCount;
            int numberOfAIHunters = 0;
            int numberOfAIRunners = 0;

            if (numberOfPlayers == roomLimit)
            {
                numberOfAIHunters = 0;
            }

            else if (numberOfPlayers >= roomLimit / 2)
            {
                numberOfHunters = numberOfPlayers - numberOfRunners;
                numberOfAIHunters = roomLimit - numberOfPlayers;
            }

            else
            {
                numberOfAIHunters = roomLimit / 2;
                numberOfAIRunners = (roomLimit / 2) - numberOfPlayers;
                numberOfRunners = numberOfPlayers;
                numberOfHunters = 0;
            }

            return (numberOfRunners, numberOfHunters, numberOfAIRunners, numberOfAIHunters);

            // roomLimit = 12, numberOfPlayers = 0,  ratioVector = (1, 1) = 0, 0, 6, 6
            // roomLimit = 12, numberOfPlayers = 1,  ratioVector = (1, 1) = 1, 0, 5, 6
            // roomLimit = 12, numberOfPlayers = 2,  ratioVector = (1, 1) = 2, 0, 4, 6
            // roomLimit = 12, numberOfPlayers = 3,  ratioVector = (1, 1) = 3, 0, 3, 6
            // roomLimit = 12, numberOfPlayers = 4,  ratioVector = (1, 1) = 4, 0, 2, 6
            // roomLimit = 12, numberOfPlayers = 5,  ratioVector = (1, 1) = 5, 0, 1, 6
            // roomLimit = 12, numberOfPlayers = 6,  ratioVector = (1, 1) = 6, 0, 0, 6
            // roomLimit = 12, numberOfPlayers = 7,  ratioVector = (1, 1) = 6, 1, 0, 5
            // roomLimit = 12, numberOfPlayers = 8,  ratioVector = (1, 1) = 6, 2, 0, 4
            // roomLimit = 12, numberOfPlayers = 9,  ratioVector = (1, 1) = 6, 3, 0, 3
            // roomLimit = 12, numberOfPlayers = 10, ratioVector = (1, 1) = 6, 4, 0, 2
            // roomLimit = 12, numberOfPlayers = 11, ratioVector = (1, 1) = 6, 5, 0, 1
            // roomLimit = 12, numberOfPlayers = 12, ratioVector = (1, 1) = 6, 6, 0, 0

            // TODO : Fix this
            // roomLimit = 12, numberOfPlayers = 7, ratioVector = (1, 2) = 4, 3, 0, 5
            // roomLimit = 12, numberOfPlayers = 7, ratioVector = (2, 1) = 8, -1, 0, 5 // !!
        }

        public void CalculateRolesUnitTest()
        {
            int maxPlayers = 8;

            for (int i = 0; i <= maxPlayers; i++)
            {
                Debug.LogError(CalculateRoles(maxPlayers, i, new Vector2(1, 1)));
            }
        }

        #endregion

        #region Classes, Enums, and Structs

        [Serializable]
        public class GameConfiguration
        {
            public int MatchMakingTime;
            public int TakePositionTime;
            public int GameplayTime;
            //public int GameRestartWaitTime;
            public int MaxPlayersInRoom;
            public Vector2 RunnersToHuntersRatio;
            public int GainingMoneyTimeInterval;
            public int MoneyPerInterval;
        }

        #endregion
    }
}
