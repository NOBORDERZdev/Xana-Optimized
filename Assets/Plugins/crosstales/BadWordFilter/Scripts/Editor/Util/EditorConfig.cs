#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Crosstales.BWF.EditorUtil
{
   /// <summary>Editor configuration for the asset.</summary>
   [InitializeOnLoad]
   public static class EditorConfig
   {
      #region Variables

      /// <summary>Enable or disable update-checks for the asset.</summary>
      public static bool UPDATE_CHECK = EditorConstants.DEFAULT_UPDATE_CHECK;

      /// <summary>Enable or disable adding compile define "CT_BWF" for the asset.</summary>
      public static bool COMPILE_DEFINES = EditorConstants.DEFAULT_COMPILE_DEFINES;

      /// <summary>Enable or disable the icon in the hierarchy.</summary>
      public static bool HIERARCHY_ICON = EditorConstants.DEFAULT_HIERARCHY_ICON;

      /// <summary>Is the configuration loaded?</summary>
      public static bool _isLoaded;

      private static string _assetPath;
      private const string _idPath = "Documentation/id/";
      private static readonly string _idName = $"{EditorConstants.ASSET_UID}.txt";

      #endregion


      #region Constructor

      static EditorConfig()
      {
         if (!_isLoaded)
         {
            Load();
         }
      }

      #endregion


      #region Properties

      /// <summary>Returns the path to the asset inside the Unity project.</summary>
      /// <returns>The path to the asset inside the Unity project.</returns>
      public static string ASSET_PATH
      {
         get
         {
            if (_assetPath == null)
            {
               try
               {
                  if (Crosstales.Common.Util.FileHelper.ExistsFile(Application.dataPath + EditorConstants.DEFAULT_ASSET_PATH + _idPath + _idName))
                  {
                     _assetPath = EditorConstants.DEFAULT_ASSET_PATH;
                  }
                  else
                  {
                     string[] files = System.IO.Directory.GetFiles(Application.dataPath, _idName, System.IO.SearchOption.AllDirectories);

                     if (files.Length > 0)
                     {
                        string name = files[0].Substring(Application.dataPath.Length);
                        _assetPath = name.Substring(0, name.Length - _idPath.Length - _idName.Length).Replace("\\", "/");
                     }
                     else
                     {
                        Debug.LogWarning($"Could not locate the asset! File not found: {_idName}");
                        _assetPath = EditorConstants.DEFAULT_ASSET_PATH;
                     }
                  }
               }
               catch (System.Exception ex)
               {
                  Debug.LogWarning($"Could not locate asset: {ex}");
               }
            }

            return _assetPath;
         }
      }

      /// <summary>Returns the path of the prefabs.</summary>
      /// <returns>The path of the prefabs.</returns>
      public static string PREFAB_PATH => ASSET_PATH + EditorConstants.PREFAB_SUBPATH;

      #endregion


      #region Public static methods

      /// <summary>Resets all changeable variables to their default value.</summary>
      public static void Reset()
      {
         _assetPath = null;

         UPDATE_CHECK = EditorConstants.DEFAULT_UPDATE_CHECK;
         COMPILE_DEFINES = EditorConstants.DEFAULT_COMPILE_DEFINES;
         HIERARCHY_ICON = EditorConstants.DEFAULT_HIERARCHY_ICON;
      }

      /// <summary>Loads all changeable variables.</summary>
      public static void Load()
      {
         _assetPath = null;

         if (Crosstales.Common.Util.CTPlayerPrefs.HasKey(EditorConstants.KEY_UPDATE_CHECK))
            UPDATE_CHECK = Crosstales.Common.Util.CTPlayerPrefs.GetBool(EditorConstants.KEY_UPDATE_CHECK);

         if (Crosstales.Common.Util.CTPlayerPrefs.HasKey(EditorConstants.KEY_COMPILE_DEFINES))
            COMPILE_DEFINES = Crosstales.Common.Util.CTPlayerPrefs.GetBool(EditorConstants.KEY_COMPILE_DEFINES);

         if (Crosstales.Common.Util.CTPlayerPrefs.HasKey(EditorConstants.KEY_HIERARCHY_ICON))
            HIERARCHY_ICON = Crosstales.Common.Util.CTPlayerPrefs.GetBool(EditorConstants.KEY_HIERARCHY_ICON);

         _isLoaded = true;
      }

      /// <summary>Saves all changeable variables.</summary>
      public static void Save()
      {
         Crosstales.Common.Util.CTPlayerPrefs.SetBool(EditorConstants.KEY_UPDATE_CHECK, UPDATE_CHECK);
         Crosstales.Common.Util.CTPlayerPrefs.SetBool(EditorConstants.KEY_COMPILE_DEFINES, COMPILE_DEFINES);
         Crosstales.Common.Util.CTPlayerPrefs.SetBool(EditorConstants.KEY_HIERARCHY_ICON, HIERARCHY_ICON);

         Crosstales.Common.Util.CTPlayerPrefs.Save();
      }

      #endregion
   }
}
#endif
// © 2015-2024 crosstales LLC (https://www.crosstales.com)