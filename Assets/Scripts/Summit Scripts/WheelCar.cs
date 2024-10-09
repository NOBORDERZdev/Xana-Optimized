using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WheelCar : MonoBehaviour, IInRoomCallbacks
{

    public Transform FirstPlayerPos, SecondPlayerPos, ThirdPlayerPos, ForthPlayerPos;
    public bool isfirstPlayerEmpty=true, issecondPlayerEmpty = true, isThirdPlayerEmpty = true, isForthPlayerEmpty=true;
    public Dictionary<Player,int> PlayerSeat = new Dictionary<Player,int>();

    public PhotonView view;

    Vector3 newRot;
    float counter = 0;
    float time = 90f;
    Vector3 currentRot;

    private void Start()
    {
        
        CarAdded();
        newRot = transform.eulerAngles + new Vector3(-90, 0, 0);
        currentRot = transform.eulerAngles;
    }

    private void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) { return; }




        if (counter < 1)
        {
            if (GiantWheelManager.Instance.Stop)
            {
                return;
            }
            counter += Time.deltaTime;
            counter = counter / time;


            transform.eulerAngles = Vector3.Slerp(currentRot, newRot, counter);

        }
        else
        {
            counter = 0;
            newRot = transform.eulerAngles + new Vector3(-90, 0, 0);
            currentRot = transform.eulerAngles;
        }


    }

    public void CarAdded()
    {
   
        
        transform.parent = GiantWheelManager.Instance.Wheel.transform;

        GiantWheelManager.Instance.CarAdded = true;
        GiantWheelManager.Instance.car = this;
       
    }



    #region Photon
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            if (!newPlayer.IsLocal){

               

            }
        }

    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        switch (PlayerSeat[otherPlayer])
        {
            case 1:
                isfirstPlayerEmpty = true; break;
            case 2:
                issecondPlayerEmpty = true; break;
            case 3:
                isThirdPlayerEmpty = true; break;
            case 4:
                isForthPlayerEmpty = true; break;
        }

        PlayerSeat.Remove(otherPlayer);
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

    }


    #endregion
}
