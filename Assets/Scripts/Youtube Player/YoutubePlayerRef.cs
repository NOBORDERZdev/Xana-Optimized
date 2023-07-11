using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoutubePlayerRef : MonoBehaviour
{ 
    public GameObject liveStreamerRef;
    public GameObject normalPlayerParent;

    public Texture defaultTexture;

    public static YoutubePlayerRef instance;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        liveStreamerRef.GetComponent<Renderer>().sharedMaterial.mainTexture = defaultTexture;
        gameObject.AddComponent<YoutubeVideoByScene>();
    }

}
