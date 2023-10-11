/*
 * ReferencesFinder: A Remote Desktop Protocol Implementation
 *
 * Copyright 2011-2023 Muneeb Ullah <https://muneebullah.com/>
 * https://github.com/Lucius47
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains a context menu function to find scene references of selected gameObject and its components.
/// </summary>
public class ReferencesFinder : MonoBehaviour
{
    /// <summary>
    /// Finds scene references of selected gameObject and its components.
    /// </summary>
    [MenuItem("GameObject/Check References", false, 1)]
    public static void Find()
    {
        // get root objects in scene
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects( rootObjects );


        // get all components of selected gameObject
        object[] componentsOnSelectedObj = Selection.activeGameObject.GetComponentsInChildren<MonoBehaviour>();

        // iterate through all root objects
        for (int i = 0; i < rootObjects.Count; ++i)
        {
            GameObject gameObject = rootObjects[i];

            var components = gameObject.GetComponentsInChildren<MonoBehaviour>();

            foreach (var component in components)
            {
                foreach ( FieldInfo FI in component.GetType().GetFields () )
                {
                    var field = FI.GetValue(component);

                    // Debug.LogError("RFM field: " + field, (GameObject)field);
                    // Debug.LogError("RFM activeGameObject: " + Selection.activeGameObject, (GameObject)Selection.activeGameObject);
                        
                    if (componentsOnSelectedObj.Contains(field) || field == Selection.activeGameObject)
                    {
                        Debug.LogError("Reference found on: " + field, component);
                    }
                    // else
                    // {
                    //     Debug.LogError("No references found");
                    // }
                }
            }
        }
    }
}
