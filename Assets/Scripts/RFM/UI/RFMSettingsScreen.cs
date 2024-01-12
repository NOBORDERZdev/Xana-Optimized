using UnityEngine;


namespace RFM.UI
{
    public class RFMSettingsScreen : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image[] images;
        [SerializeField] private UnityEngine.UI.Image[] handlesImages;
        [SerializeField] private Sprite xanaSliderHandleSprite;
        [SerializeField] private Sprite rfmSliderHandleSprite;

        private void OnEnable()
        {
            // if this is RFM world, we need to show a pink color for the specified images, otherwise we need to show a blue color
            if (/*WorldItemView.m_EnvName.Contains("RFMDummy")*/RFM.Globals.IsRFMWorld)
            {
                foreach (var image in images)
                {
                    image.color = new Color(1f, 0.212f, 0.827f, 1f);
                }

                foreach (var image in handlesImages)
                {
                    image.sprite = rfmSliderHandleSprite;
                }
            }
            else
            {
                foreach (var image in images)
                {
                    image.color = new Color(0f, 0.561f, 1f, 1f);
                }
                foreach (var image in handlesImages)
                {
                    image.sprite = xanaSliderHandleSprite;
                }
            }
        }
    }
}
