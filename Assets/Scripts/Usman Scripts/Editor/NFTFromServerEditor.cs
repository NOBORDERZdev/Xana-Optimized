using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NFTFromServer))]
public class NFTFromServerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Reference to the target object
        NFTFromServer myScript = (NFTFromServer)target;

        // Start updating the serialized object
        serializedObject.Update();

        // Get each serialized property
        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true); // Move to the first visible property

        // Loop through all properties
        do
        {
            if (property.name == "_ExhibitComponentType")
            {
                // Always show the exhibitComponentType enum first
                EditorGUILayout.PropertyField(property, true);
            }
            else if (property.name == "_ExhibitSize")
            {
                // Conditionally show exhibitSize if exhibitComponentType is Dynamic
                if (myScript._ExhibitComponentType == NFTFromServer.ExhibitComponentType.Dynamic)
                {
                    EditorGUILayout.PropertyField(property, new GUIContent("Exhibit Size"), true);
                }
            }
            else
            {
                // Show all other properties
                EditorGUILayout.PropertyField(property, true);
            }

        } while (property.NextVisible(false)); // Loop through remaining properties

        // Apply modified properties to the serialized object
        serializedObject.ApplyModifiedProperties();
    }

}
