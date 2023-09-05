#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RFM
{
    /// <summary>
    /// Displays fields of a static class in an editor window
    /// </summary>
    /// <remarks>muneeb.nbzi@gmail.com</remarks>
    public class StaticClassDisplay : EditorWindow
    {
        string myString = "Nuh Uh";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;
    
        // Add menu item named "My Window" to the Window menu
        [MenuItem("Window/Static Class Display")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(StaticClassDisplay));
        }
    
        void OnGUI()
        {
            
            GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
            
            myString = Globals.player?.name;
            myString = EditorGUILayout.TextField ("Text Field", myString);
        
            groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle ("Toggle", myBool);
            myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup ();
        }
    }
}

#endif