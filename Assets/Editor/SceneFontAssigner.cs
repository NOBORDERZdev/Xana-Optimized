using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SceneFontAssigner : EditorWindow
{
    private Font selectedFont;

    [MenuItem("Tools/EditorScipts-AP/Scene Font Assigner")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SceneFontAssigner));
    }

    private void OnGUI()
    {
        GUILayout.Label("Scene Font Assigner", EditorStyles.boldLabel);
        selectedFont = (Font)EditorGUILayout.ObjectField("Font", selectedFont, typeof(Font), false);

        if (GUILayout.Button("Assign Font"))
        {
            AssignFont();
        }
    }

    private void AssignFont()
    {
        Text[] textComponents = FindObjectsOfType<Text>(true);

        foreach (Text textComponent in textComponents)
        {
            Undo.RecordObject(textComponent, "Assign Font");
            textComponent.font = selectedFont;
            EditorUtility.SetDirty(textComponent);
        }

        Debug.Log("Font assigned to " + textComponents.Length + " Text components in the scene.");
    }
}