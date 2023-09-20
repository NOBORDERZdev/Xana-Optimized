using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomDataLobby : MonoBehaviour {

    [NonReorderable]
    public List<LobbyData> Lobbies= new List<LobbyData>();

    private void Start() {
       
    }

    /// <summary>
    /// Add new Room in list in corresponding the lobby.
    /// </summary>
    /// <param name="info"> room data</param>
    public void AddRoom(RoomInfo info)
    {
        if (info !=null)
        {
            //Getting Lobby Name from room Name
            string LobbyName = GetLobbyName(info.Name);
            if(!IsLobbyExist(LobbyName)){  // Lobby dose not exists. So, add new lobby 
               Lobbies.Add(new LobbyData(LobbyName));
            }

            LobbyData result = Lobbies.Find(p => p.LobbyName.Equals(LobbyName));
            if (result!=null && result.RoomList.Count>=0)
            {
                RoomInfo isAlreadyRoomAdded = result.RoomList.Find(p => p.Name.Equals(info.Name));
                if (isAlreadyRoomAdded != null) 
                    result.RoomList.Add(info);
            }
        }
    }


    /// <summary>
    /// To check is lobby already exist in list
    /// </summary>
    bool IsLobbyExist(string name){
        foreach (LobbyData lobby in Lobbies)
        {
            if (lobby.LobbyName == name)
            {
              return true;
            }
        }
         return false;
    }


    /// <summary>
    /// Get Lobby name from the room name
    /// </summary>
    /// <param name="roomName"></param>
    /// <returns></returns>
    string GetLobbyName(string roomName)
    {
        string temp = roomName;
        char[] charsToTrim = { '0','1','2','3','4','5','6','7','8','9' };
        string lobbyName = roomName.TrimEnd(charsToTrim);
        return lobbyName;
    }

}

[Serializable]
public class LobbyData{ 
  public string LobbyName;
  [NonReorderable]
  [SerializeField]
  public List<RoomInfo> RoomList = new List<RoomInfo>();

    public LobbyData(string lobbyName) {
        LobbyName = lobbyName;    
    }
}