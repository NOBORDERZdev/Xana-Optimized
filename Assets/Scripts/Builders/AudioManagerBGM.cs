using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
public class AudioManagerBGM : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip currentClip;
    AudioPropertiesBGM audioPropertiesBGM;

    private void OnEnable()
    {
        BuilderEventManager.BGMDownloader += AudioBGM;
        BuilderEventManager.BGMStart += BGMStart;
    }

    private void OnDisable()
    {
        BuilderEventManager.BGMDownloader -= AudioBGM;
        BuilderEventManager.BGMStart -= BGMStart;
    }

    private void AudioBGM(AudioPropertiesBGM audioPropertiesBGM)
    {
        this.audioPropertiesBGM = audioPropertiesBGM;
        if (!audioPropertiesBGM.dataAudioBGM.pathAudioBGM.IsNullOrEmpty())
            StartCoroutine(setAudioFromUrl(audioPropertiesBGM.dataAudioBGM.pathAudioBGM));
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
                currentClip = clip;
                audioSource.loop = audioPropertiesBGM.dataAudioBGM.audioLoopBGM;
                audioSource.volume = audioPropertiesBGM.dataAudioBGM.audioVolume;
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

    void BGMStart()
    {
        if (currentClip != null)
        {
            audioSource.clip = currentClip;
            audioSource.Play();
        }
    }
}
