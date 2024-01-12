using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace RFM.Character
{
    public class PlayerRunner : Runner/*, IPunObservable*/
    {
        private TMPro.TextMeshProUGUI _showMoney;
        private MoreMountains.Feedbacks.MMScaleShaker _moneyScaleShaker;

        //[HideInInspector] public string nickName;
        [HideInInspector] public float timeSurvived;
        [HideInInspector] public int Money = 0;

        private int viewIDOfHunterThatCaughtThisRunner = -999;
        
        private void OnEnable()
        {
            EventsManager.onGameStart += OnGameStarted;
            EventsManager.onGameTimeup += OnGameOver;

            PhotonNetwork.NetworkingClient.EventReceived += ReceivePhotonEvents;
        }
        
        private void OnDisable()
        {
            EventsManager.onGameStart -= OnGameStarted;
            EventsManager.onGameTimeup -= OnGameOver;

            PhotonNetwork.NetworkingClient.EventReceived -= ReceivePhotonEvents;
        }

        internal override void OnGameStarted()
        {
            if (!this.enabled) return;
            base.OnGameStarted();

            nickName = PhotonNetwork.LocalPlayer.NickName;
            if (Globals.IsLocalPlayerHunter) return;
            
            _showMoney = RFM.Managers.RFMUIManager.Instance.showMoney;
            _moneyScaleShaker = _showMoney.gameObject.GetComponent<MoreMountains.Feedbacks.MMScaleShaker>();
            _showMoney.text = "00";
            Money = 0;
            _showMoney.gameObject.SetActive(true);
            //StartCoroutine(TimeSurvived());
            //StartCoroutine(AddMoney());
            InvokeRepeating(nameof(AddMoney),
                RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval,
                RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval);
        }

         
        //private IEnumerator TimeSurvived() 
        //{
        //    timeSurvived = 0;
        //    while(true) 
        //    {
        //        timeSurvived += 1;
        //        yield return new WaitForSecondsRealtime(1);
        //    }
        //}

        private void OnGameOver()
        {
            if (!this.enabled) return;
            CancelInvoke(nameof(AddMoney));
            //StopCoroutine(TimeSurvived());
            //StopCoroutine(AddMoney());


            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "money", Money } });
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "timeSurvived", timeSurvived } });

            // RFM.Managers.RFMUIManager.Instance.RunnerCaught(PhotonNetwork.LocalPlayer.NickName, Money, timeSurvived);
            PhotonNetwork.Destroy(transform.root.gameObject);
        }

        private void AddMoney()
        {
            Money += RFM.Managers.RFMManager.CurrentGameConfiguration.MoneyPerInterval;
            timeSurvived += RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval;
            _showMoney.text = Money.ToString("F0") + "";
            _moneyScaleShaker.Play();
        }

        //private IEnumerator AddMoney()
        //{
        //    while(true)
        //    {
        //        yield return new WaitForSecondsRealtime(
        //            RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval);
                
        //        Money += RFM.Managers.RFMManager.CurrentGameConfiguration.MoneyPerInterval;
        //        _showMoney.text = Money.ToString("F0") + "";
        //        _moneyScaleShaker.Play();
        //    }
        //}

        
        public void PlayerRunnerCaught(int hunterViewID)
        {
            CancelInvoke(nameof(AddMoney));
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "money", Money } });
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "timeSurvived", timeSurvived } });

            // Find the hunter that caught this runner and increment their reward multiplier.
            var hunterPV = PhotonView.Find(hunterViewID);
            if (hunterPV != null)
            {
                if (hunterPV.TryGetComponent(out NPCHunter hunter))
                {

                    var oldValue = 0;
                    if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(hunter.nickName + "rewardMultiplier"))
                    {
                        oldValue = (int)PhotonNetwork.CurrentRoom.CustomProperties[hunter.nickName + "rewardMultiplier"];
                    }
                    else
                    {
                        PhotonNetwork.CurrentRoom.SetCustomProperties(
                            new ExitGames.Client.Photon.Hashtable { { hunter.nickName + "rewardMultiplier", 0 } });
                    }

                    PhotonNetwork.CurrentRoom.SetCustomProperties(
                        new ExitGames.Client.Photon.Hashtable { { hunter.nickName + "rewardMultiplier", oldValue + 1 } }, // to be set
                        new ExitGames.Client.Photon.Hashtable { { hunter.nickName + "rewardMultiplier", oldValue } } // expected value
                        );

                    ////hunter.rewardMultiplier++;
                    //if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(hunter.nickName + "rewardMultiplier"))
                    //{
                    //    PhotonNetwork.CurrentRoom.CustomProperties[hunter.nickName + "rewardMultiplier"] =
                    //        (int)PhotonNetwork.CurrentRoom.CustomProperties[hunter.nickName + "rewardMultiplier"] + 1;
                    //}
                    //else
                    //{
                    //    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                    //    { { hunter.nickName + "rewardMultiplier", 1 } });
                    //}
                }
                if (hunterPV.TryGetComponent(out PlayerHunter _))
                {
                    var oldValue = 0;
                    if (hunterPV.Owner.CustomProperties.ContainsKey("rewardMultiplier"))
                    {
                        oldValue = (int)hunterPV.Owner.CustomProperties["rewardMultiplier"];
                    }
                    else
                    {
                        hunterPV.Owner.SetCustomProperties(
                            new ExitGames.Client.Photon.Hashtable { { "rewardMultiplier", 0 } });
                    }

                    hunterPV.Owner.SetCustomProperties(
                        new ExitGames.Client.Photon.Hashtable { {"rewardMultiplier", oldValue + 1 } }, // to be set
                        new ExitGames.Client.Photon.Hashtable { {"rewardMultiplier", oldValue } } // expected value
                        );

                    //if (hunterPV.Owner.CustomProperties.ContainsKey("rewardMultiplier"))
                    //{
                    //    hunterPV.Owner.CustomProperties["rewardMultiplier"] =
                    //        (int)hunterPV.Owner.CustomProperties["rewardMultiplier"] + 1;
                    //}
                    //else
                    //{
                    //    hunterPV.Owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                    //    { { "rewardMultiplier", 1 } });
                    //}
                }
            }

            PhotonNetwork.Destroy(transform.root.gameObject);
            RFM.Managers.RFMManager.Instance.PlayerCaught(hunterViewID);
        }
        

        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //if (!this.enabled) return;
            //if (stream.IsWriting)
            //{
            //    stream.SendNext(Money);
            //    stream.SendNext(timeSurvived);
            //}
            //else if (stream.IsReading)
            //{
            //    this.Money = (int)stream.ReceiveNext();
            //    this.timeSurvived = (float)stream.ReceiveNext();
            //}
        }

        private void ReceivePhotonEvents(EventData photonEvent)
        {
            //if (!this.enabled) return;
            switch (photonEvent.Code)
            {
                case PhotonEventCodes.PlayerRunnerCaught:
                    {
                        int runnerViewID = (int)((object[])photonEvent.CustomData)[0];
                        int hunterViewID = (int)((object[])photonEvent.CustomData)[1];


                        if (runnerViewID == RFM.Globals.player.GetComponent<PhotonView>().ViewID)
                        {
                            if (viewIDOfHunterThatCaughtThisRunner == -999) // if this runner has not been caught yet
                            {
                                viewIDOfHunterThatCaughtThisRunner = hunterViewID;
                                PlayerRunnerCaught(hunterViewID);
                            }
                        }
                        break;
                    }
            }
        }
    }
}
