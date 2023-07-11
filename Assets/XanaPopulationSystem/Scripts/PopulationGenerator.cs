using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PopulationGenerator : MonoBehaviour
{
    public CharacterData characterData;
    private GenerationType generationType;
    [Range(3 , 6)]
    public int MaxNumberToSpawn;
    

    private List<GameObject> _currentPopulationObjects;
    private List<Transform> Waypoints;
    private Transform [] wpData;
    private bool isError;


    public void GeneratePopulation()
    {
        LoadWayPoints();
        if (!isError)
        {
            _currentPopulationObjects = new List<GameObject>();
            int randomAmount = 2;
            if (randomAmount > wpData.Length)
            {
                randomAmount = wpData.Length;
            }
            switch (generationType)
            {
                
                case GenerationType.Sequential:
                    for (int i = 0; i < randomAmount; i++)
                    {
                        _currentPopulationObjects.Add(characterData.CreateSequentialCharacter());
                        _currentPopulationObjects[i].transform.parent = gameObject.transform;
                        _currentPopulationObjects[i].transform.position = wpData[i].position;
                        _currentPopulationObjects[i].transform.rotation = Quaternion.Euler(_currentPopulationObjects[i].transform.rotation.x , Random.Range(0 , 180f) , _currentPopulationObjects[i].transform.rotation.z);
                    }

                    break;
                case GenerationType.Random:
                    for (int i = 0; i < randomAmount; i++)
                    {
                        _currentPopulationObjects.Add(characterData.CreateRandomCharacter());
                        _currentPopulationObjects[i].transform.parent = gameObject.transform;
                        _currentPopulationObjects[i].transform.position = wpData[i].position;
                        _currentPopulationObjects[i].transform.rotation = Quaternion.Euler(_currentPopulationObjects[i].transform.rotation.x ,Random.Range(0 , 180f) , _currentPopulationObjects[i].transform.rotation.z);
                        MapAtlasUV atlasUV = _currentPopulationObjects[i].GetComponent<MapAtlasUV>();
                        if (atlasUV)
                        {
                            atlasUV.MapAtlasUVs();
                        }
                    }

                    break;
            }
        }
        else
        {
            Debug.LogError("Something went wrong in Population System.See Above Errors.");
            isError = false;
        }

    }
    

    private void LoadWayPoints()
    {

        #region Scriptable Approach

        // if (wpData)
        // {
        //     int randomRange = Random.Range(3, wpData.waypoints.Count);
        //     for (int i = 0; i < randomRange; i++)
        //     {
        //         _currentPopulationObjects[i].transform.position = wpData.waypoints[i].position;
        //         _currentPopulationObjects[i].transform.rotation = wpData.waypoints[i].rotation;
        //         _currentPopulationObjects[i].SetActive(true);
        //     }
        // }
        // else
        // {
        //     Debug.LogError("Way point data not found or they are less than number to spawn variable , population will not be placed properly : Ask Moazan");
        // }

        #endregion

        GameObject WayPointParent = GameObject.Find("PopulationSpawnPoints");
        if (WayPointParent)
        {
            wpData = WayPointParent.GetComponentsInChildren<Transform>();
            if (wpData == null && wpData.Length == 0)
            {
                Debug.LogError("PopulationSpawnPoints has no waypoints as children");
                isError = true;
            }
        }
        else
        {
            Debug.LogError("PopulationSpawnPoints not Found");
            isError = true;
        }


    }

    #region Fill Utility 

    [ContextMenu("Find and Fill Waypoints")]
    public void FillWayPoints()
    {
        GameObject[] positions = GameObject.FindGameObjectsWithTag("waypoint");
        WaypointsData wpData = Resources.Load<WaypointsData>("Environment Data/" + FeedEventPrefab.m_EnvName + " Data/PopulationWayPointData/WaypointsData");
        Debug.Log(FeedEventPrefab.m_EnvName);
        
        string path = "Assets/Resources/Environment Data/" + FeedEventPrefab.m_EnvName + " Data";

        if (!wpData)
        {
            path = "Assets/Resources/Environment Data/" + FeedEventPrefab.m_EnvName + " Data";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            WaypointsData Prop = null;
            if (!Directory.Exists(path + "/PopulationWaypointData"))
            {
                Directory.CreateDirectory(path + "/PopulationWaypointData");
                Prop = ScriptableObject.CreateInstance<WaypointsData>();
#if UNITY_EDITOR
                AssetDatabase.CreateAsset(Prop, path + "/PopulationWayPointData/WaypointsData.asset");
                AssetDatabase.SaveAssets();
#endif
            }

        }
        
        wpData = Resources.Load<WaypointsData>("Environment Data/" + FeedEventPrefab.m_EnvName + " Data/PopulationWayPointData/WaypointsData");
        
        

        if (wpData)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                TransformData data = new TransformData(positions[i].transform.position, positions[i].transform.rotation);
                wpData.AddWayPoint(data);
            }
        }
        else
        {
            Debug.LogError("Way point data not found , population will not be placed properly : Ask Moazan");
        }
    }

    #endregion
}

public enum GenerationType
{
    Sequential,
    Random
}
