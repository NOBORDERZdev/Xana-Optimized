using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XanaFightingPlayer : MonoBehaviour
{
    public AvatarController avatarController;
    public string cloth;
    void Start()
    {

    }

    void Update()
    {

    }
    private void OnTransformParentChanged()
    {
        Debug.LogError("OnTransformParentChanged");
        if (transform.parent.GetComponent<ControlsScript>())
        {
            Debug.LogError("playernum: " + transform.parent.GetComponent<ControlsScript>().playerNum + " actor number: " + PhotonNetwork.LocalPlayer.ActorNumber);
            if (transform.parent.GetComponent<ControlsScript>().playerNum == 1)
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                {
                    cloth = avatarController.staticClothJson = FightingGameManager.instance.player1Data.clothJson;
                }
                else
                {
                    cloth = avatarController.staticClothJson = FightingGameManager.instance.player2Data.clothJson;
                }
            }
            else
            {
                if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
                {
                    cloth = avatarController.staticClothJson = FightingGameManager.instance.player1Data.clothJson;
                }
                else
                {
                    cloth = avatarController.staticClothJson = FightingGameManager.instance.player2Data.clothJson;
                }
            }
            avatarController.OnEnable();
        }
    }
}
