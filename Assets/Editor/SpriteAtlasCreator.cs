using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.U2D;

public class SpriteAtlasCreator : MonoBehaviour
{
    [MenuItem("Tools/Create Sprite Atlas")]
    public static void CreateSpriteAtlas()
    {
        // Find all SpriteRenderers, Images, and RawImages in the scene, including inactive objects
        SpriteRenderer[] spriteRenderers = Resources.FindObjectsOfTypeAll<SpriteRenderer>();
        Image[] uiImages = Resources.FindObjectsOfTypeAll<Image>();
        RawImage[] rawImages = Resources.FindObjectsOfTypeAll<RawImage>();

        // Create a new SpriteAtlas
        SpriteAtlas spriteAtlas = new SpriteAtlas();

        // Set some default settings for the sprite atlas (optional)
        SpriteAtlasPackingSettings packingSettings = spriteAtlas.GetPackingSettings();
        packingSettings.enableRotation = true;
        packingSettings.enableTightPacking = true;
        spriteAtlas.SetPackingSettings(packingSettings);

        // Use a HashSet to collect unique sprites and textures
        HashSet<Object> uniqueObjects = new HashSet<Object>();

        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            if (renderer != null && renderer.sprite != null)
            {
                uniqueObjects.Add(renderer.sprite);
            }
        }

        foreach (Image img in uiImages)
        {
            if (img != null && img.sprite != null)
            {
                uniqueObjects.Add(img.sprite);
            }
        }

        foreach (RawImage rawImg in rawImages)
        {
            if (rawImg != null && rawImg.texture != null)
            {
                uniqueObjects.Add(rawImg.texture);
            }
        }

        // Convert the HashSet to an array and add all collected unique objects to the SpriteAtlas
        SpriteAtlasExtensions.Add(spriteAtlas, new List<Object>(uniqueObjects).ToArray());

        // Create a folder if it doesn't exist to save the atlas
        string atlasFolderPath = "Assets/SpriteAtlases";
        if (!AssetDatabase.IsValidFolder(atlasFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "SpriteAtlases");
        }

        // Save the SpriteAtlas asset in the folder
        string atlasPath = $"{atlasFolderPath}/SpriteAtlas.asset";
        AssetDatabase.CreateAsset(spriteAtlas, atlasPath);
        AssetDatabase.SaveAssets();

        Debug.Log("Sprite Atlas created successfully at " + atlasPath);
    }
}
