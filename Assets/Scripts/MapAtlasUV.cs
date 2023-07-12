using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAtlasUV : MonoBehaviour
{
    public SkinnedMeshRenderer[] renderers;
    public MeshFilter[] filters;
    public Material atlasMaterial;
    
    [ContextMenu("ChangeUVs")]
    public void MapAtlasUVs()
    {
        int length = renderers.Length;

        if (length == filters.Length)
        {
            for (int i = 0; i < length; i++)
            {
                //renderers[i].material = atlasMaterial;
              
                renderers[i].sharedMesh.uv = filters[i].sharedMesh.uv;
            }
        }

    }
}
