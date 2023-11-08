using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace RFM.Character
{
    public class PlayerRunner : MonoBehaviour, IPunObservable
    {
        private TMPro.TextMeshProUGUI _showMoney;
        private MoreMountains.Feedbacks.MMScaleShaker _moneyScaleShaker;

        public string nickName;
        public float timeSurvived;
        public int Money = 0;
        
        private void OnEnable()
        {
            EventsManager.onGameStart += OnGameStarted;
            EventsManager.onGameTimeup += OnGameOver;
        }
        
        private void OnDisable()
        {
            EventsManager.onGameStart -= OnGameStarted;
            EventsManager.onGameTimeup -= OnGameOver;
        }

        private void OnGameStarted()
        {
            nickName = PhotonNetwork.LocalPlayer.NickName;
            if (Globals.IsLocalPlayerHunter) return;
            
            _showMoney = RFM.Managers.RFMUIManager.Instance.showMoney;
            _moneyScaleShaker = _showMoney.gameObject.GetComponent<MoreMountains.Feedbacks.MMScaleShaker>();
            _showMoney.text = "00";
            Money = 0;
            _showMoney.gameObject.SetActive(true);
            StartCoroutine(TimeSurvived());
            StartCoroutine(AddMoney());
            //InvokeRepeating(nameof(AddMoney),
            //    RFM.Managers.RFMManager/*.Instance*/.CurrentGameConfiguration.GainingMoneyTimeInterval,
            //    RFM.Managers.RFMManager/*.Instance*/.CurrentGameConfiguration.GainingMoneyTimeInterval);
        }

         
        private IEnumerator TimeSurvived() 
        {
            timeSurvived = 0;
            while(true) 
            {
                timeSurvived += 1;
                yield return new WaitForSecondsRealtime(1);
            }
        }

        private void OnGameOver()
        {
            //CancelInvoke(nameof(AddMoney));
            StopCoroutine(TimeSurvived());
            StopCoroutine(AddMoney());


            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "money", Money } });
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "timeSurvived", timeSurvived } });

            RFM.Managers.RFMUIManager.Instance.RunnerCaught(PhotonNetwork.LocalPlayer.NickName, Money, timeSurvived);
            PhotonNetwork.Destroy(transform.root.gameObject);
        }

        private IEnumerator AddMoney()
        {
            while(true)
            {
                yield return new WaitForSecondsRealtime(
                    RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval);
                
                Money += RFM.Managers.RFMManager.CurrentGameConfiguration.MoneyPerInterval;
                _showMoney.text = Money.ToString("F0") + "";
                _moneyScaleShaker.Play();
            }
        }

        public void PlayerRunnerCaught(NPCHunter npcHunter)
        {
            StopCoroutine(TimeSurvived());
            StopCoroutine(AddMoney()); PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "money", Money } });
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "timeSurvived", timeSurvived } });

            RFM.Managers.RFMUIManager.Instance.RunnerCaught(PhotonNetwork.LocalPlayer.NickName, Money, timeSurvived);
            PhotonNetwork.Destroy(transform.root.gameObject);
            EventsManager.PlayerCaught(npcHunter);
        }
        
        public void PlayerRunnerCaughtByPlayer(PlayerHunter npcHunter)
        {
            StopCoroutine(TimeSurvived());
            StopCoroutine(AddMoney());
            RFM.Managers.RFMUIManager.Instance.RunnerCaught(PhotonNetwork.LocalPlayer.NickName, Money, timeSurvived);
            PhotonNetwork.Destroy(transform.root.gameObject);
            EventsManager.PlayerCaughtByPlayer(npcHunter);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(Money);
                stream.SendNext(timeSurvived);
            }
            else if (stream.IsReading)
            {
                this.Money = (int)stream.ReceiveNext();
                this.timeSurvived = (float)stream.ReceiveNext();
            }
        }
    }
}
