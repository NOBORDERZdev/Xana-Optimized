using System;
using UnityEngine;

namespace RFM
{
    public class RFMUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject helpPanel;
        [SerializeField] private GameObject instructionsPanelPanel;

        private GameObject _controlsCanvas; // 375

        private bool _wasInstructionsPanelActive;

        private void Awake()
        {
            _controlsCanvas = GameObject.FindGameObjectWithTag("NewCanvas");
        }

        private void OnEnable()
        {
            RFM.EventsManager.onToggleHelpPanel += ToggleHelpPanel;
            RFM.EventsManager.onCountdownStart += OnCountdownStart;
        }

        private void OnDisable()
        {
            RFM.EventsManager.onToggleHelpPanel -= ToggleHelpPanel;
            RFM.EventsManager.onCountdownStart -= OnCountdownStart;
        }
        
        private void OnCountdownStart()
        {
            instructionsPanelPanel.SetActive(false);
        }

        public void ToggleHelpPanel()
        {
            if (helpPanel.activeInHierarchy)
            {
                _controlsCanvas.SetActive(true);
                helpPanel.SetActive(false);

                if (_wasInstructionsPanelActive)
                {
                    instructionsPanelPanel.SetActive(true);
                    _wasInstructionsPanelActive = false;
                }
            }
            else
            {
                _controlsCanvas.SetActive(false);
                helpPanel.SetActive(true);
                if (instructionsPanelPanel.activeInHierarchy)
                {
                    instructionsPanelPanel.SetActive(false);
                    _wasInstructionsPanelActive = true;
                }
            }
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
