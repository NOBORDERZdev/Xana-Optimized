using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace RFM
{
    public class RFMManager : MonoBehaviourPunCallbacks
    {
        public static RFMManager Instance;

        public Transform lobbySpawnPoint;
        [SerializeField] public Transform playersSpawnArea;
        [SerializeField] private GameObject hunterPrefab;
        [SerializeField] private GameObject huntersCage;
        [SerializeField] private FollowNPC npcCameraPrefab;
        [SerializeField] public Transform huntersSpawnArea;

        [SerializeField] private TextMeshProUGUI countDownText;
        [SerializeField] private TextMeshProUGUI gameplayTimeText, statusTMP;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private RectTransform leaderboardEntryContainer;
        [SerializeField] private LeaderboardEntry leaderboardEntryPrefab;

        private NPC_Manager npcManager;
        private GameObject mainCam, gameCanvas;
        private FollowNPC npcCamera;
        private PlayerControllerNew player;
        private RFMMissionsManager missionsManager;

        public static int NumOfActivePlayers;

        private void Awake()
        {
            Instance = this;
            missionsManager = GetComponent<RFMMissionsManager>();
            EventsManager.OnHideCanvasElements();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            EventsManager.onPlayerCaught += PlayerCaught;
            EventsManager.onRestarting += ActivatePlayer;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            EventsManager.onPlayerCaught -= PlayerCaught;
            EventsManager.onRestarting -= ActivatePlayer;
        }

        private void Start()
        {
            Globals.gameState = Globals.GameState.InLobby;
            mainCam = GameObject.FindGameObjectWithTag(Globals.MAIN_CAMERA_TAG);
            gameCanvas = GameObject.FindGameObjectWithTag(Globals.CANVAS_TAG);
            
            gameOverPanel.SetActive(false);
            gameplayTimeText.transform.parent.gameObject.SetActive(false);

            photonView.RPC(nameof(PlayerJoined), RpcTarget.AllBuffered);
            Debug.LogError("RFM PlayerJoined() RPC Requested by " + PhotonNetwork.NickName);
            
            InvokeRepeating(nameof(CheckForGameStartCondition), 1, 1);
        }

        #region Public Methods

        public void ForceStartGame()
        {
            Debug.LogError("ForceStartGame: " + Globals.gameState);
            if (Globals.gameState == Globals.GameState.InLobby)
            {
                StartCoroutine(StartRFM());
            }
        }

        private void AddLeaderboardEntry(string name, int amount)
        {
            var entry = Instantiate(leaderboardEntryPrefab, leaderboardEntryContainer);
            entry.Init(name, amount.ToString());
        }

        #endregion

        #region Private Methods

        [PunRPC]
        private void PlayerJoined()
        {
            CheckForGameStartCondition();
        }

        private void CheckForGameStartCondition()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= Globals.minNumberOfPlayer)
            {
                if (Globals.gameState != Globals.GameState.InLobby) return;
                StartCoroutine(StartRFM());
            }
            else
            {
                Globals.gameState = Globals.GameState.InLobby;
                countDownText.transform.parent.gameObject.SetActive(false);
                statusTMP.gameObject.SetActive(false);
                
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
            countDownText.transform.parent.gameObject.SetActive(true);
            statusTMP.text = "Countdown";
            statusTMP.gameObject.SetActive(true);
            
            huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Up");

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }

            NumOfActivePlayers = PhotonNetwork.PlayerList.Length;

            yield return StartCoroutine(Timer.SetDurationAndRunEnumerator(
                Globals.countDownTime,
                null, countDownText));
            

            if (PhotonNetwork.IsMasterClient)
            {
                var numberOfPlayers = PhotonNetwork.PlayerList.Length;
                int numberOfPlayerHunters = 0;
                
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    if (i < numberOfPlayers / 2)
                    {
                        numberOfPlayerHunters++;
                    }
                }

                // var numberOfEscapees = numberOfPlayers - numberOfHunters;
                numberOfPlayerHunters = 0; // delete this line to enable player hunters.

                Hashtable properties = new Hashtable { { "numberOfHunters", numberOfPlayerHunters } };
                PhotonNetwork.MasterClient.SetCustomProperties(properties);

                SpawnHunters(Globals.numOfAIHunters);
                
                photonView.RPC(nameof(ResetPosition), RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        private void CreateLeaderboardEntry(Dictionary<string, int> entry)
        {
            AddLeaderboardEntry(entry.ElementAt(0).Key, entry.ElementAt(0).Value);
        }

        // [PunRPC]
        // private void StartCountDownRPC()
        // {
        //     Globals.gameState = Globals.GameState.Countdown;
        //     countDownText.transform.parent.gameObject.SetActive(true);
        //     statusTMP.text = "Countdown";
        //     statusTMP.transform.parent.gameObject.SetActive(true);
        //     
        //     
        //     Timer.SetDurationAndRun(Globals.countDownTime, null, countDownText);
        // }

        [PunRPC]
        private void ResetPosition()
        {
            int numOfHunters = 0;
            
            if (PhotonNetwork.MasterClient.CustomProperties.TryGetValue("numberOfHunters", out var x))
            {
                numOfHunters = (int)x;
            }

            if (numOfHunters > 0)
            {
                Debug.LogError(PhotonNetwork.NickName + " Spawning as Hunter. RFM");
                
                statusTMP.text = "Catch the Escapees!";
                statusTMP.gameObject.SetActive(true);

                Globals.gameState = Globals.GameState.TakePosition;
                
                Timer.SetDurationAndRun(Globals.takePositionTime, AfterTakePositionTimerHunter, countDownText);

                var hunterPosition = huntersSpawnArea.position;
                var randomHunterPos = new Vector3(
                    hunterPosition.x + Random.Range(-3, 3),
                    hunterPosition.y,
                    hunterPosition.z + Random.Range(-3, 3));

                Globals.player.transform.SetPositionAndRotation(randomHunterPos, Quaternion.identity);

                Hashtable properties = new Hashtable { { "numberOfHunters", numOfHunters - 1 } };
                PhotonNetwork.MasterClient.SetCustomProperties(properties);
            }

            else // Spawning as Escapee
            {
                Debug.LogError(PhotonNetwork.NickName + " Spawning as Escapee. RFM");
                
                statusTMP.text = "Run far from the Hunters!";
                statusTMP.gameObject.SetActive(true);

                Globals.gameState = Globals.GameState.TakePosition;
            
            
                Timer.SetDurationAndRun(Globals.takePositionTime, AfterTakePositionTimer, countDownText);

                var position = playersSpawnArea.position;
                var randomPos = new Vector3(
                    position.x + Random.Range(-4, 5),
                    position.y,
                    position.z + Random.Range(-2, 3));

                Globals.player.transform.SetPositionAndRotation(randomPos, Quaternion.identity);
            }
        }
        
        private void AfterTakePositionTimerHunter()
        {
            Globals.player.gameObject.AddComponent<NPC>();
            Globals.IsLocalPlayerHunter = true;
            huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Down");
            countDownText.transform.parent.gameObject.SetActive(false);

            StartGameplay();
        }
        

        private void AfterTakePositionTimer()
        {
            Globals.IsLocalPlayerHunter = false;
            huntersCage.GetComponent<Animator>().Play("RFM Hunters Cage Door Down");
            countDownText.transform.parent.gameObject.SetActive(false);

            StartGameplay();
        }

        private void SpawnHunters(int numOfHunters)
        {
            Debug.LogError("numOfHunters: " + numOfHunters);
            for (int i = 0; i < numOfHunters; i++)
            {
                PhotonNetwork.InstantiateRoomObject(hunterPrefab.name,
                    huntersSpawnArea.position + new Vector3(Random.Range(-2, 2), 0, 
                        Random.Range(-2, 2)),
                    huntersSpawnArea.rotation);
            }
        }

        [PunRPC]
        private void StartGameplay()
        {
            EventsManager.StartGame();
            Globals.gameState = Globals.GameState.Gameplay;
            gameplayTimeText.transform.parent.gameObject.SetActive(true);
            countDownText.transform.parent.gameObject.SetActive(false);
            statusTMP.gameObject.SetActive(false);

            
            Timer.SetDurationAndRun(Globals.gameplayTime, GameplayTimeOver, gameplayTimeText);

            npcManager = gameObject.GetComponent<NPC_Manager>();
            if (npcManager)
            {
                npcManager.Init();
                return;
            }

            npcManager = gameObject.AddComponent<NPC_Manager>();
        }

        private async void GameplayTimeOver()
        {
            gameplayTimeText.transform.parent.gameObject.SetActive(false);
            EventsManager.GameTimeup();
            Globals.gameState = Globals.GameState.GameOver;
            statusTMP.text = "Time's Up!";
            statusTMP.gameObject.SetActive(false);
            // gameOverTMP.text = npcManager.TotalActivePlayers() > 0 ? "YOUR TEAM WON!" : "YOUR TEAM LOST!";
            // Calculate local player's score.
            gameOverPanel.SetActive(true);

            var dict = new Dictionary<string, int>() { { PhotonNetwork.LocalPlayer.NickName, missionsManager.Money } };
            photonView.RPC(nameof(CreateLeaderboardEntry), RpcTarget.All, dict);

            await Task.Delay(Globals.gameRestartWait); 
            
            gameOverPanel.SetActive(false);
            EventsManager.GameRestarting();
            
            // destroy all entries of leaderboard
            foreach (Transform entry in leaderboardEntryContainer)
            {
                Destroy(entry.gameObject);
            }
            //
            
            if (Globals.gameState == Globals.GameState.GameOver
                && PhotonNetwork.CurrentRoom.PlayerCount >= Globals.minNumberOfPlayer)
            {
                StartCoroutine(StartRFM());
            }
        }
        
        [PunRPC]
        public void LocalPlayerCaughtByHunter(int viewID)
        {
            if (Globals.player.GetComponentInChildren<PhotonView>().ViewID == viewID)
            {
                Debug.LogError("RFM LocalPlayerCaughtByHunter viewID = " + viewID);
                
                if (Globals.gameState != Globals.GameState.Gameplay) return;

                mainCam.SetActive(false);
                gameCanvas.SetActive(false);
                statusTMP.text = "You've been caught!";
                statusTMP.gameObject.SetActive(true);
                Globals.player.transform.root.gameObject.SetActive(false);

                var dict = new Dictionary<int, int>();
                dict.Add(0, PhotonNetwork.LocalPlayer.ActorNumber);
                photonView.RPC(nameof(DeactivateNPCPlayer), RpcTarget.Others, dict);

                npcCamera = Instantiate(npcCameraPrefab);
                npcCamera.Init(transform/*.CameraTarget*/);
            }
        }

        private void PlayerCaught(NPC catcher)
        {
            if (Globals.gameState != Globals.GameState.Gameplay) return;
        
            mainCam.SetActive(false);
            gameCanvas.SetActive(false);
            statusTMP.text = "You've been caught!";
            statusTMP.gameObject.SetActive(true);
            Globals.player.transform.root.gameObject.SetActive(false);
        
            var dict = new Dictionary<int, int>();
            dict.Add(0, PhotonNetwork.LocalPlayer.ActorNumber);
            photonView.RPC(nameof(DeactivateNPCPlayer), RpcTarget.Others, dict);
        
            if (npcCamera == null)
            {
                npcCamera = Instantiate(npcCameraPrefab);
            }
            npcCamera.Init(catcher.CameraTarget);
        }

        private void ActivatePlayer()
        {
            Globals.player.transform.root.gameObject.SetActive(true);
            mainCam.SetActive(true);
            gameCanvas.SetActive(true);

            var dict = new Dictionary<int, int>();
            dict.Add(0, PhotonNetwork.LocalPlayer.ActorNumber);
            photonView.RPC(nameof(ActivateNPCPlayer), RpcTarget.Others, dict);

            if (npcCamera != null)
            {
                Destroy(npcCamera.gameObject);
            }
        }

        [PunRPC]
        private void DeactivateNPCPlayer(Dictionary<int, int> dict)
        {
            if (npcManager == null) return;

            npcManager.DeactivatePlayer(dict[0]);
        }

        [PunRPC]
        private void ActivateNPCPlayer(Dictionary<int, int> dict)
        {
            if (npcManager == null) return;

            npcManager.ActivatePlayer(dict[0]);
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
            Debug.LogFormat("RFM OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        }

        #endregion
        
        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 200, 75), PhotonNetwork.IsMasterClient.ToString());
            GUI.Label(new Rect(10, 30, 200, 75), Globals.gameState.ToString());
        }
    }
}