using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureReleaser : MonoBehaviour
{
    public RenderTexture renderTexture;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDisable()
    {
        // optimize the render texture data     // AR changes start
        renderTexture.Release();
       // renderTexture = null;
        Resources.UnloadUnusedAssets();
        System.GC.Collect();                           // AR changes end
    }

}
