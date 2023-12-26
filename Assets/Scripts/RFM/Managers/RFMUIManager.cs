using Photon.Pun;
using System.Linq;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using RFM.Character;
using UnityEngine.SocialPlatforms.Impl;
using Photon.Realtime;

namespace RFM.Managers
{
    public class RFMUIManager : MonoBehaviour
    {
        public static RFMUIManager Instance;
        
        [SerializeField] private GameObject helpPanel;
        [SerializeField] private GameObject instructionsPanelPanel;
        [SerializeField] private GameObject setLayoutPanel;
        [SerializeField] private GameObject rearViewMirror;
        // Leaderboard
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private RectTransform runnersLeaderboardEntryContainer;
        [SerializeField] private RectTransform huntersLeaderboardEntryContainer;
        [SerializeField] private LeaderboardEntry leaderboardEntryPrefab;

        [Header("XanaStones")]
        [SerializeField] private TextMeshProUGUI playerXanaStones;
        //[SerializeField] private TextMeshProUGUI playerEarnedXanaStones;
        [SerializeField] private GameObject xanaStonePopup;
        [SerializeField] private GameObject notEnoughXanaStonesPopup;

        // HUD
        public TextMeshProUGUI showMoney;

        private GameObject _controlsCanvas; // 375
        private GameObject _mainCanvas;

        private bool _wasInstructionsPanelActive;

        private Dictionary<string[], int> runnersScores;
        private Dictionary<string[], int> huntersScores;

        [SerializeField] private GameObject restartButton;


        private void Awake()
        {
            if (Instance == null) Instance = this;
            
            _controlsCanvas = GameObject.FindGameObjectWithTag("NewCanvas");
            _mainCanvas = GameObject.FindGameObjectWithTag(Globals.CANVAS_TAG);
            XanaConstants.xanaConstants.minimap = 0;
            ReferrencesForDynamicMuseum.instance.minimap.SetActive(false); // TODO temporary fix

            showMoney.text = "000";

            runnersScores = new Dictionary<string[], int>();
            huntersScores = new Dictionary<string[], int>();

            gameOverPanel.SetActive(false);
        }

        private void OnEnable()
        {
            RFM.EventsManager.onToggleHelpPanel += ToggleHelpPanel;
            RFM.EventsManager.onCountdownStart += OnCountdownStart;
            RFM.EventsManager.onGameTimeup += OnGameOver;
            RFM.EventsManager.onShowScores += OnShowScores;
            RFM.EventsManager.onTakePositionTimeStart += OnTakePosition;
            RFM.EventsManager.onShowRearViewMirror += OnShowRearViewMirror;
            // CanvasButtonsHandler.inst.setControlsLayoutBtnRFM.onClick.AddListener(ToggleLayoutPanel);
        }

        private void OnDisable()
        {
            RFM.EventsManager.onToggleHelpPanel -= ToggleHelpPanel;
            RFM.EventsManager.onCountdownStart -= OnCountdownStart;
            RFM.EventsManager.onGameTimeup -= OnGameOver;
            RFM.EventsManager.onShowScores -= OnShowScores;
            RFM.EventsManager.onShowRearViewMirror -= OnShowRearViewMirror;
            // CanvasButtonsHandler.inst.setControlsLayoutBtnRFM.onClick.RemoveAllListeners();
        }
        
        private void OnCountdownStart()
        {
            instructionsPanelPanel.SetActive(false);
        }

        private void OnTakePosition() 
        {
            //leave empty for now
        }

        private void OnShowRearViewMirror(bool active)
        {
            rearViewMirror.SetActive(active);
        }

        public void ToggleLayoutPanel() 
        {
            if (setLayoutPanel.activeInHierarchy)
            {
                setLayoutPanel.SetActive(false);

            }
            else
            {
                setLayoutPanel.SetActive(true);

            }
        }

        public void ToggleHelpPanel()
        {
            if (helpPanel.activeInHierarchy)
            {
                _controlsCanvas.SetActive(true);
                helpPanel.SetActive(false);

                if (_wasInstructionsPanelActive)
                {
                    instructionsPanelPanel.SetActive(true);
                    _wasInstructionsPanelActive = false;
                }
            }
            else
            {
                _controlsCanvas.SetActive(false);
                helpPanel.SetActive(true);
                if (instructionsPanelPanel.activeInHierarchy)
                {
                    instructionsPanelPanel.SetActive(false);
                    _wasInstructionsPanelActive = true;
                }
            }
        }

        public void HomeButtonClicked()
        {
            var sceneManage = FindObjectOfType<SceneManage>();

            if (sceneManage)
            {
                sceneManage.ReturnToHome(true);
                if (AddressableDownloader.RemoveAddresablesAction != null)
                {
                    AddressableDownloader.RemoveAddresablesAction?.Invoke();
                }
            }
        }
        
        private void OnGameOver()
        {
            showMoney.transform.parent.gameObject.SetActive(false);
            rearViewMirror.SetActive(false);
            gameOverPanel.SetActive(true);

            _mainCanvas.SetActive(false);
            CanvasButtonsHandler.inst.ShowRFMButtons(false);
            int earnedMoney = 0;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("money", out object _money))
            {
                earnedMoney = (int)_money * RFM.Globals.xanaStoneFee;
            }

            //playerEarnedXanaStones.text = $"You have earned <color=purple>${earnedMoney}</color> XanaStones";

            if (PhotonNetwork.IsMasterClient)
            {
                restartButton.SetActive(true);
            }
        }

        public void RestartButtonClicked()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            GetComponent<PhotonView>().RPC(nameof(RestartRFM), RpcTarget.AllBuffered);
            
        }

        // Enable the restart button 

        [PunRPC]
        private void RestartRFM()
        {
            Awake();
            // Destroy all children of leaderboardEntryContainer
            foreach (Transform child in runnersLeaderboardEntryContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in huntersLeaderboardEntryContainer)
            {
                Destroy(child.gameObject);
            }
            //gameOverPanel.gameObject.SetActive(false);

            RFM.Managers.RFMManager.Instance.RestartRFM();
        }

        public void RunnerCaught(string nickName, int money, float timeSurvived)
        {
            string[] array = { nickName, timeSurvived.ToString() };
            runnersScores.Add(array, money);
        }


        private void OnShowScores()
        {
            //foreach (var runner in FindObjectsOfType<RFM.Character.NPCRunner>())
            //{
            //    // add name, time survived, money of each runner to the scores dictionary
            //    string[] array = { runner.nickName, runner.timeSurvived.ToString() };
            //    scores.Add(array, runner.money);
            //}

            foreach (var npcHunter in FindObjectsOfType<NPCHunter>())
            {
                string[] array = { "Hunter" + " [H]", 0.ToString() };
                huntersScores.Add(array, npcHunter.rewardMultiplier * 100); // TODO : change 100 to the participation amount
            }

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue("isHunter", out object _isHunter))
                {
                    if ((bool)_isHunter) // player was a hunter
                    {
                        int rewardMultiplier = 0;
                        if (player.CustomProperties.TryGetValue("rewardMultiplier", out object _rewardMultiplier))
                        {
                            rewardMultiplier = (int)_rewardMultiplier;
                        }

                        string[] array = { player.NickName + " [H]", 0.ToString() };
                        huntersScores.Add(array, rewardMultiplier * 100); // TODO : change 100 to the participation amount
                    }
                    else // player was a runner
                    {
                        int money = 0;
                        float timeSurvived = 0;
                        if (player.CustomProperties.TryGetValue("money", out object _money))
                        {
                            money = (int)_money;
                        }
                        if (player.CustomProperties.TryGetValue("timeSurvived", out object _timeSurvived))
                        {
                            timeSurvived = (float)_timeSurvived;
                        }

                        string[] array = { player.NickName, timeSurvived.ToString() };
                        runnersScores.Add(array, money);
                    }
                }
            }


            runnersScores = runnersScores.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            huntersScores = huntersScores.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            foreach (var score in runnersScores)
            {
                var entry = Instantiate(leaderboardEntryPrefab, runnersLeaderboardEntryContainer);
                entry.Init(score.Key[0], score.Value, score.Key[1], 1 + runnersScores.Keys.ToList().IndexOf(score.Key));
            }

            foreach (var score in huntersScores)
            {
                var entry = Instantiate(leaderboardEntryPrefab, huntersLeaderboardEntryContainer);
                entry.Init(score.Key[0], score.Value, score.Key[1], 1 + huntersScores.Keys.ToList().IndexOf(score.Key));
            }
        }

        public void ShowXanaStonePopup()
        {
            xanaStonePopup.SetActive(true);
            playerXanaStones.text = $"XanaStones: ${RFMEconomyManager.money}";
        }

        public void Button_PlayWithXanaStones(int amount)
        {
            if (RFMEconomyManager.PayToPlayRFM(amount))
            {
                RFMManager.Instance.StartMatchMaking();
                xanaStonePopup.SetActive(false);
                RFM.Globals.xanaStoneFee = amount;
            }
            else
            {
                notEnoughXanaStonesPopup.SetActive(true);
            }
        }

        public void Button_AddXanaStones() // For testing
        {
            RFMEconomyManager.money += 100;
            playerXanaStones.text = $"XanaStones: ${RFMEconomyManager.money}";
        }
    }
}
