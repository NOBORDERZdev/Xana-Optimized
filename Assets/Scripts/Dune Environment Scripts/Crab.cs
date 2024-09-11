using System.Collections;
using UnityEngine;

public class Crab : MonoBehaviour
{
    Coroutine cor;
    AudioSource _audioSource;
    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = SoundSettings.soundManagerSettings.totalVolumeSlider.value;
        SoundSettings.soundManagerSettings.OnBGMAudioMuted += DisableCrabSound;
        BuilderEventManager.BGMVolume += BGMVolume;

    }

    private void OnDisable()
    {
        SoundSettings.soundManagerSettings.OnBGMAudioMuted -= DisableCrabSound;
        BuilderEventManager.BGMVolume -= BGMVolume;

    }

    private void DisableCrabSound(bool _mute)
    {
        _audioSource.mute = _mute;
    }
    void BGMVolume(float volume)
    {
        _audioSource.volume = volume;
    }
    public void UpperStart(float upSpeed)
    {
        cor = StartCoroutine(OnUpper(upSpeed));
    }

    IEnumerator OnUpper(float upSpeed)
    {
        GetComponent<AudioSource>().Play();
        float upSize = transform.localScale.y * 3;
        float curr = 0;

        while (curr <= upSize)
        {
            float up = upSpeed * Time.deltaTime;
            curr += up;

            transform.localPosition += new Vector3(0, up, 0);

            yield return null;
        }

        curr = 0;
        yield return new WaitForSeconds(1f);
        upSize *= 2;

        GetComponent<BoxCollider>().enabled = false;

        while (curr <= upSize)
        {
            float up = upSpeed * Time.deltaTime;
            curr += up;

            transform.localPosition -= new Vector3(0, up, 0);

            yield return null;
        }
        Destroy(gameObject);
        StopCoroutine(cor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<XanaDuneControllerHandler>().TouchingCrab();
            ParticleSystem bang = GetComponentInChildren<ParticleSystem>();
            bang.transform.position = other.ClosestPoint(transform.position);
            bang.Play();
        }
    }
}
