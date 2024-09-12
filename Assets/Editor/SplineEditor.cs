using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(SplineDone))]
public class SplineEditor : Editor
{
    private ReorderableList reorderableList;
    private SerializedProperty myCustomClassList;
    private int selectedIndex = -1;

    private void OnEnable()
    {
        myCustomClassList = serializedObject.FindProperty("_anchorList");

        reorderableList = new ReorderableList(serializedObject, myCustomClassList, true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Anchor List");
            },

            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                GUIContent ObjectLabel = new GUIContent($"Anchor {index}");
                var propertyHeight = EditorGUI.GetPropertyHeight(element, null, true);
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, propertyHeight),
                    element, ObjectLabel, true);
            },

            elementHeightCallback = (int index) => {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, null, true) + 4;
            },

            onSelectCallback = (ReorderableList list) => {
                selectedIndex = list.index;
            },
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SplineDone spline = (SplineDone)target;

        if (GUILayout.Button("Add Anchor"))
        {
            Undo.RecordObject(spline, "Add Anchor");
            spline.AddAnchor();
            spline.SetDirty();
            serializedObject.Update();
        }

        if (GUILayout.Button("Remove Last Anchor"))
        {
            Undo.RecordObject(spline, "Remove Last Anchor");
            spline.RemoveLastAnchor();
            spline.SetDirty();
            serializedObject.Update();
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_dots"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_normal"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_closedLoop"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_anchorList"));

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Set All Z = 0"))
        {
            Undo.RecordObject(spline, "Set All Z = 0");
            spline.SetAllZZero();
            spline.SetDirty();
            serializedObject.Update();
        }

        if (GUILayout.Button("Set All Y = 0"))
        {
            Undo.RecordObject(spline, "Set All Y = 0");
            spline.SetAllYZero();
            spline.SetDirty();
            serializedObject.Update();
        }

        if (GUILayout.Button("Set All Y = 1"))
        {
            Undo.RecordObject(spline, "Set All Y = 1");
            spline.SetAllYOne();
            spline.SetDirty();
            serializedObject.Update();
        }

        serializedObject.Update();
        reorderableList.DoLayoutList();

        if (selectedIndex >= 0 && selectedIndex < myCustomClassList.arraySize)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selected Element", EditorStyles.boldLabel);
            var selectedElement = myCustomClassList.GetArrayElementAtIndex(selectedIndex);
            EditorGUILayout.PropertyField(selectedElement, new GUIContent($"Element {selectedIndex}"), true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void OnSceneGUI()
    {
        SplineDone spline = (SplineDone)target;
        Vector3 transformPosition = spline.transform.position;
        Quaternion rotation = spline.transform.rotation;
        List<SplineDone.Anchor> anchorList = spline.GetAnchorList();

        if (anchorList != null)
        {
            for (int i = 0; i <= anchorList.Count - 1; i++)
            {
                SplineDone.Anchor anchor = anchorList[i];

                // Apply rotation to anchor and handle positions
                Vector3 rotatedAnchorPos = rotation * anchor.position;
                Vector3 rotatedHandleAPos = rotation * anchor.handleAPosition;
                Vector3 rotatedHandleBPos = rotation * anchor.handleBPosition;

                // Draw the anchor position with a wire cube
                Handles.color = Color.red;
                Handles.DrawWireCube(transformPosition + rotatedAnchorPos, Vector3.one * .5f);
                Handles.color = Color.white;

                if (reorderableList.index != -1)
                {
                    if (spline.GetAnchorList()[reorderableList.index] == anchor)
                    {
                        // Position handle for the anchor position
                        EditorGUI.BeginChangeCheck();
                        Vector3 newPosition = Handles.PositionHandle(transformPosition + rotatedAnchorPos, Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(spline, "Change Anchor Position");
                            anchor.position = Quaternion.Inverse(rotation) * (newPosition - transformPosition);
                            spline.SetDirty();
                            serializedObject.Update();
                        }

                        // Draw and handle the position for handleA
                        Handles.color = Color.green;
                        Handles.SphereHandleCap(0, transformPosition + rotatedHandleAPos, Quaternion.identity, .5f, EventType.Repaint);
                        Handles.color = Color.white;

                        EditorGUI.BeginChangeCheck();
                        newPosition = Handles.PositionHandle(transformPosition + rotatedHandleAPos, Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(spline, "Change Anchor Handle A Position");
                            anchor.handleAPosition = Quaternion.Inverse(rotation) * (newPosition - transformPosition);
                            spline.SetDirty();
                            serializedObject.Update();
                        }

                        // Draw and handle the position for handleB
                        Handles.color = Color.blue;
                        Handles.SphereHandleCap(0, transformPosition + rotatedHandleBPos, Quaternion.identity, .5f, EventType.Repaint);
                        Handles.color = Color.white;

                        EditorGUI.BeginChangeCheck();
                        newPosition = Handles.PositionHandle(transformPosition + rotatedHandleBPos, Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(spline, "Change Anchor Handle B Position");
                            anchor.handleBPosition = Quaternion.Inverse(rotation) * (newPosition - transformPosition);
                            spline.SetDirty();
                            serializedObject.Update();
                        }
                    }
                }

                // Draw lines connecting the anchor position to handleA and handleB
                Handles.color = (spline.GetAnchorList()[reorderableList.index] == anchor) ? Color.blue : Color.white;
                Handles.DrawLine(transformPosition + rotatedAnchorPos, transformPosition + rotatedHandleAPos, 3);
                Handles.DrawLine(transformPosition + rotatedAnchorPos, transformPosition + rotatedHandleBPos, 3);
            }

            // Draw Bezier
            for (int i = 0; i < spline.GetAnchorList().Count - 1; i++)
            {
                SplineDone.Anchor anchor = spline.GetAnchorList()[i];
                SplineDone.Anchor nextAnchor = spline.GetAnchorList()[i + 1];

                Vector3 anchorPos = transformPosition + (rotation * anchor.position);
                Vector3 nextAnchorPos = transformPosition + (rotation * nextAnchor.position);
                Vector3 handleBPos = transformPosition + (rotation * anchor.handleBPosition);
                Vector3 handleAPos = transformPosition + (rotation * nextAnchor.handleAPosition);

                Handles.DrawBezier(anchorPos, nextAnchorPos, handleBPos, handleAPos, Color.red, null, 5f);
            }

            if (spline.GetClosedLoop())
            {
                SplineDone.Anchor anchor = spline.GetAnchorList()[spline.GetAnchorList().Count - 1];
                SplineDone.Anchor nextAnchor = spline.GetAnchorList()[0];

                Vector3 anchorPos = transformPosition + (rotation * anchor.position);
                Vector3 nextAnchorPos = transformPosition + (rotation * nextAnchor.position);
                Vector3 handleBPos = transformPosition + (rotation * anchor.handleBPosition);
                Vector3 handleAPos = transformPosition + (rotation * nextAnchor.handleAPosition);

                Handles.color = Color.red;
                Handles.DrawBezier(anchorPos, nextAnchorPos, handleBPos, handleAPos, Color.red, null, 5f);
            }
        }
    }
}
