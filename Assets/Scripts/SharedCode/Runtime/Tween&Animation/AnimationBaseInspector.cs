using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimationBaseInspector : MonoBehaviour
{
}
#if UNITY_EDITOR
[CustomEditor(typeof(AnimationBaseInspector))]
public class AnimationBaseInspectorEditor : Editor
{
    AnimationBase ab = null;

    void OnEnable()
    {
        ab = (target as AnimationBaseInspector).transform.GetComponent<AnimationBase>();
    }

    public override void OnInspectorGUI()
    {
        bool b = GUI.enabled;
        GUI.enabled = Application.isPlaying && (ab!=null); 
        if (GUILayout.Button("Start"))
        {
            ab.StartAnim();
        }
        if (GUILayout.Button("Stop"))
        {
            ab.StopAnim();
        }
        if (GUILayout.Button("Reset"))
        {
            ab.ResetAnim();
        }
        GUI.enabled = b; 
    }
}
#endif
