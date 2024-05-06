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
                /*SavingCharacterDataClass _CharacterData = new SavingCharacterDataClass();
                _CharacterData = _CharacterData.CreateFromJSON(FightingGameManager.instance.PlayerClothJson);
                controlsScript.fightingPlayer.profile = _CharacterData.profile;
                controlsScript.fightingPlayer.speed = _CharacterData.speed;
                controlsScript.fightingPlayer.stamina = _CharacterData.stamina;
                controlsScript.fightingPlayer.punch = _CharacterData.punch;
                controlsScript.fightingPlayer.kick = _CharacterData.kick;
                controlsScript.fightingPlayer.defence = _CharacterData.defence;
                controlsScript.fightingPlayer.special_move = _CharacterData.special_move;
                FightingDataManager.Instance.player2 = controlsScript.fightingPlayer;*/
            }
            else
            {
                if (UFE.gameMode == UFE3D.GameMode.TrainingRoom)
                {
                    avatarController.isLoadStaticClothFromJson = true;
                    avatarController.staticPlayer = false;
                    controlsScript.myInfo.characterName = ConstantsHolder.xanaConstants.defaultFightingName.ToUpper();
                }
                else if (UFE.gameMode==UFE3D.GameMode.VersusMode)
                {
                    avatarController.isLoadStaticClothFromJson = true;
                    avatarController.staticPlayer = false;
                    controlsScript.myInfo.characterName = ConstantsHolder.xanaConstants.defaultFightingName.ToUpper();
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
