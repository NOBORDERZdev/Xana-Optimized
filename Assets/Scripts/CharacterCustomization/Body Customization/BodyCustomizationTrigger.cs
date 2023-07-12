using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyCustomizationTrigger : MonoBehaviour
{
    public enum FaceMorphFeature { Eyes, Nose, Lips, EyeBrows, Face }  // To Set The "BodyCustomizerTrigger" To Modify Either The Face Or Body

    public FaceMorphFeature m_FaceMorphFeature;
    public int f_BlendShapeOne;   // Blend Shape Index : if you are using only one blendshape then give -1 in the inspector in any one.
    public int f_BlendShapeTwo;

    //WaqasAhmad
    public Vector3 eyebrowBlendValues;
    public List<BoneDataContainer> boneData;
    // Eyes 
    public List<BlendShapeContainer> blendShapeData;
    public Vector3 eyesPosition;
    public float eyes_Rotation_z = 0;

    public float m_BlendTime;
    public AnimationCurve m_AnimCurve;

    public void CustomizationTriggerTwo()
    {
        //-----------------------------------
        //StoreManager.instance.BuyStoreBtn.SetActive(false);
        StoreManager.instance.SaveStoreBtn.SetActive(true);
        StoreManager.instance.SaveStoreBtn.GetComponent<Image>().color = new Color(0f, 0.5f, 1f, 0.8f);
        StoreManager.instance.GreyRibbonImage.SetActive(false);
        StoreManager.instance.WhiteRibbonImage.SetActive(true);
        StoreManager.instance.ClearBuyItems();
        //-------------------------------------

        if (m_FaceMorphFeature == FaceMorphFeature.Lips || m_FaceMorphFeature == FaceMorphFeature.Nose || m_FaceMorphFeature == FaceMorphFeature.Face || m_FaceMorphFeature == FaceMorphFeature.Eyes)
        {

            ChangeBoneValue();
        }
        if (m_FaceMorphFeature == FaceMorphFeature.EyeBrows)
        {
            BodyCustomizer.Instance.ApplyEyeBrowsBlendShapes(eyebrowBlendValues);

        }

    }
   
    // WaqasAhmad
    void ChangeBoneValue()
    {
        if (boneData.Count > 0)
        {
            if (m_FaceMorphFeature == FaceMorphFeature.Lips)
            {
                // Mouth Bone
                CharcterBodyParts.instance.Lips.transform.localPosition = boneData[0].Pos;
                CharcterBodyParts.instance.Lips.transform.localScale = boneData[0].Scale;
                CharcterBodyParts.instance.Lips.transform.localRotation = Quaternion.Euler(boneData[0].Rotation);

                // Lip Righside
                CharcterBodyParts.instance.BothLips[1].transform.localPosition = boneData[1].Pos;
                CharcterBodyParts.instance.BothLips[1].transform.localScale = boneData[1].Scale;
                CharcterBodyParts.instance.BothLips[1].transform.localRotation = Quaternion.Euler(boneData[1].Rotation);

                // Lip Leftside
                CharcterBodyParts.instance.BothLips[0].transform.localPosition = boneData[2].Pos;
                CharcterBodyParts.instance.BothLips[0].transform.localScale = boneData[2].Scale;
                CharcterBodyParts.instance.BothLips[0].transform.localRotation = Quaternion.Euler(boneData[2].Rotation);
            }
            if (m_FaceMorphFeature == FaceMorphFeature.Nose)
            {
                // Nose Bone
                CharcterBodyParts.instance.Nose.transform.localPosition = boneData[0].Pos;
                CharcterBodyParts.instance.Nose.transform.localScale = boneData[0].Scale;
            }
            if (m_FaceMorphFeature == FaceMorphFeature.Face)
            {
                // JBone
                CharcterBodyParts.instance.JBone.transform.localScale = boneData[0].Scale;

                // HeadUpper Bone
                CharcterBodyParts.instance.ForeHead.transform.parent.transform.localScale = boneData[1].Scale;

                ResetBlendShapeForFace();
            }
        }
        if (m_FaceMorphFeature == FaceMorphFeature.Eyes)
        {
            CharcterBodyParts.instance.ApplyBlendShapeEyesValues(CharcterBodyParts.instance.Head, blendShapeData, eyesPosition, eyes_Rotation_z);
        }
        if (m_FaceMorphFeature == FaceMorphFeature.Lips)
        {
            CharcterBodyParts.instance.ApplyBlendShapeLipsValues(CharcterBodyParts.instance.Head, blendShapeData);
        }
    }

    void ResetBlendShapeForFace()
    {
        SkinnedMeshRenderer _head = BlendShapeController.instance.blendHolder;
        // Face blend References [0-9]
        for (int i = 0; i < 10; i++)
        {
            _head.SetBlendShapeWeight(BlendShapeController.instance.allBlendShapes[i].index, 0);
        }
    }
}
