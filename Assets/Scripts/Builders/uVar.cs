using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class uVar<DataType, ComponentType> where ComponentType : IuVarHandler<DataType>
{  
    bool init = false;
    [SerializeField]
    protected DataType m_value = default(DataType);
    public DataType Value
    {
        get
        {
            return m_value;
        }
        set
        {
            if (!init || m_value == null || !m_value.Equals(value))
            {
                init = true;

                m_value = value;

                if (components!=null)
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] != null) components[i].Handle(ref m_value);
                    }
                } 

                OnValueChanged();
                if (ValueChanged != null) ValueChanged();
                if (ValueChangedTo != null) ValueChangedTo(m_value);
            } 
        }
    }
    public void SetValue(DataType v)
    {
        Value = v;
    }

    public event Action ValueChanged;
    public event Action<DataType> ValueChangedTo;
    public ComponentType[] components;
    public List<IuVarHandler<DataType>> runtimeComponents;

    public void AddComponent(IuVarHandler<DataType> comp)
    {
        if (!runtimeComponents.Contains(comp))
        {
            runtimeComponents.Add(comp);
            comp.Handle(ref m_value);
        }
    }

    public void RemoveComponent(IuVarHandler<DataType> comp)
    {
        if (runtimeComponents.Contains(comp))
        {
            runtimeComponents.Remove(comp);
        }
    }

    public virtual void OnValueChanged()
    {

    }
     
    [HideInInspector]
    public bool foldOutUI = false;
}
 
public interface IuVarHandler<DataType>
{
    void Handle(ref DataType s);
} 


#if UNITY_EDITOR
public class uVarDrawer : PropertyDrawer
{ 
    float verticalSpacing = 5;

    SerializedProperty prop; 
    string[] defArrays = new string[] { "components" };
    public virtual string[] arrays { get { return defArrays; } }
    SerializedProperty foldOutUIProp;
    bool foldOutUI
    {
        get
        {
            if (prop == null) return false;
            if (foldOutUIProp == null) foldOutUIProp = prop.FindPropertyRelative("foldOutUI");
            if (foldOutUIProp == null) return false;
            return foldOutUIProp.boolValue;
        }
        set
        {
            if (foldOutUIProp == null) foldOutUIProp = prop.FindPropertyRelative("foldOutUI");
            if (foldOutUIProp!=null)
            {
                prop.FindPropertyRelative("foldOutUI").boolValue = value;
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        prop = property;

        int totalLines = GetValueLines();
        int totalSpacings = GetValueSpacings();

        if (foldOutUI)
        {
            totalLines += GetUnFoldedContentLines();
            totalSpacings += GetUnFoldedContentSpacings();

            totalSpacings+=3;
            for (int i = 0; i < arrays.Length; i++)
            {
                totalLines += (1 + property.FindPropertyRelative(arrays[i]).arraySize);
                totalSpacings++;
                continue;
            } 
        }

        return (totalLines * EditorGUIUtility.singleLineHeight) + (totalSpacings * verticalSpacing);
    }
    protected virtual int GetValueLines() { return 1; }
    protected virtual int GetValueSpacings() { return 0; }
    protected virtual int GetUnFoldedContentLines() { return 0; }
    protected virtual int GetUnFoldedContentSpacings() { return 0; }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    { 
        prop = property;
         
        pos = EditorGUI.IndentedRect(position);
        //pos = position;
        x = pos.x;
        w = pos.width;
        y = pos.y;
        spacing = 0;

        EditorGUI.BeginProperty(position, label, property);

        //if (GUI.Button(new Rect(pos.x - 100, pos.y - 1, 103, EditorGUIUtility.singleLineHeight), string.Empty))
        //{
        //    foldOutUI = !foldOutUI;
        //}

        Rect r = GetNextRect();
        Rect r0 = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, r.height);
        foldOutUI = EditorGUI.Foldout(r0, foldOutUI, new GUIContent(label.text), true);
        //GUI.Label(r, property.displayName); 

        DrawValueContent(position, property, label);

        if (foldOutUI)
        { 
            Color prevColor = GUI.contentColor;
            GUI.contentColor = Color.Lerp(prevColor, Color.gray, .3f);

            DrawFoldoutContent(position, property, label);

            spacing++;
            for (int i = 0; i < arrays.Length; i++)
            {
                DrawArrayProperty(property.FindPropertyRelative(arrays[i]));
                spacing++;
            }
            spacing++;
            GUI.contentColor = prevColor;
            //il--;
        }
        //if (GUI.Button(position, target.fName))
        //{
        //    targetObject.SendMessage(target.fName);
        //}
        EditorGUI.EndProperty();
    }

    public virtual void DrawValueContent(Rect position, SerializedProperty property, GUIContent label)
    { 
        EditorGUI.PropertyField(GetValueArea(GetCurrentRect()), property.FindPropertyRelative("m_value"), GUIContent.none);
    }
    public virtual void DrawFoldoutContent(Rect position, SerializedProperty property, GUIContent label) {  }

    void DrawArrayProperty(SerializedProperty property)
    {
        float lh = 18;
        Rect r = GetNextRect();
        Rect r0 = new Rect();
        Rect r1 = new Rect(r.x, r.y, r.width - (lh * 2), r.height);
        Rect r2 = new Rect((r.x + (r.width - lh)), r.y, (lh), r.height);

        Rect rx = r;
        rx.height = rx.height * (property.arraySize + 1);
        rx.x -= 1;
        rx.y -= 2;
        rx.width += 2;
        rx.height += 4;

        EditorGUI.HelpBox(rx, string.Empty, MessageType.None);
        GUI.Label(r1, property.displayName);
        if (GUI.Button(r2, "+"))
        {
            property.InsertArrayElementAtIndex(property.arraySize);
        }

        for (int i = 0; i < property.arraySize; i++)
        {
            r = GetNextRect();
            r0 = new Rect(r.x + 2, r.y, lh, r.height);
            r1 = new Rect(r.x + lh, r.y, r.width - 2 * lh, r.height);
            r2 = new Rect((r.x + r.width - lh), r.y/* + 1*/, (lh), r.height/* - 4*/);
            GUI.Label(r0, i.ToString());
            EditorGUI.PropertyField(r1, property.GetArrayElementAtIndex(i), GUIContent.none);
            if (GUI.Button(r2, "-"))
            {
                property.DeleteArrayElementAtIndex(i);
            }
        }
    }
    Rect pos = new Rect();
    float x = 0;
    float w = 0;
    float y = 0; 
    protected int spacing = 0;
    protected Rect GetNextRect()
    {
        y += (spacing * verticalSpacing);
        pos.x = x + 10;
        pos.y = y;
        pos.width = w - 10;
        pos.height = EditorGUIUtility.singleLineHeight;
        y += pos.height;
        spacing = 0;
        return pos;
    }
    protected Rect GetCurrentRect()
    {
        return pos;
    }

    protected Rect GetLabelArea(Rect r)
    {
        float w = EditorGUIUtility.labelWidth + (EditorGUIUtility.singleLineHeight * (1 - EditorGUI.indentLevel));
        return new Rect(r.x, r.y, w, r.height);
    }
    protected Rect GetValueArea(Rect r)
    {
        float w = EditorGUIUtility.labelWidth + (EditorGUIUtility.singleLineHeight * (1 - EditorGUI.indentLevel)) - 2;
        return new Rect(w, r.y, (r.x + r.width - w), r.height);
    }

    protected Rect GetColumnedRect(Rect fullRect, int totalColums, int startColumn, int endColumn = -1)
    {
        if (endColumn < 0) endColumn = startColumn;
        //startColumn++;
        //occupiedColumns++;
        Rect r = fullRect;
        r.x = fullRect.x + (fullRect.width / totalColums) * startColumn;
        r.width = (fullRect.width / totalColums) * (endColumn - startColumn + 1);
        return r;
    }

}
#endif