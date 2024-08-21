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
            SummitDomeShaderApplyRef.DomeText.AddComponent<TMPro.TextMeshPro>();
            SummitDomeShaderApplyRef.DomeText.GetComponent<TMPro.TextMeshPro>().font=DometextFont;
            SummitDomeShaderApplyRef.DomeText.GetComponent<MeshRenderer>().material = DomeTextMaterial;
            SummitDomeShaderApplyRef.DomeText.GetComponent<TMPro.TextMeshPro>().fontSize=4.5f;
            SummitDomeShaderApplyRef.DomeText.GetComponent<TMPro.TextMeshPro>().alignment = TMPro.TextAlignmentOptions.Center;
            SummitDomeShaderApplyRef.DomeText.GetComponent<TMPro.TextMeshPro>().text = DomeData[1];
            SummitDomeShaderApplyRef.Init();
        }
    }
}
