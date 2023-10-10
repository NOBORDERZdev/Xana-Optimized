using System;
using System.Collections;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using MoreMountains.Feedbacks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace RFM.Managers
{
    public class RFMManager : MonoBehaviourPunCallbacks
    {
        #region Photon Events Codes

        public const byte ResetPositionEventCode = 6;
        public const byte StartRFMEventCode = 7;

        #endregion

        #region Serialized Fields

        public Transform lobbySpawnPoint;
        [SerializeField] public Transform playersSpawnArea;
        [SerializeField] private GameObject huntersCage;
        [SerializeField] private FollowNPC npcCameraPrefab;
        [SerializeField] public Transform huntersSpawnArea;

        [SerializeField] private TextMeshProUGUI countDownText;
        [SerializeField] private TextMeshProUGUI gameplayTimeText, statusTMP;
        [SerializeField] private GameObject statusBG;
        [SerializeField] private GameObject gameOverPanel;
        
        //MM effects
        [SerializeField] private MMScaleShaker timerTextScaleShaker;
        [SerializeField] private MMScaleShaker countdownTimerTextScaleShaker;
        [SerializeField] private MMF_Player statusMMFPlayer;
        
        //Camera Manager
        [SerializeField] private RFMCameraManager rfmCameraManager;

        //VFX
        [SerializeField] private GameObject playerSpawnVFX, hunterSpawnVFX;
        
        #endregion

        #region Fields

        public static RFMManager Instance;
        private GameObject _mainCam, _gameCanvas;
        private FollowNPC _npcCamera;
        private PlayerControllerNew _player;
        [HideInInspector] public static GameConfiguration CurrentGameConfiguration;

        #endregion

        #region Unity Callback Methods

        private void Awake()
        {
            RFM.Globals.IsRFMWorld = true; // TODO: Do this in main menu
            Instance = this;
            EventsManager.OnHideCanvasElements();
            
            PhotonNetwork.GameVersion = Application.version + "998"; // TODO temp
        }

        
        public override void OnEnable()
        {
            base.OnEnable();
            EventsManager.onPlayerCaught += PlayerCaught;
            EventsManager.onPlayerCaughtByPlayer += PlayerCaughtByPlayer;
            PhotonNetwork.NetworkingClient.EventReceived += ReceivePhotonEvents;
        }

        
        public override void OnDisable()
        {
            base.OnDisable();
            EventsManager.onPlayerCaught -= PlayerCaught;
            EventsManager.onPlayerCaughtByPlayer -= PlayerCaughtByPlayer;
            PhotonNetwork.NetworkingClient.EventReceived -= ReceivePhotonEvents;
        }

        
        private IEnumerator Start()
        {
            yield return StartCoroutine(FetchConfigDataFromServer());

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.MaxPlayers = (byte)CurrentGameConfiguration.MaxPlayersInRoom;
            }
            
            Globals.gameState = Globals.GameState.InLobby;
            _mainCam = GameObject.FindGameObjectWithTag(Globals.MAIN_CAMERA_TAG);
            _gameCanvas = GameObject.FindGameObjectWithTag(Globals.CANVAS_TAG);
            
            gameOverPanel.SetActive(false);
            gameplayTimeText.transform.parent.gameObject.SetActive(true);
            
            Timer.SetDurationAndRun(CurrentGameConfiguration.MatchMakingTime, () =>
            {
                if (Globals.gameState == Globals.GameState.InLobby)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.RaiseEvent(StartRFMEventCode, null,
                            new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                    }
                    CancelInvoke(nameof(CheckForGameStartCondition));
                }
                
            }, gameplayTimeText);
            
            photonView.RPC(nameof(PlayerJoined), RpcTarget.AllBuffered);
            Debug.Log("RFM PlayerJoined() RPC Requested by " + PhotonNetwork.NickName);
            
            InvokeRepeating(nameof(CheckForGameStartCondition), 1, 1);
            
            //this is to turn post processing on
            var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.renderPostProcessing = true;
        }
        
        // private void OnGUI()
        // {
        //     GUI.Label(new Rect(10, 10, 200, 75), PhotonNetwork.IsMasterClient.ToString());
        //     GUI.Label(new Rect(10, 30, 200, 75), Globals.gameState.ToString());
        // }
        
        #endregion

        #region Private Methods

        [PunRPC]
        private void PlayerJoined()
        {
            CheckForGameStartCondition();
        }

        private void CheckForGameStartCondition()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                if (Globals.gameState != Globals.GameState.InLobby) return;
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.RaiseEvent(StartRFMEventCode, null,
                        new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }
            }
            else
            {
                Globals.gameState = Globals.GameState.InLobby;
                countDownText.transform.parent.gameObject.SetActive(false);
                statusBG.SetActive(false);
                
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = true;
                }
                
                StopAllCoroutines();
            }
        }


        private IEnumerator StartRFM()
        {
            EventsManager.StartCountdown();
            Globals.gameState = Globals.GameState.Countdown;
            CancelInvoke(nameof(CheckForGameStartCondition));
            
            countDownText.transform.parent.gameObject.SetActive(true);
            statusTMP.text = "Countdown";
            statusBG.SetActive(true);
            statusMMFPlayer.PlayFeedbacks();

            huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Up");

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                int roomLimit = PhotonNetwork.CurrentRoom.MaxPlayers;
                var numberOfPlayers = PhotonNetwork.PlayerList.Length;

                var roles = CalculateRoles(roomLimit, numberOfPlayers,
                    CurrentGameConfiguration.EscapeesToHuntersRatio);

                Hashtable properties = new Hashtable { { "numberOfPlayerHunters", roles.Item2 } };
                PhotonNetwork.MasterClient.SetCustomProperties(properties);

                SpawnHunters(roles.Item4);
                SpawnAIEscapees(roles.Item3);
            }

            gameplayTimeText.gameObject.SetActive(false);
            
            yield return StartCoroutine(Timer.SetDurationAndRunEnumerator(10, null, 
                countDownText, AfterEachSecondCountdownTimer));

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.RaiseEvent(ResetPositionEventCode, null,
                    new RaiseEventOptions { Receivers = ReceiverGroup.All },
                    SendOptions.SendReliable);
            }
        }
        
        
        private void ResetPosition()
        {
            int numOfHunters = 0;
            
            if (PhotonNetwork.MasterClient.CustomProperties.TryGetValue("numberOfPlayerHunters", out var x))
            {
                numOfHunters = (int)x;
            }

            if (numOfHunters > 0)
            {
                Debug.Log(PhotonNetwork.NickName + "RFM Spawning as Hunter.");
                
                statusTMP.text = "CATCH THE <#FF36D3>ESCAPEES!</color>";
                statusBG.SetActive(true);
                statusMMFPlayer.PlayFeedbacks();

                Globals.gameState = Globals.GameState.TakePosition;

                Timer.SetDurationAndRun(CurrentGameConfiguration.TakePositionTime, AfterTakePositionTimerHunter,
                    countDownText);

                var hunterPosition = huntersSpawnArea.position;
                var randomHunterPos = new Vector3(
                    hunterPosition.x + Random.Range(-2, 2),
                    hunterPosition.y,
                    hunterPosition.z + Random.Range(-2, 2));

                //play VFX
                hunterSpawnVFX.SetActive(true);
                Destroy(hunterSpawnVFX, 10f);
                Globals.player.transform.SetPositionAndRotation(randomHunterPos, Quaternion.identity);

                Hashtable properties = new Hashtable { { "numberOfPlayerHunters", numOfHunters - 1 } };
                PhotonNetwork.MasterClient.SetCustomProperties(properties);
            }

            else // Spawning as Escapee
            {
                Debug.Log(PhotonNetwork.NickName + "RFM Spawning as Escapee.");
                
                statusTMP.text = "RUN FAR FROM THE <#FF36D3>HUNTERS!</color>";
                statusBG.SetActive(true);
                statusMMFPlayer.PlayFeedbacks();

                Globals.gameState = Globals.GameState.TakePosition;

                Timer.SetDurationAndRun(CurrentGameConfiguration.TakePositionTime, 
                    AfterTakePositionTimerEscapee, countDownText,
                    AfterEachSecondCountdownTimer);

                var position = playersSpawnArea.position;
                var randomPos = new Vector3(
                    position.x + Random.Range(-2, 2),
                    position.y,
                    position.z + Random.Range(-2, 2));

                //play VFX
                playerSpawnVFX.SetActive(true);
                Destroy(playerSpawnVFX, 10f); // Causes a null reference on game restart.
                                              // Should be instantiated or disabled.
                                              
                Globals.player.transform.SetPositionAndRotation(randomPos, Quaternion.identity);
            }
        }
        
        private void AfterTakePositionTimerHunter()
        {
            Globals.player.gameObject.AddComponent<RFM.Character.PlayerHunter>();
            Globals.IsLocalPlayerHunter = true;
            huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Down");
            countDownText.transform.parent.gameObject.SetActive(false);

            StartGameplay();
        }
        

        private void AfterTakePositionTimerEscapee()
        {
            Globals.player.gameObject.AddComponent<RFM.Character.PlayerEscapee>();
            Globals.IsLocalPlayerHunter = false;
            huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Down");
            countDownText.transform.parent.gameObject.SetActive(false);

            StartGameplay();
        }

        private void SpawnHunters(int numOfHunters)
        {
            Debug.Log("RFM numOfAIHunters: " + numOfHunters);
            for (int i = 0; i < numOfHunters; i++)
            {
                PhotonNetwork.InstantiateRoomObject("HunterNPC",
                    huntersSpawnArea.position + new Vector3(Random.Range(-2, 2), 0, 
                        Random.Range(-2, 2)),
                    huntersSpawnArea.rotation);
            }
        }
        
        private void SpawnAIEscapees(int numOfEscapees)
        {
            Debug.Log("RFM numOfAIEscapees: " + numOfEscapees);
            for (int i = 0; i < numOfEscapees; i++)
            {
                PhotonNetwork.InstantiateRoomObject("EscapeeNPC",
                    playersSpawnArea.position + new Vector3(Random.Range(-2, 2), 0, 
                        Random.Range(-2, 2)),
                    playersSpawnArea.rotation);
            }
        }

        [PunRPC]
        private void StartGameplay()
        {
            EventsManager.StartGame();
            Globals.gameState = Globals.GameState.Gameplay;
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
            if(timerTextScaleShaker) timerTextScaleShaker.Play();
        }

        private void AfterEachSecondCountdownTimer(float time)
        {
            if (countdownTimerTextScaleShaker) countdownTimerTextScaleShaker.Play();
            
            //camera logic
            if (Globals.gameState == Globals.GameState.Countdown) 
            {
                //if (time < 7)
                   // rfmCameraManager.SwtichCamera(0);
                //if (time < 4)
                    //rfmCameraManager.SwtichCamera(1);
                if (time < 1)
                    rfmCameraManager.SwitchOffAllCameras();
            }
        }

        private async void GameplayTimeOver()
        {
            gameplayTimeText.transform.parent.gameObject.SetActive(false);
            EventsManager.GameOver();
            Globals.gameState = Globals.GameState.GameOver;
            statusTMP.text = "Time's Up!";
            statusBG.SetActive(false);

            gameOverPanel.SetActive(true);
            
            if (_npcCamera != null)
            {
                Destroy(_npcCamera.gameObject);
            }
            
            await Task.Delay(CurrentGameConfiguration.GameRestartWaitTime);
            EventsManager.GameRestarting();
        }
        
        // [PunRPC]
        // public void LocalPlayerCaughtByHunter(int viewID)
        // {
        //     if (Globals.player.GetComponentInChildren<PhotonView>().ViewID == viewID)
        //     {
        //         Debug.LogError("RFM LocalPlayerCaughtByHunter viewID = " + viewID);
        //         
        //         if (Globals.gameState != Globals.GameState.Gameplay) return;
        //
        //         mainCam.SetActive(false);
        //         gameCanvas.SetActive(false);
        //         statusTMP.text = "You've been caught!";
        //         statusBG.SetActive(true);
        //         statusMMFPlayer.PlayFeedbacks();
        //         //statusTMP.gameObject.SetActive(true);
        //         // Globals.player.transform.root.gameObject.SetActive(false);
        //         
        //         // photonView.RPC(nameof(DeactivateNPCPlayer), RpcTarget.Others, PhotonNetwork.LocalPlayer.ActorNumber);
        //         PhotonNetwork.Destroy(Globals.player.transform.root.gameObject);
        //         
        //         npcCamera = Instantiate(npcCameraPrefab);
        //         npcCamera.Init(transform/*.CameraTarget*/);
        //     }
        // }

        private void PlayerCaught(RFM.Character.NPCHunter catcher)
        {
            if (Globals.gameState != Globals.GameState.Gameplay) return;
        
            _mainCam.SetActive(false);
            _gameCanvas.SetActive(false);
            statusTMP.text = "You've been caught!";
            statusBG.SetActive(true);
            statusMMFPlayer.PlayFeedbacks();

            if (_npcCamera == null)
            {
                _npcCamera = Instantiate(npcCameraPrefab);
            }
            _npcCamera.Init(catcher.CameraTarget);
        }
        
        
        private void PlayerCaughtByPlayer(RFM.Character.PlayerHunter catcher)
        {
            if (Globals.gameState != Globals.GameState.Gameplay) return;
        
            _mainCam.SetActive(false);
            _gameCanvas.SetActive(false);
            statusTMP.text = "You've been caught!";
            statusBG.SetActive(true);
            statusMMFPlayer.PlayFeedbacks();

            if (_npcCamera == null)
            {
                _npcCamera = Instantiate(npcCameraPrefab);
            }
            _npcCamera.Init(catcher.transform/*cameraTarget*/);
        }
        


        private void ReceivePhotonEvents(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case ResetPositionEventCode:
                {
                    ResetPosition();
                    break;
                }
                case StartRFMEventCode:
                {
                    StartCoroutine(StartRFM());
                    break;
                }
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
            var url = "https://api.npoint.io/2b73c02e13403750bcb0";
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
                    GameRestartWaitTime = 3000,
                    MaxPlayersInRoom = 10,
                    EscapeesToHuntersRatio = Vector2.one,
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
        /// Returns the number of Escapees, Hunters, NPCEscapees, and NPCHunters
        /// </summary>
        /// <param name="roomLimit">Max number of players allowed</param>
        /// <param name="numberOfPlayers">Current number of connected players</param>
        /// <param name="ratioVector">Escapee to Hunter ratio</param>
        /// <author>Muneeb</author>
        private static (int, int, int, int) CalculateRoles(int roomLimit, int numberOfPlayers, Vector2 ratioVector)
        {
            // Calculate the total ratio
            int totalRatio = (int)(ratioVector.x + ratioVector.y);

            // Calculate the number of players for each role
            int escapeeCount = (int)(roomLimit * ratioVector[0] / totalRatio);
            int hunterCount = (int)(roomLimit * ratioVector[1] / totalRatio);

            // Adjust the counts to ensure they sum up to RoomLimit
            int totalPlayers = escapeeCount + hunterCount;
            if (totalPlayers < roomLimit)
            {
                escapeeCount += roomLimit - totalPlayers;
            }
            else if (totalPlayers > roomLimit)
            {
                // This may happen due to rounding errors, so we decrease one of the counts
                escapeeCount -= totalPlayers - roomLimit;
            }
            
            
            
            int numberOfEscapees = escapeeCount;
            int numberOfHunters = hunterCount;
            int numberOfAIHunters = 0;
            int numberOfAIEscapees = 0;

            if (numberOfPlayers == roomLimit)
            {
                numberOfAIHunters = 0;
            }
                
            else if (numberOfPlayers >= roomLimit / 2)
            {
                numberOfHunters = numberOfPlayers - numberOfEscapees;
                numberOfAIHunters = roomLimit - numberOfPlayers;
            }
                
            else
            {
                numberOfAIHunters = roomLimit / 2;
                numberOfAIEscapees = (roomLimit / 2) - numberOfPlayers;
                numberOfEscapees = numberOfPlayers;
                numberOfHunters = 0;
            }

            return (numberOfEscapees, numberOfHunters, numberOfAIEscapees, numberOfAIHunters);
        }

        #endregion
        
        #region Classes, Enums, and Structs

        [Serializable]
        public class GameConfiguration
        {
            public int MatchMakingTime;
            public int TakePositionTime;
            public int GameplayTime;
            public int GameRestartWaitTime;
            public int MaxPlayersInRoom;
            public Vector2 EscapeesToHuntersRatio;
            public int GainingMoneyTimeInterval;
            public int MoneyPerInterval;
        }
        
        #endregion
    }
}