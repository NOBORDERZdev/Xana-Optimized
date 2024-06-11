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
    private void OnEnable()
    {
        BuilderEventManager.AfterPlayerInstantiated += StartBGMSound;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= StartBGMSound;
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
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.Play();

            }
        }
    }

    //public static AudioClip ByteArrayToAudioClip(byte[] bytes, int channels, int frequency)
    //{
    //    // Convert binary array to AudioClip
    //    float[] nsamples = new float[bytes.Length / sizeof(float)];
    //    Buffer.BlockCopy(bytes, 0, nsamples, 0, bytes.Length);
    //    AudioClip clip = AudioClip.Create("Generated Clip", nsamples.Length, channels, frequency, false);
    //    clip.SetData(nsamples, 0);

    //    return clip;
    //}
}
