using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class XLRatios : MonoBehaviour
{
    public static XLRatios instance;
    [NonReorderable]
    public List<XLRatioReferences> ratioReferences;
    public GameObject LandscapeObj;
    public GameObject PotraiteObj;
    public RenderTexture renderTexture;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CloseInfoPop()
    {
        LandscapeObj.SetActive(false);
        PotraiteObj.SetActive(false);
        if (GamePlayUIHandler.inst.gameObject.activeInHierarchy)
        {
            GamePlayUIHandler.inst.gamePlayUIParent.SetActive(true);
        }
    }
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Debug.Log("---Nft Close due to Application minimized");
            CloseInfoPop();
        }
       
    }
}

[Serializable]
public class XLRatioReferences
{
    public string name;

    public GameObject l_obj;
    public TMP_Text l_Title;
    public TMP_Text l_Aurthur;
    public TMP_Text l_Description;
    public RawImage l_image;
    public VideoPlayer l_videoPlayer;
    public GameObject l_Loader;

    public GameObject p_obj;
    public TMP_Text p_Title;
    public TMP_Text p_Aurthur;
    public TMP_Text p_Description;
    public RawImage p_image;
    public VideoPlayer p_videoPlayer;
    public GameObject p_Loader;
}