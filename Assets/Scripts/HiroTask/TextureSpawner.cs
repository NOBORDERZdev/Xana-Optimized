using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using TMPro;


public class TextureSpawner : MonoBehaviour
{
    public TMP_InputField urlInputField;
    public GameObject spritePrefab;

    private Texture2D tex;
    private Sprite sprite;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void DownloadTexture()
    {
        var request = new HTTPRequest(new System.Uri(urlInputField.text), OnIconDownloadFinished);
        request.Send();
    }
    void OnIconDownloadFinished(HTTPRequest req, HTTPResponse resp)
    {
        tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        tex.LoadImage(resp.Data);
        tex.filterMode = FilterMode.Bilinear; //if no mipmap exists, this is same as bilinear
        tex.wrapMode = TextureWrapMode.Clamp;

        sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f, 3, SpriteMeshType.FullRect);
        //GameObject img = Instantitate
    }



}
