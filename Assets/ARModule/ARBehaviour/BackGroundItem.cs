using DigitalRubyShared;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackGroundItem : MonoBehaviour
{
    public int itemIndex;
    [HideInInspector] public byte R;
    [HideInInspector] public byte G;
    [HideInInspector] public byte B;
    [HideInInspector] public byte A;
    public GameObject textColor;
    public GameObject selectionImage;
    [HideInInspector] public FilterBG controller;
    
    public GameObject ContentPanel;
    public void OnClickItem()
    {
        if (this.gameObject.name == "None" && SceneManager.GetActiveScene().name == "ARModuleFaceTrackingScene")
        {
            LiveVideoRoomManager.Instance.BackgroundImage.gameObject.SetActive(false);
        }
        else
        {
            LiveVideoRoomManager.Instance.BackgroundImage.gameObject.SetActive(true);
            LiveVideoRoomManager.Instance.BackgroundImage.sprite = gameObject.transform.GetChild(0).GetComponent<Image>().sprite;
            if (this.gameObject.name == "None")
            {
                LiveVideoRoomManager.Instance.BackgroundImage.color = new Color(243f, 243f, 243f);
            }
            else
            {
                LiveVideoRoomManager.Instance.BackgroundImage.color = Color.white;
            }
        }

        ARFaceModuleManager.Instance.SelectionBorderOnBackgroundImage(itemIndex);

        ARFaceModuleManager.Instance.SelectionBorderOnBackgroundColor(0);
        /*for (int i = 0; i < transform.parent.childCount; i++)
        {
            transform.parent.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
        transform.GetChild(1).gameObject.SetActive(true);*/
    }


    public void Initializ(byte r, byte g, byte b, byte a, FilterBG ctrlr, GameObject Content)
    {
        R = r;
        G = g;
        B = b;
        A = a;
        //textColor = tvcolor;
        controller = ctrlr;
        ContentPanel = Content;
    }

    public void OnButtonClick()
    {

        //  PlayerPrefs.Save();
        FilterBG.instance.OnClickFilterItem(R,G,B,A);

        foreach (Transform obj in ContentPanel.transform)
        {
            obj.GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color32(115, 115, 115, 255);
            obj.GetChild(1).GetComponent<Image>().gameObject.SetActive(false);
        }
        textColor.GetComponent<TextMeshProUGUI>().color = Color.black;
        selectionImage.SetActive(true);
        // StartCoroutine(ButtonClick());

    }

    public void OnClickColorItem()
    {
        ARFaceModuleManager.Instance.SelectionBorderOnBackgroundImage(1);

        if (!LiveVideoRoomManager.Instance.BackgroundImage.gameObject.activeSelf)
        {
            LiveVideoRoomManager.Instance.BackgroundImage.gameObject.SetActive(true);
        }
        LiveVideoRoomManager.Instance.BackgroundImage.sprite = null;
        LiveVideoRoomManager.Instance.BackgroundImage.color = gameObject.transform.GetChild(0).GetComponent<Image>().color;

        ARFaceModuleManager.Instance.SelectionBorderOnBackgroundColor(itemIndex);
        /*for (int i = 0; i < transform.parent.childCount; i++)
        {
            transform.parent.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
        transform.GetChild(1).gameObject.SetActive(true);*/
    }

    public void OnClickFilterItem(int index)
    {
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            transform.parent.GetChild(i).GetChild(1).gameObject.SetActive(false);
            transform.parent.GetChild(i).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
        }
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().color = Color.blue;

        //LiveVideoRoomManager.Instance.mainVolume.profile = LiveVideoRoomManager.Instance.filterVolumeProfile[index];

        if (LiveVideoRoomManager.Instance.filterMainVolume.profile.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out var color))
        {
            color.colorFilter.overrideState = true;
            color.colorFilter.value = Color.white;
        }
    }

    public void OnClickSelectCharacterBtn()
    {
        if ((SceneManager.GetActiveScene().name != "ARModuleActionScene" || LiveVideoRoomManager.Instance.IsVideoScreenImageScreenAvtive) && itemIndex != 0)
        {
            Vector3 pos = new Vector3(0, 0, 2f);
            GameObject avatartItem = Instantiate(ARFaceModuleManager.Instance.allCharacterItemList[itemIndex]);
            avatartItem.transform.localPosition = ARFaceModuleManager.Instance.allCharacterItemDefaultPos;
            avatartItem.transform.localScale = ARFaceModuleManager.Instance.allCharacterItemDefaultScale;
            ARFaceModuleManager.Instance.addAvtarItem.Add(avatartItem);
            avatartItem.GetComponent<FingersPanRotateScaleComponentScript>().SetMinScaleOfAvatar();

            avatartItem.GetComponent<AvatarScript>().avatarIndexId = itemIndex;

            if (SceneManager.GetActiveScene().name == "ARModulePlanDetectionScene" && !LiveVideoRoomManager.Instance.IsVideoScreenImageScreenAvtive)
            {
                if (ARPlacement.Instance.spawnedObject != null)
                {
                    Vector3 finalPos = ARPlacement.Instance.spawnedObject.transform.localPosition;
                    finalPos.z = finalPos.z - 0.01f;
                    avatartItem.transform.localPosition = finalPos;
                    //avatartItem.transform.position = ARPlacement.Instance.spawnedObject.transform.position;
                    //Debug.LogError("AvatrItem:" + avatartItem.transform.position + "    :scale:" + avatartItem.transform.localScale);
                }
                /*if (avatartItem.GetComponent<AvatarScript>().avatarShadowPlanObj != null)
                {
                    avatartItem.GetComponent<AvatarScript>().avatarShadowPlanObj.SetActive(true);
                }*/
            }
        }
        else
        {
            if (ARFaceModuleManager.Instance.mainAvatar != null)
            {
                AvatarScript avatarScript = ARFaceModuleManager.Instance.mainAvatar.GetComponent<AvatarScript>();
                avatarScript.avatarIndexId = itemIndex;
                avatarScript.ChangeAnimationClip();

                ARFaceModuleManager.Instance.CharacterSelectionBoarderChange(itemIndex);
            }
        }
    }

    public void OnClickSelectEmojiBtn()
    {
        GameObject emoji = Instantiate(ARFaceModuleManager.Instance.EmojiItem, ARFaceModuleManager.Instance.videoEditCanvas.transform);
        emoji.GetComponent<Image>().sprite = ARFaceModuleManager.Instance.allEmojiSprite[itemIndex];
    }
}