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
            print("In else :: " + url);
            if (url.Contains("https://cdn.xana.net/"))
            {
                url = url.Replace("https://cdn.xana.net/", "https://aydvewoyxq.cloudimg.io/_xanaprod_/");
            }
            Vector2 imageSize = await GetImageSizeAsync(url);
            string aspectRatio = GetAspectRatio(imageSize);
            Debug.Log($"Image Size: {imageSize.x}x{imageSize.y}, Aspect Ratio: {aspectRatio}");

            // Add query parameters based on aspect ratio
            // Add query parameters based on aspect ratio
            switch (aspectRatio)
            {
                case "1:1":
                    url += "?width=256&height=256";
                    break;
                case "16:9":
                    url += "?width=512&height=288";
                    break;
                case "9:16":
                    url += "?width=288&height=512";
                    break;
                case "4:3":
                    url += "?width=400&height=300";
                    break;
                case "3:4":
                    url += "?width=300&height=400";
                    break;
                case "21:9":
                    url += "?width=800&height=342";
                    break;
                case "3:2":
                    url += "?width=450&height=300";
                    break;
                case "2:3":
                    url += "?width=300&height=450";
                    break;
                default:
                    // Handle other uncommon aspect ratios by preserving aspect ratio but with standard sizes
                    url += "?width=400&height=400";
                    break;
            }
            print("AFTER :: " + url);

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

    public async Task<Vector2> GetImageSizeAsync(string url)
    {
        Debug.Log($"Starting GetImageSizeAsync for URL: {url}");

        // Step 1: Make a HEAD request to get general information
        using (UnityWebRequest headRequest = UnityWebRequest.Head(url))
        {
            await headRequest.SendWebRequest();

            if (headRequest.result == UnityWebRequest.Result.ConnectionError || headRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error in GetImageSizeAsync: {headRequest.error}");
                return Vector2.zero;
            }

            // Step 2: Check if the image dimensions are in headers (commonly provided by some servers)
            string widthHeader = headRequest.GetResponseHeader("X-Image-Width");
            string heightHeader = headRequest.GetResponseHeader("X-Image-Height");

            if (!string.IsNullOrEmpty(widthHeader) && !string.IsNullOrEmpty(heightHeader) &&
                int.TryParse(widthHeader, out int width) &&
                int.TryParse(heightHeader, out int height))
            {
                Debug.Log($"Parsed Width: {width}, Parsed Height: {height}");
                return new Vector2(width, height);
            }
        }

        Debug.LogWarning("Image dimensions not found in headers. Attempting to download a small portion of the image.");

        // Step 3: Make a GET request for partial content of the image (only the initial bytes)
        using (UnityWebRequest getRequest = UnityWebRequest.Get(url))
        {
            getRequest.SetRequestHeader("Range", "bytes=0-1024"); // Request the first 1024 bytes of the file
            await getRequest.SendWebRequest();

            if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error in GetImageSizeAsync while downloading partial image: {getRequest.error}");
                return Vector2.zero;
            }

            byte[] imageData = getRequest.downloadHandler.data;

            // Step 4: Parse the image header manually to extract the width and height
            try
            {
                Vector2 imageSize = ParseImageSizeFromHeader(imageData);
                Debug.Log($"Extracted Image Width: {imageSize.x}, Height: {imageSize.y}");
                return imageSize;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error in parsing image size: {ex.Message}");
            }
        }

        Debug.LogWarning("Failed to determine image dimensions.");
        return Vector2.zero;
    }

    // Method to parse image dimensions from header
    private Vector2 ParseImageSizeFromHeader(byte[] imageData)
    {
        // This method should parse the width and height from the image header.
        // It would need to handle different formats like JPEG, PNG, etc.
        // For simplicity, here's a placeholder:

        if (imageData == null || imageData.Length < 24)
        {
            throw new System.Exception("Insufficient data to determine image size.");
        }

        // Example parsing for PNG (first 24 bytes contain size information)
        if (imageData[0] == 0x89 && imageData[1] == 0x50 && imageData[2] == 0x4E && imageData[3] == 0x47)
        {
            int width = (imageData[16] << 24) | (imageData[17] << 16) | (imageData[18] << 8) | imageData[19];
            int height = (imageData[20] << 24) | (imageData[21] << 16) | (imageData[22] << 8) | imageData[23];
            return new Vector2(width, height);
        }

        // You can add more format-specific parsing here (e.g., JPEG)

        throw new System.Exception("Unsupported image format.");
    }


    private string GetAspectRatio(Vector2 imageSize)
    {
        float width = imageSize.x;
        float height = imageSize.y;

        if (width == 0 || height == 0)
        {
            return "Unknown";
        }

        float aspectRatio = width / height;
        float inverseAspectRatio = height / width;
        float tolerance = 0.01f; // Smaller tolerance for greater precision

        // Common aspect ratios and their inverses
        if (Mathf.Abs(aspectRatio - 1f) < tolerance)
        {
            return "1:1";
        }
        else if (Mathf.Abs(aspectRatio - (16f / 9f)) < tolerance)
        {
            return "16:9";
        }
        else if (Mathf.Abs(aspectRatio - (9f / 16f)) < tolerance)
        {
            return "9:16";
        }
        else if (Mathf.Abs(aspectRatio - (4f / 3f)) < tolerance)
        {
            return "4:3";
        }
        else if (Mathf.Abs(aspectRatio - (3f / 4f)) < tolerance)
        {
            return "3:4";
        }
        else if (Mathf.Abs(aspectRatio - (16f / 10f)) < tolerance)
        {
            return "16:10";
        }
        else if (Mathf.Abs(aspectRatio - (10f / 16f)) < tolerance)
        {
            return "10:16";
        }

        // Fallback to general aspect ratio calculation
        return GetGeneralAspectRatio(width, height);
    }

    private string GetGeneralAspectRatio(float width, float height)
    {
        int roundedWidth = Mathf.RoundToInt(width);
        int roundedHeight = Mathf.RoundToInt(height);

        // Calculate GCD to simplify ratio
        int gcd = GCD(roundedWidth, roundedHeight);

        int simplifiedWidth = roundedWidth / gcd;
        int simplifiedHeight = roundedHeight / gcd;

        return $"{simplifiedWidth}:{simplifiedHeight}";
    }

    // Helper method to calculate the Greatest Common Divisor (GCD) using the Euclidean algorithm
    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
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
