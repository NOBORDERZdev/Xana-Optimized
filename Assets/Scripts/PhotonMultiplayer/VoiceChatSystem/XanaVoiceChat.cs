using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.WSA;

public class XanaVoiceChat : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject micOnBtn;
    public GameObject micOnBtnPotrait;
    public GameObject micOffBtn;
    public GameObject micOffBtnPotrait;
    public Sprite micOnSprite;
    public Sprite micOffSprite;

    private VoiceConnection voiceConnection;
    public Recorder recorder;
    public Speaker speaker;

    private Button micBtn;

    private bool canTalk;
    private bool useMic;

    public UnityAction MicToggleOff, MicToggleOn;
    public static XanaVoiceChat instance;

    [Header("Mic Toast to instatiate")]
    public GameObject mictoast;
    public Transform placetoload;
    public string MicroPhoneDevice;
    public int index;


    public void Awake()
    {
        // Commented by WaqasAhmad
        //if (instance)
        //{
        //    instance.Start();
        //    DestroyImmediate(this.gameObject);
        //}
        //else
        //{
        //    instance = this;
        //}

        if (instance == null)
        {
            instance = this;
        }
    }
    private void OnEnable()
    {
        // Added by Waqas Ahmad
        if (instance != this && instance.recorder != null)
        {

            if (instance.recorder.TransmitEnabled)
            {
                Invoke("TurnOnMic", 0.25f);

            }
            else
            {
                Invoke("TurnOffMic", 0.25f);

            }

            instance = this;
            instance.Start();
        }

        //if (ConstantsHolder.xanaConstants.mic == 1)
        //{
        //    TurnOnMic();
        //}
        //else
        //{
        //    TurnOffMic();
        //}
        //voiceConnection.SpeakerLinked += OnSpeakerCreated;
        //voiceConnection.Client.AddCallbackTarget(this);
    }


    private void Start()
    {
        Debug.Log("Xana VoiceChat Start");
        recorder = GameObject.FindObjectOfType<Recorder>();
        voiceConnection = GetComponent<VoiceConnection>();

        //InvokeRepeating(nameof(MicroPhoneName), 2f, 2f);

        if (!ScreenOrientationManager._instance.isPotrait)
        {
            // There is two instance of this script
            // one used for Landscape & one for Portrait
            // Already Called For Landscape no need to call again.

            //if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            //{
            //    Permission.RequestUserPermission(Permission.Microphone);
            //}
        }
        //Debug.Log("Environment name: " + FeedEventPrefab.m_EnvName + "   " + FeedEventPrefab.m_EnvName.Contains("Xana Festival") + "   " + FeedEventPrefab.m_EnvName.Equals("Xana Festival"));
        if (WorldItemView.m_EnvName.Contains("Xana Festival") || WorldItemView.m_EnvName.Contains("NFTDuel Tournament") || WorldItemView.m_EnvName.Contains("BreakingDown Arena")|| ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            StopRecorder();
            Debug.Log("Its an Event Scene");
            TurnOffMic();
            ConstantsHolder.xanaConstants.mic = 0;
        }
        else
        {
            //#if UNITY_ANDROID
            if (recorder != null)
            {
                recorder.AutoStart = true;
                recorder.Init(voiceConnection);
            }
            //#endif
            MicToggleOff = TurnOnMic;
            MicToggleOn = TurnOffMic;

            Debug.Log("its not an event");

            //micOffBtn = GameObject.Find("MicOffToggle");
            //micOnBtn = GameObject.Find("MicToggle");
            micOffBtn.GetComponent<Button>().onClick.AddListener(MicToggleOff);
            micOffBtnPotrait.GetComponent<Button>().onClick.AddListener(MicToggleOff);
            micOnBtn.GetComponent<Button>().onClick.AddListener(MicToggleOn);
            micOnBtnPotrait.GetComponent<Button>().onClick.AddListener(MicToggleOn);
            if (ConstantsHolder.xanaConstants.EnviornmentName == "DJ Event")
            {
                micOffBtn.SetActive(false);
                micOffBtnPotrait.SetActive(false);
                micOnBtn.SetActive(false);
                micOnBtnPotrait.SetActive(false);
                ConstantsHolder.xanaConstants.mic = 0;
            }
            StartCoroutine(CheckVoiceConnect());

            
        }
    }

    public void MicroPhoneName()
    {
        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            Debug.Log(i + " Microphone " + Microphone.devices[i]);
        }
        Debug.Log("MicroPhoneDevice" + MicroPhoneDevice);
        Debug.Log("Recorder MicroPhone Name" + recorder.MicrophoneDevice.Name);
    }
    public void TurnOnMic()
    {

        if (ConstantsHolder.xanaConstants.mic == 0)
        {
            GameObject go = Instantiate(mictoast, placetoload);
            Destroy(go, 1.5f);
            return;
        }

        print("Turn on mic");
        micOffBtn.SetActive(false);
        micOffBtnPotrait.SetActive(false);
        micOnBtn.SetActive(true);
        micOnBtnPotrait.SetActive(true);
        if (recorder != null)
            recorder.TransmitEnabled = true;


    }

    public void StopRecorder()
    {
        Debug.Log("StopRecorder");
        if (recorder != null)
        {
            recorder.AutoStart = recorder.TransmitEnabled = false;
            recorder.StopRecording();
            recorder.Init(voiceConnection);
        }
    }

    public void TurnOffMic()
    {
        print("Turn off mic");
        micOffBtn.SetActive(true);
        micOffBtnPotrait.SetActive(true);
        micOnBtn.SetActive(false);
        micOnBtnPotrait.SetActive(false);
        if(recorder != null)
        recorder.TransmitEnabled = false;
    }



    public void UpdateMicButton()
    {
        StartCoroutine(CheckVoiceConnect());
    }

    private void OnDisable()
    {
        //voiceConnection.SpeakerLinked -= OnSpeakerCreated;
        //voiceConnection.Client.RemoveCallbackTarget(this);
    }

    protected virtual void OnSpeakerCreated(Speaker _speaker)
    {
        Debug.Log("Speaker is Created");
        speaker = _speaker;
    }

    IEnumerator CheckVoiceConnect()
    {
        while (!PhotonVoiceNetwork.Instance.Client.IsConnected)
        {
            //Debug.Log("Still Connecting");
            yield return null;
        }
        //recorder.TransmitEnabled = true;
        recorder.DebugEchoMode = false;
        if (ConstantsHolder.xanaConstants.mic == 1 && !ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            TurnOnMic();
        }
        else
        {
            TurnOffMic();
        }


        //GetLocalSpeaker();
        //ToggleVoiceChat(false);
    }

    void GetLocalSpeaker()
    {
        Debug.Log("Checking if local speaker ready");
        StartCoroutine(WaitAndSearchForLocalSpeaker());
    }

    IEnumerator WaitAndSearchForLocalSpeaker()
    {
        Speaker[] _speaker = FindObjectsOfType<Speaker>(); ;

        while (speaker == null)
        {
            //  Debug.Log("Get Local Speaker: " + _speaker.Length);

            yield return null;

            //if (voiceConnection.speak.Count != 0)
            //    speaker = voiceConnection.linkedSpeakers[0];
        }

        ToggleVoiceChat(false);
    }

    public void ToggleMic(bool _useMic)
    {
        //if (_useMic && !canTalk)
        //{
        //    ShowVoiceChatDialogBox();
        //    return;
        //}

        //useMic = _useMic;

        //if (useMic)
        //{


        //    micOnBtn.SetActive(true);
        //    micOffBtn.SetActive(false);

        //    recorder.TransmitEnabled = true;
        //}
        //else
        //{
        //    micOnBtn.SetActive(false);
        //    micOffBtn.SetActive(true);

        //    recorder.TransmitEnabled = false;
        //}
    }

    void ShowVoiceChatDialogBox()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, "Please turn on \"Voice\" from the settings", 0);
                toastObject.Call("show");
            }));
        }
#endif
    }

    public void ToggleVoiceChat(bool _canTalk)
    {
        canTalk = _canTalk;

        if (!canTalk)
        {
            Debug.Log("Speaker: " + speaker);

            if (speaker != null)
            {
                speaker.GetComponent<AudioSource>().mute = true;

            }
            if (recorder != null)
            {
                ToggleMic(false);
            }
        }
        else
        {
            if (speaker != null)
            {
                speaker.GetComponent<AudioSource>().mute = false;
            }
            if (recorder != null)
            {
                //ToggleMic(true);
            }
        }
    }


}
