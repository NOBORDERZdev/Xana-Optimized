using UnityEngine;
using TMPro;

namespace RFM
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameTMP, amountTMP, rankText, timeSurvivedText;
        public int money;

        public void Init(string name, int amount, float timeSurvived/*, int playerRank*/)
        {
            money = amount;
            nameTMP.text = name;
            amountTMP.text = money.ToString();
            timeSurvivedText.text = timeSurvived.ToString();

            var rank = transform.childCount - (transform.GetSiblingIndex() + 1);
            rankText.text = rank.ToString();
        }
    }
}
