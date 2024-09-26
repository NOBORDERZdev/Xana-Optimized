using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;

public class CharcterBodyParts : MonoBehaviour
{
    public static CharcterBodyParts instance;
    [Tooltip("Default Texture for pent and shirt")]
    public Texture Shirt_Texture, Pent_Texture, Shoe_Texture, Eye_Texture, TattooDefaultTexture;
    // For Eye Color Slider Require Some new Textures
    public Texture Eye_Mask_Texture, Eye_Color_Texture;
    private string shirt_TextureName, Pent_TextureName, Shoes_TextureName, Glove_TextureName,
        Skin_ColorName, Hair_ColorName, Lip_ColorName, Eyebrow_ColorName, Eye_ColorName, // Using For Color Modification through Slider
        eyeLen_TextureName, eyeLashes_TextureName, Makeup_TextureName, GredientColorName, SssIntensity, EyeBrrow_TextureName;


    //Mask Texture Name in shader
    private string faceTattoo_MaskPropertyName, chestTattoo_MaskPropertyName, legsTattoo_MaskPropertyName, armTattoo_MaskPropertyName, mustache_MaskPropertyName, eyeLid_MaskPropertyName;
    //Color Property Names in Shader 
    private string facetattooColorPropertyName, chesttattooColorPropertyName, legstattooColorPropertyName, armtattooColorPropertyName, mustacheColorPropertyName, eyeLidColorPropertyName;
    //Color Constrast Property Names in Shader
    private string faceTattooContrastPropertyName, chestTattooContrastPropertyName, legsTattooContrastPropertyName, armTattooPropertyContrastName, mustacheTattooContrastPropertyName, eyeLidContrastPropertyName;

    //single color for tattoo and mustache
    [HideInInspector]
    public string faceTattooColor = "Black";
    [HideInInspector]
    public string faceTattooColorDefault = "Black";
    [HideInInspector]
    public string foreheafTattooColor = "BronzeBlack";
    string chestTattooColor = "Balck";
    string legsTattooColor = "BronzeBlack";
    string armTattooColor = "BronzeBlack";
    string mustacheColor = "Brown";
    string eyeLidColor = "OrangeBrown";

    [SerializeField]
    public Color DefaultSkinColor, DefaultEyebrowColor, DefaultHairColor, DefaultLipColor, DefaultGredientColor;
    public SkinnedMeshRenderer Body;
    [Tooltip("Region for Bones")]
    public GameObject[] BothEyes, EyeIner, EyesOut, BothLips;
    public GameObject Body_Bone, JBone, Head, Nose, Lips, PelvisBone, ForeHead, headAll;

    [Tooltip("Values For Bones Movement as slider take float")]
    [HideInInspector]
    public float scaleEyeX, EyeScaleY, MouthX, MouthY, NoseX, NoseY, ForeHeadX, ForeHeadY, ForeheadZ, JBoneX, JBoneY, JBoneZ, Lipscalex;

    [Header("skinToon Color List")]
    public List<Color> skinColor;
    public List<Color> lipColor;
    public List<Color> skinGredientColor;
    public List<Color> hairColor;
    public List<Color> eyeBrowsColor;
    public List<Color> eyeColor;
    public List<Color> lipColorPalette;
    public List<GameObject> _scaleBodyParts;


    //[Header("Testing")]
    //public TextMeshProUGUI boneName;
    //public TextMeshProUGUI xvalue, yvalue;
    //[HideInInspector]
    public List<BoneDataContainer> BonesData = new List<BoneDataContainer>();

    public Texture2D defaultMakeup, defaultEyelashes, defaultEyebrow;
    public Sprite defaultPngForSkinIcon;
    public static Action<Color> OnSkinColorApply;
    public float defaultSssValue;
    public Color PresetGredientColor;
    //public List<BoneDataContainer> DefaultBones = new List<BoneDataContainer>();

    private Material characterHeadMat, characterBodyMat;


    public AvatarController avatarController;

    private void Awake()
    {
        instance = this;
        shirt_TextureName = "_Shirt_Mask";
        Pent_TextureName = "_Pant_Mask";
        Shoes_TextureName = "_Shoes_Mask";
        Glove_TextureName = "_Gloves_Mask";
        Skin_ColorName = "_BaseColor";
        Hair_ColorName = "_Color";
        Lip_ColorName = "_Lips_Color";
        Eyebrow_ColorName = "_BaseColor";
        Eye_ColorName = "_Mask_Color";
        eyeLen_TextureName = "_Main_Trexture";
        eyeLashes_TextureName = "_BaseMap";
        Makeup_TextureName = "_Base_Texture";
        GredientColorName = "_Color";
        SssIntensity = "_SSS_Intensity";
        EyeBrrow_TextureName = "_BaseMap";

        //tattoo mask and color property names
        faceTattoo_MaskPropertyName = "_Face_Tattoo_Mask";
        chestTattoo_MaskPropertyName = "_Chest_Tattoo_Mask";
        legsTattoo_MaskPropertyName = "_Legs_Tattoo_Mask";
        armTattoo_MaskPropertyName = "_Arm_Tattoo_Mask";
        mustache_MaskPropertyName = "_Mustache_Mask";
        eyeLid_MaskPropertyName = "_Eyelid_Mask";
        facetattooColorPropertyName = "_Face_Tattoo_Color";
        chesttattooColorPropertyName = "_Chest_Tattoo_Color";
        legstattooColorPropertyName = "_Legs_Tattoo_Color";
        armtattooColorPropertyName = "_Arm_Tattoo_Color";
        mustacheColorPropertyName = "_Mustache_Color";
        eyeLidColorPropertyName = "_Eyelid_Color";
        faceTattooContrastPropertyName = "_Face_Tattoo_Contrast";
        chestTattooContrastPropertyName = "_Chest_Tattoo_Contrast";
        legsTattooContrastPropertyName = "_Legs_Tattoo_Contrast";
        armTattooPropertyContrastName = "_Arm_Tattoo_Contrast";
        mustacheTattooContrastPropertyName = "_Mustache_Tattoo_Contrast";
        eyeLidContrastPropertyName = "_Eyelid_Contrast";

        defaultSssValue = 2.5f;
        IntCharacterBones();
    }

    private void Start()
    {
        blend = BlendShapeImporter.Instance;
        avatarController = GetComponent<AvatarController>();

        characterHeadMat = Head.GetComponent<SkinnedMeshRenderer>().materials[2];
        characterBodyMat = Body_Bone.GetComponent<SkinnedMeshRenderer>().materials[0];
    }


    //Set Texture For Shirt
    public void TextureForShirt(Texture texture)
    {
        Body.materials[0].SetTexture(shirt_TextureName, texture);
    }


    // Set texture For 
    public void TextureForPant(Texture texture)
    {
        Body.materials[0].SetTexture(Pent_TextureName, texture);
    }

    public void TextureForShoes(Texture texture)
    {
        Body.materials[0].SetTexture(Shoes_TextureName, texture);
    }

    public void TextureForGlove(Texture texture)
    {
        Body.materials[0].SetTexture(Glove_TextureName, texture);
    }

    // Set Default 
    public void DefaultTexture(bool ApplyClothMask = true)
    {
        if (ApplyClothMask)
        {
            Body.materials[0].SetTexture(Pent_TextureName, Pent_Texture);
            Body.materials[0].SetTexture(shirt_TextureName, Shirt_Texture);
            Body.materials[0].SetTexture(Shoes_TextureName, Shoe_Texture);
        }
        Body.materials[0].SetColor(Skin_ColorName, DefaultSkinColor);
        Body.materials[0].SetColor(GredientColorName, DefaultGredientColor);
        Body.materials[0].SetFloat(SssIntensity, defaultSssValue);

        SkinnedMeshRenderer HeadMeshComponent = Head.GetComponent<SkinnedMeshRenderer>();

        //HeadMeshComponent.material.SetColor(Skin_ColorName, DefaultSkinColor);
        //HeadMeshComponent.material.SetColor(Lip_ColorName, DefaultLipColor);
        //HeadMeshComponent.material.SetColor(Eyebrow_ColorName, DefaultEyebrowColor);

        HeadMeshComponent.materials[2].SetColor(Skin_ColorName, DefaultSkinColor);
        HeadMeshComponent.materials[2].SetColor(Lip_ColorName, DefaultLipColor);

        HeadMeshComponent.materials[0].SetTexture(eyeLen_TextureName, Eye_Texture);
        // After EyeShader update need to pass this texture to another property
        HeadMeshComponent.materials[0].SetTexture("_Mask_texture", Eye_Texture);

        HeadMeshComponent.materials[1].SetTexture(EyeBrrow_TextureName, defaultEyebrow);
        HeadMeshComponent.materials[1].SetColor(Eyebrow_ColorName, DefaultEyebrowColor);

        HeadMeshComponent.materials[3].SetTexture(eyeLashes_TextureName, defaultEyelashes);
        HeadMeshComponent.materials[2].SetTexture(Makeup_TextureName, defaultMakeup);
        HeadMeshComponent.materials[2].SetColor(GredientColorName, DefaultGredientColor);
        HeadMeshComponent.materials[2].SetFloat(SssIntensity, defaultSssValue);
        //Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(SssIntensity, 0f);

        //set dafault texture for shader to not impact on any other property like skin color 
        RemoveTattoo(null, null, CurrentTextureType.FaceTattoo);
        RemoveTattoo(null, null, CurrentTextureType.ChestTattoo);
        RemoveTattoo(null, null, CurrentTextureType.LegsTattoo);
        RemoveTattoo(null, null, CurrentTextureType.ArmTattoo);
        RemoveMustacheTexture(null, null);
        RemoveEyeLidTexture(null, null);



    }



    BlendShapeImporter blend;
    public void ValuesForSliderXY(List<int> morphsList)
    {
        float min = 0;
        float max = 100;
        float currentValue = 0;
        int blendIndex = 0;

        #region Comment Section 
        //if (BlendShapeController.instance.allBlendShapes[morphsList[0]].boneAvailable)
        //{
        //    print("Yes Bone");
        //    foreach (var item in BlendShapeController.instance.allBlendShapes[morphsList[0]].boneData)
        //    {
        //        print("---- AxesDetails : " + item.workingAxes);


        //        if (item.workingAxes == AxesDetails.z)
        //        {
        //            if (BlendShapeController.instance.allBlendShapes[morphsList[0]].AxisType == AxisType.X_Axis)
        //            {
        //                // X-Slider
        //                if (item.workingAxes == AxesDetails.x)
        //                {
        //                    // Set Min Max Values
        //                    blend.SliderX.maxValue = item.maxValue;
        //                    blend.SliderX.minValue = item.minValue;

        //                    // Setting Slider Value
        //                    Transform bone = BlendShapeController.instance.allBlendShapes[morphsList[0]].boneObj.transform;
        //                    blend.SliderX.value = GetBoneCurrentValue('x', item.workingTransform, bone);
        //                }
        //            }
        //        }

        //        // X-Slider
        //        else if (item.workingAxes == AxesDetails.x)
        //        {
        //            // Set Min Max Values
        //            blend.SliderX.maxValue = item.maxValue;
        //            blend.SliderX.minValue = item.minValue;

        //            // Setting Slider Value
        //            Transform bone = BlendShapeController.instance.allBlendShapes[morphsList[0]].boneObj.transform;
        //            blend.SliderX.value = GetBoneCurrentValue('x', item.workingTransform, bone);

        //            blend.SliderX.onValueChanged.AddListener(delegate { BoneSliderCallBack('x', item.workingTransform, bone, blend.SliderX.value); });
        //            blend.SliderX.gameObject.SetActive(true);
        //        }

        //        //Y-Slider
        //        else if (item.workingAxes == AxesDetails.y)
        //        {
        //            // Set Min Max Values
        //            blend.SliderY.maxValue = item.maxValue;
        //            blend.SliderY.minValue = item.minValue;

        //            // Setting Slider Value
        //            Transform bone = BlendShapeController.instance.allBlendShapes[morphsList[0]].boneObj.transform;
        //            blend.SliderY.value = GetBoneCurrentValue('x', item.workingTransform, bone);

        //            blend.SliderY.onValueChanged.AddListener(delegate { BoneSliderCallBack('y', item.workingTransform, bone, blend.SliderY.value); });
        //            blend.SliderY.gameObject.SetActive(true);
        //        }

        //        // XY Both Axis with single slider
        //        else if (item.workingAxes == AxesDetails.xy)
        //        {
        //            // Get Bone Reference
        //            Transform bone = BlendShapeController.instance.allBlendShapes[morphsList[0]].boneObj.transform;

        //            print("Sub Axis :" + BlendShapeController.instance.allBlendShapes[morphsList[0]].AxisType);
        //            // Check which slider require 
        //            // X Slider or Y Slider
        //            if (BlendShapeController.instance.allBlendShapes[morphsList[0]].AxisType == AxisType.X_Axis)
        //            {
        //                // Set Min Max Values
        //                blend.SliderX.maxValue = item.maxValue;
        //                blend.SliderX.minValue = item.minValue;

        //                // all axis has same value so use anyone of them
        //                blend.SliderX.value = GetBoneCurrentValue('x', item.workingTransform, bone);
        //                blend.SliderX.onValueChanged.AddListener(delegate
        //                {
        //                    // Need to modify values for all axis
        //                    BoneSliderCallBack('a', item.workingTransform, bone, blend.SliderX.value);
        //                });
        //                blend.SliderX.gameObject.SetActive(true);
        //            }
        //            else
        //            {
        //                // Set Min Max Values
        //                blend.SliderY.maxValue = item.maxValue;
        //                blend.SliderY.minValue = item.minValue;

        //                // all axis has same value so use anyone of them
        //                blend.SliderY.value = GetBoneCurrentValue('x', item.workingTransform, bone);
        //                print("Assigning Listners : " + BlendShapeController.instance.allBlendShapes[morphsList[0]].itemName);
        //                blend.SliderY.onValueChanged.AddListener(delegate
        //                {
        //                    // Need to modify values for all axis
        //                    BoneSliderCallBack('a', item.workingTransform, bone, blend.SliderY.value);
        //                });
        //                blend.SliderY.gameObject.SetActive(true);
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    //Blend Shape Working
        //    foreach (var item in morphsList)
        //    {
        //        min = BlendShapeController.instance.allBlendShapes[item].minValue;
        //        max = BlendShapeController.instance.allBlendShapes[item].maxValue;
        //        blendIndex = BlendShapeController.instance.allBlendShapes[item].index;

        //        currentValue = Head.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(blendIndex);

        //        // X-Slider
        //        if (BlendShapeController.instance.allBlendShapes[item].AxisType == AxisType.X_Axis)
        //        {
        //            // Set Min Max Values
        //            blend.SliderX.maxValue = max;
        //            blend.SliderX.minValue = min;

        //            // Setting Slider Value
        //            blend.SliderX.value = Head.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(blendIndex);

        //            // Due To some unity Issue need to create local variables
        //            int indexTemp = blendIndex;

        //            // Adding Listener
        //            blend.SliderX.onValueChanged.AddListener(delegate { SliderXCallBack(blend.SliderX.value, indexTemp); });
        //            blend.SliderX.gameObject.SetActive(true);
        //        }
        //        else // Y-Slider
        //        {
        //            // Set Min Max Values
        //            blend.SliderY.maxValue = max;
        //            blend.SliderY.minValue = min;

        //            // Setting Slider Value
        //            blend.SliderY.value = Head.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(blendIndex);

        //            // Due To some unity Issue need to create local variables
        //            int indexTemp = blendIndex;

        //            // Adding Listener
        //            blend.SliderY.onValueChanged.AddListener(delegate { SliderYCallBack(blend.SliderY.value, indexTemp); });
        //            blend.SliderY.gameObject.SetActive(true);
        //        }
        //    }
        //}

        #endregion

        for (int i = 0; i < morphsList.Count; i++)
        {
            if (BlendShapeController.instance.allBlendShapes[morphsList[i]].boneAvailable)
            {
                print("Yes Bone");
                foreach (var item in BlendShapeController.instance.allBlendShapes[morphsList[i]].boneData)
                {
                    print("---- AxesDetails : " + item.workingAxes);

                    Transform bone = BlendShapeController.instance.allBlendShapes[morphsList[i]].boneObj.transform;
                    // Has only 2 slider x & y
                    // If z axis need to modify than use above mention sliders[x,y]
                    if (item.workingAxes == AxesDetails.z)
                    {
                        if (BlendShapeController.instance.allBlendShapes[morphsList[i]].AxisType == AxisType.X_Axis)
                            SetSliderReleatedDataForBone(blend.SliderX, 'z', 'z', item, bone);
                        else
                            SetSliderReleatedDataForBone(blend.SliderY, 'z', 'z', item, bone);
                    }

                    // X-Slider
                    else if (item.workingAxes == AxesDetails.x)
                    {
                        SetSliderReleatedDataForBone(blend.SliderX, 'x', 'x', item, bone);
                    }

                    //Y-Slider
                    else if (item.workingAxes == AxesDetails.y)
                    {
                        //SetSliderReleatedDataForBone(blend.SliderY, 'x', 'y', item, bone);
                        SetSliderReleatedDataForBone(blend.SliderY, 'y', 'y', item, bone);
                    }

                    // XYZ Axis with single slider
                    else if (item.workingAxes == AxesDetails.xy)
                    {
                        // char 'a' stand for All Axis
                        if (BlendShapeController.instance.allBlendShapes[morphsList[i]].AxisType == AxisType.X_Axis)
                            SetSliderReleatedDataForBone(blend.SliderX, 'x', 'a', item, bone);
                        else
                            SetSliderReleatedDataForBone(blend.SliderY, 'x', 'a', item, bone);
                    }
                }
            }
            else
            {
                print("No Bone");
                //Blend Shape Working
                min = BlendShapeController.instance.allBlendShapes[morphsList[i]].minValue;
                max = BlendShapeController.instance.allBlendShapes[morphsList[i]].maxValue;
                blendIndex = BlendShapeController.instance.allBlendShapes[morphsList[i]].index;

                currentValue = Head.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(blendIndex);
                bool localReverse = BlendShapeController.instance.allBlendShapes[morphsList[i]].reverseMyValue;

                // X-Slider
                if (BlendShapeController.instance.allBlendShapes[morphsList[i]].AxisType == AxisType.X_Axis)
                {
                    if (!localReverse)
                    {
                        // Set Min Max Values
                        blend.SliderX.maxValue = max;
                        blend.SliderX.minValue = min;

                        // Setting Slider Value
                        blend.SliderX.value = Head.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(blendIndex);
                    }

                    // Due To some unity Issue need to create local variables
                    int indexTemp = blendIndex;

                    // Adding Listener
                    blend.SliderX.onValueChanged.AddListener(delegate { SliderXCallBack(blend.SliderX.value, indexTemp, localReverse); });
                    blend.SliderX.gameObject.SetActive(true);
                }
                else // Y-Slider
                {
                    if (!localReverse)
                    {
                        // Set Min Max Values
                        blend.SliderY.maxValue = max;
                        blend.SliderY.minValue = min;

                        // Setting Slider Value
                        blend.SliderY.value = Head.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(blendIndex);
                    }

                    // Due To some unity Issue need to create local variables
                    int indexTemp = blendIndex;

                    // Adding Listener
                    blend.SliderY.onValueChanged.AddListener(delegate { SliderYCallBack(blend.SliderY.value, indexTemp, localReverse); });
                    blend.SliderY.gameObject.SetActive(true);
                }
            }
        }
    }

    public void SliderXCallBack(float value, int blendInd, bool reverseMyValue = false)
    {
        if (reverseMyValue)
        {
            value = blend.SliderX.maxValue - value;
        }
        Head.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(blendInd, value);
    }
    public void SliderYCallBack(float value, int blendInd, bool reverseMyValue = false)
    {
        if (reverseMyValue)
        {
            float maxValue = blend.SliderY.maxValue;
            value = maxValue - value;
        }
        Head.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(blendInd, value);
    }


    float GetBoneCurrentValue(char axis, TransformDetails td, Transform bone)
    {
        if (axis == 'x')
        {
            switch (td)
            {
                case TransformDetails.scale:
                    return bone.localScale.x;

                case TransformDetails.rotation:
                    return bone.eulerAngles.x;

                case TransformDetails.postion:
                    return bone.localPosition.x;
            }
        }
        else if (axis == 'y')
        {
            switch (td)
            {
                case TransformDetails.scale:
                    return bone.localScale.y;

                case TransformDetails.rotation:
                    return bone.eulerAngles.y;

                case TransformDetails.postion:
                    return bone.localPosition.y;
            }
        }
        else if (axis == 'z')
        {
            switch (td)
            {
                case TransformDetails.scale:
                    return bone.localScale.z;

                case TransformDetails.rotation:
                    return bone.eulerAngles.z;

                case TransformDetails.postion:
                    return bone.localPosition.z;
            }
        }
        return 0;
    }
    public void BoneSliderCallBack(char axis, TransformDetails td, Transform bone, float value, bool reverseValue = false)
    {
        Vector3 tempVector = Vector3.zero;

        if (reverseValue)
            value = (-1) * value;

        if (axis == 'x')
        {
            switch (td)
            {
                case TransformDetails.scale:
                    bone.localScale = new Vector3(value, bone.localScale.y, bone.localScale.z);
                    break;

                case TransformDetails.rotation:
                    bone.eulerAngles = new Vector3(value, bone.eulerAngles.y, bone.eulerAngles.z);
                    break;

                case TransformDetails.postion:
                    bone.localPosition = new Vector3(value, bone.localPosition.y, bone.localPosition.z);
                    break;
            }
        }
        else if (axis == 'y')
        {
            switch (td)
            {
                case TransformDetails.scale:
                    bone.localScale = new Vector3(bone.localScale.x, value, bone.localScale.z);
                    break;

                case TransformDetails.rotation:
                    bone.eulerAngles = new Vector3(bone.eulerAngles.x, value, bone.eulerAngles.z);
                    break;

                case TransformDetails.postion:
                    bone.localPosition = new Vector3(bone.localPosition.x, value, bone.localPosition.z);
                    break;
            }
        }
        else if (axis == 'z')
        {
            switch (td)
            {
                case TransformDetails.scale:
                    bone.localScale = new Vector3(bone.localScale.x, bone.localScale.y, value);
                    break;

                case TransformDetails.rotation:
                    bone.eulerAngles = new Vector3(bone.eulerAngles.x, bone.eulerAngles.y, value);
                    break;

                case TransformDetails.postion:
                    bone.localPosition = new Vector3(bone.localPosition.x, bone.localPosition.y, value);
                    break;
            }
        }
        else if (axis == 'a') // All Axis
        {
            switch (td)
            {
                case TransformDetails.scale:
                    bone.localScale = Vector3.one * value;
                    break;

                case TransformDetails.rotation:
                    bone.eulerAngles = Vector3.one * value;
                    break;

                case TransformDetails.postion:
                    bone.localPosition = Vector3.one * value;
                    break;
            }
        }
    }
    void SetSliderReleatedDataForBone(UnityEngine.UI.Slider selectedSlider, char effectedAxis, char callBackAxis, BoneData item, Transform bone)
    {
        // Set Min Max Values
        selectedSlider.maxValue = item.maxValue;
        selectedSlider.minValue = item.minValue;

        // Setting Slider Value
        selectedSlider.value = GetBoneCurrentValue(effectedAxis, item.workingTransform, bone);
        selectedSlider.onValueChanged.AddListener(delegate
        {
            BoneSliderCallBack(callBackAxis, item.workingTransform, bone, selectedSlider.value, item.reverseMyvalue);
        });

        selectedSlider.gameObject.SetActive(true);
    }


    public void ApplyGredientColor(Color gredientColor, GameObject applyOn)
    {
        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(GredientColorName, gredientColor);
        applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(GredientColorName, gredientColor);
    }
    public void ApplyGredientDefault(GameObject applyOn)
    {
        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(GredientColorName, DefaultGredientColor);
        applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(GredientColorName, DefaultGredientColor);
    }


    public void ChangeSkinColor(int colorInd)
    {
        //Head.GetComponent<SkinnedMeshRenderer>().materials[2].color = skinColor[colorInd];
        //Body.materials[0].color = skinColor[colorInd];

        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, skinColor[colorInd]);
        ApplyGredientColor(skinGredientColor[colorInd], this.gameObject);
        Body.materials[0].SetColor(Skin_ColorName, skinColor[colorInd]);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(SssIntensity, defaultSssValue);
        //Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(SssIntensity, 0f);

        Body.materials[0].SetFloat(SssIntensity, defaultSssValue);

    }
    public void ChangeSkinColor(Color color)
    {
        //print("Change Skin 2 : " + color);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, color);
        Body.materials[0].SetColor(Skin_ColorName, color);

    }
    public void ChangeSkinColorSlider(int colorInd)
    {
        OnSkinColorApply?.Invoke(skinColor[colorInd]);
    }

    public void ChangeLipColor(int colorInd)
    {
        //print("Lip Working Here : " + colorInd);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, lipColor[colorInd]);


        //if (colorInd != 0) // is not Deafult lip
        //{
        //    Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, lipColor[colorInd]);
        //}
        //else
        //{
        //    Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, DefaultLipColor);
        //}
    }
    public void ChangeLipColorForPalette(int colorInd)
    {
        //print("Lip Working Here : " + colorInd);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, lipColorPalette[colorInd]);
    }
    public void ChangeLipColor(Color color)
    {
        //print("Lip Working Here : " + color);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, color);
    }
    public void ChangeEyebrowColor(Color color)
    {
        //print("Change Eyebrow From Slider : " + color);
        Head.GetComponent<SkinnedMeshRenderer>().materials[1].SetColor(Eyebrow_ColorName, color);
    }
    public void ChangeEyebrowColor(int index)
    {
        //print("Change Eyebrow From Slider : " + eyeBrowsColor[index]);
        Head.GetComponent<SkinnedMeshRenderer>().materials[1].SetColor(Eyebrow_ColorName, eyeBrowsColor[index]);
    }
    public void ChangeHairColor(Color color)
    {
        // print("Change Hair From Slider : " + color);
        if (avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("_Band"))
        {
            // For Band using Eye Shader so variable name is Changed 
            // Variable is equal to eyename
            avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Eye_ColorName, color);
        }
        else if (avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials.Length > 1) // In case Of Hat there is 2 material
        {
            if (avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("Cap") ||
               avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("Hat"))
                avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[1].SetColor(Hair_ColorName, color);
            else
                avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, color);
        }
        else
            avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, color);

        //avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, color);
    }
    public void ChangeHairColor(int colorId)
    {
        //print("Change Hair From Color Panel : " + hairColor[colorId]);
        if (avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("_Band"))
        {
            // For Band using Eye Shader so variable name is Changed 
            // Variable is equal to eyename
            avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Eye_ColorName, hairColor[colorId]);
        }
        else if (avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials.Length > 1) // In case Of Hat there is 2 material
        {
            if (avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("Cap") ||
               avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("Hat"))
                avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[1].SetColor(Hair_ColorName, hairColor[colorId]);
            else
                avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, hairColor[colorId]);
        }
        else
            avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, hairColor[colorId]);

        //avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, color);
    }
    public void ChangeEyeColor(Color color)
    {
        //print("Change eye Color From Slider : " + color);
        // Change color if default texture selected
        // else do nothing

        //if default texture than update mask so color reflect init
        // _Main_Trexture //Color_Texture 
        // _Mask_texture
        // _Emission_Texture

        //print("Eye Texture Name : " + Head.GetComponent<SkinnedMeshRenderer>().materials[0].GetTexture("_Main_Trexture").name);
        string currentTextureName = Head.GetComponent<SkinnedMeshRenderer>().materials[0].GetTexture("_Main_Trexture").name.ToLower();
        if (currentTextureName == "xana_eye_default" || currentTextureName == "eye_color_texture")
        {
            if (currentTextureName == "xana_eye_default")
            {
                Head.GetComponent<SkinnedMeshRenderer>().materials[0].SetTexture("_Main_Trexture", Eye_Color_Texture);
                Head.GetComponent<SkinnedMeshRenderer>().materials[0].SetTexture("_Mask_texture", Eye_Mask_Texture);
            }
            Head.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Eye_ColorName, color);
        }
    }
    public void ChangeEyeColor(int index)
    {
        print("Change eye Color From Slider : " + eyeColor[index]);
        // Change color if default texture selected
        // else do nothing

        //if default texture than update mask so color reflect init
        // _Main_Trexture //Color_Texture 
        // _Mask_texture
        // _Emission_Texture

        print("Eye Texture Name : " + Head.GetComponent<SkinnedMeshRenderer>().materials[0].GetTexture("_Main_Trexture").name);
        string currentTextureName = Head.GetComponent<SkinnedMeshRenderer>().materials[0].GetTexture("_Main_Trexture").name.ToLower();
        if (currentTextureName == "xana_eye_default" || currentTextureName == "eye_color_texture")
        {
            if (currentTextureName == "xana_eye_default")
            {
                Head.GetComponent<SkinnedMeshRenderer>().materials[0].SetTexture("_Main_Trexture", Eye_Color_Texture);
                Head.GetComponent<SkinnedMeshRenderer>().materials[0].SetTexture("_Mask_texture", Eye_Mask_Texture);
            }
            Head.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Eye_ColorName, eyeColor[index]);
        }
    }
    //public void SetIntensityDefault() {
    //    Body.materials[0].SetColor(GredientColorName, Color.black);
    //    Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(SssIntensity, 0);
    //   // Body.materials[0].SetFloat(SssIntensity, 0);
    //}

    /// <summary>
    /// Call for have SssIntensity, SkinGredientColor
    /// </summary>
    /// <param name="savedColor"></param>
    /// <param name="skinGredientColor"></param>
    /// <param name="SssValue"></param>
    public void ImplementSavedSkinColor(Color savedColor, /*Color skinGredientColor,*/ float SssValue)
    {
        //Head.GetComponent<SkinnedMeshRenderer>().materials[2].color = savedColor;
        //Body.materials[0].color = savedColor;
        print("Change Skin 3 : " + savedColor);

        if (new Vector3(savedColor.r, savedColor.b, savedColor.g) != new Vector3(0.00f, 0.00f, 0.00f) /*!SkinColor.Compare(Color.black)*/)
        {
            Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, savedColor);
            Body.materials[0].SetColor(Skin_ColorName, savedColor);
            //Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(GredientColorName, DefaultGredientColor);
            //Body.materials[0].SetColor(GredientColorName, DefaultGredientColor);
        }
        else
        {
            Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, DefaultSkinColor);
            Body.materials[0].SetColor(Skin_ColorName, DefaultSkinColor);
            //Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(GredientColorName, skinGredientColor);
            //Body.materials[0].SetColor(GredientColorName, skinGredientColor);
        }
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(SssIntensity, SssValue);
        // Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(SssIntensity, 0);

        // Body.materials[0].SetFloat(SssIntensity, SssValue);
    }

    /// <summary>
    /// Call for not having SsIntensirty, GredientColorName
    /// </summary>
    /// <param name="savedColor"></param>
    /// <param name="skinGredientColor"></param>
    /// <param name="SssValue"></param>
    public void ImplementSavedSkinColor(Color savedColor)
    {
        //Head.GetComponent<SkinnedMeshRenderer>().materials[2].color = savedColor;
        //Body.materials[0].color = savedColor;
        print("Change Skin 3 : " + savedColor);

        if (new Vector3(savedColor.r, savedColor.b, savedColor.g) != new Vector3(0.00f, 0.00f, 0.00f) /*!SkinColor.Compare(Color.black)*/)
        {
            Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, savedColor);
            Body.materials[0].SetColor(Skin_ColorName, savedColor);
        }
        else
        {
            Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, DefaultSkinColor);
            Body.materials[0].SetColor(Skin_ColorName, DefaultSkinColor);
        }
        //Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(GredientColorName, DefaultGredientColor);
        //Body.materials[0].SetColor(GredientColorName, DefaultGredientColor);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(SssIntensity, defaultSssValue);
        //Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(SssIntensity, 0f);

        //Body.materials[0].SetFloat(SssIntensity, defaultSssValue);
    }

    /// <summary>
    ///  Call for not having SsIntensirty, GredientColorName
    /// </summary>
    /// <param name="SkinColor"></param>
    /// <param name="LipColor"></param>
    /// <param name="applyOn"></param>
    /// <returns></returns>
    public IEnumerator ImplementColors(Color SkinColor, Color LipColor, GameObject applyOn)
    {
        print("Change Skin 4 : " + SkinColor);
        yield return new WaitForSeconds(0f);
        if (new Vector3(SkinColor.r, SkinColor.b, SkinColor.g) != new Vector3(0.00f, 0.00f, 0.00f) /*!SkinColor.Compare(Color.black)*/)
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, SkinColor);
            applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(Skin_ColorName, SkinColor);
        }
        else
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, DefaultSkinColor);
            applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(Skin_ColorName, DefaultSkinColor);
        }

        if (new Vector3(LipColor.r, LipColor.b, LipColor.g) != new Vector3(0.00f, 0.00f, 0.00f))
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, LipColor);
        }
        else
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, DefaultLipColor);
        }

    }
    public IEnumerator ImplementColors(Color SkinColor, Color LipColor, Color HairColor, Color EyebrowColor, Color EyeColor, GameObject applyOn)
    {
        print("Change Skin 4 : " + SkinColor);
        yield return new WaitForSeconds(0f);
        if (new Vector3(SkinColor.r, SkinColor.b, SkinColor.g) != new Vector3(0.00f, 0.00f, 0.00f) /*!SkinColor.Compare(Color.black)*/)
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, SkinColor);
            applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(Skin_ColorName, SkinColor);
        }
        else
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, DefaultSkinColor);
            applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(Skin_ColorName, DefaultSkinColor);
        }

        if (new Vector3(LipColor.r, LipColor.b, LipColor.g) != new Vector3(0.00f, 0.00f, 0.00f))
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, LipColor);
        }
        else
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, DefaultLipColor);
        }

        // Hair
        if (new Vector3(HairColor.r, HairColor.b, HairColor.g) != new Vector3(0.00f, 0.00f, 0.00f))
        {
            if (applyOn.GetComponent<CharcterBodyParts>().avatarController.wornHair != null)
                applyOn.GetComponent<CharcterBodyParts>().avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, HairColor);
        }

        // EyeBrow
        if (new Vector3(EyebrowColor.r, EyebrowColor.b, EyebrowColor.g) != new Vector3(0.00f, 0.00f, 0.00f) && EyebrowColor != Color.white)
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[1].SetColor(Eyebrow_ColorName, EyebrowColor);
        }
        else
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[1].SetColor(Eyebrow_ColorName, DefaultEyebrowColor);
        }

        // Eyes
        if (new Vector3(EyeColor.r, EyeColor.b, EyeColor.g) != new Vector3(0.00f, 0.00f, 0.00f))
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Eye_ColorName, EyeColor);
        }
        else
        {
            applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Eye_ColorName, Color.white);
        }
    }

    // public enum ObjColor { skinColor,lipColor,hairColor,eyebrowColor,eyeColor}
    public IEnumerator ImplementColors(Color _color, SliderType _objColor, GameObject applyOn)
    {
        yield return new WaitForSeconds(0f);

        switch (_objColor)
        {
            case SliderType.Skin:
                if (new Vector3(_color.r, _color.b, _color.g) != new Vector3(0.00f, 0.00f, 0.00f) /*!SkinColor.Compare(Color.black)*/)
                {
                    applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, _color);
                    applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(Skin_ColorName, _color);
                }
                else
                {
                    applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, DefaultSkinColor);
                    applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(Skin_ColorName, DefaultSkinColor);
                }
                break;

            case SliderType.HairColor:
                if (new Vector3(_color.r, _color.b, _color.g) != new Vector3(0.00f, 0.00f, 0.00f) /*!SkinColor.Compare(Color.black)*/)
                {
                    AvatarController ac = applyOn.GetComponent<AvatarController>();
                    if (ac.wornHair != null)
                    {
                        //ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, _color);
                        if (ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("_Band"))
                        {
                            // For Band using Eye Shader so variable name is Changed 
                            // Variable is equal to eyename
                            ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Eye_ColorName, _color);
                        }
                        else if (ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials.Length > 1) // In case Of Hat there is 2 material
                        {
                            if (ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("Cap") ||
                               ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("Hat"))
                                ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[1].SetColor(Hair_ColorName, _color);
                            else
                                ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, _color);
                        }
                        else
                            ac.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Hair_ColorName, _color);
                    }

                }
                break;

            case SliderType.EyeBrowColor:
                if (new Vector3(_color.r, _color.b, _color.g) != new Vector3(0.00f, 0.00f, 0.00f) && _color != Color.white)
                {
                    applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[1].SetColor(Eyebrow_ColorName, _color);
                }
                else
                {
                    applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[1].SetColor(Eyebrow_ColorName, DefaultEyebrowColor);
                }
                break;

            case SliderType.EyesColor:
                if (new Vector3(_color.r, _color.b, _color.g) != new Vector3(0.00f, 0.00f, 0.00f))
                {
                    applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Eye_ColorName, _color);
                }
                else
                {
                    applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[0].SetColor(Eye_ColorName, Color.white);
                }
                break;

            case SliderType.LipsColor:
                if (new Vector3(_color.r, _color.b, _color.g) != new Vector3(0.00f, 0.00f, 0.00f) /*!SkinColor.Compare(Color.black)*/)
                {
                    applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, _color);
                }
                else
                {
                    applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, DefaultLipColor);
                }
                break;

            default:
                break;
        }
    }


    ///// <summary>
    /////  Call for  having SsIntensirty, GredientColorName
    ///// </summary>
    ///// <param name="SkinColor"></param>
    ///// <param name="LipColor"></param>
    ///// <param name="applyOn"></param>
    ///// <returns></returns>
    //public IEnumerator ImplementColors(Color SkinColor, Color LipColor, Color gredientColor, GameObject applyOn)
    //{
    //    print("Change Skin 4 : " + SkinColor);
    //    yield return new WaitForSeconds(0);
    //    if (new Vector3(SkinColor.r, SkinColor.b, SkinColor.g) != new Vector3(0.00f, 0.00f, 0.00f) /*!SkinColor.Compare(Color.black)*/)
    //    {
    //        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, SkinColor);
    //        //applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(SssIntensity, 0);
    //        applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(Skin_ColorName, SkinColor);

    //    }
    //    else
    //    {
    //        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Skin_ColorName, DefaultSkinColor);
    //        //applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(SssIntensity, 0);
    //        applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(Skin_ColorName, DefaultSkinColor);


    //    }

    //    if (new Vector3(LipColor.r, LipColor.b, LipColor.g) != new Vector3(0.00f, 0.00f, 0.00f))
    //    {
    //        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, LipColor);
    //    }
    //    else
    //    {
    //        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(Lip_ColorName, DefaultLipColor);
    //    }

    //    if (new Vector3(gredientColor.r, gredientColor.b, gredientColor.g) != new Vector3(0.00f, 0.00f, 0.00f) )
    //    {
    //        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(GredientColorName, gredientColor);
    //        applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(GredientColorName, gredientColor);
    //    }
    //    else
    //    {
    //        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(GredientColorName, DefaultGredientColor);
    //        applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetColor(GredientColorName, DefaultGredientColor);
    //        //applyOn.GetComponent<CharcterBodyParts>().Body.materials[0].SetFloat(SssIntensity, defaultSssValue);
    //    }

    //}

    /// <summary>
    /// Intializa Character Bones from the character body script for saving.
    /// </summary>
    public void IntCharacterBones()
    {
        // BonesData.Clear();
        // Eye
        foreach (var bone in BothEyes)
        {
            BonesData.Add(new BoneDataContainer(bone.name, bone.gameObject, bone.transform.localPosition, bone.transform.localEulerAngles, bone.transform.localScale));
        }

        // Eye Inner
        foreach (var bone in EyeIner)
        {
            BonesData.Add(new BoneDataContainer(bone.name, bone.gameObject, bone.transform.localPosition, bone.transform.localEulerAngles, bone.transform.localScale));
        }

        // Eye out
        foreach (var bone in EyesOut)
        {
            BonesData.Add(new BoneDataContainer(bone.name, bone.gameObject, bone.transform.localPosition, bone.transform.localEulerAngles, bone.transform.localScale));
        }

        // Lips
        foreach (var bone in BothLips)
        {
            BonesData.Add(new BoneDataContainer(bone.name, bone.gameObject, bone.transform.localPosition, bone.transform.localEulerAngles, bone.transform.localScale));
        }

        Transform singleBone;
        // jaw
        singleBone = JBone.transform;
        BonesData.Add(new BoneDataContainer(singleBone.name, singleBone.gameObject, singleBone.transform.localPosition, singleBone.transform.localEulerAngles, singleBone.transform.localScale));
        // head
        singleBone = Head.transform;
        BonesData.Add(new BoneDataContainer(singleBone.name, singleBone.gameObject, singleBone.transform.localPosition, singleBone.transform.localEulerAngles, singleBone.transform.localScale));
        // nose
        singleBone = Nose.transform;
        BonesData.Add(new BoneDataContainer(singleBone.name, singleBone.gameObject, singleBone.transform.localPosition, singleBone.transform.localEulerAngles, singleBone.transform.localScale));
        // mouth
        singleBone = Lips.transform;
        BonesData.Add(new BoneDataContainer(singleBone.name, singleBone.gameObject, singleBone.transform.localPosition, singleBone.transform.localEulerAngles, singleBone.transform.localScale));
        // Fore head
        singleBone = ForeHead.transform;
        BonesData.Add(new BoneDataContainer(singleBone.name, singleBone.gameObject, singleBone.transform.localPosition, singleBone.transform.localEulerAngles, singleBone.transform.localScale));
        // Head all
        singleBone = headAll.transform;
        BonesData.Add(new BoneDataContainer(singleBone.name, singleBone.gameObject, singleBone.transform.localPosition, singleBone.transform.localEulerAngles, singleBone.transform.localScale));
        //[WaqasAhmad] New Bone Add After character Scaling From Design End
        singleBone = headAll.transform.transform.parent.transform;
        BonesData.Add(new BoneDataContainer(singleBone.name, singleBone.gameObject, singleBone.transform.localPosition, singleBone.transform.localEulerAngles, singleBone.transform.localScale));



        // Fat
        foreach (var bone in _scaleBodyParts)
        {
            BonesData.Add(new BoneDataContainer(bone.name, bone.gameObject, bone.transform.localPosition, bone.transform.localEulerAngles, bone.transform.localScale));
        }
        //// saving default Transfrom of bones
        //foreach (var item in BonesData)
        //{
        //    DefaultBones.Add(new  BoneDataContainer(item.Name,item.Obj, item.Obj.transform.position, item.Obj.transform.localScale));
        //}

        // EyesBlinking.instance.StoreBlendShapeValues();          // Added by Ali Hamza
        //StartCoroutine(EyesBlinking.instance.BlinkingStartRoutine());
    }

    public void ApplyMaskTexture(string type, Texture texture, GameObject applyOn)
    {
        if (type.Contains("Chest") || type.Contains("Shirt") || type.Contains("arabic"))
        {
            applyOn.GetComponent<CharcterBodyParts>().TextureForShirt(texture);

            //CharcterBodyParts.instance.TextureForShirt(texture);
        }
        else if (type.Contains("Legs") || type.Contains("pant") || type.Contains("Pants"))
        {
            applyOn.GetComponent<CharcterBodyParts>().TextureForPant(texture);
        }
        else if (type.Contains("Feet") || type.Contains("Shose", System.StringComparison.CurrentCultureIgnoreCase) || type.Contains("Plain_mask", System.StringComparison.CurrentCultureIgnoreCase))
        {
            applyOn.GetComponent<CharcterBodyParts>().TextureForShoes(texture);
        }
        else if (type.Contains("Glove"))
        {
            applyOn.GetComponent<CharcterBodyParts>().TextureForGlove(texture);
        }
    }

    //public void ApplyTexture(string name, Texture texture)
    //{
    //    if (name.Contains("shirt", System.StringComparison.CurrentCultureIgnoreCase))
    //    {
    //        CharcterBodyParts.instance.TextureForShirt(texture);

    //    }
    //    else if (name.Contains("pant", System.StringComparison.CurrentCultureIgnoreCase))
    //    {
    //        CharcterBodyParts.instance.GetComponent<CharcterBodyParts>().TextureForPant(texture);
    //    }
    //    else if (name.Contains("shoes") || name.Contains("Shoes") || name.Contains("Shose", System.StringComparison.CurrentCultureIgnoreCase))
    //    {
    //        CharcterBodyParts.instance.TextureForShoes(texture);
    //    }
    //}
    public void ApplyEyeLenTexture(Texture texture, GameObject applyOn)
    {
        Material mainMaterial = applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[0];
        // _Main_Trexture
        // _Mask_texture
        // _Emission_Texture

        mainMaterial.SetTexture(eyeLen_TextureName, texture);

        // Update Mask Texture As well & reset Its Color

        if (texture.name.ToLower() == "eye_color_texture" && mainMaterial.GetColor(Eye_ColorName) != Color.white)
        {
            mainMaterial.SetTexture("_Mask_texture", Eye_Mask_Texture);
        }
        else
        {
            mainMaterial.SetTexture("_Mask_texture", Eye_Color_Texture);
            mainMaterial.SetColor(Eye_ColorName, Color.white);
        }
        // After EyeShader update need to pass this texture to another property
        //applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[0].SetTexture("_Emission_Texture", texture);
    }
    public void ApplyEyeBrowTexture(Texture texture, GameObject applyOn)
    {
        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[1].SetTexture(EyeBrrow_TextureName, texture);
        if (BlendShapeController.instance != null)
            BlendShapeController.instance.ResetEyeBrowBlendValues();
    }

    public void ApplyEyeBrow(Texture texture, GameObject applyOn)
    {
        Head.GetComponent<SkinnedMeshRenderer>().materials[1].SetTexture(EyeBrrow_TextureName, texture);
    }

    public void ApplyTattoo(Texture texture, GameObject applyOn, CurrentTextureType nFTOjectType)
    {
        characterHeadMat = Head.GetComponent<SkinnedMeshRenderer>().materials[2];
        characterBodyMat = Body_Bone.GetComponent<SkinnedMeshRenderer>().materials[0];
        switch (nFTOjectType)
        {
            case CurrentTextureType.FaceTattoo:
                characterHeadMat.SetTexture(faceTattoo_MaskPropertyName, texture);
                characterHeadMat.SetColor(facetattooColorPropertyName, avatarController.GetColorCode(faceTattooColor));
                characterHeadMat.SetFloat(faceTattooContrastPropertyName, 7f);
                break;
            case CurrentTextureType.ChestTattoo:
                characterBodyMat.SetTexture(chestTattoo_MaskPropertyName, texture);
                characterBodyMat.SetColor(chesttattooColorPropertyName, avatarController.GetColorCode(chestTattooColor));
                characterBodyMat.SetFloat(chestTattooContrastPropertyName, 7f);
                break;
            case CurrentTextureType.LegsTattoo:
                characterBodyMat.SetTexture(legsTattoo_MaskPropertyName, texture);
                characterBodyMat.SetColor(legstattooColorPropertyName, avatarController.GetColorCode(legsTattooColor));
                characterBodyMat.SetFloat(legsTattooContrastPropertyName, 7f);
                break;
            case CurrentTextureType.ArmTattoo:
                characterBodyMat.SetTexture(armTattoo_MaskPropertyName, texture);
                characterBodyMat.SetColor(armtattooColorPropertyName, avatarController.GetColorCode(armTattooColor));
                characterBodyMat.SetFloat(armTattooPropertyContrastName, 7f);
                break;


        }
    }
    public void RemoveTattoo(Texture texture, GameObject applyOn, CurrentTextureType nFTOjectType)
    {
        characterHeadMat = Head.GetComponent<SkinnedMeshRenderer>().materials[2];
        characterBodyMat = Body_Bone.GetComponent<SkinnedMeshRenderer>().materials[0];
        switch (nFTOjectType)
        {
            case CurrentTextureType.FaceTattoo:
                characterHeadMat.SetTexture(faceTattoo_MaskPropertyName, TattooDefaultTexture);
                characterHeadMat.SetColor(facetattooColorPropertyName, Color.white);
                characterHeadMat.SetFloat(faceTattooContrastPropertyName, -.3f);
                break;
            case CurrentTextureType.ChestTattoo:
                characterBodyMat.SetTexture(chestTattoo_MaskPropertyName, TattooDefaultTexture);
                characterBodyMat.SetColor(chesttattooColorPropertyName, Color.white);
                characterBodyMat.SetFloat(chestTattooContrastPropertyName, -.3f);
                break;
            case CurrentTextureType.LegsTattoo:
                characterBodyMat.SetTexture(legsTattoo_MaskPropertyName, TattooDefaultTexture);
                characterBodyMat.SetColor(legstattooColorPropertyName, Color.white);
                characterBodyMat.SetFloat(legsTattooContrastPropertyName, -.3f);
                break;
            case CurrentTextureType.ArmTattoo:
                characterBodyMat.SetTexture(armTattoo_MaskPropertyName, TattooDefaultTexture);
                characterBodyMat.SetColor(armtattooColorPropertyName, Color.white);
                characterBodyMat.SetFloat(armTattooPropertyContrastName, -.3f);
                break;


        }
    }
    public void ApplyMustacheTexture(Texture texture, GameObject applyOn)
    {
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetTexture(mustache_MaskPropertyName, texture);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(mustacheColorPropertyName, avatarController.GetColorCode(mustacheColor));
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(mustacheTattooContrastPropertyName, 5f);
    }

    public void RemoveMustacheTexture(Texture texture, GameObject applyOn)
    {
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetTexture(mustache_MaskPropertyName, TattooDefaultTexture);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(mustacheColorPropertyName, Color.white);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(mustacheTattooContrastPropertyName, -.3f);
    }

    public void ApplyEyeLidTexture(Texture texture, GameObject applyOn)
    {
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetTexture(eyeLid_MaskPropertyName, texture);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(eyeLidColorPropertyName, avatarController.GetColorCode(eyeLidColor));
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(eyeLidContrastPropertyName, 5f);
    }

    public void RemoveEyeLidTexture(Texture texture, GameObject applyOn)
    {
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetTexture(eyeLid_MaskPropertyName, TattooDefaultTexture);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(eyeLidColorPropertyName, Color.white);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(eyeLidContrastPropertyName, -.3f);
    }



    public Color GetEyebrowColor()
    {
        return Head.GetComponent<SkinnedMeshRenderer>().materials[1].GetColor(Eyebrow_ColorName);
    }
    public Color GetBodyColor()
    {
        return Body.materials[0].GetColor(Skin_ColorName);
    }
    public Color GetHairColor()
    {
        if (avatarController != null &&
     avatarController.wornHair != null &&
     avatarController.wornHair.GetComponent<SkinnedMeshRenderer>() != null &&
     avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials.Length > 0 &&
     avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("_Band"))
        {
            // For Band using Eye Shader so variable name is Changed 
            // Variable is equal to eyename
            return avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].GetColor(Eye_ColorName);
        }
        else if (avatarController != null &&
        avatarController.wornHair != null &&
        avatarController.wornHair.GetComponent<SkinnedMeshRenderer>() != null 
        && avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials.Length > 1) // In case Of Hat there is 2 material
        {
            if (avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("Cap") ||
               avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].name.Contains("Hat"))
                return avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[1].GetColor(Hair_ColorName);
            else
                return avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].GetColor(Hair_ColorName);
        }
        else
            return avatarController.wornHair.GetComponent<SkinnedMeshRenderer>().materials[0].GetColor(Hair_ColorName);
    }
    public Color GetEyeColor()
    {
        return Head.GetComponent<SkinnedMeshRenderer>().materials[0].GetColor(Eye_ColorName);
    }
    public Color GetSkinGredientColor()
    {
        return Body.materials[0].GetColor(GredientColorName);
    }

    public float GetSssIntensity()
    {
        return Head.GetComponent<SkinnedMeshRenderer>().materials[2].GetFloat(SssIntensity);
    }
    public Color GetLipColor()
    {
        return Head.GetComponent<SkinnedMeshRenderer>().materials[2].GetColor(Lip_ColorName);
    }

    public void DefaultBlendShapes(GameObject applyOn)
    {
        SkinnedMeshRenderer effectedHead = applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>();
        //blend shapes

        for (int i = 0; i < effectedHead.sharedMesh.blendShapeCount; i++)
        {
            effectedHead.SetBlendShapeWeight(i, 0);
        }
    }
    public void LoadBlendShapes(SavingCharacterDataClass data, GameObject applyOn)
    {
        SkinnedMeshRenderer effectedHead = applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>();
        //blend shapes

        for (int i = 0; i < effectedHead.sharedMesh.blendShapeCount - 1; i++)
        {
            if (data.FaceBlendsShapes != null && data.FaceBlendsShapes.Length > 0)
            {
                //if (i < data.FaceBlendsShapes.Length)
                //{

                //    if (i == 32)
                //        effectedHead.SetBlendShapeWeight(i, 0);
                //    else
                //        effectedHead.SetBlendShapeWeight(i, data.FaceBlendsShapes[i]);
                //}
                //else
                //    effectedHead.SetBlendShapeWeight(i, 0);

                // Added By WaqasAhmad
                // if BlendCount & Blend In File are Same Then Assign Blend Value
                // Else Set Blend Values to Default

                if (data.FaceBlendsShapes.Length == effectedHead.sharedMesh.blendShapeCount || data.FaceBlendsShapes.Length == effectedHead.sharedMesh.blendShapeCount - 1)
                {
                    effectedHead.SetBlendShapeWeight(i, data.FaceBlendsShapes[i]);
                }
                else
                {
                    effectedHead.SetBlendShapeWeight(i, 0);
                }
            }
        }
        EyesBlinking.instance.StoreBlendShapeValues();          // Added by Ali Hamza
    }
    public void ApplyBlendShapeEyesValues(GameObject applyOn, List<BlendShapeContainer> data, Vector3 eyesPos, float rotationz)
    {
        SkinnedMeshRenderer blendRender = applyOn.GetComponent<SkinnedMeshRenderer>();

        if (data.Count > 0)
        {
            for (int i = 0; i < data.Count; i++)
            {
                blendRender.SetBlendShapeWeight(data[i].blendShapeind, data[i].blendShapeValue);
            }
        }
        EyesBlinking.instance.StoreBlendShapeValues();          // Added by Ali Hamza

        // ***** Reset to Default If Custom chance made
        BothEyes[0].transform.localPosition = new Vector3(-0.052f, 0.1106f, 0.122f);
        BothEyes[1].transform.localPosition = new Vector3(0.052f, 0.1106f, 0.122f);

        EyeIner[0].transform.localPosition = new Vector3(0f, 0f, -0.039f);
        EyeIner[1].transform.localPosition = new Vector3(0f, 0f, 0.039f);

        EyesOut[0].transform.localPosition = new Vector3(0f, 0f, -0.039f);
        EyesOut[1].transform.localPosition = new Vector3(0f, 0f, 0.039f);

        for (int i = 0; i < BothEyes.Length; i++)
        {
            BothEyes[i].transform.localScale = Vector3.one;
            EyeIner[i].transform.localScale = Vector3.one;
            EyesOut[i].transform.localScale = Vector3.one;
        }

        // *****



        // SettingPosition
        BothEyes[1].transform.localPosition = eyesPos;

        // Inverst x Position for other eye
        eyesPos.x *= (-1);
        BothEyes[0].transform.localPosition = eyesPos;


        BothEyes[0].transform.localEulerAngles = new Vector3(0, 0, rotationz);
        BothEyes[1].transform.localEulerAngles = new Vector3(-180, 0, rotationz);
    }
    public void ApplyBlendShapeLipsValues(GameObject applyOn, List<BlendShapeContainer> data)
    {
        SkinnedMeshRenderer blendRender = applyOn.GetComponent<SkinnedMeshRenderer>();

        if (data.Count > 0)
        {
            for (int i = 0; i < data.Count; i++)
            {
                blendRender.SetBlendShapeWeight(data[i].blendShapeind, data[i].blendShapeValue);
            }
        }
    }
    public void ApplyEyeLashes(Texture texture, GameObject applyOn)
    {
        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[3].SetTexture(eyeLashes_TextureName, texture);
    }

    public void ApplyMakeup(Texture texture, GameObject applyOn)
    {
        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetTexture(Makeup_TextureName, texture);
    }


    /// <summary>
    /// Set Sss Intentsity for worlds
    /// </summary>
    /// <param name="value"> intensity value for shadder</param>
    /// <param name="applyOn"> player gameobject on which apply value</param>
    public void SetSssIntensity(float value, GameObject applyOn)
    {
        applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetFloat(SssIntensity, value);
        print("HEAD shader SSs for gmaeplay is " + applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[2].GetFloat(SssIntensity));
        applyOn.GetComponent<CharcterBodyParts>().Body.GetComponent<SkinnedMeshRenderer>().materials[0].SetFloat(SssIntensity, value);
        print("BODY shader SSs for gmaeplay is " + applyOn.GetComponent<CharcterBodyParts>().Body.GetComponent<SkinnedMeshRenderer>().materials[0].GetFloat(SssIntensity));

    }

    //public string GetEyeLashesName(GameObject applyOn) {
    //    string name;
    //    name = applyOn.GetComponent<CharcterBodyParts>().Head.GetComponent<SkinnedMeshRenderer>().materials[3].GetTexture(eyeLashes_TextureName);
    //}


    /// <summary>
    /// To Apply Gredient for preset data.
    /// </summary>
    public void ApplyPresiteGredient()
    {
        Body.materials[0].SetColor(GredientColorName, PresetGredientColor);
        Head.GetComponent<SkinnedMeshRenderer>().materials[2].SetColor(GredientColorName, PresetGredientColor);
    }


    /// <summary>
    /// To hide player meshes for camera man account
    /// </summary>
    public void HidePlayer()
    {
        if (Body)
            Body.gameObject.SetActive(false);
        if (Head)
            Head.gameObject.SetActive(false);
        if (avatarController.wornHair)
            avatarController.wornHair.SetActive(false);
        if (avatarController.wornPant)
            avatarController.wornPant.SetActive(false);
        if (avatarController.wornShirt)
            avatarController.wornShirt.SetActive(false);
        if (avatarController.wornShose)
            avatarController.wornShose.SetActive(false);
        if (avatarController.wornEyewearable)
            avatarController.wornEyewearable.SetActive(false);
        if (avatarController.wornGloves)
            avatarController.wornGloves.SetActive(false);
        if (avatarController.wornChain)
            avatarController.wornChain.SetActive(false);

        avatarController.GetComponent<ArrowManager>().PhotonUserName.gameObject.SetActive(false);
        //.gameObject.SetActive(false);
    }



    /// <summary>
    /// To Show the player
    /// </summary>
    public void ShowPlayer()
    {
        if (Body)
            Body.gameObject.SetActive(true);
        if (Head)
            Head.gameObject.SetActive(true);
        if (avatarController.wornHair)
            avatarController.wornHair.SetActive(true);
        if (avatarController.wornPant)
            avatarController.wornPant.SetActive(true);
        if (avatarController.wornShirt)
            avatarController.wornShirt.SetActive(true);
        if (avatarController.wornShose)
            avatarController.wornShose.SetActive(true);
        if (avatarController.wornEyewearable)
            avatarController.wornEyewearable.SetActive(true);
        if (avatarController.wornGloves)
            avatarController.wornGloves.SetActive(true);
        if (avatarController.wornChain)
            avatarController.wornChain.SetActive(true);
        avatarController.GetComponent<ArrowManager>().PhotonUserName.gameObject.SetActive(true);
    }


    /// <summary>
    /// To Set Default texture on character
    /// </summary>
    /// <param name="type"> texture type</param>
    /// <param name="applyOn">object on which texture is to be apply</param>
    public void SetTextureDefault(CurrentTextureType type, GameObject applyOn)
    {
        switch (type)
        {
            case CurrentTextureType.Null:
                break;
            case CurrentTextureType.FaceTattoo:
                break;
            case CurrentTextureType.ChestTattoo:
                break;
            case CurrentTextureType.LegsTattoo:
                break;
            case CurrentTextureType.ArmTattoo:
                break;
            case CurrentTextureType.Mustache:
                applyOn.GetComponent<CharcterBodyParts>().RemoveMustacheTexture(null, applyOn);
                break;
            case CurrentTextureType.EyeLid:
                break;
            case CurrentTextureType.EyeLense:
                applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLenTexture(applyOn.GetComponent<CharcterBodyParts>().Eye_Texture, applyOn);
                break;
            case CurrentTextureType.EyeLashes:
                applyOn.GetComponent<CharcterBodyParts>().ApplyEyeLashes(applyOn.GetComponent<CharcterBodyParts>().defaultEyelashes, applyOn);
                break;
            case CurrentTextureType.EyeBrows:
                applyOn.GetComponent<CharcterBodyParts>().ApplyEyeBrowTexture(applyOn.GetComponent<CharcterBodyParts>().defaultEyebrow, applyOn);
                break;
            case CurrentTextureType.Skin:
                break;
            case CurrentTextureType.Lip:
                applyOn.GetComponent<CharcterBodyParts>().RemoveEyeLidTexture(null, applyOn);
                break;
            case CurrentTextureType.Makeup:
                applyOn.GetComponent<CharcterBodyParts>().ApplyMakeup(applyOn.GetComponent<CharcterBodyParts>().defaultMakeup, applyOn);
                break;
            default:
                break;
        }

    }
}