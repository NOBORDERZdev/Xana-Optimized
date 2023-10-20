using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuilderData
{
    public static List<SpawnPointData> spawnPoint=new List<SpawnPointData>();
    public static List<DownloadQueueData> preLoadspawnPoint = new List<DownloadQueueData>();
    public static ServerData mapData;
}

[System.Serializable]
public class SpawnPointData
{
    public GameObject spawnObject;
    public bool IsActive;
}

[System.Serializable]
public class DownloadQueueData
{
    public string ItemID;
    public string RuntimeItemName;
    public Vector3 Position;    //local position of the item
    public Vector3 Scale;
    public Quaternion Rotation;
    public bool IsActive;
}

