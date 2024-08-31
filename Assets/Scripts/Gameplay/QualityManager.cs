using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class QualityManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _landscapeQualityToggles;
    [SerializeField]
    private GameObject[] _portraitQualityToggles;
    [SerializeField]
    private RenderPipelineAsset[] _qualityLevels;
    void Start()
    {
        if (PlayerPrefs.GetInt("DefaultQuality") == 0 && PlayerPrefs.GetInt("QualitySettings") == 0)
        {
            AdjustQualityBasedOnDevice();
        }
        else
        {
            SetQualitySettings(PlayerPrefs.GetInt("QualitySettings"));
        }
    }
    public void SetQualityToggles(int index)
    {
        if (ScreenOrientationManager._instance.isPotrait)
        {
            foreach (GameObject go in _portraitQualityToggles)
            {
                go.SetActive(false);
            }
            _portraitQualityToggles[index].SetActive(true);
        }
        else
        {
            foreach (GameObject go in _landscapeQualityToggles)
            {
                go.SetActive(false);
            }
            _landscapeQualityToggles[index].SetActive(true);
        }
    }
    public void SetQualitySettings(int index)
    {
        if (QualitySettings.GetQualityLevel() != index)
        {
            PlayerPrefs.SetInt("QualitySettings", index);
            QualitySettings.SetQualityLevel(index);
            QualitySettings.renderPipeline = _qualityLevels[index];
            SetQualityToggles(index);
        }
    }
    void AdjustQualityBasedOnDevice()
    {
        int systemMemory = SystemInfo.systemMemorySize;
        if (Application.platform == RuntimePlatform.Android)
        {
            AdjustQualityForAndroid(systemMemory);
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            AdjustQualityForIOS(systemMemory);
        }
        else
        {
            SetQualitySettings(2); // High
        }
    }
    void AdjustQualityForIOS(int systemMemory)
    {
        if (systemMemory < 2048)
        {
            SetQualitySettings(0); // Low
        }
        else if (systemMemory < 4096)
        {
            SetQualitySettings(1); // Medium
        }
        else if (systemMemory <= 6144)
        {
            SetQualitySettings(2);
        }
        else
        {
            SetQualitySettings(3);
        }
    }

    void AdjustQualityForAndroid(int systemMemory)
    {
        if (systemMemory < 2048)
        {
            SetQualitySettings(0); // Low
        }
        else if (systemMemory < 4096)
        {
            SetQualitySettings(1); // Medium
        }
        else
        {
            SetQualitySettings(2); // High
        }
    }
}
