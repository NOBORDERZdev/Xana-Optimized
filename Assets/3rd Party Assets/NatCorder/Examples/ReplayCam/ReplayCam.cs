/* 
*   NatCorder
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCorder.Examples {

    using UnityEngine;
    using System.Collections;
    using Clocks;
    using Inputs;
    
    public class ReplayCam : MonoBehaviour {

        [Header("Recording")]
        public int videoWidth = 1280;
        public int videoHeight = 720;
        public bool recordMicrophone;

        private IMediaRecorder videoRecorder;
        private CameraInput cameraInput;
        private AudioInput audioInput;
        private AudioSource microphoneSource;

        private IEnumerator Start () {
            // Start microphone
            microphoneSource = gameObject.AddComponent<AudioSource>();
            microphoneSource.mute =
            microphoneSource.loop = true;
            microphoneSource.bypassEffects =
            microphoneSource.bypassListenerEffects = false;
            microphoneSource.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
            yield return new WaitUntil(() => Microphone.GetPosition(null) > 0);
            microphoneSource.Play();
        }

        private void OnDestroy () {
            // Stop microphone
            microphoneSource.Stop();
            Microphone.End(null);
        }

        public void StartRecording () {
            // Start recording
            var frameRate = 30;
            var sampleRate = recordMicrophone ? AudioSettings.outputSampleRate : 0;
            var channelCount = recordMicrophone ? (int)AudioSettings.speakerMode : 0;
            var recordingClock = new RealtimeClock();
            videoRecorder = new MP4Recorder(
                videoWidth,
                videoHeight,
                frameRate,
                sampleRate,
                channelCount,
                recordingPath => {
                    Debug.Log($"Saved recording to: {recordingPath}");
                    var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "file://" : "";
                    Handheld.PlayFullScreenMovie($"{prefix}{recordingPath}");
                }
            );
            // Create recording inputs
            cameraInput = new CameraInput(videoRecorder, recordingClock, Camera.main);
            audioInput = recordMicrophone ? new AudioInput(videoRecorder, recordingClock, microphoneSource, true) : null;
            // Unmute microphone
            microphoneSource.mute = audioInput == null;
        }

        public void StopRecording () {
            // Stop recording
            audioInput?.Dispose();
            cameraInput.Dispose();
            videoRecorder.Dispose();
            // Mute microphone
            microphoneSource.mute = true;
        }
    }
}