using TMPro;
using UnityEditor;
using UnityEngine;

public class EnableGPUInstancing : EditorWindow
{
    [MenuItem("Tools/Enable GPU Instancing and Skip Built-In Shaders")]
    public static void EnableInstancingForCustomMaterials()
    {
        // Get all renderers in the scene
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        int materialsChecked = 0;
        int materialsUpdated = 0;
        int builtInMaterialsSkipped = 0;

        // Loop through each renderer
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            bool materialsChanged = false;

            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                if (material != null)
                {
                    materialsChecked++;

                    // Check if material uses a built-in shader (like unity_builtin_extra)
                    if (material.shader.name.StartsWith("Standard") || material.shader.name.Contains("unity_builtin_extra"))
                    {
                        Debug.Log($"[SKIPPED] Built-in shader: {material.shader.name} on object: {renderer.gameObject.name}");
                        builtInMaterialsSkipped++;
                        continue; // Skip this material, don't modify built-in shaders
                    }

                    // Enable instancing for custom shaders
                    if (!material.enableInstancing)
                    {
                        material.enableInstancing = true;
                        materialsChanged = true;
                        materialsUpdated++;
                        Debug.Log($"GPU instancing enabled for material: {material.name} on object: {renderer.gameObject.name}");
                    }
                    else
                    {
                        Debug.Log($"[ALREADY ENABLED] Material {material.name} on {renderer.gameObject.name} already has GPU instancing enabled.");
                    }
                }
            }

            // Apply the modified materials back to the renderer
            if (materialsChanged)
            {
                renderer.sharedMaterials = materials;
            }
        }

        // Output results
        Debug.Log($"{materialsUpdated} materials had GPU instancing enabled out of {materialsChecked} materials checked.");
        Debug.Log($"{builtInMaterialsSkipped} materials using built-in shaders were skipped.");

        TMP_Text[] tmpTextObjects = FindObjectsOfType<TMP_Text>();

        foreach (TMP_Text tmpText in tmpTextObjects)
        {
            // Access the shared material of the TMP object
            Material tmpMaterial = tmpText.fontSharedMaterial;

            // Check if the material is valid
            if (tmpMaterial != null)
            {
                // Ensure the material's shader supports instancing
                if (tmpMaterial.shader != null && !tmpMaterial.shader.isSupported)
                {
                    Debug.LogWarning("Shader " + tmpMaterial.shader.name + " does not support instancing.");
                    continue;
                }

                // Enable GPU instancing if it's not already enabled
                if (!tmpMaterial.enableInstancing)
                {
                    tmpMaterial.enableInstancing = true;
                    Debug.Log("Enabled GPU instancing for TMP material: " + tmpMaterial.name);
                }
                else
                {
                    Debug.Log("TMP material " + tmpMaterial.name + " already has GPU instancing enabled.");
                }
            }
            else
            {
                Debug.LogWarning("TMP object " + tmpText.name + " has no material assigned.");
            }
        }


    }
}
