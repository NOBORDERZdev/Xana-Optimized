using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

namespace RFM.Managers
{
    public class RFMMissionsManager : MonoBehaviour
    {
        public TextMeshProUGUI showMoney;
        public MMScaleShaker moneyScaleShaker;

        private Dictionary<string, int> _scores;

        public int Money { get; private set; } = 0;

        private void OnEnable()
        {
            EventsManager.onGameStart += OnGameStarted;
            EventsManager.onGameTimeup += OnGameEnded;
            EventsManager.onPlayerCaught += OnPlayerCaught;
            EventsManager.onRestarting += RestartingGame;
        }
        
        private void OnDisable()
        {
            EventsManager.onGameStart -= OnGameStarted;
            EventsManager.onGameTimeup -= OnGameEnded;
            EventsManager.onPlayerCaught -= OnPlayerCaught;
            EventsManager.onRestarting -= RestartingGame;
        }

        private void Start()
        {
            showMoney.text = "00";
            showMoney.gameObject.SetActive(false);
        }

        private void OnGameStarted()
        {
            if (Globals.IsLocalPlayerHunter) return;
            
            Money = 0;
            _scores = new Dictionary<string, int>();
            showMoney.gameObject.SetActive(true);

            InvokeRepeating(nameof(AddMoney), RFMManager.Instance.CurrentGameConfiguration.GainingMoneyTimeInterval,
                RFMManager.Instance.CurrentGameConfiguration.GainingMoneyTimeInterval);
        }

        private void RestartingGame()
        {
            showMoney.gameObject.SetActive(false);
        }

        private void OnPlayerCaught(RFM.Character.NPCHunter catcher)
        {
            OnGameEnded();
        }

        private void OnGameEnded()
        {
            //RPCCalls.Instance.CallRPC(SendMoneyToMaster, RpcTarget.MasterClient, 
            //    PhotonNetwork.NickName, _money);
            // send scores to other players
            
            CancelInvoke(nameof(AddMoney));
        }

        // private void SendMoneyToMaster (string playerName, int money)
        // {
        //     Debug.LogError("RFM SendMoneyToMaster() " + playerName + ": " + money);
        //     
        //     //_scores.Add(playerName, money);
        // }

        private void AddMoney()
        {
            Money += RFMManager.Instance.CurrentGameConfiguration.MoneyPerInterval;
            showMoney.text = Money.ToString("F0") + "";
            moneyScaleShaker.Play();
        }
    }
}