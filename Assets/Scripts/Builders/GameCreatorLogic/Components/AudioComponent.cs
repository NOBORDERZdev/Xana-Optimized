using System.Collections;
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
        if (this.audioComponentData.audioPath != "")
            StartCoroutine(SetAudioFromUrl(this.audioComponentData.audioPath));
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.CompareTag("PhotonLocalPlayer") && _other.gameObject.GetComponent<PhotonView>().IsMine && audioClip != null)
        {
            GamificationComponentData.instance.audioSource.clip = audioClip;
            GamificationComponentData.instance.audioSource.Play();
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("PhotonLocalPlayer") && collision.gameObject.GetComponent<PhotonView>().IsMine)
        {
            float stopAudioAfterSeconds = audioClip.length;
            if (stopAudioAfterSeconds > 3) stopAudioAfterSeconds = 3; // Some clips are too long (up to 130 seconds)
            Invoke(nameof(StopAudio), stopAudioAfterSeconds);
        }
    }

    private void StopAudio()
    {
        GamificationComponentData.instance.audioSource.Stop();
    }


    private IEnumerator SetAudioFromUrl(string file_name)
    {
        string[] parts = file_name.Split('_');
        int channels = int.Parse(parts[1]);
        int frequency = int.Parse(parts[2]);
        string name = parts[3];
        string extension = Path.GetExtension(file_name);
        Debug.Log($"Channels: {channels} ,Frequency: {frequency} ,Name: {name} ,Extension: {extension}");

        using (UnityWebRequest www = UnityWebRequest.Get(file_name))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
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
            www.Dispose();
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


    #region BehaviourControl
    private void StartComponent()
    {

    }
    private void StopComponent()
    {


    }

    public override void StopBehaviour()
    {
        if (isPlaying)
        {
            isPlaying = false;
            StopComponent();
        }
    }

    public override void PlayBehaviour()
    {
        isPlaying = true;
        StartComponent();
    }

    public override void ToggleBehaviour()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
            PlayBehaviour();
        else
            StopBehaviour();
    }
    public override void ResumeBehaviour()
    {
        PlayBehaviour();
    }

    public override void AssignItemComponentType()
    {
        _componentType = Constants.ItemComponentType.AudioComponent;
    }

    public override void CollisionExitBehaviour()
    {
        //throw new NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //CollisionEnter();
    }

    #endregion
}