using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class GameAct : MonoBehaviour
{
    [System.Serializable]
    public class GA_Function
    {
        public UnityEvent function;
        public GameObject requiredActiveObject;
        public float delay;
    }
    [System.Serializable]
    public class GA_ObjectState
    {
        public GameObject gameObject;
        public bool state;
        public float delay;
    }
    [System.Serializable]
    public class GA_BehavioursState
    {
        public Behaviour behaviour;
        public bool state;
        public float delay;
    }
    [System.Serializable]
    public class GA_Sound {
        public AudioSource source = null; 
        public AudioClip audio = null;
        public float delay = 0;
        public float volume = 1;
    }
    [System.Serializable]
    public class GA_Animation
    {
        public enum Act { Start, Stop, Reset }
        public AnimationBase comp;
        public Act act;
        public bool waitForEnd;
        public float delay;
        //public float speedX=1;
    }
    [System.Serializable]
    public class GA_GameAct
    {
        public enum Act { Start, Stop }
        public GameAct comp;
        public Act act;
        public float delay;
        //public float speedX=1;
    }

    [HideInInspector] public bool loop;
    [HideInInspector] public bool startOnEnable;
    [HideInInspector] public int partsRunning; 
    [Space] public List<GA_Function> functions = new List<GA_Function>();
    [HideInInspector] public List<GA_ObjectState> gameObjects = new List<GA_ObjectState>(); 
    [HideInInspector] public List<GA_BehavioursState> behaviours = new List<GA_BehavioursState>();
    [HideInInspector] public List<GA_Sound> sounds = new List<GA_Sound>(); 
    [HideInInspector] public List<GA_Animation> animations = new List<GA_Animation>();  
    [HideInInspector] public List<GA_GameAct> gameActs = new List<GA_GameAct>();

    [HideInInspector] public bool objectsEx, animsEx, soundsEx;

    void OnEnable() {
        if (startOnEnable) StartAct();
    }
    void OnDisable()
    {
        StopAct();
    }

    public void StartAct() {
        if (partsRunning != 0) StopAct();
        if (gameObject.activeSelf)
        {
            if (functions.Count > 0) StartCoroutine("InvokeFunctions");
            if (gameObjects.Count > 0) StartCoroutine("InvokeObjectsState");
            if (behaviours.Count > 0) StartCoroutine("InvokeComponentsState");
            if (sounds.Count > 0) StartCoroutine("InvokeSounds");
            if (animations.Count > 0) StartCoroutine("InvokeAnimations");
            if (gameActs.Count > 0) StartCoroutine("InvokeGameActs");
        }
    }

    public void StopAct()
    {
        if (partsRunning == 0) return;

        StopCoroutine("InvokeFunctions");
        StopCoroutine("InvokeObjectsState");
        StopCoroutine("InvokeComponentsState");
        StopCoroutine("InvokeSounds");
        StopCoroutine("InvokeAnimations");
        StopCoroutine("InvokeGameActs");
        //if (audioSource != null) audioSource.Stop();

        partsRunning = 0;
    }

    IEnumerator InvokeFunctions()
    {
        partRunning(true);
        for (int i = 0; i < functions.Count; i++)
        {
            if(functions[i].delay > 0) yield return new WaitForSeconds(functions[i].delay);
            if (functions[i].requiredActiveObject!=null)
            {
                if (functions[i].requiredActiveObject.activeSelf) functions[i].function.Invoke();
            }
            else functions[i].function.Invoke();
        }
        partRunning(false);
    }
    IEnumerator InvokeObjectsState()
    {
        partRunning(true);
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (gameObjects[i].delay > 0) yield return new WaitForSeconds(gameObjects[i].delay);

            if (gameObjects[i].gameObject != null) gameObjects[i].gameObject.SetActive(gameObjects[i].state);
            else Debug.LogError("Unassigned reference: GameObject of " + i.ToString(), this);
        }
        partRunning(false);
    }
    IEnumerator InvokeComponentsState()
    {
        partRunning(true);
        for (int i = 0; i < behaviours.Count; i++)
        {
            if (behaviours[i].delay > 0) yield return new WaitForSeconds(behaviours[i].delay);

            if (behaviours[i].behaviour != null) behaviours[i].behaviour.enabled = behaviours[i].state;
            else Debug.LogError("Unassigned reference: component of " + i.ToString(), this);
        }
        partRunning(false);
    }
    IEnumerator InvokeSounds()
    {
        partRunning(true);
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].delay > 0) yield return new WaitForSeconds(sounds[i].delay);
             
            if (sounds[i].source != null)
            {
                if (sounds[i].audio != null)
                {
                    sounds[i].source.PlayOneShot(sounds[i].audio, sounds[i].volume);
                }
                else Debug.LogError("Unassigned reference: Audio Clip at " + i.ToString(), this);
            }
            else Debug.LogError("Unassigned reference: Audio Source", this); 
        }
        partRunning(false);
    }
    IEnumerator InvokeAnimations()
    {
        partRunning(true);
        for (int i = 0; i < animations.Count; i++)
        {
            if (animations[i].delay > 0) yield return new WaitForSeconds(animations[i].delay);
            //animationComp[animations[i].clip.name].speed = animations[i].speedX;
            //animationComp[animations[i].clip.name].time = animationComp[animations[i].clip.name].length;
            if (animations[i].comp != null)
            {
                switch (animations[i].act)
                {
                    case GA_Animation.Act.Start:
                        animations[i].comp.StartAnim();
                        break;
                    case GA_Animation.Act.Stop:
                        animations[i].comp.StopAnim();
                        break;
                    case GA_Animation.Act.Reset:
                        animations[i].comp.ResetAnim();
                        break;
                    default:
                        break;
                }
                if(animations[i].waitForEnd) yield return new WaitForSeconds(animations[i].comp.Duration);
            }
            else Debug.LogError("Unassigned reference: Animation Component", this);
        }
        partRunning(false);
    }
    IEnumerator InvokeGameActs()
    {
        partRunning(true);
        for (int i = 0; i < gameActs.Count; i++)
        {
            if (gameActs[i].delay > 0) yield return new WaitForSeconds(gameActs[i].delay);
            //animationComp[animations[i].clip.name].speed = animations[i].speedX;
            //animationComp[animations[i].clip.name].time = animationComp[animations[i].clip.name].length;
            if (gameActs[i].comp != null)
            {
                switch (gameActs[i].act)
                {
                    case GA_GameAct.Act.Start:
                        gameActs[i].comp.StartAct();
                        break;
                    case GA_GameAct.Act.Stop:
                        gameActs[i].comp.StopAct();
                        break; 
                    default:
                        break;
                }
                yield return new WaitForSeconds(gameActs[i].comp.getRunningTime());
            }
            else Debug.LogError("Unassigned reference: Game Act", this);
        }
        partRunning(false);
    }

    void partRunning(bool add) {
        if (add) partsRunning++;
        else partsRunning--;
        //print("partRunning " + add + " " + partsRunning);
        if (partsRunning == 0 && loop) StartAct();
    }

    public float getRunningTime() {
        float t = 0,tt=0;

        for (int i = 0; i < functions.Count; i++)
        {
            tt += functions[i].delay;
        }
        if (tt > t) t = tt;
        tt = 0;

        for (int i = 0; i < gameObjects.Count; i++)
        {
            tt += gameObjects[i].delay;
        }
        if (tt > t) t = tt;
        tt = 0;

        for (int i = 0; i < behaviours.Count; i++)
        {
            tt += behaviours[i].delay;
        }
        if (tt > t) t = tt;
        tt = 0;

        for (int i = 0; i < sounds.Count; i++)
        {
            tt += sounds[i].delay;
        }
        if (tt > t) t = tt;
        tt = 0;

        for (int i = 0; i < animations.Count; i++)
        {
            tt += animations[i].delay;
            if (animations[i].waitForEnd) tt += Mathf.Abs(animations[i].comp.Duration);
        }
        if (tt > t) t = tt;
        tt = 0;

        for (int i = 0; i < gameActs.Count; i++)
        {
            tt += gameActs[i].delay;
            tt += Mathf.Abs(gameActs[i].comp.getRunningTime());
        }
        if (tt > t) t = tt;
        tt = 0;

        //print(t);
        return t;
    }
}

#region editor
#if UNITY_EDITOR  
[CustomEditor(typeof(GameAct))]
public class GameActEditor : Editor
{
    public GameAct script;

    ReorderableList gameObjectsList;
    ReorderableList behavioursList;
    ReorderableList audiosList;
    ReorderableList animationsList;
    ReorderableList gameActsList;

    void OnEnable()
    {
        script = target as GameAct;

        gameObjectsList = new ReorderableList(
            serializedObject, 
            serializedObject.FindProperty("gameObjects"), 
            true, true, true, true); 

        gameObjectsList.drawHeaderCallback = (Rect rect) => 
            { 
                EditorGUI.LabelField(rect, "Game Object | State | Delay"); 
            };

        gameObjectsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
            {
                var element = gameObjectsList.serializedProperty.GetArrayElementAtIndex(index);  
                rect.y += 2;
                EditorGUI.PropertyField(
                    cellRect(rect, 0, 7.5f),
                    element.FindPropertyRelative("gameObject"), GUIContent.none);
                EditorGUI.PropertyField(
                    cellRect(rect, 7.5f, 1),
                    element.FindPropertyRelative("state"), GUIContent.none); 
                EditorGUI.PropertyField(
                    cellRect(rect, 8.5f, 1.5f),
                    element.FindPropertyRelative("delay"), GUIContent.none); 
            };


        behavioursList = new ReorderableList(
            serializedObject,
            serializedObject.FindProperty("behaviours"),
            true, true, true, true);

        behavioursList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Behaviour | State | Delay");
        };

        behavioursList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = behavioursList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                cellRect(rect, 0, 7.5f),
                element.FindPropertyRelative("behaviour"), GUIContent.none);
            EditorGUI.PropertyField(
                cellRect(rect, 7.5f, 1),
                element.FindPropertyRelative("state"), GUIContent.none);
            EditorGUI.PropertyField(
                cellRect(rect, 8.5f, 1.5f),
                element.FindPropertyRelative("delay"), GUIContent.none);
        };



        audiosList = new ReorderableList(
            serializedObject, 
            serializedObject.FindProperty("sounds"), 
            true, true, true, true); 

        audiosList.drawHeaderCallback = (Rect rect) => 
            { 
                EditorGUI.LabelField(rect, "Audio Source | clip | volume | Delay"); 
            };

        audiosList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
            {
                var element = audiosList.serializedProperty.GetArrayElementAtIndex(index);  
                rect.y += 2;
                EditorGUI.PropertyField(
                    cellRect(rect, 0, 4),
                    element.FindPropertyRelative("source"), GUIContent.none);
                EditorGUI.PropertyField(
                    cellRect(rect, 4, 3),
                    element.FindPropertyRelative("clip"), GUIContent.none);
                EditorGUI.PropertyField(
                    cellRect(rect, 7, 1.5f),
                    element.FindPropertyRelative("volume"), GUIContent.none); 
                EditorGUI.PropertyField(
                    cellRect(rect, 8.5f, 1.5f),
                    element.FindPropertyRelative("delay"), GUIContent.none); 
            }; 



        animationsList = new ReorderableList(
            serializedObject, 
            serializedObject.FindProperty("animations"), 
            true, true, true, true); 

        animationsList.drawHeaderCallback = (Rect rect) => 
            { 
                EditorGUI.LabelField(rect, "Animation | act | Wait for End | Delay"); 
            };

        animationsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
            {
                var element = animationsList.serializedProperty.GetArrayElementAtIndex(index);  
                rect.y += 2;
                EditorGUI.PropertyField(
                    cellRect(rect, 0, 4.5f),
                    element.FindPropertyRelative("comp"), GUIContent.none);
                EditorGUI.PropertyField(
                    cellRect(rect, 4.5f, 3.5f),
                    element.FindPropertyRelative("act"), GUIContent.none);
                EditorGUI.PropertyField(
                    cellRect(rect, 8f, .5f),
                    element.FindPropertyRelative("waitForEnd"), GUIContent.none);
                EditorGUI.PropertyField(
                    cellRect(rect, 8.5f, 1.5f),
                    element.FindPropertyRelative("delay"), GUIContent.none); 
            };



        gameActsList = new ReorderableList(
            serializedObject,
            serializedObject.FindProperty("gameActs"),
            true, true, true, true);

        gameActsList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Game Act | act | Delay");
        };

        gameActsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = gameActsList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                cellRect(rect, 0, 4.5f),
                element.FindPropertyRelative("comp"), GUIContent.none);
            EditorGUI.PropertyField(
                cellRect(rect, 4.5f, 4f),
                element.FindPropertyRelative("act"), GUIContent.none);
            EditorGUI.PropertyField(
                cellRect(rect, 8.5f, 1.5f),
                element.FindPropertyRelative("delay"), GUIContent.none);
        };
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        serializedObject.Update();

        EditorGUILayout.Space();
        gameObjectsList.DoLayoutList();

        EditorGUILayout.Space();
        behavioursList.DoLayoutList();

        EditorGUILayout.Space(); 
        audiosList.DoLayoutList();

        EditorGUILayout.Space(); 
        animationsList.DoLayoutList();

        EditorGUILayout.Space();
        gameActsList.DoLayoutList();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        script.loop = EditorGUILayout.Toggle("Loop", script.loop);
        script.startOnEnable = EditorGUILayout.Toggle("Start On Enable", script.startOnEnable);
        EditorGUILayout.EndVertical();
        if (GUILayout.Button("Start", GUILayout.Width(50), GUILayout.Height(30))) if(Application.isPlaying) script.StartAct();
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();  
    }
 
    Rect cellRect(Rect fullRect, float x, float w)
    {
        float cellWidth = fullRect.width / 10;
        return new Rect(fullRect.x + cellWidth*x + 2*x, fullRect.y, w * cellWidth - 2*x, EditorGUIUtility.singleLineHeight);
    }
}
#endif
#endregion