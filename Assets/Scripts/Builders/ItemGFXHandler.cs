using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
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
        if ((ItemBase.categoryId.Value + ItemBase.subCategoryId.Value).Equals("EFT02")) UpdateMaterialShaders();
    }

    public void SetMaterialColorFromItemData(Color color)
    {
        if (color.Equals(Color.white)) return;

        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].materials.ForEachItem((d) =>
            {
                d.SetColor(Constants.BaseColor, color);
            });
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
                        if (d.shader.name.Contains("Procedural") || d.shader.name.Contains("Ubershader"))
                        {
                            //Debug.LogFormat("{0}-{1}", _renderers[i].name, d.shader.name);
                            //Debug.LogFormat("{0}-{1}", d.shader.isSupported, d.shader.subshaderCount);
                            if (s == null || !s.name.Equals(d.shader.name))
                            {
                                //Debug.Log("finding shader: " + d.shader.name);
                                s = Shader.Find(d.shader.name);
                            }
                            if (s != null)
                            {
                                //Debug.Log("found shader: " + s.name);
                                //Debug.LogFormat("{0}-{1}", s.isSupported, s.subshaderCount);
                                d.shader = s;
                            }
                        }
                    });
                }
            }
        }
    }
}
