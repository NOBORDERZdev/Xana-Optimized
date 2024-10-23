using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using System.Collections.Generic;

public class AddressableBuilder : MonoBehaviour
{
    [MenuItem("Tools/Build Addressables One by One")]
    public static void BuildAddressablesOneByOne()
    {
        // Get the Addressable settings
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings == null)
        {
            Debug.LogError("Addressable settings could not be found. Please ensure you have Addressables set up.");
            return;
        }

        // Get all groups
        List<AddressableAssetGroup> groups = settings.groups;

        // Iterate through each group and build it one by one
        foreach (AddressableAssetGroup group in groups)
        {
            // Skip groups that are read-only or don't have any schemas (like default groups)
            if (group == null || group.ReadOnly || group.Schemas.Count == 0)
            {
                Debug.LogWarning($"Skipping group: {group?.Name ?? "Unknown"}");
                continue;
            }

            // Store original state of 'IncludeInBuild' for all groups
            Dictionary<AddressableAssetGroup, bool> groupBuildState = new Dictionary<AddressableAssetGroup, bool>();
            foreach (var g in groups)
            {
                var schema = g.GetSchema<BundledAssetGroupSchema>();
                if (schema != null)
                {
                    groupBuildState[g] = schema.IncludeInBuild;
                    schema.IncludeInBuild = (g == group); // Enable only the current group
                }
            }

            // Log which group is being built
            Debug.Log($"Building group: {group.Name}");

            // Clean the player content before building
            AddressableAssetSettings.CleanPlayerContent();

            // Build the addressables (this will only include the enabled group)
            AddressableAssetSettings.BuildPlayerContent();

            Debug.Log($"Successfully built group: {group.Name}");

            // Restore all groups to their original 'IncludeInBuild' state
            foreach (var g in groups)
            {
                var schema = g.GetSchema<BundledAssetGroupSchema>();
                if (schema != null && groupBuildState.ContainsKey(g))
                {
                    schema.IncludeInBuild = groupBuildState[g];
                }
            }
        }

        Debug.Log("Finished building all addressable groups.");
    }
}
