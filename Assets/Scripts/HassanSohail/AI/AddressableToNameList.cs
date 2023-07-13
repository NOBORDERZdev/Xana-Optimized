using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XanaAi;

public class AddressableToNameList : MonoBehaviour
{
    public List<GameObject> upper;
    public List<GameObject> lower;
    public List<GameObject> hair;
    public List<GameObject> shoes;

    [SerializeField] public List<Texture2D> Makeup;
    [SerializeField] public List<Texture2D> EyeTexture;
    [SerializeField] public List<Texture2D> EyeBrrow;
    [SerializeField] public List<Texture2D> EyeLashes;
    public AiAppearance AiAppearance;
    private void Start()
    {
        
    }

    private void OnValidate()
    {
        Set();
    }
    public void Set()
    {
        AiAppearance.Uppers.Clear();
        AiAppearance.Lower.Clear();
        AiAppearance.Hair.Clear();
        AiAppearance.Shoes.Clear();
        AiAppearance.Makeup.Clear();
        AiAppearance.EyeTexture.Clear();
        AiAppearance.EyeBrrow.Clear();
        AiAppearance.EyeLashes.Clear();
        foreach (var item in upper)
        {
            AiAppearance.Uppers.Add(item.name);
        }
        foreach (var item in lower)
        {
            AiAppearance.Lower.Add(item.name);
        }
        foreach (var item in hair)
        {
            AiAppearance.Hair.Add(item.name);
        }
        foreach (var item in shoes)
        {
            AiAppearance.Shoes.Add(item.name);
        }

        foreach (var item in Makeup)
        {
            AiAppearance.Makeup.Add(item.name);
        }

        foreach (var item in EyeTexture)
        {
            AiAppearance.EyeTexture.Add(item.name);
        }

        foreach (var item in EyeBrrow)
        {
            AiAppearance.EyeBrrow.Add(item.name);
        }
        foreach (var item in EyeLashes)
        {
            AiAppearance.EyeLashes.Add(item.name);
        }
    }
}
