using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterAsRoomObject : MonoBehaviour
{
    public string roomName = "";
    private RoomDataManager roomDataManager;

    void Start()
    {
        roomDataManager = this.transform.root.gameObject.GetComponent<RoomDataManager>();
        roomDataManager.RegisterRoom(roomName, this.gameObject);
    }


}
