using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuilderData
{
    public static List<SpawnPointData> spawnPoint=new List<SpawnPointData>();
    public static ServerData mapData;
}

[System.Serializable]
public class SpawnPointData
{
    public GameObject spawnObject;
    public bool IsActive;
}
