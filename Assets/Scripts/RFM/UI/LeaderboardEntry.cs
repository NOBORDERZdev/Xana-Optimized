using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RFM
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameTMP, amountTMP;

        public void Init(string name, string amount)
        {
            nameTMP.text = name;
            amountTMP.text = amount;
        }
    }
}
