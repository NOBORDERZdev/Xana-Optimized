using TMPro;
using UnityEngine;

namespace RFM
{
    public class RFMMainMenuButton : MonoBehaviour
    {
        public TextMeshProUGUI text;

        private void Start()
        {
            #if UNITY_EDITOR || UNITY_STANDALONE
            RFM.Globals.DevMode = true;
            #endif

            text.text = RFM.Globals.DevMode ? "RFM Dev Mode On" : "RFM Dev Mode Off";
        }

        public void ToggleDevMode()
        {
            RFM.Globals.DevMode = !RFM.Globals.DevMode;
            text.text = RFM.Globals.DevMode ? "RFM Dev Mode On" : "RFM Dev Mode Off";
        }
    }
}