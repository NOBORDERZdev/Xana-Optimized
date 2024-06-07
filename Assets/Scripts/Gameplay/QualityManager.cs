using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class QualityManager : MonoBehaviour
{
    public Toggle[] LandscapeQualityToggles;
    public Toggle[] PortraitQualityToggles;
    public RenderPipelineAsset[] QualityLevels;
    void Start()
    {
        //int i = PlayerPrefs.GetInt("QualitySettings");
        if (PlayerPrefs.GetInt("DefaultQuality") == 0 && PlayerPrefs.GetInt("QualitySettings") == 0)
        {
            PlayerPrefs.SetInt("QualitySettings", 1);
            PlayerPrefs.SetInt("DefaultQuality", 1);
        }
        //QualityToggles[PlayerPrefs.GetInt("QualitySettings")].isOn = true;
        SetQualityToggles(PlayerPrefs.GetInt("QualitySettings"));
        //SetQualitySettings(PlayerPrefs.GetInt("QualitySettings"));
    }
    public void SetQualityToggles(int index)
    {
        if(ScreenOrientationManager._instance.isPotrait)
            PortraitQualityToggles[index].isOn = true;
        else
            LandscapeQualityToggles[index].isOn = true;
    }
    public void SetQualitySettings(int index)
    {
        if (QualitySettings.GetQualityLevel() != index)
        {
            PlayerPrefs.SetInt("QualitySettings", index);
            QualitySettings.SetQualityLevel(index);
            QualitySettings.renderPipeline = QualityLevels[index];
        }
    }
}
