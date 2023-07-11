using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureErrorEvents : MonoBehaviour
{
    public Button retryButton;
    // Start is called before the first frame update
    private void OnEnable()
    {
        retryButton.onClick.AddListener(GalleryImageManager.Instance.Retry);
    }
    
    private void OnDisable()
    {
        retryButton.onClick.RemoveListener(GalleryImageManager.Instance.Retry);
    }
}
