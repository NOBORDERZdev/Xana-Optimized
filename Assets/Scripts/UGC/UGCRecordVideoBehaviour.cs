using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NatCorder.Clocks;
using NatCorder;
using NatCorder.Inputs;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Video;

public class UGCRecordVideoBehaviour : MonoBehaviour
{
    public static UGCRecordVideoBehaviour instance;
    [Header("Recording")]
    public int videoWidth ;
    public int videoHeight ;
    public bool recordMicrophone;
    private IMediaRecorder videoRecorder;
    private CameraInput cameraInput;
    private AudioInput audioInput;
    private AudioSource microphoneSource;
    public string videoRecordingPath;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        videoWidth = Screen.width;
        videoHeight = Screen.height;
        //Debug.Log("RecordVideo width:" + videoWidth + "    :height:" + videoHeight);

        if (videoWidth % 2 == 1 )
        {
            videoWidth = (int)(Screen.width )+1;
            //Debug.Log("RecordVideo111 width:" + videoWidth);
        }

        if (videoHeight % 2 == 1)
        {
            videoHeight = (int)(Screen.height +1);
            //Debug.Log("RecordVideo111 height:" + videoHeight);
        }
        //videoTexture.width = Screen.width;
        //videoTexture.height = Screen.height;        
    }

    private IEnumerator Start()
    {       
        // Start microphone
        microphoneSource = gameObject.AddComponent<AudioSource>();
        microphoneSource.mute = true;
        microphoneSource.loop = true;
        microphoneSource.bypassEffects = true;
        microphoneSource.bypassListenerEffects = false;
        microphoneSource.clip = Microphone.Start("", true, 10, AudioSettings.outputSampleRate);
        yield return new WaitUntil(() => Microphone.GetPosition(null) > 0);
        microphoneSource.Play();
    }    

    private void OnDestroy()
    {
        // Stop microphone
        microphoneSource.Stop();
        Microphone.End(null);
    }

    public void StartRecording()
    {
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
                videoRecordingPath = recordingPath;
                //var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "file://" : "";
                var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "" : "";
                // Handheld.PlayFullScreenMovie($"{prefix}{recordingPath}");

                
            }
        );
        // Create recording inputs
        cameraInput = new CameraInput(videoRecorder, recordingClock, Camera.main);
        audioInput = recordMicrophone ? new AudioInput(videoRecorder, recordingClock, microphoneSource, true) : null;
        // Unmute microphone
        microphoneSource.mute = audioInput == null; 
    }

    public void StopRecording()
    {
        //pressed = false;
        // Stop recording
        audioInput?.Dispose();
        cameraInput.Dispose();
        videoRecorder.Dispose();
        // Mute microphone
        microphoneSource.mute = true;
    }
}

//yyuyyuyyiyiytturrrd  essss  333   sss sss  sss  sss  sss  sss  sss  sss