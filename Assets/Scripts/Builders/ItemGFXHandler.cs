using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

public class ItemGFXHandler : ItemComponent
{
    Renderer[] _renderers;

    private void Awake()
    {
        base.Awake();
        _renderers = GetComponentsInChildren<Renderer>();
    }

    public void SetMaterialColorFromItemData(Color color)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].materials.ForEachItem((d) =>
            {
                d.SetColor(Constants.BaseColor, color);
            });
        }
    }
}
