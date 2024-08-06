using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearingRendererTexture : MonoBehaviour
{
    [SerializeField]
    public Renderer[] RendererComponents; // An array to hold multiple Renderer components

    void OnDisable()
    {
        foreach (Renderer renderer in RendererComponents)
        {
            if (renderer == null || renderer.sharedMaterial == null)
            {
                continue; // Skip if renderer or sharedMaterial is null
            }

            // Clear references
            renderer.sharedMaterial = null;
            DestroyImmediate(renderer);
        }
    }
}
