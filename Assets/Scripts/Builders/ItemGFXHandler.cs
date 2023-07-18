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
}
