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
                    StarImg.sprite = AvatarSpawnerOnDisconnect.Instance.FavouriteAnimationSprite;
                    return;
                }
                else
                {
                    StarImg.sprite = AvatarSpawnerOnDisconnect.Instance.NormalAnimationSprite;
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
                    StarImg.sprite = AvatarSpawnerOnDisconnect.Instance.FavouriteAnimationSprite;
                    return;
                }
                else
                {
                    StarImg.sprite = AvatarSpawnerOnDisconnect.Instance.NormalAnimationSprite;
                }
            }
            else                                              
                StarImg.sprite = AvatarSpawnerOnDisconnect.Instance.NormalAnimationSprite;
        }
    }

    public bool danceAnim;
    public void OnButtonClick() //add by kamran
    {
        if (PlayerController.isJoystickDragging == true)
        {
            return;
        }
        EmoteAnimationHandler.Instance.StopAllCoroutines();

        if (EmoteAnimationHandler.Instance.currentAnimationTab == "Sit & lying")
        {

            if (EmoteAnimationHandler.Instance.lastAnimClickButton != null && this.gameObject == EmoteAnimationHandler.Instance.lastAnimClickButton && !this.gameObject.name.Contains("Sit") && !this.gameObject.name.Contains("Laydown"))
                return;
            else if (EmoteAnimationHandler.Instance.lastAnimClickButton != null)
                EmoteAnimationHandler.Instance.lastAnimClickButton.GetComponent<LoadButtonClick>().highlighter.SetActive(false);

            highlighter.SetActive(true);
            EmoteAnimationHandler.Instance.lastAnimClickButton = this.gameObject;
            if (GamePlayButtonEvents.inst != null && GamePlayButtonEvents.inst.selectionPanelOpen)
            {
                OnSaveDataOnButton();
                return;
            }

            if (EmoteAnimationHandler.Instance.alreadyRuning)
            {
                EmoteAnimationHandler.remoteUrlAnimation = objectUrl;
                EmoteAnimationHandler.remoteUrlAnimationName = animationName;
                EmoteAnimationHandler.Instance.Load(objectUrl, prefabObj);
            }
            try
            {
                GameplayEntityLoader.instance.leftJoyStick.transform.GetChild(0).GetComponent<OnScreenStick>().movementRange = 0;

            }
            catch (Exception e)
            {

            }
        }
        else
        {
            if (danceAnim)
            {
                if (EmoteAnimationHandler.Instance.lastAnimClickButton != null && this.gameObject == EmoteAnimationHandler.Instance.lastAnimClickButton && !this.gameObject.name.Contains("Sit") && !this.gameObject.name.Contains("Laydown") && !this.gameObject.name.Contains("Dance") && !this.gameObject.name.Contains("dance"))
                    return;
                else if (EmoteAnimationHandler.Instance.lastAnimClickButton != null)
                    EmoteAnimationHandler.Instance.lastAnimClickButton.GetComponent<LoadButtonClick>().highlighter.SetActive(false);

                highlighter.SetActive(true);
                EmoteAnimationHandler.Instance.lastAnimClickButton = this.gameObject;
                if (GamePlayButtonEvents.inst != null && GamePlayButtonEvents.inst.selectionPanelOpen)
                {
                    OnSaveDataOnButton();
                    return;
                }

                if (EmoteAnimationHandler.Instance.alreadyRuning)
                {
                    EmoteAnimationHandler.remoteUrlAnimation = objectUrl;
                    EmoteAnimationHandler.remoteUrlAnimationName = animationName;
                    EmoteAnimationHandler.Instance.Load(objectUrl, prefabObj);
                }
                try
                {
                    GameplayEntityLoader.instance.leftJoyStick.transform.GetChild(0).GetComponent<OnScreenStick>().movementRange = 0;

                }
                catch (Exception e)
                {

                }
                danceAnim = false;
            }
            else
            {
                EmoteAnimationHandler.Instance.StopAnimation();
                AnimationStopped();
                danceAnim = true;
            }
        }
    }

    public void OnSaveDataOnButton()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();
        AnimationData animData = new AnimationData();
        animData.animationName = animationName;
        animData.animationURL = objectUrl;
        animData.thumbURL = thumbUrl;
        animData.bgColor = GetComponent<Image>().color;
        GamePlayButtonEvents.inst.OnAnimationSelect(animData);
    }
    private void AnimationStopped()
    {
        highlighter.SetActive(false);
    }

    private void OnAnimationStarted(string animName)
    {
        if (animationName.Equals(animName)) highlighter.SetActive(true);
        else highlighter.SetActive(false);
    }
}
