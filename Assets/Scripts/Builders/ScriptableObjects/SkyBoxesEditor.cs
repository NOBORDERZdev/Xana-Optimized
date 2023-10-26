#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkyBoxesData))]
public class SkyBoxesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (SkyBoxesData)target;

        if (GUILayout.Button("Add to Counter", GUILayout.Height(40)))
        {
            script.Assign();
        }

    }
}
#endif