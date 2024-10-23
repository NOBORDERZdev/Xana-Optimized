using UnityEditor;
using UnityEngine;
using System.IO;

public class CameraCaptureToPNG : MonoBehaviour
{
    [MenuItem("Tools/Capture Camera to PNG")]
    public static void CaptureCameraToPNG()
    {
        // Find the camera (make sure your scene has a camera tagged as "MainCamera")
        Camera camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("No camera tagged as MainCamera found in the scene.");
            return;
        }

        // Set the camera's clear flags to Solid Color and the background color to transparent
        CameraClearFlags originalClearFlags = camera.clearFlags;
        Color originalBackgroundColor = camera.backgroundColor;

        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0, 0, 0, 0); // Fully transparent

        // Render the camera's view to a RenderTexture
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        camera.targetTexture = renderTexture;
        camera.Render();

        // Create a Texture2D to store the rendered image
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Save the texture as a PNG file
        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, "CapturedImage.png");
        File.WriteAllBytes(path, bytes);
        Debug.Log($"Saved PNG with transparency to: {path}");

        // Clean up
        camera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(renderTexture);
        DestroyImmediate(texture);

        // Restore the camera's original settings
        camera.clearFlags = originalClearFlags;
        camera.backgroundColor = originalBackgroundColor;
    }
}
