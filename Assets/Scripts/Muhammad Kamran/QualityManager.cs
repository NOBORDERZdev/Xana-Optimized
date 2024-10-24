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
        BuilderEventManager.AfterPlayerInstantiated += SetQuality;
       
    }
    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= SetQuality;
    }
    private void SetQuality()
    {
        if (PlayerPrefs.GetInt("QualitySettings", 0) == 0)
        {
            //AdjustQualityBasedOnDevice();
            SetQualitySettings(0); // Low
        }
        else
        {
            int temp = PlayerPrefs.GetInt("QualitySettings", 0);
            SetQualitySettings(temp);
        }
    }

    public void SetQualityToggles(int index)
    {
            foreach (GameObject go in _portraitQualityToggles)
            {
                go.SetActive(false);
            }
            _portraitQualityToggles[index].SetActive(true);
        
            foreach (GameObject go in _landscapeQualityToggles)
            {
                go.SetActive(false);
            }
            _landscapeQualityToggles[index].SetActive(true);
        
    }
    public void SetQualitySettings(int index)
    {
        SetQualityToggles(index);
        if (QualitySettings.GetQualityLevel() != index)
        {
            PlayerPrefs.SetInt("QualitySettings", index);
            QualitySettings.SetQualityLevel(index);
            QualitySettings.renderPipeline = _qualityLevels[index];
        } 
    }

}
