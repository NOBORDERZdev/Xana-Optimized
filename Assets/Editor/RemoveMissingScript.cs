using UnityEditor;
using UnityEngine;

public class RemoveMissingScript : MonoBehaviour
{
    [MenuItem("Tools/EditorScipts-AP/Remove Missing Script")]
    static void RemoveMissingScripts()
    {
        GameObject[] g = Selection.gameObjects;
        for (int x=0;x<g.Length;x++)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(g[x]);
            FindChild(g[x].transform);
        }
    }


    static GameObject FindChild(Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(child.gameObject);
            if(child.childCount>0)
            {
                FindChild(child);
            }
        }
        return null;
    }
}
