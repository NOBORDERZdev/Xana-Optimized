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
using System.Threading.Tasks;
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
            //if (instance.recorder.TransmitEnabled)
            //{
            //    Invoke("TurnOnMic", 0.25f);

            //}
            //else
            //{
            //    Invoke("TurnOffMic", 0.25f);

            //}

            instance = this;
            //instance.Start();
        }
        BuilderEventManager.AfterPlayerInstantiated += CheckMicPermission;
    }
    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= CheckMicPermission;
    }

    private void CheckMicPermission()
    {
        if (!ScreenOrientationManager._instance.isPotrait)
        {
            // There is two instance of this script
            // one used for Landscape & one for Portrait
            // Already Called For Landscape no need to call again.
            if (Application.isEditor)
            {
                PermissionPopusSystem.Instance.onCloseAction += SetMic;
                PermissionPopusSystem.Instance.textType = PermissionPopusSystem.TextType.Mic;
                PermissionPopusSystem.Instance.OpenPermissionScreen();
            }
            else
            {
                NativeGallery.Permission permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
#if UNITY_ANDROID
                if (permission == NativeGallery.Permission.ShouldAsk)
                {
                    PermissionPopusSystem.Instance.onCloseAction += SetMic;
                    PermissionPopusSystem.Instance.textType = PermissionPopusSystem.TextType.Mic;
                    PermissionPopusSystem.Instance.OpenPermissionScreen();
                }
                else
                {
                    SetMic();
                }
#elif UNITY_IOS
                if(PlayerPrefs.GetInt("MicPermission", 0) == 0){
                     PermissionPopusSystem.Instance.onCloseAction += SetMic;
            PermissionPopusSystem.Instance.textType = PermissionPopusSystem.TextType.Mic;
            PermissionPopusSystem.Instance.OpenPermissionScreen();
                }
                else
                {
                    SetMic();
                }
#endif
            }
        }
    }

    public void SetMic()      //Start()
    {
        PermissionPopusSystem.Instance.onCloseAction -= SetMic;
#if !UNITY_EDITOR && UNITY_IOS
        PlayerPrefs.SetInt("MicPermission", 1);
#endif
        recorder = GameObject.FindObjectOfType<Recorder>();
        voiceConnection = GetComponent<VoiceConnection>();

        if (!ScreenOrientationManager._instance.isPotrait)
        {
            // There is two instance of this script
            // one used for Landscape & one for Portrait
            // Already Called For Landscape no need to call again.

            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
        }
        if (WorldItemView.m_EnvName.Contains("Xana Festival") || WorldItemView.m_EnvName.Contains("NFTDuel Tournament") || WorldItemView.m_EnvName.Contains("BreakingDown Arena"))
        {
            StopRecorder();
            TurnOffMic();
            ConstantsHolder.xanaConstants.mic = 0;
        }
        else
        {
            if (recorder != null)
            {
                recorder.AutoStart = true;
                recorder.Init(voiceConnection);
            }

            if (ConstantsHolder.xanaConstants.pushToTalk)
            {
                micOffBtn.AddComponent<PushToTalk>();
                micOffBtnPotrait.AddComponent<PushToTalk>();
                TurnOffMic();
            }
            else
            {
                MicToggleOff = TurnOnMic;
                MicToggleOn = TurnOffMic;

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
    }
    public void TurnOnMic()
    {
        if (ConstantsHolder.xanaConstants.mic == 0)
        {
            GameObject go = Instantiate(mictoast, placetoload);
            Destroy(go, 1.5f);
            return;
        }
        micOffBtn.SetActive(false);
        micOffBtnPotrait.SetActive(false);
        micOnBtn.SetActive(true);
        micOnBtnPotrait.SetActive(true);
        if (recorder != null)
            recorder.TransmitEnabled = true;
    }

    public void TurnOffMic()
    {
        micOffBtn.SetActive(true);
        micOffBtnPotrait.SetActive(true);
        micOnBtn.SetActive(false);
        micOnBtnPotrait.SetActive(false);
        if (recorder != null)
            recorder.TransmitEnabled = false;
    }


    //Overriding methods for push to talk 
    public async void PushToTalk(bool canTalk)
    {
        if (canTalk)
        {
            micOffBtn.transform.GetChild(0).gameObject.SetActive(false);
            micOffBtnPotrait.transform.GetChild(0).gameObject.SetActive(false);
            if (recorder != null)
                recorder.TransmitEnabled = true;

        }
        else
        {
            micOffBtn.transform.GetChild(0).gameObject.SetActive(true);
            micOffBtnPotrait.transform.GetChild(0).gameObject.SetActive(true);
            while (recorder.IsCurrentlyTransmitting)
            {
                await Task.Delay(1000);
            }
            if (recorder != null)
                recorder.TransmitEnabled = false;

        }
    }

    public void StopRecorder()
    {
        if (recorder != null)
        {
            recorder.AutoStart = recorder.TransmitEnabled = false;
            recorder.StopRecording();
            recorder.Init(voiceConnection);
        }
    }



    IEnumerator CheckVoiceConnect()
    {
        while (!PhotonVoiceNetwork.Instance.Client.IsConnected)
        {
            yield return null;
        }
        recorder.DebugEchoMode = false;
        if (ConstantsHolder.xanaConstants.mic == 1 && !ConstantsHolder.xanaConstants.pushToTalk)
        {
            TurnOnMic();
        }
        else
        {
            TurnOffMic();
        }
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

}
