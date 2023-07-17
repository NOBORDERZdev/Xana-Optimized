using Metaverse;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.OnScreen;
using WebSocketSharp;
using TMPro;

public class LoadButtonClick : MonoBehaviour
{
    public string objectUrl;
    public string thumbUrl;
    public string animationName;
    public TextMeshProUGUI AnimationText;
    public EmoteFilterManager controller;
    public GameObject highlighter;
    public GameObject ContentPanel;
    public GameObject prefabObj;
    public Image StarImg;

    public void Initializ(string animUrl, string animname, EmoteFilterManager ctrlr, GameObject Content, string thumbURL = null)
    {
        objectUrl = animUrl;
        animationName = animname;
        controller = ctrlr;
        ContentPanel = Content;
        thumbUrl = thumbURL;
        char[] _c = animationName.ToCharArray();
        string stringWithoutDigit = "";
        string stringwithDigit = "";
        foreach (char c in _c)
        {
            if (!char.IsDigit(c))
                stringWithoutDigit += c;
            else
                stringwithDigit += c;

        }
        //Debug.LogError(stringwithDigit+"----"+stringWithoutDigit);
        stringWithoutDigit = TextLocalization.GetLocaliseTextByKey(stringWithoutDigit);
        AnimationText.text = stringWithoutDigit +stringwithDigit;
        for (int i = 0; i < 10; i++)
        {
            string data = PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + i);
            if (!data.IsNullOrEmpty())
            {
                AnimationData d = JsonUtility.FromJson<AnimationData>(data);
                if (animationName == d.animationName)
                {
                    StarImg.sprite = AvatarManager.Instance.FavouriteAnimationSprite;
                    return;
                }
                else
                {
                    StarImg.sprite = AvatarManager.Instance.NormalAnimationSprite;
                }
            }
        }
    }

    public void OnEnable()
    {
        for (int i = 0; i < 10; i++)
        {
            string data = PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + i);
            if (!data.IsNullOrEmpty())
            {
                AnimationData d = JsonUtility.FromJson<AnimationData>(data);
                if (animationName == d.animationName)
                {
                    StarImg.sprite = AvatarManager.Instance.FavouriteAnimationSprite;
                    return;
                }
                else
                {
                    StarImg.sprite = AvatarManager.Instance.NormalAnimationSprite;
                }
            }
            else                                                     // AH Working
                StarImg.sprite = AvatarManager.Instance.NormalAnimationSprite;
        }
    }

    public void OnButtonClick()
    {
        //if (EmoteAnimationPlay.Instance.animatorremote != null && EmoteAnimationPlay.Instance.lastAnimClickButton!=null && this.gameObject != EmoteAnimationPlay.Instance.lastAnimClickButton)
        if (EmoteAnimationPlay.Instance.lastAnimClickButton != null && this.gameObject == EmoteAnimationPlay.Instance.lastAnimClickButton && !this.gameObject.name.Contains("Sit") && !this.gameObject.name.Contains("Laydown"))
            return;
        else if (EmoteAnimationPlay.Instance.lastAnimClickButton != null)
            EmoteAnimationPlay.Instance.lastAnimClickButton.GetComponent<LoadButtonClick>().highlighter.SetActive(false);
        //  GamePlayButtonEvents ui = GamePlayButtonEvents.inst;
        //foreach (Transform obj in ContentPanel.transform)
        //{
        //    obj.gameObject.transform.GetChild(2).gameObject.SetActive(false);
        //}
        highlighter.SetActive(true);
        EmoteAnimationPlay.Instance.lastAnimClickButton = this.gameObject;
        if (GamePlayButtonEvents.inst != null && GamePlayButtonEvents.inst.selectionPanelOpen)
        {
            OnSaveDataOnButton();
            return;
        }

        if (EmoteAnimationPlay.Instance.alreadyRuning)
        {
            //LoadFromFile.animClick = true;
            EmoteAnimationPlay.remoteUrlAnimation = objectUrl;
            EmoteAnimationPlay.remoteUrlAnimationName = animationName;
            //  PlayerPrefs.Save();
            //prefabObj.transform.GetChild(3).gameObject.SetActive(true);
            EmoteAnimationPlay.Instance.Load(objectUrl, prefabObj);

            //foreach (Transform obj in ContentPanel.transform)
            //{

            //    obj.gameObject.transform.GetChild(2).gameObject.SetActive(false);
            //    // }

            //}
            //highlighter.SetActive(true);
        }
        try
        {
            LoadFromFile.instance.leftJoyStick.transform.GetChild(0).GetComponent<OnScreenStick>().movementRange = 0;

        }
        catch (Exception e)
        {

        }
        // StartCoroutine(ButtonClick());
    }

    public void OnSaveDataOnButton()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        AnimationData animData = new AnimationData();
        //StarImg.sprite = AvatarManager.Instance.FavouriteAnimationSprite;
        animData.animationName = animationName;
        animData.animationURL = objectUrl;
        animData.thumbURL = thumbUrl;
        animData.bgColor = GetComponent<Image>().color;
        GamePlayButtonEvents.inst.OnAnimationSelect(animData);
    }
    private void AnimationStopped(string animName)
    {
        //AssetBundle.UnloadAllAssetBundles(false);
        //Resources.UnloadUnusedAssets();
        highlighter.SetActive(false);
    }

    private void OnAnimationStarted(string animName)
    {
        if (animationName.Equals(animName)) highlighter.SetActive(true);
        else highlighter.SetActive(false);
    }
}
