using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseRenderTexture : MonoBehaviour
{
    private RenderTexture renderTexture;

    private void OnEnable()
    {
        if (!renderTexture)
        {
            renderTexture = new RenderTexture(1024, 1024, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm);
            renderTexture.antiAliasing = 4;
            renderTexture.useMipMap = true;
            renderTexture.filterMode = FilterMode.Trilinear;

            this.GetComponent<Camera>().targetTexture = renderTexture;   // my changes
            UserLoginSignupManager.instance.AiPresetImage.texture = renderTexture;
            UserLoginSignupManager.instance.AiPresetImageforEditProfil.texture = renderTexture;
        }
    }

    //private void OnDisable()
    //{
    //    // optimize the render texture data     // AR changes start
       
    //        this.GetComponent<Camera>().targetTexture = null;
    //        Object.Destroy(renderTexture);

    //        //renderTexture.Release();
    //        //renderTexture = null;
    //        Resources.UnloadUnusedAssets();
    //    }
   

}
