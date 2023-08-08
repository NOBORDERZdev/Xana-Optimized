using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingGameManager : MonoBehaviour
{
    internal FightingGameManager instance;
    public bool startDirectly = false;

    public UFE3D.CharacterInfo P1SelectedChar;
    public UFE3D.CharacterInfo P2SelectedChar;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (startDirectly) 
        {
            print("Starting"); //kush
            UFE.SetPlayer1(P1SelectedChar);
            UFE.SetPlayer2(P2SelectedChar);
            UFE.StartLoadingBattleScreen();
        }
    }
}
