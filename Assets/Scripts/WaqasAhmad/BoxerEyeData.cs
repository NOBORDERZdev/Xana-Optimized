using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerEyeData : MonoBehaviour
{
    public List<BoneBlendContainer> items;
    public static BoxerEyeData instance;
    public CharacterBodyParts bodyParts;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
       // bodyParts = GameManager.Instance.mainCharacter.GetComponent<CharacterBodyParts>();
    }
    /// <summary>
    /// Find Item of item
    /// </summary>
    /// 
    int GetIndex(string EyeName)
    {
        if (EyeName.Contains("Default"))
        {
            return 0; 
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].styleName.ToLower().Contains(EyeName.ToLower()))
                {
                    return i;
                }
            }

            return 0;
        }
    }

    /// <summary>
    /// Make EyeShape By Changing Bones & blend Shapes
    /// </summary>
    /// <param name="eyeName"></param>
    public void SetNFTData(string eyeName)
    {
        int index =  GetIndex(eyeName);
        //CharcterBodyParts bodyParts = CharcterBodyParts.instance.gameObject.GetComponent<CharcterBodyParts>();
        //AvatarController  avatarController = GameManager.Instance.mainCharacter.GetComponent<AvatarController>();
        SkinnedMeshRenderer blendShaps = bodyParts.head;

        // Reset BlendShapes to Default
        //bodyParts.DefaultBlendShapes(bodyParts.gameObject);

        // Reset Bones to Default
        //avatarController.ResetBonesDefault(bodyParts);

        if (index == 0)
        {
            // 0 is for Default Item 
            // Already Set Item to Default
            return;
        }
        else
        {
            // Setting BlendShape
            for (int i = 0; i < items[index].blends.Count; i++)
            {
                blendShaps.SetBlendShapeWeight(items[index].blends[i].index, items[index].blends[i].value);
            }

            // Setting Bones
            for (int i = 0; i < items[index].bones.Count; i++)
            {
                items[index].bones[i].boneObj.transform.localPosition = items[index].bones[i].bonePosition;
                items[index].bones[i].boneObj.transform.localScale = items[index].bones[i].boneScale;
                items[index].bones[i].boneObj.transform.localEulerAngles = items[index].bones[i].boneRotation;
            }
        }
    }

    public bool isBoneMissing = false;

    CharacterBodyParts body;

    public int boneIndex = 0;
    public void NextBone()
    {
        boneIndex++;
        SkinnedMeshRenderer blendShaps = bodyParts.head;
        bodyParts.gameObject.GetComponent<AvatarController>().ResetBonesDefault(bodyParts);

        Debug.Log("Shape  : " + items[boneIndex].styleName);

        // Setting BlendShape
        for (int i = 0; i < items[boneIndex].blends.Count; i++)
        {
            blendShaps.SetBlendShapeWeight(items[boneIndex].blends[i].index, items[boneIndex].blends[i].value);
        }

        // Setting Bones
        for (int i = 0; i < items[boneIndex].bones.Count; i++)
        {
            items[boneIndex].bones[i].boneObj.transform.localPosition = items[boneIndex].bones[i].bonePosition;
            items[boneIndex].bones[i].boneObj.transform.localScale = items[boneIndex].bones[i].boneScale;
            items[boneIndex].bones[i].boneObj.transform.localEulerAngles = items[boneIndex].bones[i].boneRotation;

            if (items[boneIndex].bones[i].boneName != items[boneIndex].bones[i].boneName)
            {
                Debug.Log("Bone Not Matched  : " + items[boneIndex].styleName +" - "+ items[boneIndex].bones[i].boneName);
            }
        }


    }

    private void OnValidate()
    {
        // If Bones Reference missing 
        // Pass bones reference in 0 index used for normal 1st ind & Call this method
        if (isBoneMissing)
        {
            isBoneMissing = false;
            for (int index = 2; index < items.Count; index++)
            {
                for (int i = 0; i < items[index].bones.Count; i++)
                {
                    items[index].bones[i].boneObj = items[1].bones[i].boneObj;
                }
            }
        }
    }
}

[System.Serializable]
public class BoneBlendContainer
{
    public string styleName;
    public List<BlendData> blends;
    public List<NFTBoneData> bones;
}

[System.Serializable]
public class BlendData
{
    public int index;
    public float value;
}

[System.Serializable]
public class NFTBoneData
{
    public string boneName;
    public GameObject boneObj;
    public Vector3 bonePosition;
    public Vector3 boneRotation;
    public Vector3 boneScale;

    
}