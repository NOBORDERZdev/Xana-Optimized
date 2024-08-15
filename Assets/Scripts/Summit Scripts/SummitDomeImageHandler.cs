using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SummitDomeImageHandler : MonoBehaviour
{
    public XANASummitDataContainer XANASummitDataContainer;
    public Cubemap cubemap;
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
            XanaWorldDownloader.AllDomes[i].GetComponent<SummitDomeShaderApply>().DomeMeshRenderer.materials[1].SetTexture("_Cubemap", cubemap);
            XanaWorldDownloader.AllDomes[i].GetComponent<SummitDomeShaderApply>().ImageUrl = XANASummitDataContainer.GetDomeImage(XanaWorldDownloader.AllDomes[i].GetComponent<SummitDomeShaderApply>().DomeId);
            XanaWorldDownloader.AllDomes[i].GetComponent<SummitDomeShaderApply>().Init();
        }
    }
}
