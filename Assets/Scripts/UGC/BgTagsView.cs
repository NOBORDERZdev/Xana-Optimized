using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BgTagsView : MonoBehaviour
{
    public int index;
    public string tagName;
    public string _name;
    public TextMeshProUGUI tagNameText;

    public GameObject highlighter;
    public Image icon;
    public void InitBgTags(BackgroundList _backgroundList)
    {
        tagName = _backgroundList.category;
        tagNameText.text = tagName;
    }
    public void InitBg(BackgroundList _backgroundList)
    {
        index = _backgroundList.id;
        tagName = _backgroundList.category;
    }
}
