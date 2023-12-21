using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvProDirectionalSound : MonoBehaviour
{
    public MediaPlayer mediaPlayer;
    [Space(5)]
    public float maxDistance = 10f; // Max distance for full volume
    public float minDistance = 2f; // Min distance for minimum volume
    [Space(5)]
    [Range(0.2f, 0.5f)]
    public float updateInterval = 0.5f; // Time interval for volume update

    private Transform playerCam; // Reference to your player object or camera
    private WaitForSeconds updateDelay;

    private void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("MainCamera").transform;

        updateDelay = new WaitForSeconds(updateInterval);
        StartCoroutine(AdjustScreenVolume());
    }

    IEnumerator AdjustScreenVolume()
    {
        while (true)
        {
            // Calculate the distance between player/camera and video source
            float distance = Vector3.Distance(playerCam.position, transform.position);

            // Clamp the distance within the range
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            // Map the distance to the volume level (adjust this mapping based on your needs)
            float mappedVolume = 1f - Mathf.InverseLerp(minDistance, maxDistance, distance);
            //Debug.Log("<color=red> Distance: " + distance + "</color>");  
            //Debug.Log("<color=red> Volume: " + mappedVolume + "</color>");

            // Set the video volume using the third-party package's method
            mediaPlayer.AudioVolume = mappedVolume;

            yield return updateDelay;
        }
    }


}
