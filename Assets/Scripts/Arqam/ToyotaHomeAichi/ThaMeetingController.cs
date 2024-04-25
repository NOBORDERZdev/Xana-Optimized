using System.Collections.Generic;
using Photon.Realtime;

namespace Photon.Pun.Demo.PunBasics
{
    public class ThaMeetingController : MonoBehaviourPunCallbacks
    {
        private InterRoomCommunication communicationComponent;

        void Start()
        {
            //only user can back to toyota world when press on exit btn
            if (ConstantsHolder.xanaConstants.toyotaMeetingStatus == ConstantsHolder.MeetingStatus.Inprogress)
            {
                ConstantsHolder.xanaConstants.isBackToParentScane = true;
                ConstantsHolder.xanaConstants.parentSceneName = "D_Infinity_Labo";
            }

            communicationComponent = ConstantsHolder.xanaConstants.gameObject.GetComponent<InterRoomCommunication>();
        }

        public override void OnConnectedToMaster()
        {

        }

        public override void OnJoinedLobby()
        {

        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {

        }
        public override void OnCreatedRoom()
        {
        }
        public override void OnLeftLobby()
        {
            int userId = ReferencesForGamePlay.instance.m_34player.GetComponent<PhotonView>().Controller.ActorNumber;
            communicationComponent.BroadcastUserJoinInterviewRoom(userId);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {

        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {

        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            int userId = ReferencesForGamePlay.instance.m_34player.GetComponent<PhotonView>().Controller.ActorNumber;
            communicationComponent.BroadcastUserJoinInterviewRoom(userId);
        }
        public override void OnJoinedRoom()
        {

        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {

        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {

        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {

        }


    }
}
