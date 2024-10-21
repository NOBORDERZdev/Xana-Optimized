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
    private string ImageUrl;
    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(ImageUrl))
            AssetCache.Instance.RemoveFromMemory(ImageUrl,true);
    }

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
            //print("In else :: " + url);
            if (url.Contains("https://cdn.xana.net/xanaprod/Defaults/"))
            {
                url = url.Replace("https://cdn.xana.net/xanaprod/Defaults/", "https://aydvewoyxq.cloudimg.io/" + (APIBasepointManager.instance.IsXanaLive ? "_xanaprod_" : "_apitestxana_") + "/xanaprod/Defaults/");
            }
            Vector2 imageSize = await GetImageSizeAsync(url);
            var aspectRatioToSize = new Dictionary<string, string>
            {
                { "1:1", "?width=256&height=256" },
                { "16:9", "?width=512&height=288" },
                { "9:16", "?width=288&height=512" },
                { "4:3", "?width=400&height=300" },
                { "3:4", "?width=300&height=400" },
                { "21:9", "?width=800&height=342" },
                { "3:2", "?width=450&height=300" },
                { "2:3", "?width=300&height=450" },
                { "16:10", "?width=640&height=400" },
                { "10:16", "?width=400&height=640" },
                { "10:1", "?width=500&height=50" }, 
                { "1:10", "?width=50&height=500" }  
            };
            string aspectRatio = GetAspectRatio(imageSize);
            //Debug.Log($"Image Size: {imageSize.x}x{imageSize.y}, Aspect Ratio: {aspectRatio} , URL {url}");

            if (aspectRatioToSize.TryGetValue(aspectRatio, out string sizeParams))
            {
                url += sizeParams;
            }
            //else
            //{
            //    // Handle other uncommon aspect ratios by preserving aspect ratio but with standard sizes
            //    url += "?width=400&height=400";
            //}
            //print("AFTER :: " + url);
            ImageUrl = url;
            if (AssetCache.Instance.HasFile(url))
            {
                Debug.LogError("File found kurime :- "+url);
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
        //Debug.Log($"Starting GetImageSizeAsync for URL: {url}");

        // Step 1: Make a HEAD request to get general information
        using (UnityWebRequest headRequest = UnityWebRequest.Head(url))
        {
            await headRequest.SendWebRequest();

            if (headRequest.result == UnityWebRequest.Result.ConnectionError || headRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error in GetImageSizeAsync: {headRequest.error}");
                return Vector2.zero;
            }

            // Step 2: Check if the image dimensions are in headers (commonly provided by some servers)
            string widthHeader = headRequest.GetResponseHeader("X-Image-Width");
            string heightHeader = headRequest.GetResponseHeader("X-Image-Height");

            if (!string.IsNullOrEmpty(widthHeader) && !string.IsNullOrEmpty(heightHeader) &&
                int.TryParse(widthHeader, out int width) &&
                int.TryParse(heightHeader, out int height))
            {
                //Debug.Log($"Parsed Width: {width}, Parsed Height: {height}");
                return new Vector2(width, height);
            }
        }

        //Debug.LogWarning("Image dimensions not found in headers. Attempting to download a small portion of the image.");

        // Step 3: Make a GET request for partial content of the image (only the initial bytes)
        using (UnityWebRequest getRequest = UnityWebRequest.Get(url))
        {
            getRequest.SetRequestHeader("Range", "bytes=0-1024"); // Request the first 1024 bytes of the file
            await getRequest.SendWebRequest();

            if (getRequest.result == UnityWebRequest.Result.ConnectionError || getRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"Error in GetImageSizeAsync while downloading partial image: {getRequest.error}");
                return Vector2.zero;
            }

            byte[] imageData = getRequest.downloadHandler.data;

            // Step 4: Parse the image header manually to extract the width and height
            try
            {
                Vector2 imageSize = ParseImageSizeFromHeader(imageData);
                //Debug.Log($"Extracted Image Width: {imageSize.x}, Height: {imageSize.y}");
                return imageSize;
            }
            catch (System.Exception ex)
            {
                Debug.Log($"Error in parsing image size: {ex.Message}");
            }
        }

        Debug.Log("Failed to determine image dimensions.");
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

        // Calculate and round the aspect ratio to three decimal places
        float aspectRatio = Mathf.Round((width / height) * 1000f) / 1000f;

        // Debugging: Log the calculated aspect ratio
        //Debug.Log($"Calculated Aspect Ratio: {aspectRatio}, Width: {width}, Height: {height}");

        // Define common aspect ratios for direct comparison
        float aspect1_1 = 1.0f;
        float aspect16_9 = Mathf.Round((16f / 9f) * 1000f) / 1000f;
        float aspect9_16 = Mathf.Round((9f / 16f) * 1000f) / 1000f;
        float aspect4_3 = Mathf.Round((4f / 3f) * 1000f) / 1000f;
        float aspect3_4 = Mathf.Round((3f / 4f) * 1000f) / 1000f;
        float aspect16_10 = Mathf.Round((16f / 10f) * 1000f) / 1000f;
        float aspect10_16 = Mathf.Round((10f / 16f) * 1000f) / 1000f;
        float aspect10_1 = 10.0f;  // Add for 10:1
        float aspect1_10 = 0.1f;   // Add for 1:10

        // Check for common aspect ratios without using a tolerance value
        if (Mathf.Approximately(aspectRatio, aspect1_1))
        {
            //Debug.Log("Matched Aspect Ratio: 1:1");
            return "1:1";
        }
        else if (Mathf.Approximately(aspectRatio, aspect16_9))
        {
            //Debug.Log("Matched Aspect Ratio: 16:9");
            return "16:9";
        }
        else if (Mathf.Approximately(aspectRatio, aspect9_16))
        {
            //Debug.Log("Matched Aspect Ratio: 9:16");
            return "9:16";
        }
        else if (Mathf.Approximately(aspectRatio, aspect4_3))
        {
            //Debug.Log("Matched Aspect Ratio: 4:3");
            return "4:3";
        }
        else if (Mathf.Approximately(aspectRatio, aspect3_4))
        {
            //Debug.Log("Matched Aspect Ratio: 3:4");
            return "3:4";
        }
        else if (Mathf.Approximately(aspectRatio, aspect16_10))
        {
            //Debug.Log("Matched Aspect Ratio: 16:10");
            return "16:10";
        }
        else if (Mathf.Approximately(aspectRatio, aspect10_16))
        {
            //Debug.Log("Matched Aspect Ratio: 10:16");
            return "10:16";
        }
        else if (Mathf.Approximately(aspectRatio, aspect10_1))
        {
            //Debug.Log("Matched Aspect Ratio: 10:1");
            return "10:1";
        }
        else if (Mathf.Approximately(aspectRatio, aspect1_10))
        {
            //Debug.Log("Matched Aspect Ratio: 1:10");
            return "1:10";
        }

        // Fallback to general aspect ratio calculation
        return GetGeneralAspectRatio(width, height);
    }


    private string GetGeneralAspectRatio(float width, float height)
    {
        int roundedWidth = Mathf.RoundToInt(width);
        int roundedHeight = Mathf.RoundToInt(height);

        // Calculate GCD to simplify the ratio
        int gcd = GCD(roundedWidth, roundedHeight);

        int simplifiedWidth = roundedWidth / gcd;
        int simplifiedHeight = roundedHeight / gcd;

        // Debugging: Log the simplified general aspect ratio
        //Debug.Log($"Simplified General Aspect Ratio: {simplifiedWidth}:{simplifiedHeight}");

        // Approximate the aspect ratio to common values
        float aspectRatio = (float)simplifiedWidth / simplifiedHeight;

        // Define common aspect ratios to compare against
        Dictionary<string, float> commonRatios = new Dictionary<string, float>
        {
            { "1:1", 1f },
            { "16:9", 16f / 9f },
            { "9:16", 9f / 16f },
            { "4:3", 4f / 3f },
            { "3:4", 3f / 4f },
            { "16:10", 16f / 10f },
            { "10:16", 10f / 16f }
        };

        float tolerance = 0.05f; // Adjust this value for greater accuracy

        foreach (var ratio in commonRatios)
        {
            if (Mathf.Abs(aspectRatio - ratio.Value) < tolerance)
            {
                //Debug.Log($"Matched Approximate Aspect Ratio: {ratio.Key}");
                return ratio.Key;
            }
        }

        // Return the simplified ratio as a string if it doesn't match common aspect ratios
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
