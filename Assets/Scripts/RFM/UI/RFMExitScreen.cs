using UnityEngine;


namespace RFM.UI
{
    public class RFMExitScreen : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image yesButtonImage;

        private void OnEnable()
        {
            // if this is RFM world, we need to show a purple color for the yes button, otherwise we need to show a blue color
            if (/*WorldItemView.m_EnvName.Contains("RFMDummy")*/RFM.Globals.IsRFMWorld)
            {
                yesButtonImage.color = new Color(1f, 0.212f, 0.827f, 1f);
            }
            else
            {
                yesButtonImage.color = new Color(0f, 0.561f, 1f, 1f);
            }
        }
    }
}
