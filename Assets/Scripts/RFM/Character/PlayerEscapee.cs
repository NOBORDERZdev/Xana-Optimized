using Photon.Pun;
using UnityEngine;

namespace RFM.Character
{
    public class PlayerEscapee : MonoBehaviour
    {
        public void PlayerEscapeeCaught(NPCHunter npcHunter)
        {
            PhotonNetwork.Destroy(transform.root.gameObject);
            EventsManager.PlayerCaught(npcHunter);
        }
    }
}
