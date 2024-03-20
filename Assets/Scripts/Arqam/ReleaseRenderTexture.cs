using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseRenderTexture : MonoBehaviour
{
    private RenderTexture renderTexture;

    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable()
    {
        if (!renderTexture)
        {
            renderTexture = new RenderTexture(1024, 1024, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
            renderTexture.antiAliasing = 4;
            renderTexture.useMipMap = true;
            renderTexture.filterMode = FilterMode.Trilinear;

            this.GetComponent<Camera>().targetTexture = renderTexture;   // my changes
            UserLoginSignupManager.instance.aiPresetImage.texture = renderTexture;
        }
    }

    private void OnDisable()
    {
        // optimize the render texture data     // AR changes start
        this.GetComponent<Camera>().targetTexture = null;
        Object.Destroy(renderTexture);

        //renderTexture.Release();
        //renderTexture = null;
        Resources.UnloadUnusedAssets();
    }

}
