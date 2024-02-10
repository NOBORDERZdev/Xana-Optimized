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

    CharcterBodyParts charBodyParts;
    private void Start()
    {
        charBodyParts = GameManager.Instance.mainCharacter.GetComponent<CharcterBodyParts>();
    }


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
        print("###### Name  " + gameObject.name);
        if (boneData.Count > 0)
        {
            if (m_FaceMorphFeature == FaceMorphFeature.Lips)
            {
                // Mouth Bone
                charBodyParts.Lips.transform.localPosition = boneData[0].Pos;
                charBodyParts.Lips.transform.localScale = boneData[0].Scale;
                charBodyParts.Lips.transform.localRotation = Quaternion.Euler(boneData[0].Rotation);

                // Lip Righside
                charBodyParts.BothLips[1].transform.localPosition = boneData[1].Pos;
                charBodyParts.BothLips[1].transform.localScale = boneData[1].Scale;
                charBodyParts.BothLips[1].transform.localRotation = Quaternion.Euler(boneData[1].Rotation);

                // Lip Leftside
                charBodyParts.BothLips[0].transform.localPosition = boneData[2].Pos;
                charBodyParts.BothLips[0].transform.localScale = boneData[2].Scale;
                charBodyParts.BothLips[0].transform.localRotation = Quaternion.Euler(boneData[2].Rotation);
            }
            if (m_FaceMorphFeature == FaceMorphFeature.Nose)
            {
                // Nose Bone
                charBodyParts.Nose.transform.localPosition = boneData[0].Pos;
                charBodyParts.Nose.transform.localScale = boneData[0].Scale;
            }
            if (m_FaceMorphFeature == FaceMorphFeature.Face)
            {
                // JBone
                charBodyParts.JBone.transform.localScale = boneData[0].Scale;

                // HeadUpper Bone
                charBodyParts.ForeHead.transform.parent.transform.localScale = boneData[1].Scale;

                ResetBlendShapeForFace();
            }
        }
        if (m_FaceMorphFeature == FaceMorphFeature.Eyes)
        {
            charBodyParts.ApplyBlendShapeEyesValues(charBodyParts.head.gameObject, blendShapeData, eyesPosition, eyes_Rotation_z);
        }
        if (m_FaceMorphFeature == FaceMorphFeature.Lips)
        {
            charBodyParts.ApplyBlendShapeLipsValues(charBodyParts.head.gameObject, blendShapeData);
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
