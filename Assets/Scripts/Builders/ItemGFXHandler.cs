using UnityEngine;

public class ItemGFXHandler : ItemComponent
{
    internal Renderer[] _renderers;

    private void Awake()
    {
        base.Awake();
        _renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnEnable()
    {
        //if (gameObject.name.Contains("pfEFT02"))
        //    UpdateMaterialShaders();
    }

    public void SetMaterialColorFromItemData(Color color)
    {
        if (!this.gameObject.activeInHierarchy)
            return;
        if (color.Equals(Color.white)) return;

        for (int i = 0; i < _renderers.Length; i++)
        {
            for (int j = 0; j < _renderers[i].materials.Length; j++)
            {
                if (_renderers[i].materials[j].HasProperty("_Color"))
                {
                    color.a = _renderers[i].materials[j].color.a;
                    _renderers[i].materials[j].SetColor(Constants.BaseColor, color);

                }
            }
        }
    }

    private void UpdateMaterialShaders()
    {
        //Debug.Log(ItemBase.assetId.Value);
        Shader s = null;
        if (_renderers != null && _renderers.Length > 0)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null)
                {
                    _renderers[i].sharedMaterials.ForEachItem((d) =>
                    {
                        string shaderName = d.shader.name;

                        if (shaderName.Contains("Procedural") || shaderName.Contains("Particles Additive Alpha8") || shaderName.Contains("Ubershader"))
                        {
                            d.shader = Shader.Find(shaderName);
                        }
                        else if (d.name == "mtEFT0210004_PlaceMode")
                        {
                            d.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");
                        }
                        else if (d.name == "mtEFT0210005_PlaceMode" || d.name == "mtEFT0210026_PlaceMode_04")
                        {
                            d.shader = Shader.Find("Mobile/Particles/Alpha Blended");
                        }
                        else if (d.name == "mtEFT0210018_PlaceMode" || d.name == "mtEFT0210021_PlaceMode")
                        {
                            d.shader = Shader.Find("Legacy Shaders/Particles/Additive");
                        }
                        else if (d.name == "mtEFT0210019_PlaceMode_02" || d.name == "mtEFT0210026_PlaceMode")
                        {
                            d.shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply");
                        }
                    });
                }
            }
        }
    }

    public override void AssignItemComponentType()
    {
    }

    public override void CollisionExitBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //throw new System.NotImplementedException();
    }
}
