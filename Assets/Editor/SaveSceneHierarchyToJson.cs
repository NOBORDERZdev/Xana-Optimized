using UnityEngine;
using UnityEditor;
using System.IO;


public class SaveParentObjectsToJson : EditorWindow
{
    //priority Tag Names 
    public const string high = "High";
    public const string medium = "Medium";
    public const string low = "Low";
    private GameObject AssetParent;
    [MenuItem("Tools/EditorScipts-AP/Create Json Of Scene Objects")]
    static void ShowWindow()
    {
        GetWindow<SaveParentObjectsToJson>("Save Parent Objects to JSON");
    }

    public void OnGUI()
    {
        AssetParent = EditorGUILayout.ObjectField("Assets Parent Object", AssetParent, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("Save to JSON"))
        {
            if (AssetParent != null)
            {
                string jsonPath = EditorUtility.SaveFilePanel("Scene Objects transform", Application.persistentDataPath, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, "json");

                if (!string.IsNullOrEmpty(jsonPath))
                {
                    if (AssetParent.GetComponent<XanaWorldInfoHolder>())
                        SaveParentObjectsToJsonFile(jsonPath, AssetParent.GetComponent<XanaWorldInfoHolder>());
                    else
                        SaveParentObjectsToJsonFile(jsonPath);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please add a asset parent and make sure XanaWorldInfoHolder.cs is attached on it", "OK");
            }
        }
    }

    public void SaveParentObjectsToJsonFile(string filePath, XanaWorldInfoHolder worldJsonHolder = null)
    {
        SceneData sceneObjectsData = new SceneData();
        Transform[] rootObjects = AssetParent.transform.GetAllChildren();//UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (Transform rootObject in rootObjects)
        {
            // Only include parent objects (objects with no children)
            ObjectsInfo data = new ObjectsInfo();

            if (rootObject.GetComponent<SetPrefabInfo>() != null)
            {
                data.addressableKey = rootObject.GetComponent<SetPrefabInfo>().downloadKey;
            }
            else
                data.addressableKey=rootObject.name;
            data.name = rootObject.name;
            data.position = rootObject.transform.localPosition;
            data.rotation = rootObject.transform.rotation;
            data.scale = rootObject.transform.localScale;
            data.objectBound = CalculateBoundsWithChildren(rootObject.transform);
            data.tagName = rootObject.tag;
            data.layerIndex = rootObject.gameObject.layer;
            data.subWorldComponent = GetSubworldComponent(rootObject.gameObject);
            data.subWorldIndex = GetSubworldIndex(rootObject.gameObject);
            data.priority = GetObjectPriority(rootObject.gameObject);
            data.isActive = rootObject.gameObject.activeSelf;
            data.lightmapData = GetLightmapData(rootObject.gameObject);
            data.summitDomeInfo=GetDomeId(rootObject.gameObject);
            sceneObjectsData.SceneObjects.Add(data);
        }

        string jsonData = JsonUtility.ToJson(sceneObjectsData);

        // Use a custom serialization method to write JSON data
        File.WriteAllText(filePath, jsonData);
        if (worldJsonHolder != null)
            worldJsonHolder.worldJson = jsonData;
        Debug.Log("Parent objects saved to JSON: " + filePath);
    }

    public Bounds CalculateBoundsWithChildren(Transform target)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }

        // If the target doesn't have a renderer, calculate bounds of its children
        Bounds bounds = new Bounds(target.position, Vector3.zero);
        foreach (Transform child in target)
        {
            Bounds childBounds = CalculateBoundsWithChildren(child);
            bounds.Encapsulate(childBounds);
        }

        return bounds;
    }

    private LightmapData[] GetLightmapData(GameObject prefab)
    {
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>();
        LightmapData[] lightmapData = new LightmapData[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            lightmapData[i] = new LightmapData();
            lightmapData[i].lightmapIndex = renderers[i].lightmapIndex;
            lightmapData[i].lightmapScaleOffset = renderers[i].lightmapScaleOffset;
        }
        return lightmapData;
    }

    public Priority GetObjectPriority(GameObject rootObject)
    {
        if(rootObject.GetComponent<SetPrefabInfo>()!=null)
        {
            return rootObject.GetComponent<SetPrefabInfo>().objectPriority;
        }
        return Priority.defaultPriority;
    }

    public SummitDomeInfo GetDomeId(GameObject rootObject)
    {
        SummitDomeInfo summitDomeInfo = new SummitDomeInfo();
        if (rootObject.GetComponent<SetDomeId>()!=null)
        {
            summitDomeInfo.domeIndex = rootObject.GetComponent<SetDomeId>().domeId;
            return summitDomeInfo;
        }
        summitDomeInfo.domeIndex = 0;
        return summitDomeInfo;
    }

    public bool GetSubworldComponent(GameObject rootObject)
    {
        if (rootObject.GetComponent<SummitSubWorldIndex>() != null)
        {
            return true;
        }
        return false;
    }

    public int GetSubworldIndex(GameObject rootObject)
    {
        if (rootObject.GetComponent<SummitSubWorldIndex>() != null)
        {
            int subworldIndex = rootObject.GetComponent<SummitSubWorldIndex>().SubworldIndex;
            return subworldIndex;
        }
        return -1;
    }

    
}



