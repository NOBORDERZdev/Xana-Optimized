using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
using UnityEditor;

public class AssosiatedDomainAdding
{
    static string[] associatedDomains;
   
    private static void OnProstProcessBuildIOS(string pathToBuiltProject)
    {
        //This is the default path to the default pbxproj file. Yours might be different
        string projectPath = "/Unity-iPhone.xcodeproj/project.pbxproj";
        //Default target name. Yours might be different
        string targetName = "Unity-iPhone";
        //Set the entitlements file name to what you want but make sure it has this extension
        string entitlementsFileName = "Unity-iPhone.entitlements";

        var entitlements = new ProjectCapabilityManager(pathToBuiltProject + projectPath, entitlementsFileName, targetName);
        entitlements.AddAssociatedDomains(new string[] { "applinks:unitytesting.page.link" });
        //Apply
        entitlements.WriteToFile();
    }
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.iOS)
            OnProstProcessBuildIOS(pathToBuiltProject);
    }

    
}
