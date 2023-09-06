using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XanaFightingPlayer : MonoBehaviour
{
    public AvatarController avatarController;
    void Start()
    {

    }

    void Update()
    {

    }
    private void OnTransformParentChanged()
    {
        if (transform.parent.GetComponent<ControlsScript>())
        {
            if (transform.parent.GetComponent<ControlsScript>().playerNum == 1)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                {
                    avatarController.staticClothJson = FightingGameManager.instance.player1Data.clothJson;
                }
            }
            else
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
                {
                    avatarController.staticClothJson = FightingGameManager.instance.player2Data.clothJson;
                }
            }
            avatarController.OnEnable();
        }
    }
}
