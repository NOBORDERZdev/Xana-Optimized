using UnityEngine;

namespace RFM
{
    public class RFMCard : MonoBehaviour
    {
        private void Start()
        {
            GetComponentInChildren<TMPro.TextMeshProUGUI>().text = cardType.ToString();
        }

        public CardType cardType;

        public enum CardType
        {
            Invisibility, SpeedBoost
        }
    }
}
