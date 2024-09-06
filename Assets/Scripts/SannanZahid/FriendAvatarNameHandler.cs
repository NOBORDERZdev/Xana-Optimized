using Photon.Voice;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class FriendAvatarNameHandler : MonoBehaviour
{
    TMPro.TMP_Text _onScreenName;
    public void SetFriendName(string nameOfFriend)
    {
        _onScreenName.text = nameOfFriend;
    }
}
