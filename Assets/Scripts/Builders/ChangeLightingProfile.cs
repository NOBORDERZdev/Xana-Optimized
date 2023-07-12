using UnityEngine;
using UnityEngine.Rendering;

public class ChangeLightingProfile : MonoBehaviour
{
    public RenderPipelineAsset xanaProfile;
    public RenderPipelineAsset builderProfile;


    private void Start()
    {
        if(XanaConstants.xanaConstants.isBuilderScene)
        {
            if (QualitySettings.renderPipeline != builderProfile)
                ApplyBuilderSetting();
        }
        else
        {
            if (QualitySettings.renderPipeline != xanaProfile)
                ApplyXanaSetting();
        }
    }

    public void ApplyXanaSetting()
    {
        QualitySettings.renderPipeline = xanaProfile;
        GraphicsSettings.renderPipelineAsset = xanaProfile;
    }

    public void ApplyBuilderSetting()
    {
        QualitySettings.renderPipeline = builderProfile;
        GraphicsSettings.renderPipelineAsset = builderProfile;
    }
}
