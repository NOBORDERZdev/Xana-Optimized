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
            PlayerPrefs.SetInt("QualitySettings", 1);
            PlayerPrefs.SetInt("DefaultQuality", 1);
        }
        SetQualitySettings(PlayerPrefs.GetInt("QualitySettings"));
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
}
