using UnityEngine;
using UnityEditor;
using TMPro;

public class TMPFontAssigner : EditorWindow
{
    private TMP_FontAsset selectedFont;

    [MenuItem("Tools/TMP Font Assigner")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<TMPFontAssigner>("TMP Font Assigner");
    }

    private void OnGUI()
    {
        GUILayout.Label("TMP Font Assigner", EditorStyles.boldLabel);
        selectedFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Font", selectedFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Assign Font"))
        {
            AssignFont();
        }
    }

    private void AssignFont()
    {
        TextMeshProUGUI[] textComponents = FindObjectsOfType<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI textComponent in textComponents)
        {
            Undo.RecordObject(textComponent, "Assign Font");
            textComponent.font = selectedFont;
            EditorUtility.SetDirty(textComponent);
        }

        Debug.Log("Font assigned to " + textComponents.Length + " TextMeshProUGUI components.");
    }
}