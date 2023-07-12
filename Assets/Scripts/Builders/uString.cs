using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class uString : uVar<string, uStringComponent>
{
    public Text[] textComps = new Text[] { };
    public TMPro.TextMeshProUGUI[] textProComps = new TMPro.TextMeshProUGUI[] { };

    public override void OnValueChanged()
    {
        if (textComps != null)
        {
            for (int i = 0; i < textComps.Length; i++)
            {
                if (textComps[i] != null) textComps[i].text = m_value;
            }
        }

        if (textProComps != null)
        {
            for (int i = 0; i < textProComps.Length; i++)
            {
                if (textProComps[i] != null) textProComps[i].text = m_value;
            }
        }
    }
}

public abstract class uStringComponent : MonoBehaviour, IuVarHandler<string>
{
    public abstract void Handle(ref string s);
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(uString))]
public class uStringDrawer : uVarDrawer
{
    string[] arr = new string[] { "textComps", "textProComps", "components" };
    public override string[] arrays
    {
        get
        {
            return arr;
        }
    }
}
#endif