using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AHFrameManager : MonoBehaviour
{
    public static AHFrameManager instance;
    public GameObject framePrefab;
    public Vector3 frameLocalPos;
    public Vector3 frameLocalRot;
    public Vector3 frameLocalScale;
    public Vector3 spotLightPos;
    public Vector3 spotLightScale;
    public Vector3 colliderSize;
    public Vector3 coliderPos;
    public Vector3 spotLightPrefabPos;
    public GameObject spotLightPrefab;


    public GameObject frame;
    public Material FrameMaterial;
    public string squarSize;
    public string VideosquarSize;
    public string potraiteSize;
    public string landscapeSize;
    public List<ExhibitShowData> exhibitsSizes;


    public AHObjectPooler ref_PMYObjectPooler;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetTransformForFrameSpotLight(int index)
    {
        frameLocalPos = exhibitsSizes[index].FrameLocalPos;
        frameLocalScale = exhibitsSizes[index].FrameLocalScale;
        spotLightPos = exhibitsSizes[index].SpotLightPos;
        colliderSize = exhibitsSizes[index].ColliderSize;
        coliderPos = exhibitsSizes[index].ColiderPos;
        spotLightPrefabPos = exhibitsSizes[index].SpotLightPrefabPos;
    }

    [Serializable]
    public class ExhibitShowData
    {
        public string name;
        public Vector3 FrameLocalScale;
        public Vector3 FrameLocalPos;
        public Vector3 SpotLightPos;
        public Vector3 SpotLightPrefabPos;
        public Vector3 ColliderSize;
        public Vector3 ColiderPos;
    }
}
