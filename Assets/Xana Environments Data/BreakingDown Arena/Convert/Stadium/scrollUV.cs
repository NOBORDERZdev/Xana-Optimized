using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollUV : MonoBehaviour
{
    public float scrollX = 1.0f;
    public float scrolly = 0.0f; 

    private void Start() 
    {
        
    }

    private void Update() 
    {
        float offsetX = Time.time * scrollX;
        float offsetY = Time.time * scrolly;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offsetX, offsetY); 
    }
}
