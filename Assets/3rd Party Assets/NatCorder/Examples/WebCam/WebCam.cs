/* 
*   NatCorder
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCorder.Examples {

    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;
    using Clocks;

    public class WebCam : MonoBehaviour {

        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;

        private WebCamTexture webcamTexture;
        private MP4Recorder videoRecorder;
        private IClock clock;
        private Color32[] pixelBuffer;

        private IEnumerator Start () {
            // Request camera permission
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
                yield break;
            // Start the WebCamTexture
            webcamTexture = new WebCamTexture(1280, 720, 30);
            webcamTexture.Play();
            // Display webcam
            yield return new WaitUntil(() => webcamTexture.width != 16 && webcamTexture.height != 16); // Workaround for weird bug on macOS
            rawImage.texture = webcamTexture;
            aspectFitter.aspectRatio = (float)webcamTexture.width / webcamTexture.height;
            pixelBuffer = webcamTexture.GetPixels32();
        }

        public void StartRecording () {
            // Start recording
            clock = new RealtimeClock();
            videoRecorder = new MP4Recorder(
                webcamTexture.width,
                webcamTexture.height,
                30,
                0,
                0,
                recordingPath => {
                    Debug.Log($"Saved recording to: {recordingPath}");
                    var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "file://" : "";
                    Handheld.PlayFullScreenMovie($"{prefix}{recordingPath}");
                }
            );
        }

        public void StopRecording () {
            // Stop recording
            videoRecorder.Dispose();
            videoRecorder = null;
        }

        void Update () {
            // Record frames
            webcamTexture.GetPixels32(pixelBuffer);
            videoRecorder?.CommitFrame(pixelBuffer, clock.Timestamp);
        }
    }
}