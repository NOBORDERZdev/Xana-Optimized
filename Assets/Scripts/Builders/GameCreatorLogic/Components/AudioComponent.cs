using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using Models;
using Photon.Pun;

public class AudioComponent : ItemComponent
{
    public AudioClip audioClip;
    AudioComponentData audioComponentData;

    public void Init(AudioComponentData audioComponentData)
    {
        this.audioComponentData = audioComponentData;
        StartCoroutine(setAudioFromUrl(this.audioComponentData.audioPath));
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine && audioClip != null)
        {
            GamificationComponentData.instance.audioSource.clip = audioClip;
            GamificationComponentData.instance.audioSource.Play();
        }
    }

    IEnumerator setAudioFromUrl(string file_name)
    {
        string[] parts = file_name.Split('_');
        int channels = int.Parse(parts[1]);
        int frequency = int.Parse(parts[2]);
        string name = parts[3];
        string extension = Path.GetExtension(file_name);
        Debug.Log("Channels: " + channels + " ,Frequency: " + frequency + " ,Name: " + name + " ,Extension: " + extension);

        using (UnityWebRequest www = UnityWebRequest.Get(file_name))
        {
            yield return www.Send();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                byte[] data = www.downloadHandler.data;
                // Use the byte data as needed
                AudioClip clip = ByteArrayToAudioClip(data, channels, frequency);
                audioClip = clip;
            }
        }
    }

    public static AudioClip ByteArrayToAudioClip(byte[] bytes, int channels, int frequency)
    {
        // Convert binary array to AudioClip
        float[] nsamples = new float[bytes.Length / sizeof(float)];
        Buffer.BlockCopy(bytes, 0, nsamples, 0, bytes.Length);
        AudioClip clip = AudioClip.Create("Generated Clip", nsamples.Length, channels, frequency, false);
        clip.SetData(nsamples, 0);

        return clip;
    }
}
