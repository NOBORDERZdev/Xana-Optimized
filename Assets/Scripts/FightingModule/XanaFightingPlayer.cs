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
    private void OnTransformParentChanged()
    {
        Debug.LogError("OnTransformParentChanged");
        if (transform.parent.GetComponent<ControlsScript>())
        {
            ControlsScript controlsScript = transform.parent.GetComponent<ControlsScript>();
            Debug.LogError("playernum: " + controlsScript.playerNum + " actor number: " + PhotonNetwork.LocalPlayer.ActorNumber);
            if (controlsScript.playerNum == 1)
            {
                if (UFE.gameMode == UFE3D.GameMode.TrainingRoom)
                {
                    avatarController.isLoadStaticClothFromJson = false;
                    avatarController.staticPlayer = true;
                    controlsScript.myInfo.characterName = PlayerPrefs.GetString("PlayerName").ToUpper();
                }
                else if (UFE.gameMode==UFE3D.GameMode.VersusMode)
                {
                    avatarController.isLoadStaticClothFromJson = false;
                    avatarController.staticPlayer = true;
                    controlsScript.myInfo.characterName = PlayerPrefs.GetString("PlayerName").ToUpper();
                }
                else
                {
                    if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                    {
                        cloth = avatarController.staticClothJson = FightingGameManager.instance.player1Data.clothJson;
                        controlsScript.myInfo.characterName = FightingGameManager.instance.player1Data.name.ToUpper();
                    }
                    else
                    {
                        cloth = avatarController.staticClothJson = FightingGameManager.instance.player2Data.clothJson;
                        controlsScript.myInfo.characterName = FightingGameManager.instance.player2Data.name.ToUpper();
                    }
                }
            }
            else
            {
                if (UFE.gameMode == UFE3D.GameMode.TrainingRoom)
                {
                    avatarController.isLoadStaticClothFromJson = true;
                    avatarController.staticPlayer = false;
                    controlsScript.myInfo.characterName = XanaConstants.xanaConstants.defaultFightingName.ToUpper();
                }
                else if (UFE.gameMode==UFE3D.GameMode.VersusMode)
                {
                    avatarController.isLoadStaticClothFromJson = true;
                    avatarController.staticPlayer = false;
                    controlsScript.myInfo.characterName = XanaConstants.xanaConstants.defaultFightingName.ToUpper();
                }
                else
                {
                    if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
                    {
                        cloth = avatarController.staticClothJson = FightingGameManager.instance.player1Data.clothJson;
                        controlsScript.myInfo.characterName = FightingGameManager.instance.player1Data.name.ToUpper();
                    }
                    else
                    {
                        cloth = avatarController.staticClothJson = FightingGameManager.instance.player2Data.clothJson;
                        controlsScript.myInfo.characterName = FightingGameManager.instance.player2Data.name.ToUpper();
                    }
                }
            }
            avatarController.OnEnable();
        }
    }
}
