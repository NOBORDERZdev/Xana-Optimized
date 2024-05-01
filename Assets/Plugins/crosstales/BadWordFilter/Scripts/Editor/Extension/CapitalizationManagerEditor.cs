#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.BWF.EditorUtil;

namespace Crosstales.BWF.EditorExtension
{
   /// <summary>Custom editor for the 'CapitalizationManager'-class.</summary>
   [CustomEditor(typeof(Crosstales.BWF.Manager.CapitalizationManager))]
   public class CapitalizationManagerEditor : Editor
   {
      #region Variables

      private Crosstales.BWF.Manager.CapitalizationManager _script;

      private string _inputText = "COME ON, TEST ME User!";
      private string _outputText;

      private static bool _showTD;

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         _script = (Crosstales.BWF.Manager.CapitalizationManager)target;

         if (_script.isActiveAndEnabled)
            _script.Load();
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         EditorHelper.SeparatorUI();

         if (_script.isActiveAndEnabled)
         {
            if (_script.isReady)
            {
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
                     {
                        if (GUILayout.Button(new GUIContent(" Contains", EditorHelper.Icon_Contains, "Contains any extensive capitalizations?")))
                           _outputText = _script.Contains(_inputText).ToString();

                        if (GUILayout.Button(new GUIContent(" Get", EditorHelper.Icon_Get, "Get all extensive capitalizations.")))
                           _outputText = string.Join(", ", _script.GetAll(_inputText).ToArray());

                        if (GUILayout.Button(new GUIContent(" Replace", EditorHelper.Icon_Replace, "Check and replace all extensive capitalizations.")))
                           _outputText = _script.ReplaceAll(_inputText);

                        if (GUILayout.Button(new GUIContent(" Mark", EditorHelper.Icon_Mark, "Mark all extensive capitalizations.")))
                           _outputText = _script.Mark(_inputText);
                     }
                     GUILayout.EndHorizontal();
                  }
                  else
                  {
                     EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
                  }

                  EditorGUI.indentLevel--;
               }
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