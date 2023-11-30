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

            if (playerRank is 1 or 2 or 3)
            {
                nameTMP.color = new Color(255, 54, 211);
                GetComponent<UnityEngine.UI.Image>().color = new Color(255, 54, 211);
            }
        }
    }
}
