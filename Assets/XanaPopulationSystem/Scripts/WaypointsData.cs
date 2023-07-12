using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaypointsData" , menuName = "WaypointsData")]
public class WaypointsData : ScriptableObject
{
    public List<TransformData> waypoints;

    public void AddWayPoint(TransformData wayPoint)
    {
        if (waypoints == null)
        {
            waypoints = new List<TransformData>();
        }
        waypoints.Add(wayPoint);
    }
}

[Serializable]
public class TransformData
{
    public Vector3 position;
    public Quaternion rotation;


    public TransformData(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
