using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace RFM.Character
{
    public class PlayerRunner : Runner
    {
        private TMPro.TextMeshProUGUI _showMoney;
        private MoreMountains.Feedbacks.MMScaleShaker _moneyScaleShaker;

        public GameObject playerBody;
        //[HideInInspector] public int Money = 0;
        private bool gainingMoney = false;
        private float timeElapsed = 0f;

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

        private void Update()
        {
            if (!this.enabled) return;
            if (gainingMoney)
            {
                if (timeElapsed >= RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval)
                {
                    timeElapsed = 0f;
                    AddMoney();
                }
                else
                {
                    timeElapsed += Time.deltaTime;
                }
            }
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
            //Money = 0;
            money = 0;
            _showMoney.gameObject.SetActive(true);

            gainingMoney = true;
            //InvokeRepeating(nameof(AddMoney),
            //    RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval,
            //    RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval);
        }

        private void OnGameOver()
        {
            if (!this.enabled) return;
            gainingMoney = false;
            //CancelInvoke(nameof(AddMoney));

            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "money", money } });
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "timeSurvived", timeSurvived } });

            PhotonNetwork.Destroy(transform.root.gameObject);
        }

        private void AddMoney()
        {
            money += RFM.Managers.RFMManager.CurrentGameConfiguration.MoneyPerInterval;
            timeSurvived += RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval;
            _showMoney.text = money.ToString("F0") + "";
            _moneyScaleShaker.Play();
        }

        
        public void PlayerRunnerCaught(int hunterViewID)
        {
            Debug.LogError("RFM I was caught by " + hunterViewID);
            gainingMoney = false;
            //CancelInvoke(nameof(AddMoney));
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "money", money } });
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "timeSurvived", timeSurvived } });

            // Find the hunter that caught this runner and increment their reward multiplier.
            var hunterPV = PhotonView.Find(hunterViewID);
            if (hunterPV != null)
            {
                if (hunterPV.TryGetComponent(out NPCHunter hunter))
                {
                    var oldValue = 0;
                    /* if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(hunter.nickName + "rewardMultiplier"))
                     {
                         oldValue = (int)PhotonNetwork.CurrentRoom.CustomProperties[hunter.nickName + "rewardMultiplier"];
                     }
                     else
                     {
                         PhotonNetwork.CurrentRoom.SetCustomProperties(
                             new ExitGames.Client.Photon.Hashtable { { hunter.nickName + "rewardMultiplier", 0 } });
                     }*/

                    oldValue = hunter.RewardMultiplier;

                    hunter.RewardMultiplier = oldValue + 1;

                    PhotonNetwork.CurrentRoom.SetCustomProperties(
                        new ExitGames.Client.Photon.Hashtable { { hunter.nickName + "rewardMultiplier", oldValue + 1 } }, // to be set
                        new ExitGames.Client.Photon.Hashtable { { hunter.nickName + "rewardMultiplier", oldValue } } // expected value
                        );
                }
                if (hunterPV.TryGetComponent(out PlayerHunter _playerHunter))
                {
                    var oldValue = 0;
                    /*if (hunterPV.Owner.CustomProperties.ContainsKey("rewardMultiplier"))
                    {
                        oldValue = (int)hunterPV.Owner.CustomProperties["rewardMultiplier"];
                    }
                    else
                    {
                        hunterPV.Owner.SetCustomProperties(
                            new ExitGames.Client.Photon.Hashtable { { "rewardMultiplier", 0 } });
                    }*/

                    oldValue = _playerHunter.RewardMultiplier;

                    _playerHunter.RewardMultiplier = oldValue + 1;

                    hunterPV.Owner.SetCustomProperties(
                        new ExitGames.Client.Photon.Hashtable { {"rewardMultiplier", oldValue + 1 } }, // to be set
                        new ExitGames.Client.Photon.Hashtable { {"rewardMultiplier", oldValue } } // expected value
                        );
                }
            }

            PhotonNetwork.Destroy(transform.root.gameObject);
            RFM.Managers.RFMManager.Instance.PlayerCaught(hunterViewID);
        }
        

        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(money);
                stream.SendNext(timeSurvived);
            }
            else
            {
                money = (int)stream.ReceiveNext();
                timeSurvived = (float)stream.ReceiveNext();
            }
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
