using CSCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SummitBGMSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public XANASummitDataContainer summitDataContainer;
    AudioClip clip;
    private void OnEnable()
    {
        BuilderEventManager.AfterPlayerInstantiated += StartBGMSound;
        GamePlayButtonEvents.OnExitButtonXANASummit += StopBGM;
        BuilderEventManager.loadBGMDirectly += SetBGMDirectly;
        BuilderEventManager.StopBGM += StopBGM;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= StartBGMSound;
        GamePlayButtonEvents.OnExitButtonXANASummit -= StopBGM;
        BuilderEventManager.StopBGM -= StopBGM;
    }

    void StartBGMSound()
    {
        if (ConstantsHolder.isFromXANASummit)
        {
            string audioUrl=summitDataContainer.GetAudioFile(ConstantsHolder.domeId);
            StartCoroutine(SetAudioFromUrl(audioUrl));
        }
    }

    IEnumerator SetAudioFromUrl(string file_name)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(file_name,AudioType.MPEG))
        {
            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();

            }
        }
    }

    void SetBGMDirectly(string url)
    {
        StartCoroutine(SetAudioFromUrl(url));
    }


    void StopBGM()
    {
        audioSource.Stop();
        audioSource.clip = null;
        Destroy(clip); 
    }
    
}
