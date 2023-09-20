using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using MoreMountains.Feedbacks;
using RFM.Character;

namespace RFM
{
    public class RFMMissionsManager : MonoBehaviour
    {
        private int _money = 0;
        public TextMeshProUGUI showMoney;
        public MMScaleShaker moneyScaleShaker;

        private Dictionary<string, int> _scores;

        public int Money { get { return _money; } }

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
            
            _money = 0;
            _scores = new Dictionary<string, int>();
            showMoney.gameObject.SetActive(true);

            InvokeRepeating(nameof(AddMoney), RFMManager.Instance.CurrentGameConfiguration.GainingMoneyTimeInterval,
                RFMManager.Instance.CurrentGameConfiguration.GainingMoneyTimeInterval);
        }

        private void RestartingGame()
        {
            showMoney.gameObject.SetActive(false);
        }

        private void OnPlayerCaught(NPCHunter catcher)
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

        private void SendMoneyToMaster (string playerName, int money)
        {
            Debug.LogError("RFM SendMoneyToMaster() " + playerName + ": " + money);
            
            //_scores.Add(playerName, money);
        }

        private void AddMoney()
        {
            _money += RFMManager.Instance.CurrentGameConfiguration.MoneyPerInterval;
            showMoney.text = _money.ToString("F0") + "";
            moneyScaleShaker.Play();
        }
    }
}