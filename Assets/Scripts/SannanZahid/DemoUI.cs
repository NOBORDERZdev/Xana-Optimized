using System.Collections.Generic;
using DynamicScrollRect;
using UnityEngine;

public class DemoUI : MonoBehaviour
{
    [SerializeField] private DynamicScrollRect.ScrollContent _content = null;
    [SerializeField] private int _itemCount = 50;
    
    private void Awake()
    {
        /*Application.targetFrameRate = 60;
        List<ScrollItemDefault> contentDatas = new List<ScrollItemDefault>();
        _content.TotalItems = _itemCount;
        for (int i = 0; i < _itemCount; i++)
        {
            contentDatas.Add(new ScrollItemDefault(i));
        }
        //_content.InitScrollContent(contentDatas);*/
    }
}