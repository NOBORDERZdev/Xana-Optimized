using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RFM
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameTMP, amountTMP, rankText, timeSurvivedText;
        public int money;
        
        public void Init(string name, int amount, float timeSurvived, int playerRank)
        {
            money = amount;
            nameTMP.text = name;
            amountTMP.text = money.ToString();
            timeSurvivedText.text = timeSurvived.ToString();
            rankText.text = playerRank.ToString();
        }
    }
}
