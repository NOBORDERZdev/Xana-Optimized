using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToBoxerAvatar : MonoBehaviour
{
    public GameObject defaultSkinLight;
    public GameObject PaleIvory_SkinLight;
    public GameObject Porcelean_SkinLight;
    public GameObject Ivory_SkinLight;
    public GameObject Sienna_SkinLight;
    public GameObject Sand_SkinLight;
    public GameObject Almond_SkinLight;
    public GameObject GoldenBrown_SkinLight;
    public GameObject Umber_SkinLight;
    public GameObject Burgundy_SkinLight;
    public GameObject Cacao_SkinLight;


    public SkinnedMeshRenderer headBlendShape;
    public SkinnedMeshRenderer bodyBlenShape;

    public Shader eyeNormalShader;
    public Shader nftEyeShader;
    public Material eyeMaterial;


    private void OnEnable()
    {
        BoxerNFTEventManager.OnNFTequipShaderUpdate += OnNFTEquipShaderUpdate;
        BoxerNFTEventManager.OnNFTUnequipShaderUpdate += OnNFTUnequipShaderUpdate;

        BoxerNFTEventManager.NFTLightUpdate += SwitchLight;
    }

    private void OnDisable()
    {
        BoxerNFTEventManager.OnNFTequipShaderUpdate -= OnNFTEquipShaderUpdate;
        BoxerNFTEventManager.OnNFTUnequipShaderUpdate -= OnNFTUnequipShaderUpdate;

        BoxerNFTEventManager.NFTLightUpdate -= SwitchLight;
    }


    public void OnNFTEquipShaderUpdate()
    {
        eyeMaterial.shader = nftEyeShader;

        headBlendShape.materials[2].SetFloat("_Normal_Strength", .8f);
        bodyBlenShape.materials[0].SetFloat("_Normal_Strength", .8f);

        headBlendShape.SetBlendShapeWeight(54, 100);
        bodyBlenShape.SetBlendShapeWeight(0, 100);
    }

    public void OnNFTUnequipShaderUpdate()
    {
        eyeMaterial.shader = eyeNormalShader;

        headBlendShape.materials[2].SetFloat("_Normal_Strength", 0f);
        bodyBlenShape.materials[0].SetFloat("_Normal_Strength", 0f);

        headBlendShape.SetBlendShapeWeight(54, 0);
        bodyBlenShape.SetBlendShapeWeight(0, 0);
    }

    public void SwitchLight(LightPresetNFT lightPresetNFT)
    {
        DisableAllLighting();
        switch (lightPresetNFT)
        {
            case LightPresetNFT.DefaultSkin:
                defaultSkinLight.SetActive(true);
                break;
            case LightPresetNFT.PaleIvory:
                PaleIvory_SkinLight.SetActive(true);
                break;
            case LightPresetNFT.Porcelean:
                Porcelean_SkinLight.SetActive(true);
                break;
            case LightPresetNFT.Ivory:
                Ivory_SkinLight.SetActive(true);
                break;
            case LightPresetNFT.Sienna:
                Sienna_SkinLight.SetActive(true);
                break;
            case LightPresetNFT.Sand:
                Sand_SkinLight.SetActive(true);
                break;
            case LightPresetNFT.Almond:
                Almond_SkinLight.SetActive(true);
                break;
            case LightPresetNFT.GoldenBrown:
                GoldenBrown_SkinLight.SetActive(true);
                break;
            case LightPresetNFT.Umber:
                Umber_SkinLight.SetActive(true);
                break;
            case LightPresetNFT.Burgundy:
                Burgundy_SkinLight.SetActive(true);
                break;
            case LightPresetNFT.Cacao:
                Cacao_SkinLight.SetActive(true);
                break;
        }
    }

    void DisableAllLighting()
    {
        defaultSkinLight.SetActive(false);
        PaleIvory_SkinLight.SetActive(false);
        Porcelean_SkinLight.SetActive(false);
        Ivory_SkinLight.SetActive(false);
        Sienna_SkinLight.SetActive(false);
        Sand_SkinLight.SetActive(false);
        Almond_SkinLight.SetActive(false);
        GoldenBrown_SkinLight.SetActive(false);
        Umber_SkinLight.SetActive(false);
        Burgundy_SkinLight.SetActive(false);
        Cacao_SkinLight.SetActive(false);
    }



    public void OnFullCostumeWear()
    {
        headBlendShape.materials[2].SetFloat("_Normal_Strength", 0f);
        bodyBlenShape.materials[0].SetFloat("_Normal_Strength", 0f);

        headBlendShape.SetBlendShapeWeight(54, 0);
        bodyBlenShape.SetBlendShapeWeight(0, 0);
    }
}
