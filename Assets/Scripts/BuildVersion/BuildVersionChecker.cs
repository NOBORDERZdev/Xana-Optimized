#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildVersionChecker : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report)
    {
        BuildScriptableObject buildScriptableObject = ScriptableObject.CreateInstance<BuildScriptableObject>();

        //buildScriptableObject.BuildNumber = PlayerSettings.macOS.buildNumber;
#if UNITY_STANDALONE
        //buildScriptableObject.BuildNumber=PlayerSettings.
#endif

#if UNITY_IOS
        buildScriptableObject.BuildNumber = PlayerSettings.iOS.buildNumber;
#endif

#if UNITY_ANDROID
        buildScriptableObject.BuildNumber = PlayerSettings.Android.bundleVersionCode.ToString();
#endif
        if (File.Exists("Assets/Resources/BuildVersion/BuildVersion.asset"))
            AssetDatabase.DeleteAsset("Assets/Resources/BuildVersion/BuildVersion.asset"); // delete any old build.asset
        AssetDatabase.CreateAsset(buildScriptableObject, "Assets/Resources/BuildVersion/BuildVersion.asset"); // create the new one with correct build number before build starts
        AssetDatabase.SaveAssets();

    }
}
#endif