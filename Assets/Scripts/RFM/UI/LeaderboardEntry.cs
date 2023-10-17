using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RFM
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameTMP, amountTMP;

        public int money;

        public void Init(string name, int amount)
        {
            money = amount;
            nameTMP.text = name;
            amountTMP.text = money.ToString();
        }
    }
}
