using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearRendererTexturedata : MonoBehaviour
{
    [SerializeField]
    public Renderer RendererComponent;

    // Update is called once per frame
    void OnDisable()
    {
        if (RendererComponent != null && RendererComponent.sharedMaterial != null)
        {
            // Clear references to avoid potential issues
            RendererComponent.sharedMaterial = null;
            DestroyImmediate(RendererComponent);
        }
    }
}
