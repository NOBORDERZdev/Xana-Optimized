using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine.Video;
using SuperStar.Helpers;
using UnityEngine.SocialPlatforms.Impl;

public class DisplayImage : MonoBehaviour
{
    public Image imageComponent;
    public RectTransform imageFrame;
    public GameObject bufferPanel;

    private bool _SetframeRatio = true;

    public async Task<string> DownloadImageFromURL(string url)
    {
        if (url.EndsWith(".mp4") || url.EndsWith(".avi") || url.EndsWith(".mov"))
        {
            // Create a VideoPlayer component
            VideoPlayer videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = url;

            // Wait for the video to prepare
            while (!videoPlayer.isPrepared)
            {
                await Task.Delay(100);
            }

            // Capture the first frame of the video and create a sprite
            RenderTexture renderTexture = new RenderTexture((int)videoPlayer.width, (int)videoPlayer.height, 24);
            videoPlayer.targetTexture = renderTexture;
            videoPlayer.frame = 0;
            videoPlayer.Play();
            videoPlayer.SetDirectAudioMute(0, true);

            // Wait for a moment to ensure the frame is captured
            await Task.Delay(800);

            // Read pixels from the RenderTexture
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();

            // Create a sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            // Set the sprite to the image component
            imageComponent.sprite = sprite;
            imageComponent.enabled = true;
            imageComponent.preserveAspect = true;

            if (_SetframeRatio)
            {
                // Calculate desired dimensions based on sprite's aspect ratio
                float spriteAspectRatio = sprite.rect.width / sprite.rect.height;
                float frameAspectRatio = imageFrame.rect.width / imageFrame.rect.height;
                float newWidth, newHeight;
                if (spriteAspectRatio >= frameAspectRatio)
                {
                    newWidth = imageFrame.rect.width;
                    newHeight = newWidth / spriteAspectRatio;
                }
                else
                {
                    newHeight = imageFrame.rect.height;
                    newWidth = newHeight * spriteAspectRatio;
                }

                // Update the sizeDelta of the imageFrame
                imageFrame.sizeDelta = new Vector2(newWidth, newHeight);

                if (gameObject.GetComponentInParent<BoxCollider>() != null)
                    gameObject.GetComponentInParent<BoxCollider>().size = new Vector3(newWidth, newHeight, gameObject.GetComponentInParent<BoxCollider>().size.z);
            }
            imageComponent.color = Color.white;
            bufferPanel.SetActive(false);

            // Clean up VideoPlayer component
            Destroy(videoPlayer);
            // return null;
        }
        else
        {
            if (AssetCache.Instance.HasFile(url))
            {
                await Task.Delay(Random.Range(100, 500));
                AssetCache.Instance.LoadSpriteIntoImage(imageComponent, url, changeAspectRatio: false);
                ResizeImage();
            }
            else
            {
                AssetCache.Instance.EnqueueOneResAndWait(url, url, (success) =>
                {
                    if (success)
                    {
                        AssetCache.Instance.LoadSpriteIntoImage(imageComponent, url, changeAspectRatio: false);
                        ResizeImage();
                    }
                });
            }
        }
        return null;
    }

    void ResizeImage()
    {
        Sprite sprite = imageComponent.sprite;
        imageComponent.enabled = true;
        imageComponent.preserveAspect = true;
        if (_SetframeRatio)
        {
            // Calculate desired dimensions based on sprite's aspect ratio
            float spriteAspectRatio = sprite.rect.width / sprite.rect.height;
            float frameAspectRatio = imageFrame.rect.width / imageFrame.rect.height;
            float newWidth, newHeight;
            if (spriteAspectRatio >= frameAspectRatio)
            {
                // Calculate dimensions based on width
                newWidth = imageFrame.rect.width;
                newHeight = newWidth / spriteAspectRatio;
            }
            else
            {
                // Calculate dimensions based on height
                newHeight = imageFrame.rect.height;
                newWidth = newHeight * spriteAspectRatio;
            }
            // Update the sizeDelta of the imageFrame
            imageFrame.sizeDelta = new Vector2(newWidth, newHeight);
            if (gameObject.GetComponentInParent<BoxCollider>() != null)
                gameObject.GetComponentInParent<BoxCollider>().size = new Vector3(newWidth, newHeight, gameObject.GetComponentInParent<BoxCollider>().size.z);
        }
        bufferPanel.SetActive(false);
        imageComponent.color = Color.white;
    }
    public void ResetImgage()
    {
        imageComponent.sprite = null;
        imageComponent.color = new Color32(38, 40, 46, 255);
    }
}
