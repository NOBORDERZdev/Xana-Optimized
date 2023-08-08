/// <summary>
/// Settng up Player Cameras and provide naming info on VS Screen
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TextMeshProUGUI playerNameText;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            GameObject cineMachineObj = GameObject.FindGameObjectWithTag("CineMachine");
            cineMachineObj.GetComponent<CinemachineFreeLook>().Follow = gameObject.transform;
            cineMachineObj.GetComponent<CinemachineFreeLook>().LookAt = gameObject.transform;

            WaitPanelItems.instance._firstPlayerName.text = PhotonNetwork.PlayerList[0].NickName;
            WaitPanelItems.instance._firstPlayerGloves.text = "Gloves: Will get from APIs";
            WaitPanelItems.instance._firstPlayerNeckChains.text = "NeckChain: Will get from APIs";
            WaitPanelItems.instance._firstPlayerTattoo.text = "Tattoo: Will get from APIs";
            WaitPanelItems.instance._firstPlayerPants.text = "Pant: Will get from APIs";
            WaitPanelItems.instance._firstPlayerMuscles.text = "Muscles: Will get from APIs";
        }
        else
        {
            WaitPanelItems.instance._secondPlayerName.text = PhotonNetwork.PlayerList[1].NickName;
            WaitPanelItems.instance._secondPlayerGloves.text = "Gloves: Will get from APIs2";
            WaitPanelItems.instance._secondPlayerNeckChains.text = "NeckChain: Will get from APIs2";
            WaitPanelItems.instance._secondPlayerTattoo.text = "Tattoo: Will get from APIs2";
            WaitPanelItems.instance._secondPlayerPants.text = "Pant: Will get from APIs2";
            WaitPanelItems.instance._secondPlayerMuscles.text = "Muscles: Will get from APIs2";
        }

        SetPlayerUI();
    }

    void SetPlayerUI()
    {
        playerNameText.text = photonView.Owner.NickName;
    }
}
