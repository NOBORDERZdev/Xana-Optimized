#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.BWF.EditorUtil;

namespace Crosstales.BWF.EditorExtension
{
   /// <summary>Custom editor for the 'BWFManager'-class.</summary>
   [InitializeOnLoad]
   [CustomEditor(typeof(BWFManager))]
   public class BWFManagerEditor : Editor
   {
      #region Variables

      private BWFManager _script;

      private string _inputText = "MARTIANS are asses.... => watch mypage.com";
      private string _outputText;

      private static bool _showStats;
      private static bool _showTD;

      #endregion


      #region Static constructor

      static BWFManagerEditor()
      {
         EditorApplication.hierarchyWindowItemOnGUI += hierarchyItemCB;
      }

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         _script = (BWFManager)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (_script.isActiveAndEnabled)
         {
            EditorHelper.SeparatorUI();

            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            _showStats = EditorGUILayout.Foldout(_showStats, "Stats");
            EditorStyles.foldout.fontStyle = FontStyle.Normal;

            if (_showStats)
            {
               EditorGUI.indentLevel++;

               GUILayout.Label($"Ready:\t\t{(_script.isReady ? "Yes" : "No")}");

               if (_script.isReady)
               {
                  GUILayout.Label($"Sources:\t{_script.Sources().Count}");
                  GUILayout.Label($"Regex Count:\t{_script.TotalRegexCount}");
               }

               EditorGUI.indentLevel--;
            }

            EditorHelper.SeparatorUI();

            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            _showTD = EditorGUILayout.Foldout(_showTD, "Test-Drive");
            EditorStyles.foldout.fontStyle = FontStyle.Normal;

            if (_showTD)
            {
               EditorGUI.indentLevel++;

               if (Crosstales.BWF.Util.Helper.isEditorMode)
               {
                  _inputText = EditorGUILayout.TextField(new GUIContent("Input Text", "Text to check."), _inputText);

                  EditorHelper.ReadOnlyTextField("Output Text", _outputText);

                  GUILayout.Space(8);

                  GUILayout.BeginHorizontal();
                  if (GUILayout.Button(new GUIContent(" Contains", EditorHelper.Icon_Contains, "Contains any bad words?")))
                     _outputText = _script.Contains(_inputText).ToString();

                  if (GUILayout.Button(new GUIContent(" Get", EditorHelper.Icon_Get, "Get all bad words.")))
                     _outputText = string.Join(", ", _script.GetAll(_inputText).ToArray());

                  if (GUILayout.Button(new GUIContent(" Replace", EditorHelper.Icon_Replace, "Check and replace all bad words.")))
                     _outputText = _script.ReplaceAll(_inputText);

                  if (GUILayout.Button(new GUIContent(" Mark", EditorHelper.Icon_Mark, "Mark all bad words.")))
                     _outputText = _script.Mark(_inputText);

                  GUILayout.EndHorizontal();
               }
               else
               {
                  EditorHelper.SeparatorUI();
                  EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
               }

               EditorGUI.indentLevel--;
            }
         }
         else
         {
            EditorHelper.SeparatorUI();
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion


      #region Private methods

      private static void hierarchyItemCB(int instanceID, Rect selectionRect)
      {
         if (EditorConfig.HIERARCHY_ICON)
         {
            //Color cc = GUI.contentColor;
            //Color bc = GUI.backgroundColor;

            //GUI.backgroundColor = Color.green;
            //GUI.contentColor = Color.yellow;

            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (go != null && go.GetComponent<BWFManager>())
            {
               Rect r = new Rect(selectionRect);
               r.x = r.width - 4;

               GUI.Label(r, EditorHelper.Logo_Asset_Small);
            }

            //GUI.contentColor = cc;
            //GUI.backgroundColor = bc;
         }
      }

      #endregion
   }
}
#endif
// © 2016-2024 crosstales LLC (https://www.crosstales.com)