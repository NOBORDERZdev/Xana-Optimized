using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RealisticTerrainMaterials")]
public class RealisticTerrainMaterials : ScriptableObject
{
    public List<RealisticMaterialData> terrainMaterials;

    public RealisticMaterialData GetRealisticMaterialData(int id)
    {
        return terrainMaterials.Find((d) => d.id == id);
    }

    public Sprite GetSprite(int id)
    {
        return terrainMaterials.Find((d) => d.id == id).thumbnail;
    }

    public Material GetMaterial(int id)
    {
        return terrainMaterials.Find((d) => d.id == id).material;
    }
}
