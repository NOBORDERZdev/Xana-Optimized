using Photon.Pun;
using System.Linq;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using RFM.Character;
using UnityEngine.SocialPlatforms.Impl;

namespace RFM.Managers
{
    public class RFMUIManager : MonoBehaviour
    {
        public static RFMUIManager Instance;
        
        [SerializeField] private GameObject helpPanel;
        [SerializeField] private GameObject instructionsPanelPanel;
        
        // Leaderboard
        [SerializeField] private RectTransform leaderboardEntryContainer;
        [SerializeField] private LeaderboardEntry leaderboardEntryPrefab;

        // HUD
        public TextMeshProUGUI showMoney;

        private GameObject _controlsCanvas; // 375

        private bool _wasInstructionsPanelActive;

        private Dictionary<string[], int> scores;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            
            _controlsCanvas = GameObject.FindGameObjectWithTag("NewCanvas");
            XanaConstants.xanaConstants.minimap = 0;
            ReferrencesForDynamicMuseum.instance.minimap.SetActive(false); // TODO temporary fix

            showMoney.text = "00";

            scores = new Dictionary<string[], int>();
        }

        private void OnEnable()
        {
            RFM.EventsManager.onToggleHelpPanel += ToggleHelpPanel;
            RFM.EventsManager.onCountdownStart += OnCountdownStart;
            RFM.EventsManager.onGameTimeup += OnGameOver;
            RFM.EventsManager.onShowScores += OnShowScores;
        }

        private void OnDisable()
        {
            RFM.EventsManager.onToggleHelpPanel -= ToggleHelpPanel;
            RFM.EventsManager.onCountdownStart -= OnCountdownStart;
            RFM.EventsManager.onGameTimeup -= OnGameOver;
            RFM.EventsManager.onShowScores -= OnShowScores;
        }
        
        private void OnCountdownStart()
        {
            instructionsPanelPanel.SetActive(false);
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
            }
        }
        
        private void OnGameOver()
        {
            showMoney.gameObject.SetActive(false);
        }

        public void EscapeeCaught(string nickName, int money, float timeSurvived, bool isNPC = false)
        {
            string[] array = { nickName, timeSurvived.ToString() };
            scores.Add(array, money);

            //if (isNPC) // only master should send the RPC if it's an NPC.
            //           // Otherwise, there will be multiple enteries for one NPC
            //{
            //    if (PhotonNetwork.IsMasterClient)
            //    {
            //        RFMManager.Instance.photonView.RPC(nameof(CreateLeaderboardEntry), RpcTarget.All,
            //            nickName, money, timeSurvived);
            //    }
            //}
            //else
            //{
            //    RFMManager.Instance.photonView.RPC(nameof(CreateLeaderboardEntry), RpcTarget.All,
            //    nickName, money, timeSurvived);
            //}
        }

        //int rank = 0;

        [PunRPC]
        private void CreateLeaderboardEntry(string nickName, int money, float timeSurvived)
        {
            //string[] array = { nickName , timeSurvived.ToString()};

            //scores.Add(array, money);
        }


        private void OnShowScores()
        {
            scores = scores.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            foreach (var score in scores)
            {
                var entry = Instantiate(leaderboardEntryPrefab, leaderboardEntryContainer);
                entry.Init(score.Key[0], score.Value, score.Key[1], 1 + scores.Keys.ToList().IndexOf(score.Key));
            }

            //var npcEscapees = FindObjectsOfType<NPCEscapee>();
            //var playerEscapees = FindObjectsOfType<PlayerEscapee>();

            //npcEscapees = npcEscapees.OrderByDescending(x => x.money).ToArray();
            //playerEscapees = playerEscapees.OrderByDescending(x => x.Money).ToArray();

            //Debug.LogError("RFM npcEscapees: " + npcEscapees.Length);
            //Debug.LogError("RFM playerEscapees: " + playerEscapees.Length);

            //for (int i = 0; i < playerEscapees.Length; i++)
            //{
            //    PlayerEscapee playerEscapee = playerEscapees[i];
            //    var entry = Instantiate(leaderboardEntryPrefab, leaderboardEntryContainer);
            //    entry.Init(playerEscapee.nickName, playerEscapee.Money, playerEscapee.timeSurvived, i);
            //}

            //for (int i = 0; i < npcEscapees.Length; i++)
            //{
            //    NPCEscapee npcEscapee = npcEscapees[i];
            //    var entry = Instantiate(leaderboardEntryPrefab, leaderboardEntryContainer);
            //    entry.Init(npcEscapee.nickName, npcEscapee.money, npcEscapee.timeSurvived, i);
            //}
        }
    }
}
