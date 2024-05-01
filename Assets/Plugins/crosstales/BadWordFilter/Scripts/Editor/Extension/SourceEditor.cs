#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.BWF.EditorUtil;

namespace Crosstales.BWF.EditorExtension
{
   /// <summary>Custom editor for the 'Source'-class.</summary>
   [CustomEditor(typeof(Crosstales.BWF.Data.Source))]
   public class SourceEditor : Editor
   {
      #region Variables

      private Crosstales.BWF.Data.Source _script;

      #endregion


      #region Editor methods

      private void OnEnable()
      {
         _script = (Crosstales.BWF.Data.Source)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (string.IsNullOrEmpty(_script.SourceName))
            UnityEditor.EditorGUILayout.HelpBox("The 'Source Name' is empty! Please add a name.", UnityEditor.MessageType.Error);

         if (!Crosstales.Common.Util.NetworkHelper.isURL(_script.URL) && _script.Resource == null)
         {
            UnityEditor.EditorGUILayout.HelpBox("The 'URL' or 'Resource' is empty or invalid! Please add at least one source.", UnityEditor.MessageType.Error);
         }
         else
         {
            if (_script.Resource == null && _script.IsResourceFallback)
               UnityEditor.EditorGUILayout.HelpBox("The 'Resource' is empty and fallback is enabled! Please disable it or add a source", UnityEditor.MessageType.Warning);
         }

         EditorHelper.SeparatorUI();

         GUILayout.Label("Stats", EditorStyles.boldLabel);
         GUILayout.Label($"Regex Count:\t{_script.RegexCount}");

         if (GUI.changed)
         {
            //Debug.Log("Changed");
            UnityEditor.EditorUtility.SetDirty(_script);
            UnityEditor.AssetDatabase.SaveAssets();
         }
      }

      #endregion
   }
}
#endif
// © 2020-2024 crosstales LLC (https://www.crosstales.com)