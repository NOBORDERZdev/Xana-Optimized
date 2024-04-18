using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingDataManager : MonoBehaviour
{
    public FightingPlayer player1 = new FightingPlayer();
    public FightingPlayer player2 = new FightingPlayer();
    public static FightingDataManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {

    }
}

[System.Serializable]
public class FightingPlayer
{
    public string profile;
    public int speed;
    public int stamina;
    public int punch;
    public int kick;
    public int defence;
    public int special_move;
}