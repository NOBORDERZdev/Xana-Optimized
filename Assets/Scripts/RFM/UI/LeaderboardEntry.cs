using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RFM
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameTMP, amountTMP, rankText, timeSurvivedText;

        public void Init(string name, string amount, float timeSurvived, int playerRank)
        {
            nameTMP.text = name;
            amountTMP.text = amount;
            timeSurvivedText.text = timeSurvived.ToString();
            rankText.text = playerRank.ToString();
        }
    }
}
