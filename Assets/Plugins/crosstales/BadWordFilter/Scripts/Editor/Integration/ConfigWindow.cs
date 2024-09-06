#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Crosstales.BWF.EditorUtil;
using Crosstales.BWF.Util;

namespace Crosstales.BWF.EditorIntegration
{
   /// <summary>Editor window extension.</summary>
   public class ConfigWindow : ConfigBase
   {
      #region Variables

      private int _tab;
      private int _lastTab;
      private string _inputText = "MARTIANS are asses.... => watch mypage.com";
      private string _outputText;

      private Vector2 _scrollPosPrefabs;
      private Vector2 _scrollPosTD;

      #endregion


      #region EditorWindow methods

      [MenuItem("Tools/" + Constants.ASSET_NAME + "/ Configuration...", false, EditorHelper.MENU_ID + 1)]
      public static void ShowWindow()
      {
         GetWindow(typeof(ConfigWindow));
      }

      public static void ShowWindow(int tab)
      {
         ConfigWindow window = GetWindow(typeof(ConfigWindow)) as ConfigWindow;
         if (window != null) window._tab = tab;
      }

      private void OnEnable()
      {
         titleContent = new GUIContent(Constants.ASSET_NAME_SHORT, EditorHelper.Logo_Asset_Small);
      }

      private void OnGUI()
      {
         _tab = GUILayout.Toolbar(_tab, new[] { "Config", "Prefabs", "TD", "Help", "About" });

         if (_tab != _lastTab)
         {
            _lastTab = _tab;
            GUI.FocusControl(null);
         }

         switch (_tab)
         {
            case 0:
            {
               showConfiguration();

               EditorHelper.SeparatorUI(6);

               GUILayout.BeginHorizontal();
               {
                  if (GUILayout.Button(new GUIContent(" Save", EditorHelper.Icon_Save, "Saves the configuration settings for this project.")))
                  {
                     save();
                  }

                  if (GUILayout.Button(new GUIContent(" Reset", EditorHelper.Icon_Reset, "Resets the configuration settings for this project.")))
                  {
                     if (EditorUtility.DisplayDialog("Reset configuration?", $"Reset the configuration of {Constants.ASSET_NAME}?", "Yes", "No"))
                     {
                        Config.Reset();
                        EditorConfig.Reset();
                        save();
                     }
                  }
               }
               GUILayout.EndHorizontal();

               GUILayout.Space(6);
               break;
            }
            case 1:
               showPrefabs();
               break;
            case 2:
               showTestDrive();
               break;
            case 3:
               showHelp();
               break;
            default:
               showAbout();
               break;
         }
      }

      private void OnInspectorUpdate()
      {
         Repaint();
      }

      #endregion


      #region Private methods

      private void showPrefabs()
      {
         _scrollPosPrefabs = EditorGUILayout.BeginScrollView(_scrollPosPrefabs, false, false);
         {
            GUILayout.Label("Available Prefabs", EditorStyles.boldLabel);

            GUILayout.Space(6);

            GUI.enabled = !EditorHelper.isBWFInScene;

            GUILayout.Label(Constants.MANAGER_SCENE_OBJECT_NAME);

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, $"Adds a '{Constants.MANAGER_SCENE_OBJECT_NAME}'-prefab to the scene.")))
            {
               EditorHelper.InstantiatePrefab(Constants.MANAGER_SCENE_OBJECT_NAME);
            }

            GUI.enabled = true;

            if (EditorHelper.isBWFInScene)
            {
               GUILayout.Space(6);
               EditorGUILayout.HelpBox("All available prefabs are already in the scene.", MessageType.Info);
            }

            GUILayout.Space(6);
         }
         EditorGUILayout.EndScrollView();
      }

      private void showTestDrive()
      {
         GUILayout.Space(3);
         GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

         if (Helper.isEditorMode)
         {
            //if (BWFManager.isReady && EditorHelper.isBWFInScene)
            if (EditorHelper.isBWFInScene)
            {
               _scrollPosTD = EditorGUILayout.BeginScrollView(_scrollPosTD, false, false);
               {
                  _inputText = EditorGUILayout.TextField(new GUIContent("Input Text", "Text to check."), _inputText);

                  EditorHelper.ReadOnlyTextField("Output Text", _outputText);
               }
               EditorGUILayout.EndScrollView();

               EditorHelper.SeparatorUI();

               GUILayout.BeginHorizontal();
               {
                  if (GUILayout.Button(new GUIContent(" Contains", EditorHelper.Icon_Contains, "Contains any bad words?")))
                  {
                     BWFManager.Instance.Load();
                     _outputText = BWFManager.Instance.Contains(_inputText).ToString();
                  }

                  if (GUILayout.Button(new GUIContent(" Get", EditorHelper.Icon_Get, "Get all bad words.")))
                  {
                     BWFManager.Instance.Load();
                     _outputText = string.Join(", ", BWFManager.Instance.GetAll(_inputText).ToArray());
                  }

                  if (GUILayout.Button(new GUIContent(" Replace", EditorHelper.Icon_Replace, "Check and replace all bad words.")))
                  {
                     BWFManager.Instance.Load();
                     _outputText = BWFManager.Instance.ReplaceAll(_inputText);
                  }

                  if (GUILayout.Button(new GUIContent(" Mark", EditorHelper.Icon_Mark, "Mark all bad words.")))
                  {
                     BWFManager.Instance.Load();
                     _outputText = BWFManager.Instance.Mark(_inputText);
                  }
               }
               GUILayout.EndHorizontal();

               GUILayout.Space(6);
            }
            else
            {
               EditorHelper.BWFUnavailable();
            }
         }
         else
         {
            EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2016-2024 crosstales LLC (https://www.crosstales.com)