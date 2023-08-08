/// <summary>
/// Using this for player names and other infos references to show in VS screen
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class WaitPanelItems : MonoBehaviourPunCallbacks
{
    [Header("First Player Attributes")]
    public TextMeshProUGUI _firstPlayerName;
    public TextMeshProUGUI _firstPlayerGloves;
    public TextMeshProUGUI _firstPlayerNeckChains;
    public TextMeshProUGUI _firstPlayerTattoo;
    public TextMeshProUGUI _firstPlayerPants;
    public TextMeshProUGUI _firstPlayerMuscles;

    [Space(10)]
    [Header("Second Player Attributes")]
    public TextMeshProUGUI _secondPlayerName;
    public TextMeshProUGUI _secondPlayerGloves;
    public TextMeshProUGUI _secondPlayerNeckChains;
    public TextMeshProUGUI _secondPlayerTattoo;
    public TextMeshProUGUI _secondPlayerPants;
    public TextMeshProUGUI _secondPlayerMuscles;

    public static WaitPanelItems instance;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
