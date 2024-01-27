using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingDataManager : MonoBehaviour
{
    public FightingPlayer player1=new FightingPlayer();
    public FightingPlayer player2=new FightingPlayer();
    public static FightingDataManager instance;
    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

[System.Serializable]
public class FightingPlayer
{
    public string Profile;
    public int speed;
    public int stamina;
    public int punch;
    public int kick;
    public int defence;
    public int special_move;
}