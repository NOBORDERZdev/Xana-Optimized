using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class AssignCustomComponent : EditorWindow
{
    //private string label = "Enter tag Here";
    private int selectedTagIndex = 0;

    [MenuItem("Tools/EditorScipts-AP/Apply Rigidbody & Tag")]

    static void ShowWindow()
    {
        GetWindow<AssignCustomComponent>("Apply Rigidbody component to selected");
    }

    public void OnGUI()
    {
        EditorGUILayout.Space();
        if (GUILayout.Button("Add RigidBody on Selected"))
        {
            AddRigidbody();
        }

        EditorGUILayout.Space();
        //GUILayout.Label("Select Tag for GameObjects", EditorStyles.boldLabel);

        selectedTagIndex = EditorGUILayout.Popup("Select Tag:", selectedTagIndex, UnityEditorInternal.InternalEditorUtility.tags);

        if (GUILayout.Button("Apply Tag"))
        {
            ApplyLabelToSelectedObjects(UnityEditorInternal.InternalEditorUtility.tags[selectedTagIndex]);
            this.Close();
        }
    }

    static void AddRigidbody()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            // Check if the GameObject already has a Rigidbody component
            if (obj.GetComponent<Rigidbody>() == null)
            {
                // If it doesn't, add a Rigidbody component
                obj.AddComponent<Rigidbody>();
            }
            else
            {
                obj.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }


    void ApplyLabelToSelectedObjects(string tag)
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            foreach (Transform childTransform in obj.GetComponentsInChildren<Transform>())
            {
                childTransform.tag = tag;
            }
        }
    }

}
