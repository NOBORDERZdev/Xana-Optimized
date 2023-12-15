using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using MoreMountains.Feedbacks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace RFM.Managers
{
    public class RFMManager : MonoBehaviourPunCallbacks
    {
        #region Serialized Fields
        public bool isPlayerHunter;
        //public Transform lobbySpawnPoint;
        //public GameObject playerObject;
        [SerializeField] public Transform playersSpawnArea;
        [SerializeField] private GameObject huntersCage;
        [SerializeField] private FollowNPC npcCameraPrefab;
        [SerializeField] public Transform huntersSpawnArea;

        [SerializeField] private TextMeshProUGUI countDownText;
        [SerializeField] private TextMeshProUGUI gameplayTimeText, statusTMP;
        [SerializeField] private GameObject statusBG;
        //[SerializeField] private GameObject gameOverPanel;

        //MM effects
        [SerializeField] private MMScaleShaker timerTextScaleShaker;
        [SerializeField] private MMScaleShaker countdownTimerTextScaleShaker;
        [SerializeField] private MMF_Player statusMMFPlayer;

        //Camera Manager
        [SerializeField] private RFMCameraManager rfmCameraManager;
        [SerializeField] public GameObject _mainCam;

        //VFX
        [SerializeField] private GameObject playerSpawnVFX, hunterSpawnVFX;

        #endregion

        #region Fields

        public static RFMManager Instance;

        private GameObject _gameCanvas;
        private FollowNPC _npcCamera;
        //private PlayerControllerNew _player;
        [HideInInspector] public static GameConfiguration CurrentGameConfiguration;

        //double startTime = -1;
        //float currentTime = 0;

        #endregion

        #region Unity Callback Methods

        private void Awake()
        {
            RFM.Globals.IsRFMWorld = true; // TODO: Do this in main menu
            Instance = this;
            EventsManager.OnHideCanvasElements();
        }


        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.NetworkingClient.EventReceived += ReceivePhotonEvents;

        }


        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.EventReceived -= ReceivePhotonEvents;
        }

        public void RestartRFM()
        {
            // clear all custom properties of all players
            foreach (var player in PhotonNetwork.PlayerList)
            {
                player.CustomProperties.Clear();
            }

            _mainCam.SetActive(true);
            _gameCanvas.SetActive(true);
            RFM.Globals.player.transform.root.gameObject.SetActive(true);
            StartCoroutine(Start());
        }


        private IEnumerator Start()
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            // RFMUIManager.Instance.ShowXanaStonePopup();

            Application.runInBackground = true;
            yield return StartCoroutine(FetchConfigDataFromServer());

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.MaxPlayers = (byte)CurrentGameConfiguration.MaxPlayersInRoom;
            }

            Globals.gameState = Globals.GameState.InLobby;
            //_mainCam = GameObject.FindGameObjectWithTag(Globals.MAIN_CAMERA_TAG);
            _gameCanvas = GameObject.FindGameObjectWithTag(Globals.CANVAS_TAG);
            //RFMButtonsLayoutManager.instance.LoadLayout();
            //gameOverPanel.SetActive(false);
            gameplayTimeText.transform.parent.gameObject.SetActive(true);

            StartMatchMaking();

            //this is to turn post processing on
            var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = true;
        }

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

            }, gameplayTimeText);

            //photonView.RPC(nameof(PlayerJoined), RpcTarget.AllBuffered);
            //Debug.Log("RFM PlayerJoined() RPC Requested by " + PhotonNetwork.NickName);

            //InvokeRepeating(nameof(CheckForGameStartCondition), 1, 1);

            statusTMP.text = "Waiting for other players to join:";
            statusBG.SetActive(true);
            statusMMFPlayer.PlayFeedbacks();
        }

        // private void OnGUI()
        // {
        //     GUI.Label(new Rect(10, 10, 200, 75), PhotonNetwork.IsMasterClient.ToString());
        //     GUI.Label(new Rect(10, 30, 200, 75), Globals.gameState.ToString());
        // }

        #endregion

        #region Private Methods

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

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (PhotonNetwork.PlayerListOthers.Length > 0)
                    {
                        PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer.GetNext());

                        //Debug.LogError("RPC Called");
                        //GetComponent<PhotonView>().RPC(nameof(ChangeMasterClientifAvailble), PhotonNetwork.PlayerListOthers[0]);
                        PhotonNetwork.SendAllOutgoingCommands();
                    }
                    //ChangeMasterClientifAvailble();

                }
            }
        }


        private IEnumerator StartRFM()
        {
            EventsManager.StartCountdown();
            Globals.gameState = Globals.GameState.Countdown;
            //CancelInvoke(nameof(CheckForGameStartCondition));

            countDownText.transform.parent.gameObject.SetActive(true);
            statusTMP.text = "Resetting position in:";
            statusBG.SetActive(true);
            statusMMFPlayer.PlayFeedbacks();

            huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Up");


            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                int roomLimit = PhotonNetwork.CurrentRoom.MaxPlayers;
                var numberOfPlayers = PhotonNetwork.PlayerList.Length;

                var roles = CalculateRoles(roomLimit, numberOfPlayers,
                    CurrentGameConfiguration.RunnersToHuntersRatio);

                Debug.Log($"RFM roles: {roles}");


                StartCoroutine(SpawnNPCs(roles.Item4, roles.Item3));

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
                countDownText, AfterEachSecondCountdownTimer));

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

                //Globals.player.gameObject.AddComponent<RFM.Character.PlayerHunter>();
                //RFM.Globals.player.GetComponent<RFM.Character.PlayerRunner>().enabled = false;
                //RFM.Globals.player.GetComponent<RFM.Character.PlayerHunter>().enabled = true;
                Globals.IsLocalPlayerHunter = true;

                //statusTMP.text = "CATCH THE <#FF36D3>RUNNERS!</color>";

                //var hunterPosition = huntersSpawnArea.position;
                //var randomHunterPos = new Vector3(
                //    hunterPosition.x + Random.Range(-2, 2),
                //    hunterPosition.y,
                //    hunterPosition.z + Random.Range(-2, 2));

                ////play VFX
                //hunterSpawnVFX.SetActive(true);
                //Destroy(hunterSpawnVFX, 10f);
                //Globals.player.transform.SetPositionAndRotation(randomHunterPos, Quaternion.identity);

            }

            else // Player spawning as Runner
            {
                Debug.Log($"RFM {PhotonNetwork.NickName} Spawning as Runner.");

                //Globals.player.gameObject.AddComponent<RFM.Character.PlayerRunner>();
                //RFM.Globals.player.GetComponent<RFM.Character.PlayerHunter>().enabled = false;
                //RFM.Globals.player.GetComponent<RFM.Character.PlayerRunner>().enabled = true;
                Globals.IsLocalPlayerHunter = false;

                //statusTMP.text = "<#FF36D3>HUNTERS</color> RELEASING IN:";

                //var position = playersSpawnArea.position;
                //var randomPos = new Vector3(
                //    position.x + Random.Range(-2, 2),
                //    position.y,
                //    position.z + Random.Range(-2, 2));

                ////play VFX
                //playerSpawnVFX.SetActive(true);
                //Destroy(playerSpawnVFX, 10f); // Causes a null reference on game restart.
                //                              // Should be instantiated or disabled.

                //Globals.player.transform.SetPositionAndRotation(randomPos, Quaternion.identity);
            }
        }

        float spawnOffset = 0;
        private void ResetPosition()
        {
            EventsManager.TakePositionTime();
            Globals.gameState = Globals.GameState.TakePosition;

            //bool isHunter = false;
            //if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("isHunter", out object _isHunter))
            //{
            //    isPlayerHunter = isHunter = (bool)_isHunter;
            //}


            if (/*isHunter*/RFM.Globals.IsLocalPlayerHunter) // Player spawning as Hunter
            {
                //Debug.Log($"RFM {PhotonNetwork.NickName} Spawning as Hunter.");

                //Globals.player.gameObject.AddComponent<RFM.Character.PlayerHunter>();
                //Globals.IsLocalPlayerHunter = true;

                statusTMP.text = "CATCH THE <#FF36D3>RUNNERS!</color>";
                statusBG.SetActive(true);
                statusMMFPlayer.PlayFeedbacks();

                Timer.SetDurationAndRun(CurrentGameConfiguration.TakePositionTime,
                    /*AfterTakePositionTimerHunter*/StartGameplay,
                    countDownText);

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
                //Debug.Log($"RFM {PhotonNetwork.NickName} Spawning as Runner.");

                //Globals.player.gameObject.AddComponent<RFM.Character.PlayerRunner>();
                //Globals.IsLocalPlayerHunter = false;

                statusTMP.text = "<#FF36D3>HUNTERS</color> RELEASING IN:";
                statusBG.SetActive(true);
                statusMMFPlayer.PlayFeedbacks();

                //Globals.gameState = Globals.GameState.TakePosition;

                Timer.SetDurationAndRun(CurrentGameConfiguration.TakePositionTime,
                    /*AfterTakePositionTimerRunner*/StartGameplay,
                    countDownText,
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
            //RFMCharacter.gameStartAction?.Invoke();
        }


        //private void AfterTakePositionTimerHunter()
        //{
        //    //Globals.player.gameObject.AddComponent<RFM.Character.PlayerHunter>();
        //    //Globals.IsLocalPlayerHunter = true;
        //    //huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Down");
        //    //countDownText.transform.parent.gameObject.SetActive(false);

        //    //StartGameplay();
        //}


        //private void AfterTakePositionTimerRunner()
        //{
        //    //Globals.player.gameObject.AddComponent<RFM.Character.PlayerRunner>();
        //    //Globals.IsLocalPlayerHunter = false;
        //    //huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Down");
        //    //countDownText.transform.parent.gameObject.SetActive(false);

        //    //StartGameplay();
        //}

        private IEnumerator SpawnNPCs(int numOfHunters, int numOfRunners)
        {
            var delay = (numOfHunters + numOfRunners) / 11; // 10 seconds countdown.
                                                            // Need to spawn all NPCs beofre last second


            Debug.Log("RFM numOfAIRunners: " + numOfRunners);
            GameObject hunterNPC, runnerNPC;
            for (int i = 0; i < numOfRunners; i++)
            {
                runnerNPC = PhotonNetwork.InstantiateRoomObject("RFM/RunnerNPC",
                    playersSpawnArea.position + new Vector3(
                        Random.Range(-1.0f, 1.0f), 0,
                      i),
                    playersSpawnArea.rotation);
                runnerNPC.GetComponent<NavMeshAgent>().speed = Random.Range(2f, 3.4f); // TODO: Set speed in NPCRunner.cs
                yield return new WaitForSeconds(delay);
            }


            Debug.Log("RFM numOfAIHunters: " + numOfHunters);
            for (int i = 0; i < numOfHunters; i++)
            {
                hunterNPC = PhotonNetwork.InstantiateRoomObject("RFM/HunterNPC",
                    huntersSpawnArea.position + new Vector3(Random.Range(-1.0f, 1.0f), 0,
                        Random.Range(-1.0f, 1.0f)),
                    huntersSpawnArea.rotation);
                hunterNPC.GetComponent<NavMeshAgent>().speed = Random.Range(2f, 3.4f); // TODO: Set speed in NPCHunter.cs
                yield return new WaitForSeconds(delay);
            }
        }


        //[PunRPC]
        private void StartGameplay()
        {
            EventsManager.StartGame();
            Globals.gameState = Globals.GameState.Gameplay;
            //huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Down");
            gameplayTimeText.transform.parent.gameObject.SetActive(true);
            gameplayTimeText.gameObject.SetActive(true);
            countDownText.transform.parent.gameObject.SetActive(false);
            statusBG.SetActive(false);
            statusMMFPlayer.PlayFeedbacks();

            Timer.SetDurationAndRun(CurrentGameConfiguration.GameplayTime, GameplayTimeOver,
                gameplayTimeText, AfterEachSecondGameplayTimer);
        }

        private void AfterEachSecondGameplayTimer(float time)
        {
            if (timerTextScaleShaker) timerTextScaleShaker.Play();
        }

        private void AfterEachSecondCountdownTimer(float time)
        {
            if (countdownTimerTextScaleShaker) countdownTimerTextScaleShaker.Play();

            //camera logic
            if (Globals.gameState == Globals.GameState.TakePosition)
            {

                if (time < 4)
                {
                    huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Down");
                    rfmCameraManager.SwtichCamera(1);
                }
                if (time < 1)
                    rfmCameraManager.SwitchOffAllCameras();
            }
        }

        private async void GameplayTimeOver()
        {
            gameplayTimeText.transform.parent.gameObject.SetActive(false);

            EventsManager.CalculateScores();
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
        }

        public void PlayerCaught(int hunterViewID = -1/*Transform hunter*/)
        {
            if (Globals.gameState != Globals.GameState.Gameplay) return;

            _mainCam.SetActive(false);
            _gameCanvas.SetActive(false);
            statusTMP.text = "Player caught! Spectating...";
            statusBG.SetActive(true);
            statusMMFPlayer.PlayFeedbacks();

            if (_npcCamera == null)
            {
                _npcCamera = Instantiate(npcCameraPrefab);
            }

            //var randomHunter = FindObjectOfType<RFM.Character.Hunter>();

            //if (randomHunter != null)
            //{
            //    _npcCamera.Init(randomHunter.cameraTarget);
            //}


            if (hunterViewID != -1)
            {
                var hunter = PhotonView.Find(hunterViewID).GetComponent<RFM.Character.Hunter>();
                if (hunter != null)
                {
                    _npcCamera.Init(hunter.cameraTarget);
                    return;
                }
            }

            var randomHunter = FindObjectOfType<RFM.Character.Hunter>();

            if (randomHunter != null)
            {
                if (_npcCamera == null)
                {
                    _npcCamera = Instantiate(npcCameraPrefab);
                }
                _npcCamera.Init(randomHunter.cameraTarget);
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

//public class RFMPlayerClass
//{
//    public string playerName;
//    public bool isHunter;
//    public bool isRunner;
//}