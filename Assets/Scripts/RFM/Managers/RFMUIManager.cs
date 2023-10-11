using Photon.Pun;
using RFM.Character;
using TMPro;
using UnityEngine;

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

        private void Awake()
        {
            if (Instance == null) Instance = this;
            
            _controlsCanvas = GameObject.FindGameObjectWithTag("NewCanvas");
            XanaConstants.xanaConstants.minimap = 0;
            ReferrencesForDynamicMuseum.instance.minimap.SetActive(false); // TODO temporary fix

            showMoney.text = "00";
        }

        private void OnEnable()
        {
            RFM.EventsManager.onToggleHelpPanel += ToggleHelpPanel;
            RFM.EventsManager.onCountdownStart += OnCountdownStart;
            RFM.EventsManager.onGameTimeup += OnGameOver;
        }

        private void OnDisable()
        {
            RFM.EventsManager.onToggleHelpPanel -= ToggleHelpPanel;
            RFM.EventsManager.onCountdownStart -= OnCountdownStart;
            RFM.EventsManager.onGameTimeup -= OnGameOver;
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
            // RFMManager.Instance.photonView.RPC(nameof(CreateLeaderboardEntry), RpcTarget.All, 
            //     PhotonNetwork.LocalPlayer.NickName,
            //     Globals.player.gameObject.GetComponent<RFM.Character.PlayerEscapee>().Money);
        }

        public void EscapeeCaught(string nickName, int money)
        {
            RFMManager.Instance.photonView.RPC(nameof(CreateLeaderboardEntry), RpcTarget.All, 
                nickName, money);
        }
        
        [PunRPC]
        private void CreateLeaderboardEntry(string nickName, int money)
        {
            var entry = Instantiate(leaderboardEntryPrefab, leaderboardEntryContainer);
            entry.Init(nickName, money.ToString());
        }
    }
}
