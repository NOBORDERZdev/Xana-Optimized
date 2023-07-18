using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicMuseumManager : MonoBehaviour
{
    public List<RoomData> rooms;
    public GameObject frame;
    public GameObject spotLightPrefab;
    public Material FrameMaterial;
    public string squarSize;
    public string VideosquarSize;
    public string potraiteSize;
    public string landscapeSize;
    public List<ExhibatShowData> exhibatsSizes;

    public void CloseRoom(int no) {
        if(rooms[no].Obj)
            rooms[no].Obj.SetActive(false);
        if (rooms[no].Door)
            rooms[no].Door.SetActive(true);
    }

    public void OpenRoom(int no) {
        if (rooms[no].Obj)
        {
            rooms[no].Obj.SetActive(true);
        }
        if (rooms[no].Door)
        {
            rooms[no].Obj.SetActive(true);
        }
    }
}

[Serializable]
public class RoomData { 
    public bool IsInUse;
    public GameObject Obj;
    public GameObject Door;
}

[Serializable]
public class ExhibatShowData {
    public string name;
    public Vector3 FrameLocalScale;
    public Vector3 FrameLocalPos;
    public Vector3 SpotLightPos;
    public Vector3 SpotLightPrefabPos;
    public Vector3 ColliderSize;
    public Vector3 ColiderPos;
}



