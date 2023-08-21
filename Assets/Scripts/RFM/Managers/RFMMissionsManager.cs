using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

namespace RFM
{
    public class RFMMissionsManager : MonoBehaviour
    {
        private int _money = 0;
        public TextMeshProUGUI showMoney;

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
            showMoney.text = "00 XENY";
            showMoney.gameObject.SetActive(false);
        }

        private void OnGameStarted()
        {
            if (Globals.IsLocalPlayerHunter) return;
            
            _money = 0;
            _scores = new Dictionary<string, int>();
            showMoney.gameObject.SetActive(true);
            
            InvokeRepeating(nameof(AddMoney), Globals.GainingMoneyTimeInterval, Globals.GainingMoneyTimeInterval);
        }

        private void RestartingGame()
        {
            showMoney.gameObject.SetActive(false);
        }

        private void OnPlayerCaught(NPC catcher)
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
            _money += Globals.MoneyPerInterval;
            showMoney.text = _money.ToString("F0") + " XENY";
        }
    }
}