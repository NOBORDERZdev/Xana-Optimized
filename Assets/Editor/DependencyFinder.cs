using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DependencyFinder : EditorWindow
{
    [MenuItem("Tools/EditorScipts-AP/Dependency Finder")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DependencyFinder));
    }

    private Object targetObject;
    private List<string> references;

    private void OnGUI()
    {
        GUILayout.Label("Find References in Unity", EditorStyles.boldLabel);

        targetObject = EditorGUILayout.ObjectField("Target Object", targetObject, typeof(Object), true);

        if (GUILayout.Button("Find References"))
        {
            if (targetObject != null)
            {
                references = FindReferences(targetObject);
            }
        }

        if (references != null && references.Count > 0)
        {
            GUILayout.Label("References:");
            foreach (string referencePath in references)
            {
                GUILayout.Label(referencePath);
            }
        }
    }

    private List<string> FindReferences(Object target)
    {
        string targetAssetPath = AssetDatabase.GetAssetPath(target);
        List<string> references = new List<string>();

        // Search in scene files
        string[] scenePaths = AssetDatabase.FindAssets("t:Scene");
        foreach (string scenePath in scenePaths)
        {
            string sceneFilePath = AssetDatabase.GUIDToAssetPath(scenePath);
            string[] dependencies = AssetDatabase.GetDependencies(sceneFilePath, false);

            if (ArrayUtility.Contains(dependencies, targetAssetPath))
            {
                references.Add(sceneFilePath);
            }
        }

        // Search in other assets
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in assetPaths)
        {
            string[] dependencies = AssetDatabase.GetDependencies(assetPath, false);

            if (ArrayUtility.Contains(dependencies, targetAssetPath))
            {
                references.Add(assetPath);
            }
        }

        return references;
    }
}