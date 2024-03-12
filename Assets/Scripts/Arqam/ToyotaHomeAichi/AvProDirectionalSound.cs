using RenderHeads.Media.AVProVideo;
using System.Collections;
using UnityEngine;

public class AvProDirectionalSound : MonoBehaviour
{
    public float maxDistance = 10f; // Max distance for full volume
    public float minDistance = 2f; // Min distance for minimum volume
    [Space(5)]
    [Range(0.2f, 0.5f)]
    public float updateInterval = 0.5f; // Time interval for volume update

    private Transform playerCam; // Reference to your player object or camera
    private WaitForSeconds updateDelay;
    private Coroutine volumeCoroutine;
    public MediaPlayer activePlayer;
    public AudioSource audioSource;


    private void OnEnable()
    {
        BuilderEventManager.AfterPlayerInstantiated += GetActivePlayer;
        InRoomSoundHandler.playerInRoom += Mute_UnMute_Sound;
    }
    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= GetActivePlayer;
        InRoomSoundHandler.playerInRoom -= Mute_UnMute_Sound;
    }

    private void Start()
    {
        updateDelay = new WaitForSeconds(updateInterval);
    }

    private void Mute_UnMute_Sound(bool flag)
    {
        if (flag)
        {
            audioSource.Stop();
            if (volumeCoroutine != null)
                StopCoroutine(volumeCoroutine);
            activePlayer.AudioVolume = 0f;
        }
        else
        {
            audioSource.Play();
            volumeCoroutine = StartCoroutine(AdjustScreenVolume());
        }
    }


    private void GetActivePlayer()
    {

        if (activePlayer.gameObject.activeSelf)
        {
            playerCam = ReferrencesForDynamicMuseum.instance.m_34player.transform;
            volumeCoroutine = StartCoroutine(AdjustScreenVolume());
        }
    }


    IEnumerator AdjustScreenVolume()
    {
        while (true)
        {
            if (!activePlayer.gameObject.activeSelf)
            {
                if (volumeCoroutine != null)
                    StopCoroutine(volumeCoroutine);
                yield break;
            }
            // Calculate the distance between player/camera and video source
            float distance = Vector3.Distance(playerCam.position, transform.position);

            // Clamp the distance within the range
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            // Map the distance to the volume level (adjust this mapping based on your needs)
            float mappedVolume = 1f - Mathf.InverseLerp(minDistance, maxDistance, distance);

            // Set the video volume using the third-party package's method
            activePlayer.AudioVolume = mappedVolume;
            yield return updateDelay;
        }
    }


}
