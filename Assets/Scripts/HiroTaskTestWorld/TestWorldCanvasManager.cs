using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestWorldCanvasManager : Singleton<TestWorldCanvasManager>
{
    public Button btn_ClearAll;
    public GameObject parentObj;
    public TextMeshProUGUI polygonCountText, textureCountText, characterCountText, textureEstimateMBText;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        btn_ClearAll.onClick.AddListener(ClearAll);
        ClearAll();
    }

    internal void AddPolyCount(int v)
    {
        int count = int.Parse(polygonCountText.text);
        count += v;
        polygonCountText.text = count.ToString();
    }

    internal void AddTextureCount(int v)
    {
        int count = int.Parse(textureCountText.text);
        count += v;
        textureCountText.text = count.ToString();
    }

    internal void AddCharacterCount(int v)
    {
        int count = int.Parse(characterCountText.text);
        count += v;
        characterCountText.text = count.ToString();
    }

    internal void AddTextureEstimateMBCount(float v)
    {
        float count = float.Parse(textureEstimateMBText.text);
        count += v;
        textureEstimateMBText.text = count.ToString("F2");
    }

    internal void ResetCount()
    {
        polygonCountText.text = "0";
        textureCountText.text = "0";
        characterCountText.text = "0";
        textureEstimateMBText.text = "0";
    }

    internal void ClearAll()
    {
        Debug.Log($"<color=red>ClearAll</color>");
        if(parentObj != null)
        {
            //DestroyAllChildren(parentObj);
            Destroy(parentObj);
        }
        ResetCount();
    }

    private void DestroyAllChildren(GameObject parentObj)
    {
        foreach (Transform child in parentObj.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
