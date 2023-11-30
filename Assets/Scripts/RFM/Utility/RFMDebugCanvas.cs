using Photon.Pun;
using TMPro;
using UnityEngine;

namespace RFM.Utility
{
    public class RFMDebugCanvas : MonoBehaviour
    {
        public TextMeshProUGUI isMaster;
        public TextMeshProUGUI roomName;
        public TextMeshProUGUI roomIsOpen;
        public TextMeshProUGUI maxPlayers;
        public TextMeshProUGUI currentPlayers;
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
            roomName.text = $"roomName: {PhotonNetwork.CurrentRoom.Name}";
            roomIsOpen.text = $"room isOpen: {PhotonNetwork.CurrentRoom.IsOpen}";
            maxPlayers.text = $"room maxPlayers: {PhotonNetwork.CurrentRoom.MaxPlayers}";
            currentPlayers.text = $"room currentPlayers: {PhotonNetwork.CurrentRoom.Players.Count}";
            rfmStatus.text = $"RFM Status: {RFM.Globals.gameState}";
        }
    }
}
