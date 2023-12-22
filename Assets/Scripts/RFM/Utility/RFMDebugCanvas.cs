using Photon.Pun;
using TMPro;
using UnityEngine;

namespace RFM.Utility
{
    public class RFMDebugCanvas : MonoBehaviour
    {
        public TextMeshProUGUI isMaster;
        public TextMeshProUGUI roomName;
        public TextMeshProUGUI players;
        public TextMeshProUGUI rfmStatus;

        private void Awake()
        {
            if (RFM.Globals.DevMode)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            isMaster.text = $"isMaster: {PhotonNetwork.IsMasterClient}";
            roomName.text = $"roomName: {PhotonNetwork.CurrentRoom.Name} ({(PhotonNetwork.CurrentRoom.IsOpen ? "Open" : "Closed")})";
            players.text = $"players: {PhotonNetwork.CurrentRoom.Players.Count}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
            rfmStatus.text = $"RFM Status: {RFM.Globals.gameState}";
        }
    }
}
