using Photon.Pun;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

namespace RFM
{
    public class RFMMissionsManager : MonoBehaviour
    {
        public TextMeshProUGUI showMoney;

        // private Dictionary<string, int> _scores;

        [SerializeField] private Transform pickupCardsParent;
        [SerializeField] private Transform[] pickupCardsPossibleLocations;

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
            showMoney.text = "00 XENY";
            showMoney.gameObject.SetActive(false);
        }

        private void OnGameStarted()
        {
            if (Globals.IsLocalPlayerHunter) return;
            
            Money = 0;
            // _scores = new Dictionary<string, int>();
            showMoney.gameObject.SetActive(true);
            
            InvokeRepeating(nameof(AddMoney), Globals.GainingMoneyTimeInterval, Globals.GainingMoneyTimeInterval);
            
            Invoke(nameof(SpawnPickupCards), 5);
        }

        private void SpawnPickupCards()
        {
            foreach (var location in pickupCardsPossibleLocations)
            {
                var card = PhotonNetwork.InstantiateRoomObject("RFMCard", 
                    location.position, location.rotation);
                card.transform.SetParent(pickupCardsParent);
                card.GetComponent<RFMCard>().cardType = (RFMCard.CardType)Random.Range(0,2);
            }
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

        // private void SendMoneyToMaster (string playerName, int money)
        // {
        //     Debug.LogError("RFM SendMoneyToMaster() " + playerName + ": " + money);
        //     
        //     //_scores.Add(playerName, money);
        // }

        private void AddMoney()
        {
            Money += Globals.MoneyPerInterval;
            showMoney.text = Money.ToString("F0") + " XENY";
        }
    }
}