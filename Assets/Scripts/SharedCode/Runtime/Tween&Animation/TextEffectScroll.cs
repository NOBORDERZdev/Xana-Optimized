using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextEffectScroll : MonoBehaviour 
{
    public Text refText;
    public float scrollTime = 1;
    public string prefix;
    public bool useCommas = true;
    public event System.Action SrollEnded;
    RectTransform[] textsRect;
    Text[] texts;
    string[] values;
    float time = 1;
    int dir = 1;
    bool ready = false; 

    #if UNITY_EDITOR
    [HideInInspector]public bool editorSetup = false; 
    [HideInInspector]public double testFrom = 0;
    [HideInInspector]public double testTo = 100;
    [HideInInspector]public int testSegments = 10;
    [HideInInspector]public float testTime = 5;
    [HideInInspector]public bool testScrollDown = false;
    #endif

    void Awake()
    {
        try
        {
            texts = new Text[2];
            textsRect = new RectTransform[2];
            for (int i = 0; i < 2; i++)
            {
                if(i==0) texts[i] = refText;
                else texts[i] = Instantiate(refText, refText.transform.parent) as Text;
                textsRect[i] = texts[i].GetComponent<RectTransform>();
            }  
            ready = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
            ready = false;
        }
    } 

    public void Begin(double _from, double _to, int _segments, float _time, bool _scrollDown)
    { 
        if (_from == _to)
        {
            texts[0].text = _to.ToString();
            textsRect[0].localPosition = Vector3.zero; 
            texts[1].text = _to.ToString();
            textsRect[1].localPosition = new Vector3(0, textsRect[1].rect.height, 0);
            return;
        }

        if (_segments > (int)System.Math.Abs(_to - _from)) _segments = (int)System.Math.Abs(_to - _from) + 1;
        if(_segments < 2)_segments = 2;
        _from = System.Math.Floor(_from);
        _to = System.Math.Floor(_to);

        string[] ss = new string[_segments];
        ss[0] = _from.ToFormattedString(useCommas, false, prefix);
        ss[ss.Length - 1] = _to.ToFormattedString(useCommas, false, prefix);
        for (int i = 1; i < ss.Length-1; i++) 
        {
            ss[i] = System.Math.Round(_from + (i * ((_to - _from) / _segments))).ToFormattedString(useCommas, false, prefix);
        } 

//        for (int i = 0; i < ss.Length; i++)
//        { 
//            ss[i] =  (_from + (_to - _from)*((float)i/ss.Length)).ToString();
//        } 

        Begin(ss, _time, _scrollDown);
    }

    public void Begin(string[] _values, bool _scrollDown)
    {
        Begin(_values, scrollTime, _scrollDown);
    }

    public void Begin(string[] _values, float _time, bool _scrollDown)
    {
        if (!ready) return;

        values = _values;
        time = _time;
        dir = _scrollDown ? -1 : 1;
        StopCoroutine("Effect_c");
        StartCoroutine("Effect_c");
    }

    IEnumerator Effect_c()
    {
        float t = 0;
        float tt = time / (float)(values.Length-1);
//        print(tt);
        float del = 0;
        int activeTextIndex = 0;
        int scrolledTo = 0;
        while(scrolledTo++ < values.Length-1)
        { 
//            print("a " + activeTextIndex);
//            print(scrolledTo-1);
//            print(scrolledTo);
            texts[activeTextIndex].text = values[scrolledTo-1];
            textsRect[activeTextIndex].localPosition = Vector3.zero;

            activeTextIndex = 1 - activeTextIndex;
            texts[activeTextIndex].text = values[scrolledTo];
            textsRect[activeTextIndex].localPosition = new Vector3(0,-dir * textsRect[activeTextIndex].rect.height,0);

            t = 0;
            while (t < tt)
            { 
                for (int i = 0; i < textsRect.Length; i++)
                {
                    textsRect[i].position += new Vector3(0,dir * del,0);
                }  
                t += Time.unscaledDeltaTime;
                del = textsRect[0].rect.height / tt * Time.unscaledDeltaTime * textsRect[0].lossyScale.x;
                yield return null;
            } 
        } 
        textsRect[activeTextIndex].localPosition = Vector3.zero;
        if (SrollEnded != null) SrollEnded();
    }  
}

#if UNITY_EDITOR
[CustomEditor(typeof(TextEffectScroll))]
public class TextEffectScrollEditor : Editor
{
    TextEffectScroll script;
    void OnEnable()
    {
        script = target as TextEffectScroll;
        if (!script.editorSetup)
        {
            if (script.gameObject.GetComponent<Image>() == null)
            {
                script.gameObject.AddComponent<Image>();
                if (script.gameObject.GetComponent<Mask>() == null) 
                    script.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            }

            if (script.transform.childCount==0)
            {
                script.refText = new GameObject("RefText").AddComponent<Text>();
                script.refText.text = "Text"; 
                script.refText.transform.SetParent(script.transform);
            }
            else 
            { 
                if(script.transform.GetChild(0).GetComponent<Text>() != null) 
                    script.refText = script.transform.GetChild(0).GetComponent<Text>();
            }

            script.editorSetup = true;
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        script.testFrom = EditorGUILayout.DoubleField(script.testFrom);
        script.testTo = EditorGUILayout.DoubleField(script.testTo);
        script.testSegments = EditorGUILayout.IntField(script.testSegments);
        script.testTime = EditorGUILayout.FloatField(script.testTime); 
        script.testScrollDown = EditorGUILayout.Toggle(script.testScrollDown); 
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Test"))
        {
            if (Application.isPlaying)
            {
                script.Begin(script.testFrom, script.testTo, script.testSegments, script.testTime, script.testScrollDown);
            }
        }
    }
}

#endif