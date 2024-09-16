using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GiantWheelManager : MonoBehaviour, IInRoomCallbacks
{

    public static GiantWheelManager Instance;

    private bool _carAdded;
    public bool CarAdded =false;// { get { return _carAdded; } set { if (_carAdded != value) { onCarValueChange(); }  _carAdded = value; } }

    public Transform CarPlacement, Exit;
    public Transform Wheel;
    public List<SummitPlayerRPC> SummitRPCs = new List<SummitPlayerRPC>();
    public WheelCar car;

    private bool stopWheel;
    public bool Stop;
    public Action onCarStop, EnterwheelCar;
    float counter = 0;
    Vector3 newRot;

    Vector3 currentRot;
    public GameObject WheelCar;

    private void Awake()
    {
        Instance = this;
        //rotateObject(Wheel.gameObject, new Vector3(360, 0, 0), 60f);
          newRot = Wheel.eulerAngles + new Vector3(360, 0, 0);
         currentRot = Wheel.eulerAngles;
    }




    public void CheckForWheelCar()
    {
        string name = PhotonNetwork.CurrentRoom.CustomProperties["Sector"].ToString();

        if (name == "Wheel" && stopWheel)
        {


            StartCoroutine(startwheel());
            onCarStop?.Invoke();



        }

    }

    public void AddCar()
    {
        string name = PhotonNetwork.CurrentRoom.CustomProperties["Sector"].ToString();
        if (name == "Wheel")
        {
            if (PhotonNetwork.IsMasterClient && !CarAdded)
            {
              
             var obj =   PhotonNetwork.InstantiateRoomObject("Wheel-Interior", CarPlacement.position,new Quaternion(0,90,0,0));
                //obj.transform.Rotate(new Vector3(0,-90,0));
                StartCoroutine(startwheel());
              

            }
        }
            
    }
    
    public IEnumerator startwheel()
    {
        //  Wheel.SetTrigger("Stop");
        Stop = true;
        stopWheel = false;
        yield return new WaitForSeconds(2);
        Stop = false;

    }
  
    private void FixedUpdate()
    {
       

       
        if (counter < 60)
        {
            if (Stop)
            {
                return;
            }
            counter += Time.deltaTime;
            Wheel.eulerAngles = Vector3.Lerp(currentRot, newRot, counter / 60);

        }
        else
        {
            newRot = Wheel.eulerAngles + new Vector3(360, 0, 0);
            currentRot = Wheel.eulerAngles;
            counter = 0;
        }
        

    }
    private async void rotateObject(GameObject gameObjectToMove, Vector3 eulerAngles, float duration)
    {while (true)
        {

           
        }
      
    }

    public void StopWheel( SummitPlayerRPC player)
    {
        stopWheel = true;
          SummitRPCs.Add(player);

    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
       
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
       
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
       
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }
}
