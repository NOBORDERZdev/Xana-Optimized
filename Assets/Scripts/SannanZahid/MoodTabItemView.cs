using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperStar.Helpers;
using System;

public class MoodTabItemView : MonoBehaviour
{
    public string m_ThumbnailDownloadURL;
    public Image _moodIcon;
    public TMPro.TMP_Text _moodName;
    UserAnimationPostFeature _animationPostManager;
    public void InitItem(string moodName,string imagURL, UserAnimationPostFeature animationPostManager)
    {
        m_ThumbnailDownloadURL = imagURL;
        _moodName.text = moodName;
        this._animationPostManager = animationPostManager;
    }
    private void OnEnable()
    {
        if (m_ThumbnailDownloadURL != "")
            LoadImagesFromRemote();
    }
    private void OnDisable()
    {
        AssetCache.Instance.RemoveFromMemoryDelayCoroutine(m_ThumbnailDownloadURL, true);
        _moodIcon.sprite = null;
        _moodIcon.sprite = default;
    }
    void LoadImagesFromRemote()
    {
        if (!string.IsNullOrEmpty(m_ThumbnailDownloadURL))
            StartCoroutine(DownloadAndLoadImage());
    }
    public IEnumerator DownloadAndLoadImage()
    {
        yield return null;
        if (AssetCache.Instance.HasFile(m_ThumbnailDownloadURL))
            AssetCache.Instance.LoadSpriteIntoImage(_moodIcon, m_ThumbnailDownloadURL, changeAspectRatio: true);
        else
            AssetCache.Instance.EnqueueOneResAndWait(m_ThumbnailDownloadURL, m_ThumbnailDownloadURL, (success) =>
            {
                if (success)
                    AssetCache.Instance.LoadSpriteIntoImage(_moodIcon, m_ThumbnailDownloadURL, changeAspectRatio: true);
            });
    }
    public void OnClickMood()
    {
        this._animationPostManager.SetMood(_moodName.transform.GetComponent<TextLocalization>().GetOriginalText());
    }
}
