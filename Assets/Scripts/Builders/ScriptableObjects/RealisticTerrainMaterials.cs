using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RealisticTerrainMaterials")]
public class RealisticTerrainMaterials : ScriptableObject
{
    public List<RealisticMaterialData> terrainMaterials;

    public RealisticMaterialData GetRealisticMaterialData(int id)
    {
        return terrainMaterials.Find((d) => d.id == id);
    }

    //public Sprite GetSprite(int id)
    //{
    //    return terrainMaterials.Find((d) => d.id == id).thumbnail;
    //}

    //public Material GetMaterial(int id)
    //{
    //    return terrainMaterials.Find((d) => d.id == id).material;
    //}

    public void SetData()
    {
        //foreach (RealisticMaterialData realisticMaterialData in terrainMaterials)
        //{
        //    realisticMaterialData.name = realisticMaterialData.material.name.Replace(" ", "");
        //    realisticMaterialData.shaderName = realisticMaterialData.material.shader.name;
        //}
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RealisticTerrainMaterials))]
public class TestScriptableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (RealisticTerrainMaterials)target;

        if (GUILayout.Button("Set Data"))
        {
            script.SetData();
        }

    }
}
#endif