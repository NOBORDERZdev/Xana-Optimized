using System;
using UnityEngine;

namespace RFM
{
    public class RFMUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject helpPanel;

        private void OnEnable()
        {
            RFM.EventsManager.onToggleHelpPanel += ToggleHelpPanel;
        }
        
        private void OnDisable()
        {
            RFM.EventsManager.onToggleHelpPanel -= ToggleHelpPanel;
        }

        private void ToggleHelpPanel()
        {
            helpPanel.SetActive(!helpPanel.activeInHierarchy);
        }

        public void HomeButtonClicked()
        {
            var sceneManage = FindObjectOfType<SceneManage>();

            if (sceneManage)
            {
                sceneManage.ReturnToHome(true);
            }
        }
    }
}
