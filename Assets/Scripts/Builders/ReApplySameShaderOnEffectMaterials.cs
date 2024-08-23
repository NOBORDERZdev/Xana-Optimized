using System.Collections.Generic;
using UnityEngine;

public class ReApplySameShaderOnEffectMaterials : MonoBehaviour
{
    private Renderer[] _renderers;
    private Dictionary<string, Shader> _shaderCache = new Dictionary<string, Shader>();

    private void OnEnable()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        ApplyMaterialShader();
    }

    private void ApplyMaterialShader()
    {
        foreach (Renderer renderer in _renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material == null) continue;

                string shaderName = material.shader.name;
                Shader newShader = null;

                if (_shaderCache.TryGetValue(shaderName, out newShader))
                {
                    material.shader = newShader;
                }
                else
                {
                    if (shaderName.Contains("Procedural") || shaderName.Contains("Particles Additive Alpha8") || shaderName.Contains("Ubershader"))
                    {
                        newShader = Shader.Find(shaderName);
                    }
                    else
                    {
                        switch (material.name)
                        {
                            case "mtEFT0210004_PlaceMode":
                                newShader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");
                                break;
                            case "mtEFT0210005_PlaceMode":
                            case "mtEFT0210026_PlaceMode_04":
                                newShader = Shader.Find("Mobile/Particles/Alpha Blended");
                                break;
                            case "mtEFT0210018_PlaceMode":
                            case "mtEFT0210021_PlaceMode":
                            case "mtTLP0110001_PlaceMode":
                            case "mtTLP0110001_PlaceMode02":
                            case "mtTLP0110001_PlaceMode03":
                                newShader = Shader.Find("Legacy Shaders/Particles/Additive");
                                break;
                            case "mtEFT0210019_PlaceMode_02":
                            case "mtEFT0210026_PlaceMode":
                                newShader = Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply");
                                break;
                        }
                    }

                    if (newShader != null)
                    {
                        shaderName = newShader.name;
                        _shaderCache[shaderName] = newShader;
                        material.shader = newShader;
                    }
                }
            }
        }
    }
}