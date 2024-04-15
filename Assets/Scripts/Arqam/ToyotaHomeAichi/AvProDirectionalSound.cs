using RenderHeads.Media.AVProVideo;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


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
    private float defaultMaxDis = 0;
    private Slider landscapeSlider;
    private float sliderValue = 0f;


    private void OnEnable()
    {
        InRoomSoundHandler.soundAction += Mute_UnMute_Sound;
        ChangeOrientation_waqas.switchOrientation += ChangeOrientation;
    }
    private void OnDisable()
    {
        InRoomSoundHandler.soundAction -= Mute_UnMute_Sound;
        ChangeOrientation_waqas.switchOrientation -= ChangeOrientation;
        if (volumeCoroutine != null)
            StopCoroutine(volumeCoroutine);
    }

    private void Start()
    {
        updateDelay = new WaitForSeconds(updateInterval);

        defaultMaxDis = maxDistance;

        sliderValue = SoundManagerSettings.soundManagerSettings.totalVolumeSlider.value;
        landscapeSlider = SoundManagerSettings.soundManagerSettings.totalVolumeSlider;

        //Landscape mode
        SoundManagerSettings.soundManagerSettings.totalVolumeSlider.onValueChanged.AddListener((float vol) =>
        {
            UpdateSliderValue(vol);
        });
        //Potrait mode
        SoundManagerSettings.soundManagerSettings.totalVolumeSliderPotrait.onValueChanged.AddListener((float vol) =>
        {
            UpdateSliderValue(vol);
        });

        AddTriggerOnSlider(landscapeSlider);
    }

    private void AddTriggerOnSlider(Slider slider)
    {
        EventTrigger eventTrigger = slider.gameObject.AddComponent<EventTrigger>();
        if (ChangeOrientation_waqas._instance.isPotrait)
            AddEventTrigger(eventTrigger, EventTriggerType.PointerUp, OnPointerUp2);
        else
            AddEventTrigger(eventTrigger, EventTriggerType.PointerUp, OnPointerUp);
    }


   
    void UpdateSliderValue(float value)
    {
            if (sliderValue < value)
            {
                maxDistance = Mathf.Clamp(maxDistance + sliderValue / 3, defaultMaxDis, 40f);
            }
            else if (sliderValue > value)
            {
                maxDistance = Mathf.Clamp(maxDistance - sliderValue / 3, defaultMaxDis, maxDistance);
            }
            sliderValue = value;
    }

    private void Mute_UnMute_Sound(bool flag)
    {
        audioSource.mute = flag;

        if (flag)
        {
            if (volumeCoroutine != null)
                StopCoroutine(volumeCoroutine);
            activePlayer.AudioVolume = 0f;

        }
        else
        {
            volumeCoroutine = StartCoroutine(AdjustScreenVolume());
            //maxDistance = defaultMaxDis;
        }
    }


    public void ActiveDirectionalSound()
    {
        if (activePlayer.gameObject.activeSelf)
            volumeCoroutine = StartCoroutine(AdjustScreenVolume());
    }

    IEnumerator AdjustScreenVolume()
    {
        playerCam = ReferrencesForDynamicMuseum.instance.m_34player.transform;

        while (true)
        {
            if (!activePlayer.gameObject.activeSelf)
            {
                if (volumeCoroutine != null)
                    StopCoroutine(volumeCoroutine);
                yield break;
            }
            // Calculate the distance between player/camera and video source
            if (playerCam != null)
            {
                float distance = Vector3.Distance(playerCam.position, transform.position);
                // Clamp the distance within the range
                distance = Mathf.Clamp(distance, minDistance, maxDistance);

                // Map the distance to the volume level (adjust this mapping based on your needs)
                float mappedVolume = 1f - Mathf.InverseLerp(minDistance, maxDistance, distance);

                // Set the video volume using the third-party package's method
                activePlayer.AudioVolume = mappedVolume;
                yield return updateDelay;
            }
            else
                yield break;
        }
    }

    private void AddEventTrigger(EventTrigger eventTrigger, EventTriggerType triggerType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        // Create a new Entry for the EventTrigger
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = triggerType;
        entry.callback.AddListener((data) => callback((BaseEventData)data));

        // Add the entry to the EventTrigger
        eventTrigger.triggers.Add(entry);
    }

    private void OnPointerUp(BaseEventData eventData)
    {
        if (sliderValue < 0.5f)
            maxDistance = defaultMaxDis;
    }
    private void OnPointerUp2(BaseEventData eventData)
    {
        if (sliderValue < 0.5f)
            maxDistance = defaultMaxDis;
    }

    private void ChangeOrientation()
    {
        if (ChangeOrientation_waqas._instance.isPotrait)
        {
            sliderValue = SoundManagerSettings.soundManagerSettings.totalVolumeSliderPotrait.value;
            AddTriggerOnSlider(SoundManagerSettings.soundManagerSettings.totalVolumeSliderPotrait);
        }
        else
        {
            sliderValue = SoundManagerSettings.soundManagerSettings.totalVolumeSlider.value;
            AddTriggerOnSlider(SoundManagerSettings.soundManagerSettings.totalVolumeSlider);
        }
    }


}
