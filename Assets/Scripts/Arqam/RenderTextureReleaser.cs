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
    private void OnEnable()
    {
        if (!renderTexture)
        {
            renderTexture = new RenderTexture(1024, 1024, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
            renderTexture.antiAliasing = 8;
            this.GetComponent<Camera>().targetTexture = renderTexture;   // my changes
            UserRegisterationManager.instance.renderImage.texture = renderTexture;
        }
    }

    private void OnDisable()
    {
        // optimize the render texture data     // AR changes start
        renderTexture.Release();
        renderTexture = null;
        Resources.UnloadUnusedAssets();
    }

}
