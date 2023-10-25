using UnityEngine;
using TMPro;
using System;

namespace RFM
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameTMP, amountTMP, rankText, timeSurvivedText;
        public int money;

        public void Init(string name, int amount, string timeSurvived, int playerRank)
        {
            money = amount;
            nameTMP.text = name;
            amountTMP.text = money.ToString();
            timeSurvivedText.text = timeSurvived.ToString();

            //var rank = transform.childCount - (transform.GetSiblingIndex() + 1);
            rankText.text = playerRank.ToString();
        }
    }
}
