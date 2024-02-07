using UnityEngine;


namespace RFM
{
    public class RFMAudioManager : MonoBehaviour
    {
        public static RFMAudioManager Instance { get; private set; }

        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sFXSource;
        [SerializeField] private AudioClip runnerCatchSFX;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            RFM.EventsManager.onBGMVolumeChanged += OnBGMVolumeChanged;
            RFM.EventsManager.onSFXVolumeChanged += OnSFXVolumeChanged;
        }

        private void OnDisable()
        {
            RFM.EventsManager.onBGMVolumeChanged -= OnBGMVolumeChanged;
            RFM.EventsManager.onSFXVolumeChanged -= OnSFXVolumeChanged;
        }

        private void OnBGMVolumeChanged(float vol)
        {
            bgmSource.volume = vol;
        }

        private void OnSFXVolumeChanged(float vol)
        {
            sFXSource.volume = vol;
        }

        public void PlayRunnerCatchSFX()
        {
            sFXSource.PlayOneShot(runnerCatchSFX);
        }
    }
}
