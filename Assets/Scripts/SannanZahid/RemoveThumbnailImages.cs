using SuperStar.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveThumbnailImages : MonoBehaviour
{
    public Image worldThumbnail;

    private void OnDisable()
    {
        Destroy(worldThumbnail.sprite.texture);
        worldThumbnail.sprite = null;
    }
}
