using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LocalizationScriptAdder : EditorWindow
{
    
    [MenuItem("Utilities/Localization Script Adder")]
    static void Init()
    {
        LocalizationScriptAdder window = (LocalizationScriptAdder)EditorWindow.GetWindow(typeof(LocalizationScriptAdder));
        window.Show();
    }
    [SerializeField]
    Text[] texts;
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        GUIStyle style = GUI.skin.box;
        style.alignment = TextAnchor.MiddleCenter;
        EditorGUILayout.LabelField("This Editor Script is used to attach Text Localization Script Component" +
            " on all Text Fields having Text and TextMeshProUGUI Components", style);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10f);
        if (GUILayout.Button("Set Localization on All Texts - Scene"))
        {
            GetAllTexts();
        }
        EditorGUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Set Localization on All Texts - Prefab Mode\n This button will add the required component" +
            "to all text components only in the Prefab mode", style);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Set Localization on All Texts - Prefab Mode"))
        {
            SetComponentInPrefabMode();
        }
        EditorGUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Check Multiple TextLocalization in Scene\n This button will check and remove " +
            "duplicate TextLocalization Component attached to gameobjects in Scene\n This will only work in Scene Mode", style);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Check Multiple TextLocalization in Scene"))
        {
            ClearMultipleAttachedScripts();
        }
    }
    void GetAllTexts()
    {
        var tmp = new List<Text>();
        var tmppro = new List<TextMeshProUGUI>();
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            foreach (var root in SceneManager.GetSceneAt(i).GetRootGameObjects())
            {
                tmp.AddRange(root.GetComponentsInChildren<Text>(true));
            }
        }
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            foreach (var root in SceneManager.GetSceneAt(i).GetRootGameObjects())
            {
                tmppro.AddRange(root.GetComponentsInChildren<TextMeshProUGUI>(true));
            }
        }
        Text[] texts = tmp.ToArray();
        TextMeshProUGUI[] textspro = tmppro.ToArray();
        foreach (Text textl in texts)
        {
            if (textl.gameObject.TryGetComponent(out TextLocalization textlocal))
            {
                if (textlocal.LocalizeText == null)
                    textlocal.LocalizeText = textl.gameObject.GetComponent<Text>();
            }
            else
            {
                var textlocals = textl.gameObject.AddComponent<TextLocalization>();
                textlocals.LocalizeText = textl.gameObject.GetComponent<Text>();
            }
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(textl.gameObject);
            EditorUtility.SetDirty(this);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        foreach (TextMeshProUGUI textl in textspro)
        {
            if (textl.gameObject.TryGetComponent(out TextLocalization textlocal))
            {
                if (textlocal.LocalizeText == null)
                    textlocal.LocalizeTextTMP = textl.gameObject.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                var textlocals = textl.gameObject.AddComponent<TextLocalization>();
                textlocals.LocalizeTextTMP = textl.gameObject.GetComponent<TextMeshProUGUI>();
            }
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(textl.gameObject);
            EditorUtility.SetDirty(this);
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            //Debug.Log(prefabStage.prefabContentsRoot.transform.GetChild(0).gameObject.name);
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }
        
    }
    void SetComponentInPrefabMode()
    {
        var tmp = new List<Text>();
        var tmppro = new List<TextMeshProUGUI>();
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        tmp.AddRange(prefabStage.prefabContentsRoot.GetComponentsInChildren<Text>(true));
        tmppro.AddRange(prefabStage.prefabContentsRoot.GetComponentsInChildren<TextMeshProUGUI>(true));
        Text[] texts = tmp.ToArray();
        TextMeshProUGUI[] textspro = tmppro.ToArray();
        foreach (Text textl in texts)
        {
            if (textl.gameObject.TryGetComponent(out TextLocalization textlocal))
            {
                if (textlocal.LocalizeText == null)
                    textlocal.LocalizeText = textl.gameObject.GetComponent<Text>();
            }
            else
            {
                var textlocals = textl.gameObject.AddComponent<TextLocalization>();
                textlocals.LocalizeText = textl.gameObject.GetComponent<Text>();
            }
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(textl.gameObject);
        }
        foreach (TextMeshProUGUI textl in textspro)
        {
            if (textl.gameObject.TryGetComponent(out TextLocalization textlocal))
            {
                if (textlocal.LocalizeText == null)
                    textlocal.LocalizeTextTMP = textl.gameObject.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                var textlocals = textl.gameObject.AddComponent<TextLocalization>();
                textlocals.LocalizeTextTMP = textl.gameObject.GetComponent<TextMeshProUGUI>();
            }
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(textl.gameObject);
        }
        if (prefabStage != null)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }
    void ClearMultipleAttachedScripts()
    {
        var tmp = new List<Text>();
        var tmppro = new List<TextMeshProUGUI>();
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            foreach (var root in SceneManager.GetSceneAt(i).GetRootGameObjects())
            {
                tmp.AddRange(root.GetComponentsInChildren<Text>(true));
            }
        }
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            foreach (var root in SceneManager.GetSceneAt(i).GetRootGameObjects())
            {
                tmppro.AddRange(root.GetComponentsInChildren<TextMeshProUGUI>(true));
            }
        }
        Text[] texts = tmp.ToArray();
        TextMeshProUGUI[] textspro = tmppro.ToArray();
        foreach (Text textl in texts)
        {
            TextLocalization[] localized = textl.GetComponents<TextLocalization>();
            if (localized != null)
            { 
                if (localized.Length == 1)
                {
                    // Only One TextLocalization component is attached. Proceed
                }
                else if (localized.Length > 1)
                {
                    DestroyImmediate(localized[1]);
                }
            }
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(textl.gameObject);
        }
        foreach (TextMeshProUGUI textl in textspro)
        {
            TextLocalization[] localized = textl.GetComponents<TextLocalization>();
            if (localized != null)
            { 
                if (localized.Length == 1)
                {
                    // Only One TextLocalization component is attached. Proceed
                }
                else if (localized.Length > 1)
                {
                    DestroyImmediate(localized[1]);
                }
            }
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(textl.gameObject);  
        }
        EditorUtility.SetDirty(this);
    }
}