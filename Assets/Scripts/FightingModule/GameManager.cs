using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System;
using ExitGames.Client.Photon;

namespace FightingModule
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Vars
        [SerializeField]
        GameObject playerPrefab;
        [SerializeField]
        GameObject playerPrefab2;
        [SerializeField]
        Vector3 player1Pos, player2Pos;
        [SerializeField]
        Quaternion player1Rot, player2Rot;
        [SerializeField]
        TextMeshProUGUI[] playerNames;
        CountdownManager countDownManager;

        public AudioClip bellClip;
        public GameObject endedPanel;
        public TextMeshProUGUI endedPanelText;
        public Image firstHealthBar, secondHealthBar, roundTimeImg;
        public Image firstPowerBar, secondPowerBar;
        public Toggle[] firstBarToggles, secondBarToggles;
        public TextMeshProUGUI _timerText;
        public int round = 0;
        public static GameManager instance;
        public GameObject waitPanel;
        public GameObject _playerShowcaseParent;
        public GameObject[] _showPlayer1;
        public Canvas mainCanvas;
        public GameObject mainCamera;
        public PlayableDirector timelineDirector;

        public PhotonView GmphotonView;

        [SerializeField] List<PhotonView> playerViews = new List<PhotonView>();
        #endregion

        #region Unity Funcs
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void CheckBothPlayersReady()
        {
            // Get all the PhotonViews in the scene
            PhotonView[] allPhotonViews = FindObjectsOfType<PhotonView>();

            // Check if both players are ready
            bool player1Ready = false;
            bool player2Ready = false;

            foreach (PhotonView pv in allPhotonViews)
            {
                // Assuming player 1 has ViewID 1 and player 2 has ViewID 2 (modify accordingly if different)
                if (pv.ViewID == 1001)
                {
                    player1Ready = true;
                }
                else if (pv.ViewID == 2001)
                {
                    player2Ready = true;
                }
            }

            // If both players are ready, call the respective functions and start the match
            if (player1Ready && player2Ready)
            {
                foreach (PhotonView pv in allPhotonViews)
                {
                    if (pv.ViewID != 1)
                    {
                        pv.GetComponent<PlayerReferences>().OtherPlayerReady();
                    }
                }
                photonView.RPC("StartMatch", RpcTarget.All);
            }
        }

        // Start is called before the first frame update
        public void CallStart()
        {
            mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            if (CustomizationCanvasManager._instance._noofRounds == 1)
            {
                firstBarToggles[1].gameObject.SetActive(false);
                secondBarToggles[1].gameObject.SetActive(false);
            }

            //1st player joining mechanism
            if (PhotonNetwork.IsConnected)
            {
                // int randomVal = Random.Range(0, 4);
                if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                {
                    GameObject go = PhotonNetwork.Instantiate(playerPrefab.name, player1Pos, player1Rot);
                    countDownManager = go.GetComponent<CountdownManager>(); // Taking master Countdown for timer
                }
                else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
                {
                    PhotonNetwork.Instantiate(playerPrefab2.name, player2Pos, player2Rot);
                    //    playerViews.Clear();
                    //    var temp = FindObjectsOfType<PhotonView>();
                    //    foreach (PhotonView pv in temp)
                    //        playerViews.Add(pv);
                    //    foreach (PhotonView pv in playerViews)
                    //    {
                    //        if (pv != GmphotonView)
                    //        {
                    //            if (pv.ViewID != 1)
                    //            {
                    //                pv.GetComponent<PlayerReferences>().OtherPlayerReady(); //both players has joined start getting player references
                    //            }
                    //        }
                    //    }
                    //    photonView.RPC("StartMatch", RpcTarget.All); // start match on both ends
                }
                // Call the function to check if both players are ready
                CheckBothPlayersReady();

                GmphotonView.RPC("TurnOnPlayer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.NickName);
            }
        }

        public void SkipTimeline()
        {
            // Jump the timeline to the end and disable the skip button
            timelineDirector.time = timelineDirector.duration;
        }
        #endregion

        #region PhotonPunCallBack
        //public override void OnPlayerEnteredRoom(Player newPlayer)
        //{
        //}

        public override void OnLeftRoom()
        {
            Debug.Log("On Left Room Called!");
            PhotonNetwork.LoadLevel("PlayerCustomization");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            StartCoroutine(SetEndPanel("You Win!"));
            StartCoroutine(LeftRoom());

        }
        #endregion

        #region Funcs
        public IEnumerator SetEndPanel(string val)
        {
            yield return new WaitForSeconds(0.5f);
            endedPanelText.SetText(val);
            endedPanel.SetActive(true);
        }

        IEnumerator LeftRoom()
        {
            yield return new WaitForSeconds(2f);
            Debug.Log("Leaving Room!");
            PhotonNetwork.LeaveRoom();
        }
        /// <summary>
        /// Set player for VS screen before match start
        /// </summary>


        [PunRPC]
        private void TurnOnPlayer(int i, string playerName)
        {
            int playerNum = i - 1;
            _showPlayer1[playerNum].SetActive(true);
            playerNames[playerNum].text = playerName;
        }

        [PunRPC]
        public void StartMatch()
        {
            StartCoroutine(SetRoundPanel());
        }

        /// <summary>
        /// Setting up Game Start Panel
        /// </summary>
        public IEnumerator SetRoundPanel()
        {
            yield return new WaitForSeconds(2f);
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _playerShowcaseParent.SetActive(false);
            waitPanel.SetActive(false);
            round = round + 1;

            yield return new WaitForSeconds(0.5f);
            string roundText = (CustomizationCanvasManager._instance._noofRounds == 1) ? "Ready" : "Round " + round;
            endedPanelText.SetText(roundText);
            endedPanel.SetActive(true);

            yield return new WaitForSeconds(1f);
            GameManager.instance.endedPanelText.SetText("Ready");
            yield return new WaitForSeconds(1f);
            endedPanelText.SetText((CustomizationCanvasManager._instance._noofRounds == 1) ? "Let's Fight" : "Fight");
            AudioManager.instance.PlayAudioClip(bellClip);
            yield return new WaitForSeconds(1f);
            endedPanel.SetActive(false);

            // if (PhotonNetwork.IsMasterClient)
            //     {
            if (countDownManager != null)
            {
                countDownManager.photonView.RPC("SettingUp", RpcTarget.AllBuffered);
            }
            //    }
        }
        /// <summary>
        /// Setting up panel for Round 2 and 3 
        /// </summary>

        public void ForNextRound()
        {
            StartCoroutine(SetRoundPanel());
        }
        #endregion
    }
}