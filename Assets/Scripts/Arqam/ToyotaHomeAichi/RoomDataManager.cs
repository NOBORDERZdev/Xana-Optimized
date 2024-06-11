using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDataManager : MonoBehaviour
{
    public Dictionary<string, List<GameObject>> roomDictionary = new Dictionary<string, List<GameObject>>();

    private void Start()
    {
        InRoomSoundHandler.playerInRoom += UpdateRoomData;
        BuilderEventManager.AfterWorldInstantiated += HideAllRoom;
    }
    private void OnDisable()
    {
        InRoomSoundHandler.playerInRoom -= UpdateRoomData;
        BuilderEventManager.AfterWorldInstantiated -= HideAllRoom;
    }
    public void RegisterRoom(string objectKey, GameObject roomObject)
    {

        string roomName = objectKey;

        if (roomDictionary.ContainsKey(roomName))
        {
            roomDictionary[roomName].Add(roomObject);
            roomObject.SetActive(false);
        }
        else
        {
            List<GameObject> objectList = new List<GameObject>();
            objectList.Add(roomObject);
            roomDictionary.Add(roomName, objectList);

            foreach (GameObject obj in objectList)
                obj.SetActive(false);
        }
    }

    private void ShowRoom(string roomName)
    {
        if (roomDictionary.ContainsKey(roomName))
        {
            // Enable all objects associated with this room
            foreach (GameObject roomObject in roomDictionary[roomName])
            {
                roomObject.SetActive(true);
            }
        }
        else
            Debug.LogWarning("Room not found: " + roomName);
    }

    private void HideAllRoom()
    {
        foreach (List<GameObject> roomObjects in roomDictionary.Values)
        {
            foreach (GameObject roomObject in roomObjects)
                roomObject.SetActive(false);
        }
    }
    private void UpdateRoomData(bool playerInRoom, string roomName)
    {
        if (playerInRoom)
            ShowRoom(roomName);
        else if (!playerInRoom)
            HideAllRoom();
    }


}
