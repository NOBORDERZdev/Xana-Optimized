using UnityEngine;

namespace RFM.UI
{
    public class InstructionsPanel : MonoBehaviour
    {
        [SerializeField] private int time = 30;
        private void Start()
        {
            Invoke(nameof(HidePanel), time);
        }

        public void HidePanel()
        {
            gameObject.SetActive(false);
        }
    }
}
