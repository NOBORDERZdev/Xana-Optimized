using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    private Material[] targetMaterial;
    [SerializeField]
    private List<MeshFilter> meshFilters;
    [SerializeField]
    private List<Renderer> allRenderers = new();
    private CombineInstance[] combineInstances;
    private List<Material> uniqueMaterials = new List<Material>();

    private void OnEnable()
    {
        BuilderEventManager.CombineMeshes += CombineMeshe;
    }

    private void OnDisable()
    {
        BuilderEventManager.CombineMeshes -= CombineMeshe;
    }
    public void HandleRendererEvent(Renderer[] renderer, ItemData data)
    {
        // Handle the event here
        if (!CheckComponent(data))
            allRenderers.AddRange(renderer);

    }

    bool CheckComponent(ItemData data)
    {
        //print(data.rotatorComponent);
        if (data.rotatorComponentData.IsActive || data.collectibleComponentData.IsActive || data.translateComponentData.IsActive || data.toFroComponentData.IsActive || data.scalerComponentData.IsActive || data.rotateComponentData.IsActive || data.enemyNPCComponentData.IsActive || data.blindfoldedDisplayComponentData.IsActive || data.addForceComponentData.isActive || data.avatarChangerComponentData.IsActive || data.doorKeyComponentData.IsActive || data.chestKeyComponentData.IsActive || data.speicalItemComponentData.IsActive || data.ninjaComponentData.IsActive || data.quizComponentData.IsActive || data.throwThingsComponentData.IsActive || data.physicsComponentData.PhysicsComponentIsActive)
            return true;
        else return false;
    }

    public void CombineMeshe()
    {
        if (allRenderers.Count != 0)
            StartCoroutine(Meshes());
    }

    IEnumerator Meshes()
    {
        yield return null;
        targetMaterial = new Material[allRenderers.Count];
        for (int i = 0; i < allRenderers.Count; i++)
        {
            Material[] rendererMat = allRenderers[i].materials;
            if (rendererMat.Length == 1)
            {
                targetMaterial[i] = allRenderers[i].material;
            }
        }

        for (int i = 0; i < targetMaterial.Length; i++)
        {
            if (targetMaterial[i] != null && !IsMaterialAdded(targetMaterial[i]))
            {
                meshFilters.Clear();
                for (int j = 0; j < allRenderers.Count; j++)
                {
                    if (targetMaterial[i].HasProperty("_Color") && allRenderers[j].material.HasProperty("_Color"))
                    {
                        if (allRenderers[j] != null && allRenderers[j].material != null &&
                       targetMaterial[i].name == allRenderers[j].material.name &&
                       targetMaterial[i].color == allRenderers[j].material.color)
                        {
                            MeshFilter meshFilter = allRenderers[j].GetComponent<MeshFilter>();
                            if (meshFilter != null && meshFilter.mesh != null)
                            {
                                meshFilters.Add(meshFilter);
                            }
                        }
                    }
                }
                if (meshFilters.Count > 1)
                {
                    combineInstances = new CombineInstance[meshFilters.Count];
                    for (int k = 0; k < meshFilters.Count; k++)
                    {
                        if (meshFilters[k] != null && meshFilters[k].mesh != null)
                        {
                            combineInstances[k].mesh = meshFilters[k].mesh;
                            combineInstances[k].transform = meshFilters[k].transform.localToWorldMatrix;
                            // Disable the original GameObjects to hide the individual meshes
                            meshFilters[k].gameObject.SetActive(false);
                        }
                    }
                    // Create a new combined object
                    GameObject combinedObject = new GameObject("CombinedMesh" + i.ToString());
                    combinedObject.transform.SetParent(this.transform);
                    combinedObject.transform.position = Vector3.zero;
                    combinedObject.transform.rotation = Quaternion.identity;
                    combinedObject.transform.localScale = Vector3.one;
                    MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
                    MeshRenderer combinedRenderer = combinedObject.AddComponent<MeshRenderer>();
                    Material uniqueMaterial = targetMaterial[i]; // Assign unique material
                    uniqueMaterials.Add(uniqueMaterial); // Add to uniqueMaterials list
                    if (uniqueMaterial != null)
                    {
                        combinedRenderer.sharedMaterial = uniqueMaterial;
                    }
                    Mesh combinedMesh = new Mesh();
                    combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Set UInt32 as the index format
                    combinedMesh.CombineMeshes(combineInstances, true);
                    combinedMeshFilter.mesh = combinedMesh;
                    meshFilters.Clear();
                }
            }
        }
    }
    private bool IsMaterialAdded(Material material)
    {
        foreach (Material uniqueMaterial in uniqueMaterials)
        {
            if (uniqueMaterial.name == material.name && uniqueMaterial.color == material.color)
            {
                return true;
            }
        }
        return false;
    }
}