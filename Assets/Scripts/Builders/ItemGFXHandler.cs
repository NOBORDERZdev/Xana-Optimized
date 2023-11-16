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
        if (gameObject.name.Contains("pfEFT02"))
            UpdateMaterialShaders();
    }

    public void SetMaterialColorFromItemData(Color color)
    {
        if (color.Equals(Color.white)) return;

        for (int i = 0; i < _renderers.Length; i++)
        {
            for (int j = 0; j < _renderers[i].materials.Length; j++)
            {
                color.a = _renderers[i].materials[j].color.a;
                _renderers[i].materials[j].SetColor(Constants.BaseColor, color);
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
                        if (d.shader==GamificationComponentData.instance.proceduralRingShader || d.shader==GamificationComponentData.instance.uberShader || d.shader.name.Contains("Particles Additive Alpha8"))
                        {
                            if (s == null || !s.name.Equals(d.shader.name))
                            {
                                s = Shader.Find(d.shader.name);
                            }
                            if (s != null)
                            {
                                d.shader = s;
                            }
                        }
                    });
                }
            }
        }
    }

    public override void AssignItemComponentType()
    {
    }
}
