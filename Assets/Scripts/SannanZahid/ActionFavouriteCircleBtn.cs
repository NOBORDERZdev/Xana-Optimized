using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Mozilla;
using SuperStar.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionFavouriteCircleBtn : MonoBehaviour
{
    public int IndexOfBtn = 0;
    public string AnimationName;
    public string ThumbnailURL;
    public EmoteReactionItemBtnHandler.ItemType TypeOfAction;
    public Transform HeighlightObj;
    [SerializeField] private Image _actionImg;
    private bool _actionSelected = false;
    private bool _longPress = default;
    private float _timer = 0f;
    private float longPressTimer = 2f;
    private void OnEnable()
    {
        InitializeBtn();
    }
    private void OnDisable()
    {
        if (ThumbnailURL != "")
        {
            ///>--MemoryClean Stoped AssetCache.Instance.RemoveFromMemoryDelayCoroutine(ThumbnailURL, true);
        }
    }
    private void Update()
    {
        if (!_longPress)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= longPressTimer)
        {
            ActionManager.OpenActionFavouritPanel.Invoke(true);
            _longPress = false;
            _timer = 0f;
        }
    }

    public void PointerDown()
    {
        _longPress = true;
    }
    public void PointerUp()
    {
        _timer = 0f;
        _longPress = false;
    }
    public void PointerClicked()
    {
        if (PlayerController.isJoystickDragging)
            return;
        PlayerActionInteraction();
    }
    public void ClearActionButtonData()
    {
        ClearSelectedActionData();
        AnimationName = default;
        ThumbnailURL = default;
        _actionImg.sprite = default;
        _actionImg.gameObject.SetActive(false);
        _actionSelected = false;
    }
    public void InitializeBtn()
    {
        if (PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn) != "")
        {
            ActionData actionData = JsonUtility.FromJson<ActionData>(PlayerPrefsUtility.GetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn));
            if(actionData.AnimationName != "")
            {
                AnimationName = actionData.AnimationName;
                TypeOfAction = actionData.TypeOfAction;
                ThumbnailURL = actionData.ThumbnailURL;
                _actionSelected = true;
                LoadImageFromURL();
            }
            else
            {
                ClearActionButtonData();
            }

        }
        else
        {
            ClearActionButtonData();
        }
    }
    public void SelectedAction(ActionData dataObj)
    {
        PlayerPrefsUtility.SetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn, 
            JsonUtility.ToJson(dataObj).ToString());
    }
    public void ClearSelectedActionData()
    {
        PlayerPrefsUtility.SetEncryptedString(ConstantsGod.ANIMATION_DATA + IndexOfBtn,"");
    }
    public void PlayerActionInteraction()
    {
        if(_actionSelected)
        {
            ActionData dataObj = new ActionData();
            dataObj.AnimationName = AnimationName;
            dataObj.ThumbnailURL = ThumbnailURL;
            dataObj.TypeOfAction = TypeOfAction;
            ActionCircleBtnHighlightHandler.ActivateHighlightsByIndex?.Invoke(IndexOfBtn);
            ActionManager.ActionBtnClick?.Invoke(dataObj);
        }
        else
        {
            ActionManager.OpenActionFavouritPanel.Invoke(true);
        }
    }
    private void LoadImageFromURL()
    {
        if (ThumbnailURL != "")
        {
            AssetCache.Instance.EnqueueOneResAndWait(ThumbnailURL, ThumbnailURL, (success) =>
            {
                if (success)
                {
                    AssetCache.Instance.LoadSpriteIntoImage(_actionImg, ThumbnailURL, changeAspectRatio: true);
                    _actionImg.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Download Failed");
                }
            });
        }
    }
    public void ActivateHeighlight(bool flag)
    {
        HeighlightObj.gameObject.SetActive(flag);
    }
}