using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNameInputManager : MonoBehaviour
{
    public void SetPlayerName(string _name) {
        if (string.IsNullOrEmpty(_name)) {
            print("name is empty");
            return;
        }
        print("Name : " + _name);
        PhotonNetwork.NickName = _name;
    }
}
