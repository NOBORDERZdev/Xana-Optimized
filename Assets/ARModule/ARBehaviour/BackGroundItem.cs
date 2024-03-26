using DigitalRubyShared;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ARBackgroundItem : MonoBehaviour
{
    public int itemIndex;
    [HideInInspector] public byte R;
    [HideInInspector] public byte G;
    [HideInInspector] public byte B;
    [HideInInspector] public byte A;
    public GameObject textColor;
    public GameObject selectionImage;
    [HideInInspector] public BGFilterHandler controller;
    
    public GameObject ContentPanel;
    public void OnClickItem()
    {
        if (this.gameObject.name == "None" && SceneManager.GetActiveScene().name == "ARModuleFaceTrackingScene")
        {
            VideoRoomHandler.Instance.BackgroundImage.gameObject.SetActive(false);
        }
        else
        {
            VideoRoomHandler.Instance.BackgroundImage.gameObject.SetActive(true);
            VideoRoomHandler.Instance.BackgroundImage.sprite = gameObject.transform.GetChild(0).GetComponent<Image>().sprite;
            if (this.gameObject.name == "None")
            {
                VideoRoomHandler.Instance.BackgroundImage.color = new Color(243f, 243f, 243f);
            }
            else
            {
                VideoRoomHandler.Instance.BackgroundImage.color = Color.white;
            }
        }

        ARFaceModuleHandler.Instance.SelectionBorderOnBackgroundImage(itemIndex);

        ARFaceModuleHandler.Instance.SelectionBorderOnBackgroundColor(0);
        /*for (int i = 0; i < transform.parent.childCount; i++)
        {
            transform.parent.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
        transform.GetChild(1).gameObject.SetActive(true);*/
    }


    public void Initializ(byte r, byte g, byte b, byte a, BGFilterHandler ctrlr, GameObject Content)
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
        BGFilterHandler.instance.OnClickFilterItem(R,G,B,A);

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
        ARFaceModuleHandler.Instance.SelectionBorderOnBackgroundImage(1);

        if (!VideoRoomHandler.Instance.BackgroundImage.gameObject.activeSelf)
        {
            VideoRoomHandler.Instance.BackgroundImage.gameObject.SetActive(true);
        }
        VideoRoomHandler.Instance.BackgroundImage.sprite = null;
        VideoRoomHandler.Instance.BackgroundImage.color = gameObject.transform.GetChild(0).GetComponent<Image>().color;

        ARFaceModuleHandler.Instance.SelectionBorderOnBackgroundColor(itemIndex);
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

        //VideoRoomHandler.Instance.mainVolume.profile = VideoRoomHandler.Instance.filterVolumeProfile[index];

        if (VideoRoomHandler.Instance.filterMainVolume.profile.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out var color))
        {
            color.colorFilter.overrideState = true;
            color.colorFilter.value = Color.white;
        }
    }

    public void OnClickSelectCharacterBtn()
    {
        if ((SceneManager.GetActiveScene().name != "ARModuleActionScene" || VideoRoomHandler.Instance.IsVideoScreenImageScreenAvtive) && itemIndex != 0)
        {
            Vector3 pos = new Vector3(0, 0, 2f);
            GameObject avatartItem = Instantiate(ARFaceModuleHandler.Instance.allCharacterItemList[itemIndex]);
            avatartItem.transform.localPosition = ARFaceModuleHandler.Instance.allCharacterItemDefaultPos;
            avatartItem.transform.localScale = ARFaceModuleHandler.Instance.allCharacterItemDefaultScale;
            ARFaceModuleHandler.Instance.addAvtarItem.Add(avatartItem);
            avatartItem.GetComponent<FingersPanRotateScaleComponentScript>().SetMinScaleOfAvatar();

            avatartItem.GetComponent<ARAvatarController>().avatarIndexId = itemIndex;

            if (SceneManager.GetActiveScene().name == "ARModulePlanDetectionScene" && !VideoRoomHandler.Instance.IsVideoScreenImageScreenAvtive)
            {
                if (ARPlacement.Instance.spawnedObject != null)
                {
                    Vector3 finalPos = ARPlacement.Instance.spawnedObject.transform.localPosition;
                    finalPos.z = finalPos.z - 0.01f;
                    avatartItem.transform.localPosition = finalPos;
                    //avatartItem.transform.position = ARPlacement.Instance.spawnedObject.transform.position;
                    //Debug.LogError("AvatrItem:" + avatartItem.transform.position + "    :scale:" + avatartItem.transform.localScale);
                }
                /*if (avatartItem.GetComponent<ARAvatarController>().avatarShadowPlanObj != null)
                {
                    avatartItem.GetComponent<ARAvatarController>().avatarShadowPlanObj.SetActive(true);
                }*/
            }
        }
        else
        {
            if (ARFaceModuleHandler.Instance.mainAvatar != null)
            {
                ARAvatarController avatarScript = ARFaceModuleHandler.Instance.mainAvatar.GetComponent<ARAvatarController>();
                avatarScript.avatarIndexId = itemIndex;
                avatarScript.ChangeAnimationClip();

                ARFaceModuleHandler.Instance.CharacterSelectionBoarderChange(itemIndex);
            }
        }
    }

    public void OnClickSelectEmojiBtn()
    {
        GameObject emoji = Instantiate(ARFaceModuleHandler.Instance.EmojiItem, ARFaceModuleHandler.Instance.videoEditCanvas.transform);
        emoji.GetComponent<Image>().sprite = ARFaceModuleHandler.Instance.allEmojiSprite[itemIndex];
    }
}