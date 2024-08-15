using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Networking;

public class SummitDomeShaderApply : MonoBehaviour
{
    public int DomeId;
    public string ImageUrl;
    public MeshRenderer DomeMeshRenderer;
    public Cubemap cubemap;


    public void Init()
    {
        if (!string.IsNullOrEmpty(ImageUrl))
            StartCoroutine(DownloadDomeTexture());
    }

    IEnumerator DownloadDomeTexture()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(ImageUrl);
        request.SendWebRequest();
        while (!request.isDone)
            yield return null;
        if ((request.result == UnityWebRequest.Result.ConnectionError) || (request.result == UnityWebRequest.Result.ProtocolError))
            Debug.Log(request.error);
        else
        {
            ConvertToCubemap(((DownloadHandlerTexture)request.downloadHandler).texture);
            
        }
    }

    void ConvertToCubemap(Texture2D texture)
    {
        cubemap = new Cubemap(texture.width, TextureFormat.RGBA32, false);

        // Assuming the texture is in equirectangular format
        // You need to write your own logic to convert the equirectangular texture to cubemap faces
        // This is a simplified example
        for (int face = 0; face < 6; face++)
        {
            // Create a render texture for each face
            RenderTexture renderTex = new RenderTexture(cubemap.width, cubemap.height, 0);
            Graphics.Blit(texture, renderTex);

            // Copy the render texture to the cubemap face
            RenderTexture.active = renderTex;
            cubemap.SetPixels(ReadPixels(cubemap.width, cubemap.height), (CubemapFace)face);
            RenderTexture.active = null;
        }


        cubemap.Apply();
        DomeMeshRenderer.materials[1].SetTexture("_Cubemap", cubemap);
    }

    Color[] ReadPixels(int width, int height)
    {
        Texture2D tempTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        tempTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tempTexture.Apply();
        return tempTexture.GetPixels();
    }
}
