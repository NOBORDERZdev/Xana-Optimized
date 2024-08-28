using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using TMPro;

public class ActionSelectionPanelHandler : MonoBehaviour
{
    public GameObject errorObj;

    List<AnimationData> animations;

    public void OnEnable()
    {
        errorObj.SetActive(false);
        if (GamePlayButtonEvents.inst != null)
        {
            GamePlayButtonEvents.inst.OnAnimationSelected += LoadAnimData;
            GamePlayButtonEvents.inst.AnimationDataUpdated += UpdateAnimList;
        }
        animations = new List<AnimationData>();
        LoadAnimList();
    }


    public void OnDisable()
    {
        if (GamePlayButtonEvents.inst != null)
        {
            GamePlayButtonEvents.inst.OnAnimationSelected -= LoadAnimData;
            GamePlayButtonEvents.inst.AnimationDataUpdated -= UpdateAnimList;
            GamePlayButtonEvents.inst.selectedActionIndex = -1;
        }
    }

    void LoadAnimList()
    {
        for (int i = 0; i < 10; i++)
        {
            AnimationData d = new AnimationData();
            animations.Add(d);
            string data = PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + i);
            if (!data.IsNullOrEmpty())
            {
                d = JsonUtility.FromJson<AnimationData>(data);
                animations[i] = d;
            }
        }
    }

    public void LoadAnimData(AnimationData animData)
    {
        if (CheckAnimationList(animData)) return;
        int selectedIndex = GamePlayButtonEvents.inst.selectedActionIndex;
        PlayerPrefsUtility.SetEncryptedString(ConstantsGod.ANIMATION_DATA + selectedIndex, JsonUtility.ToJson(animData));
        GamePlayButtonEvents.inst.AnimationDataUdpated(selectedIndex);
    }

    public void OnResetAnimationClick()
    {
        if (CanvasButtonsHandler.inst)                                                  // AH working
        {
            CanvasButtonsHandler.inst.AnimationBtnClose.gameObject.SetActive(false);
            CanvasButtonsHandler.inst.stopCurrentPlayingAnim = false;
        }

        for (int i = 0; i < 10; i++)
        {
            PlayerPrefsUtility.SetEncryptedString(ConstantsGod.ANIMATION_DATA + i, "");
            GamePlayButtonEvents.inst.AnimationDataUdpated(i);
        }
        GamePlayButtonEvents.inst.OnEmoteAnimationStop();
        animations = new List<AnimationData>();
        LoadAnimList();
    }

    private void UpdateAnimList(int index)
    {
        string data = PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + index);
        if (!data.IsNullOrEmpty())
        {
            AnimationData d = JsonUtility.FromJson<AnimationData>(data);
            animations[index] = d;
        }
        else
        {
            animations[index] = new AnimationData();
        }
    }


    private bool CheckAnimationList(AnimationData data)
    {
        if (Reaction_EmotePanel.instance)
        {
            if (Reaction_EmotePanel.instance.m_EmotePanel.activeInHierarchy)
            {
                if (GameManager.currentLanguage.Contains("en") && !CustomLocalization.forceJapanese)
                {
                    errorObj.GetComponent<TextMeshProUGUI>().text = "Duplicate Animation".ToString();
                }
            }
        }
        for (int i = 0; i < animations.Count; i++)
        {
            if (data != null && !animations[i].animationName.IsNullOrEmpty() && animations[i].animationName.Equals(data.animationName))
            {
                Debug.LogFormat("aniamtion name matched");
                errorObj.SetActive(true);
                return true;
            }
        }
        Debug.LogFormat("aniamtion name not here return false");
        return false;
    }
}
