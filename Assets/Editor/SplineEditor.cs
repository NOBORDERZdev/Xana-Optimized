using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(SplineDone))]
public class SplineEditor : Editor {



    private ReorderableList reorderableList;
    private SerializedProperty myCustomClassList;
    private int selectedIndex = -1;

    private void OnEnable()
    {
        myCustomClassList = serializedObject.FindProperty("_anchorList");

        reorderableList = new ReorderableList(serializedObject, myCustomClassList, true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Anchor List");
            },

            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                GUIContent ObjectLebel = new GUIContent($"Anchor {index}");
                var propertyHeight = EditorGUI.GetPropertyHeight(element, null, true);
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, propertyHeight),
                    element,ObjectLebel, true);
            },

            elementHeightCallback = (int index) =>
            {
                var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, null, true) + 4;
            },

            onSelectCallback = (ReorderableList list) =>
            {
                selectedIndex = list.index;
            },
            
            
        };
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        SplineDone spline = (SplineDone)target;

        if (GUILayout.Button("Add Anchor")) {
            Undo.RecordObject(spline, "Add Anchor");
            spline.AddAnchor();
            spline.SetDirty();
            serializedObject.Update();
        }

        if (GUILayout.Button("Remove Last Anchor")) {
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

        if (GUILayout.Button("Set All Z = 0")) {
            Undo.RecordObject(spline, "Set All Z = 0");
            spline.SetAllZZero();
            spline.SetDirty();
            serializedObject.Update();
        }
        if (GUILayout.Button("Set All Y = 0")) {
            Undo.RecordObject(spline, "Set All Y = 0");
            spline.SetAllYZero();
            spline.SetDirty();
            serializedObject.Update();
        } 
        if (GUILayout.Button("Set All Y = 1")) {
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

    public void OnSceneGUI() {
        SplineDone spline = (SplineDone)target;



 


        Vector3 transformPosition = spline.transform.position;

        List<SplineDone.Anchor> anchorList = spline.GetAnchorList();
        if (anchorList != null) {
            for (int i = 0; i < spline.GetAnchorList().Count - 1; i++)
            {
                SplineDone.Anchor anchor  = spline.GetAnchorList()[i];
                // Draw the anchor position with a wire cube
                Handles.color = Color.red;
                Handles.DrawWireCube(transformPosition + anchor.position, Vector3.one * .5f);
                Handles.color = Color.white;
                if (reorderableList.index != -1)
                {
                    if (reorderableList.index == 0)
                    {
                        if (spline.GetAnchorList()[reorderableList.index] == anchor || spline.GetAnchorList()[reorderableList.index + 1] == anchor)
                        {
                            // Position handle for the anchor position
                            EditorGUI.BeginChangeCheck();
                            Vector3 newPosition = Handles.PositionHandle(transformPosition + anchor.position, Quaternion.identity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(spline, "Change Anchor Position");
                                anchor.position = newPosition - transformPosition;
                                spline.SetDirty();
                                serializedObject.Update();
                            }

                            // Draw and handle the position for handleA
                            Handles.color = Color.green;
                            Handles.SphereHandleCap(0, transformPosition + anchor.handleAPosition, Quaternion.identity, .5f, EventType.Repaint);
                            Handles.color = Color.white;

                            EditorGUI.BeginChangeCheck();
                            newPosition = Handles.PositionHandle(transformPosition + anchor.handleAPosition, Quaternion.identity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(spline, "Change Anchor Handle A Position");
                                anchor.handleAPosition = newPosition - transformPosition;
                                spline.SetDirty();
                                serializedObject.Update();
                            }

                            // Draw and handle the position for handleB
                            Handles.color = Color.blue;
                            Handles.SphereHandleCap(0, transformPosition + anchor.handleBPosition, Quaternion.identity, .5f, EventType.Repaint);
                            Handles.color = Color.white;

                            EditorGUI.BeginChangeCheck();
                            newPosition = Handles.PositionHandle(transformPosition + anchor.handleBPosition, Quaternion.identity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(spline, "Change Anchor Handle B Position");
                                anchor.handleBPosition = newPosition - transformPosition;
                                spline.SetDirty();
                                serializedObject.Update();
                            }
                        }
                    }
                    else if (reorderableList.index == spline.GetAnchorList().Count-1)
                    {
                        if (spline.GetAnchorList()[reorderableList.index] == anchor || spline.GetAnchorList()[reorderableList.index - 1] == anchor)
                        {
                            // Position handle for the anchor position
                            EditorGUI.BeginChangeCheck();
                            Vector3 newPosition = Handles.PositionHandle(transformPosition + anchor.position, Quaternion.identity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(spline, "Change Anchor Position");
                                anchor.position = newPosition - transformPosition;
                                spline.SetDirty();
                                serializedObject.Update();
                            }

                            // Draw and handle the position for handleA
                            Handles.color = Color.green;
                            Handles.SphereHandleCap(0, transformPosition + anchor.handleAPosition, Quaternion.identity, .5f, EventType.Repaint);
                            Handles.color = Color.white;

                            EditorGUI.BeginChangeCheck();
                            newPosition = Handles.PositionHandle(transformPosition + anchor.handleAPosition, Quaternion.identity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(spline, "Change Anchor Handle A Position");
                                anchor.handleAPosition = newPosition - transformPosition;
                                spline.SetDirty();
                                serializedObject.Update();
                            }

                            // Draw and handle the position for handleB
                            Handles.color = Color.blue;
                            Handles.SphereHandleCap(0, transformPosition + anchor.handleBPosition, Quaternion.identity, .5f, EventType.Repaint);
                            Handles.color = Color.white;

                            EditorGUI.BeginChangeCheck();
                            newPosition = Handles.PositionHandle(transformPosition + anchor.handleBPosition, Quaternion.identity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(spline, "Change Anchor Handle B Position");
                                anchor.handleBPosition = newPosition - transformPosition;
                                spline.SetDirty();
                                serializedObject.Update();
                            }
                        }
                    }
                    else
                    {
                        if (spline.GetAnchorList()[reorderableList.index] == anchor || spline.GetAnchorList()[reorderableList.index - 1] == anchor || spline.GetAnchorList()[reorderableList.index + 1] == anchor)
                        {
                            // Position handle for the anchor position
                            EditorGUI.BeginChangeCheck();
                            Vector3 newPosition = Handles.PositionHandle(transformPosition + anchor.position, Quaternion.identity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(spline, "Change Anchor Position");
                                anchor.position = newPosition - transformPosition;
                                spline.SetDirty();
                                serializedObject.Update();
                            }

                            // Draw and handle the position for handleA
                            Handles.color = Color.green;
                            Handles.SphereHandleCap(0, transformPosition + anchor.handleAPosition, Quaternion.identity, .5f, EventType.Repaint);
                            Handles.color = Color.white;

                            EditorGUI.BeginChangeCheck();
                            newPosition = Handles.PositionHandle(transformPosition + anchor.handleAPosition, Quaternion.identity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(spline, "Change Anchor Handle A Position");
                                anchor.handleAPosition = newPosition - transformPosition;
                                spline.SetDirty();
                                serializedObject.Update();
                            }

                            // Draw and handle the position for handleB
                            Handles.color = Color.blue;
                            Handles.SphereHandleCap(0, transformPosition + anchor.handleBPosition, Quaternion.identity, .5f, EventType.Repaint);
                            
                            Handles.color = Color.white;

                            EditorGUI.BeginChangeCheck();
                            newPosition = Handles.PositionHandle(transformPosition + anchor.handleBPosition, Quaternion.identity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(spline, "Change Anchor Handle B Position");
                                anchor.handleBPosition = newPosition - transformPosition;
                                spline.SetDirty();
                                serializedObject.Update();
                            }
                        }
                    }
                }
                // Draw lines connecting the anchor position to handleA and handleB if the anchor is selected
                if(spline.GetAnchorList()[reorderableList.index]==anchor)
                Handles.color = Color.blue;
                else
                    Handles.color = Color.white;


                     Handles.DrawLine(transformPosition + anchor.position, transformPosition + anchor.handleAPosition,3);
                     Handles.DrawLine(transformPosition + anchor.position, transformPosition + anchor.handleBPosition,3);
               
            
        

        }

        // Draw Bezier
        for (int i = 0; i < spline.GetAnchorList().Count - 1; i++) {
                SplineDone.Anchor anchor = spline.GetAnchorList()[i];
                SplineDone.Anchor nextAnchor = spline.GetAnchorList()[i + 1];
                Handles.DrawBezier(transformPosition + anchor.position, transformPosition + nextAnchor.position, transformPosition + anchor.handleBPosition, transformPosition + nextAnchor.handleAPosition, Color.red, null, 5f);
            }

            if (spline.GetClosedLoop()) {
                // Spline is Closed Loop
                SplineDone.Anchor anchor = spline.GetAnchorList()[spline.GetAnchorList().Count - 1];
                SplineDone.Anchor nextAnchor = spline.GetAnchorList()[0];
                Handles.color = Color.red;
                Handles.DrawBezier(transformPosition + anchor.position, transformPosition + nextAnchor.position, transformPosition + anchor.handleBPosition, transformPosition + nextAnchor.handleAPosition, Color.red, null,5f);
            }


        }
    }

}
