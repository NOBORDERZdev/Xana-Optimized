using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace RFM.Character
{
    public class PlayerEscapee : MonoBehaviour
    {
        private TMPro.TextMeshProUGUI _showMoney;
        private MoreMountains.Feedbacks.MMScaleShaker _moneyScaleShaker;

        public float timeSurvived;
        public int Money { get; private set; } = 0;
        
        private void OnEnable()
        {
            EventsManager.onGameStart += OnGameStarted;
            EventsManager.onGameTimeup += OnGameOver;
            // EventsManager.onPlayerCaught += OnPlayerCaught;
        }
        
        private void OnDisable()
        {
            EventsManager.onGameStart -= OnGameStarted;
            EventsManager.onGameTimeup -= OnGameOver;
            // EventsManager.onPlayerCaught -= OnPlayerCaught;
        }

        private void OnGameStarted()
        {
            if (Globals.IsLocalPlayerHunter) return;
            
            _showMoney = RFM.Managers.RFMUIManager.Instance.showMoney;
            _moneyScaleShaker = _showMoney.gameObject.GetComponent<MoreMountains.Feedbacks.MMScaleShaker>();
            _showMoney.text = "00";
            Money = 0;
            _showMoney.gameObject.SetActive(true);
            StartCoroutine(TimeSurvived());
            InvokeRepeating(nameof(AddMoney),
                RFM.Managers.RFMManager/*.Instance*/.CurrentGameConfiguration.GainingMoneyTimeInterval,
                RFM.Managers.RFMManager/*.Instance*/.CurrentGameConfiguration.GainingMoneyTimeInterval);
        }

         
        IEnumerator TimeSurvived() 
        {
            timeSurvived = 0;
            while(true) 
            {
                timeSurvived += 1;
                yield return new WaitForSecondsRealtime(1);
            }

        }

        // private void OnPlayerCaught(NPCHunter catcher)
        // {
        //     OnGameOver();
        // }

        private void OnGameOver()
        {
            // _showMoney.gameObject.SetActive(false);
            CancelInvoke(nameof(AddMoney));
            StopCoroutine(TimeSurvived());
            RFM.Managers.RFMUIManager.Instance.EscapeeCaught(PhotonNetwork.LocalPlayer.NickName, Money, timeSurvived);
            PhotonNetwork.Destroy(transform.root.gameObject);
        }

        private void AddMoney()
        {
            Money += RFM.Managers.RFMManager/*.Instance*/.CurrentGameConfiguration.MoneyPerInterval;
            _showMoney.text = Money.ToString("F0") + "";
            _moneyScaleShaker.Play();
        }

        public void PlayerEscapeeCaught(NPCHunter npcHunter)
        {
            CancelInvoke(nameof(AddMoney));
            RFM.Managers.RFMUIManager.Instance.EscapeeCaught(PhotonNetwork.LocalPlayer.NickName, Money, timeSurvived);
            PhotonNetwork.Destroy(transform.root.gameObject);
            EventsManager.PlayerCaught(npcHunter);
        }
        
        public void PlayerEscapeeCaughtByPlayer(PlayerHunter npcHunter)
        {
            CancelInvoke(nameof(AddMoney));
            RFM.Managers.RFMUIManager.Instance.EscapeeCaught(PhotonNetwork.LocalPlayer.NickName, Money, timeSurvived);
            PhotonNetwork.Destroy(transform.root.gameObject);
            EventsManager.PlayerCaughtByPlayer(npcHunter);
        }
    }
}
