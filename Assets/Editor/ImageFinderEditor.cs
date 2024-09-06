using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class ImageFinderEditor : EditorWindow
{
    public string targetFolder;
    public SceneAsset scene;
    [MenuItem("Tools/EditorScipts-AP/Move Images to Folder")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<ImageFinderEditor>("Find Images and Move");
    }

    private void OnGUI()
    {
        GUILayout.Label("Image Finder", EditorStyles.boldLabel);
        targetFolder = EditorGUILayout.TextField(targetFolder);


        if (GUILayout.Button("Find Image"))
        {
            MoveImagesToFolder();
        }

        if (GUILayout.Button("Find Raw Image"))
        {
            MoveRawImagesToFolder();
        }
    }

    List<Image> Images = new List<Image>();
    Image[] FindNthImageChild(GameObject root)
    {
        for (int i = 0; i < root.transform.childCount; i++)
        {
            Debug.Log(root.transform.GetChild(i).name);
            if (root.transform.GetChild(i).GetComponent<Image>())
                Images.Add(root.transform.GetChild(i).GetComponent<Image>());

            if (root.transform.GetChild(i).transform.childCount > 0)
                FindNthImageChild(root.transform.GetChild(i).gameObject);
        }

        return Images.ToArray();
    }

    RawImage[] FindNthRawImageChild(GameObject root)
    {
        List<RawImage> images = new List<RawImage>();

        for (int i = 0; i < root.transform.childCount; i++)
        {
            if (root.transform.GetChild(i).GetComponent<RawImage>())
                images.Add(root.transform.GetChild(i).GetComponent<RawImage>());

            if (root.transform.GetChild(i).transform.childCount > 0)
                FindNthRawImageChild(root.transform.GetChild(i).gameObject);
        }

        return images.ToArray();
    }


    void MoveImagesToFolder()
    {
        // Replace "ImagesFolder" with the path of the folder where you want to move the images
        //string targetFolder = "Assets/UI/HomeSceneUI";

        // Find all Image components in the scene


        Image[] images = FindObjectsOfType<Image>(true);
        //Transform[] temp = FindObjectsOfType<RectTransform>(true);
        //for (int i = 0; i < temp.Length; i++)
        //{
        //    Images.Clear();
        //    Debug.Log(temp[i].name);
        //    Image[] images = FindNthImageChild(temp[i].gameObject);
        //    Debug.Log("Count :- "+images.Length);
            if (images == null || images.Length == 0)
            {
                Debug.Log("No Images found on UI components in the scene.");
                return;
            }

            // Create the target folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder(targetFolder))
            {
                AssetDatabase.CreateFolder("Assets/UI", EditorSceneManager.GetActiveScene().name + " Graphics");
                targetFolder = "Assets/UI/" + EditorSceneManager.GetActiveScene().name + " Graphics";
            }

            foreach (Image image in images)
            {
                // Get the sprite associated with the Image component
                Sprite sprite = image.sprite;
                if (sprite == null)
                    continue;

                // Get the sprite asset path
                string spriteAssetPath = AssetDatabase.GetAssetPath(sprite);

                // Move the sprite asset to the target folder
                string newPath = Path.Combine(targetFolder, Path.GetFileName(spriteAssetPath));
                string s=AssetDatabase.MoveAsset(spriteAssetPath, newPath);
                Debug.Log(s);
            }

            // Refresh the AssetDatabase to apply changes
            AssetDatabase.Refresh();
            Debug.Log("All Images on UI components moved to the target folder.");
        //}
    }

    void MoveRawImagesToFolder()
    {
        // Replace "ImagesFolder" with the path of the folder where you want to move the images
        //string targetFolder = "Assets/UI/HomeSceneUI";

        // Find all Image components in the scene
        RawImage[] images = FindObjectsOfType<RawImage>(true);

        //GameObject temp = FindObjectOfType<GameObject>();
        //RawImage[] images = FindNthRawImageChild(temp);

        if (images == null || images.Length == 0)
        {
            Debug.Log("No Raw Images found on UI components in the scene.");
            return;
        }

        // Create the target folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            AssetDatabase.CreateFolder("Assets/UI", EditorSceneManager.GetActiveScene().name + " Graphics");
            targetFolder = "Assets/UI/" + EditorSceneManager.GetActiveScene().name + " Graphics";
        }

        foreach (RawImage image in images)
        {
            // Get the sprite associated with the Image component
            Texture texture = image.texture;
            if (texture == null)
                continue;

            // Get the sprite asset path
            string spriteAssetPath = AssetDatabase.GetAssetPath(texture);

            // Move the sprite asset to the target folder
            string newPath = Path.Combine(targetFolder, Path.GetFileName(spriteAssetPath));
            AssetDatabase.MoveAsset(spriteAssetPath, newPath);
        }

        // Refresh the AssetDatabase to apply changes
        AssetDatabase.Refresh();
        Debug.Log("All Images on UI components moved to the target folder.");
    }
}