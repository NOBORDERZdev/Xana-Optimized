#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.BWF.EditorUtil;

namespace Crosstales.BWF.EditorExtension
{
   /// <summary>Custom editor for the 'BadWordManager'-class.</summary>
   [CustomEditor(typeof(Crosstales.BWF.Manager.BadWordManager))]
   public class BadWordManagerEditor : Editor
   {
      #region Variables

      private Crosstales.BWF.Manager.BadWordManager _script;

      private string _inputText = "Martians are assholes...";
      private string _outputText;

      private static bool _showStats;
      private static bool _showTD;

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         _script = (Crosstales.BWF.Manager.BadWordManager)target;

         if (_script.isActiveAndEnabled)
            _script.Load();
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (_script.BadWordProviderLTR == null || _script.BadWordProviderLTR.Count == 0)
            EditorGUILayout.HelpBox($"No 'BadWord Provider LTR' added!{System.Environment.NewLine}If you want to use this functionality, please add your desired 'BadWord Provider LTR'.", MessageType.Info);

         if (_script.BadWordProviderRTL == null || _script.BadWordProviderRTL.Count == 0)
            EditorGUILayout.HelpBox($"No 'Bad Word Provider RTL' added!{System.Environment.NewLine}If you want to use this functionality, please add your desired 'BadWord Provider RTL'.", MessageType.Info);

         EditorHelper.SeparatorUI();

         if (_script.isActiveAndEnabled)
         {
            EditorStyles.foldout.fontStyle = FontStyle.Bold;
            _showStats = EditorGUILayout.Foldout(_showStats, "Stats");
            EditorStyles.foldout.fontStyle = FontStyle.Normal;

            if (_showStats)
            {
               EditorGUI.indentLevel++;

               GUILayout.Label($"Ready:\t\t{(_script.isReady ? "Yes" : "No")}");

               if (_script.isReady)
               {
                  GUILayout.Label($"Sources:\t{_script.Sources.Count}");
                  GUILayout.Label($"Regex Count:\t{_script.TotalRegexCount}");
               }

               EditorGUI.indentLevel--;
            }

            EditorHelper.SeparatorUI();

            if (_script.BadWordProviderLTR?.Count > 0 || _script.BadWordProviderRTL?.Count > 0)
            {
               //EditorHelper.SeparatorUI();
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
                     EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
                  }

                  EditorGUI.indentLevel--;
               }
            }
            else
            {
               EditorGUILayout.HelpBox("Please add a 'Bad Word Provider'!", MessageType.Warning);
            }
         }
         else
         {
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      public override bool RequiresConstantRepaint()
      {
         return true;
      }

      #endregion
   }
}
#endif
// © 2016-2024 crosstales LLC (https://www.crosstales.com)