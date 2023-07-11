/* 
*   NatCorder
*   Copyright (c) 2019 Yusuf Olokoba
*/

namespace NatCorder.Examples {

    using UnityEngine;
    using Clocks;
    using Inputs;

    public class Giffy : MonoBehaviour {
        
        [Header("GIF Settings")]
        public int imageWidth = 640;
        public int imageHeight = 480;
        public float frameDuration = 0.1f; // seconds

        private GIFRecorder gifRecorder;
        private CameraInput cameraInput;

        public void StartRecording () {
            // Start recording
            gifRecorder = new GIFRecorder(
                imageWidth,
                imageHeight,
                frameDuration,
                gifPath => {
                    Debug.Log($"Saved animated GIF image to: {gifPath}");
                    var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "file://" : "";
                    Application.OpenURL($"{prefix}{gifPath}");
                }
            );
            // Create a camera input
            cameraInput = new CameraInput(gifRecorder, new RealtimeClock(), Camera.main);
            // Get a real GIF look by skipping frames
            cameraInput.frameSkip = 4;
        }

        public void StopRecording () {
            // Stop the recording
            cameraInput.Dispose();
            gifRecorder.Dispose();
        }
    }
}