using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SummitDomeImageHandler : MonoBehaviour
{
    public XANASummitDataContainer XANASummitDataContainer;
    public TMPro.TMP_FontAsset DometextFont;
    public Material DomeTextMaterial;
    void OnEnable()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += ApplyDomeShader;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= ApplyDomeShader;
    }

    void ApplyDomeShader()
    {
        for(int i=0;i<XanaWorldDownloader.AllDomes.Count;i++) 
        {
            SummitDomeShaderApply SummitDomeShaderApplyRef = XanaWorldDownloader.AllDomes[i].GetComponent<SummitDomeShaderApply>();
            string [] DomeData= XANASummitDataContainer.GetDomeImage(SummitDomeShaderApplyRef.DomeId);
            SummitDomeShaderApplyRef.ImageUrl = DomeData[0];
            SummitDomeShaderApplyRef.LogoUrl = DomeData[2];
            if(!string.IsNullOrEmpty(DomeData[1]) && string.IsNullOrEmpty(DomeData[2]))
            {
                TMPro.TextMeshPro DomeText1 = SummitDomeShaderApplyRef.DomeText.AddComponent<TMPro.TextMeshPro>();
                DomeText1.font = DometextFont;
                DomeText1.fontMaterial = DomeTextMaterial;
                DomeText1.fontSize = 4.5f;
                DomeText1.alignment = TMPro.TextAlignmentOptions.Center;
                DomeText1.text = DomeData[1];
            }
                SummitDomeShaderApplyRef.Init();
        }
    }
}
