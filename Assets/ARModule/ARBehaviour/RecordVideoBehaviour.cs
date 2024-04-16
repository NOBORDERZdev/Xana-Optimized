using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NatCorder.Clocks;
using NatCorder;
using NatCorder.Inputs;
using UnityEngine.UI;
using System.IO;

public class RecordVideoBehaviour : MonoBehaviour
{
    public static RecordVideoBehaviour instance;
    [Header("Recording")]
    public int videoWidth ;
    public int videoHeight ;
    public bool recordMicrophone;

    private IMediaRecorder videoRecorder;
    private CameraInput cameraInput;
    private AudioInput audioInput;
    private AudioSource microphoneSource;

    public RenderTexture videoTexture;

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
        if (SNSNotificationHandler.Instance != null)
        {
            SNSNotificationHandler.Instance.ResetAndInstantHideNotificationBar();
        }

        ARFaceModuleManager.Instance.OnActiveOrDisableUiRecordingTime(false);//this method is used to false button ui video racording time.......

        LiveVideoRoomManager.Instance.VideoRecordingAllUIDisable(false);
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
                //var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "file://" : "";
                var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "" : "";
                // Handheld.PlayFullScreenMovie($"{prefix}{recordingPath}");

                ARFaceModuleManager.Instance.OnActiveOrDisableUiRecordingTime(true);//this method is used to false button ui video racording time.......

                if (!ARFaceModuleManager.Instance.r_IsCapturingVideoBack)//this is check for if user back button press video capturing time do not open video view screen.......
                {
                    LiveVideoRoomManager.Instance.filterMainVolume.gameObject.SetActive(false);
                    LiveVideoRoomManager.Instance.videoPlayScreen.SetActive(true);
                    LiveVideoRoomManager.Instance.videoPlayerUIScreen.SetActive(true);
                    LiveVideoRoomManager.Instance.GetLastAvatarListCount();

                    ARFaceModuleManager.Instance.DisableBottomMainPanel(false);


                    /*if (ARFaceModuleManager.Instance.mainAvatar != null)
                    {
                        ARFaceModuleManager.Instance.mainAvatar.SetActive(false);
                    }
                    if (ARFaceModuleManager.Instance.addAvtarItem.Count != 0)
                    {
                        for (int i = 0; i < ARFaceModuleManager.Instance.addAvtarItem.Count; i++)
                        {
                            ARFaceModuleManager.Instance.addAvtarItem[i].gameObject.SetActive(false);
                        }
                    }*/
                   Debug.Log("videoPath : " + LiveVideoRoomManager.Instance.videoPath + " :Prefix:" + prefix);
                    LiveVideoRoomManager.Instance.OnStartVideoPlay($"{prefix}{recordingPath}", false);
                    LiveVideoRoomManager.Instance.videoPath = $"{prefix}{recordingPath}";
                }
                else
                {//if user canceling video capture then delete file from path
                    string tempPath = $"{prefix}{recordingPath}";
                    if (File.Exists(tempPath))
                    {
                        Debug.Log("Deleteing temp capture video file");
                        File.Delete(tempPath);
                    }
                    ARFaceModuleManager.Instance.r_IsCapturingVideoBack = false;
                }
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
        LiveVideoRoomManager.Instance.VideoRecordingAllUIDisable(true);
        //pressed = false;
        // Stop recording
        audioInput?.Dispose();
        cameraInput.Dispose();
        videoRecorder.Dispose();
        // Mute microphone
        microphoneSource.mute = true;
    }   
}