using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MsgSender : MonoBehaviour
{
    public List<bool> worksInEditor = new List<bool>();
    public List<string> msgs = new List<string>();
}


#if UNITY_EDITOR
[CustomEditor(typeof(MsgSender))]
public class MsgSenderEditor : Editor
{
    MsgSender script;
    void OnEnable()
    {
        script = target as MsgSender;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        for (int i = 0; i < script.msgs.Count; i++)
        {
            GUILayout.BeginHorizontal();
            script.worksInEditor[i] = EditorGUILayout.Toggle(script.worksInEditor[i], GUILayout.Width(10));
            script.msgs[i] = GUILayout.TextField(script.msgs[i]);
            if (GUILayout.Button("p", GUILayout.Width(20)))
            {
                script.msgs[i] = GUIUtility.systemCopyBuffer;
            }
            if (GUILayout.Button("Invoke", GUILayout.Width(50)))
            {
                if (!script.worksInEditor[i])
                {
                    if (Application.isPlaying)
                    {
                        script.transform.SendMessage(script.msgs[i]);
                    }
                }
                else script.transform.SendMessage(script.msgs[i]);
            }
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                script.worksInEditor.RemoveAt(i);
                script.msgs.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("");
        if (GUILayout.Button("+",GUILayout.Width(30)))
        {
            script.worksInEditor.Add(false);
            script.msgs.Add("");
        }
        GUILayout.EndHorizontal();
    }
}
#endif