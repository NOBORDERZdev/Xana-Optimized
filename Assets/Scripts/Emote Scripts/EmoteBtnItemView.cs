using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebSocketSharp;

public class EmoteBtnItemView : MonoBehaviour
{
    [SerializeField] Image actionImg;
    [SerializeField] Image actionImgPotrait;
    [SerializeField] EmoteSelectionBtn emoteSelectionBtnScript;
    AnimationData animData;

    private Image bgImage;

    private void Awake()
    {
        bgImage = GetComponent<Image>();
        emoteSelectionBtnScript = GetComponent<EmoteSelectionBtn>();
    }

    public void OnEnable()
    {
        actionImg.gameObject.SetActive(false);
        actionImgPotrait.gameObject.SetActive(false);
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.AnimationDataUpdated += LoadAnimData;

        LoadAnimData(transform.GetSiblingIndex());
    }
    public void OnDisable()
    {
        if (GamePlayButtonEvents.inst != null) GamePlayButtonEvents.inst.AnimationDataUpdated -= LoadAnimData;
    }

    public void LoadAnimData(int index)
    {
        if (index != transform.GetSiblingIndex())
        {
            Debug.Log("Check Index value====" + index + "transform===" + transform.GetSiblingIndex());

            return;
        }

        actionImg.gameObject.SetActive(false);
        actionImgPotrait.gameObject.SetActive(false);
        string data = PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + index);
        Debug.Log(ConstantsGod.ANIMATION_DATA + index);
        Debug.Log("Data :" + data);
        if (!data.IsNullOrEmpty())
        {
            AnimationData d = JsonUtility.FromJson<AnimationData>(data);
            if (animData != null && animData.animationName.Equals(d.animationName))
            {
                actionImg.gameObject.SetActive(true);
                actionImgPotrait.gameObject.SetActive(true);
                return;
            }
            bgImage.color = d.bgColor;
            animData = d;
            if (d.isEmote)
            {
                actionImg.gameObject.transform.localScale = Vector3.one * .6f;
                actionImgPotrait.gameObject.transform.localScale = Vector3.one * .6f;
            }
            else
            {
                actionImg.gameObject.transform.localScale = Vector3.one;
                actionImgPotrait.gameObject.transform.localScale = Vector3.one;
            }
            StartCoroutine(LoadSpriteEnv(animData.thumbURL));
        }
        else
        {
            if (gameObject.transform.GetChild(0).GetComponent<EmoteActionBtn>())
            {
                gameObject.transform.GetChild(0).GetComponent<EmoteActionBtn>().UnloadAnim();
            }
            StopAllCoroutines();
        }
    }

    IEnumerator LoadSpriteEnv(string ImageUrl)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (ImageUrl.Equals(""))
            {
                // Loader.SetActive(false);
            }
            else
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(ImageUrl);
                www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return null;
                }
                Texture2D thumbnailTexture = DownloadHandlerTexture.GetContent(www);
                thumbnailTexture.Compress(true);
                Sprite sprite = Sprite.Create(thumbnailTexture, new Rect(0, 0, thumbnailTexture.width, thumbnailTexture.height), new Vector2(0, 0));
                if (actionImg != null && actionImgPotrait != null)
                {
                    actionImg.sprite = sprite;
                    actionImgPotrait.sprite = sprite;
                    actionImg.gameObject.SetActive(true);
                    actionImgPotrait.gameObject.SetActive(true);
                }
                www.Dispose();
            }
        }
        AssetBundle.UnloadAllAssetBundles(false);
        Resources.UnloadUnusedAssets();


        if (emoteSelectionBtnScript)
            emoteSelectionBtnScript.resetBtn.gameObject.SetActive(true);
    }
}

[Serializable]
public class AnimationData
{
    public string animationName;
    public string animationURL;
    public string thumbURL;
    public Color32 bgColor;
    public bool isEmote;
}
