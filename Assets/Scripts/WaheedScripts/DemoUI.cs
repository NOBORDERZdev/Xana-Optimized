using System.Collections.Generic;
using DynamicScrollRect;
using UnityEngine;

public class DemoUI : MonoBehaviour
{
    public static DemoUI instance;

    [SerializeField] private DynamicScrollRect.ScrollContent _content = null;

    [SerializeField] public int _itemCount = 50;

    //public DynamicScrollRect.DynamicScrollRect rect;
    
    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;

        //_itemCount = rect.avatarData.Count;

        List<ScrollItemData> contentDatas = new List<ScrollItemData>();

        for (int i = 0; i < _itemCount; i++)
        {
            contentDatas.Add(new ScrollItemData(i));
        }
        
        _content.InitScrollContent(contentDatas);
    }
}
