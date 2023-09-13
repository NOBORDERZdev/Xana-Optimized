using UnityEngine;

namespace RFM
{
    public class RFMUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject helpPanel;
    
        public void ToggleHelpPanel()
        {
            helpPanel.SetActive(!helpPanel.activeInHierarchy);
        }
    }
}
