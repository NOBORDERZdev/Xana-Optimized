using System.Collections.Generic;
using WaheedDynamicScrollRect;
using UnityEngine;

public class DemoUIContent : MonoBehaviour
{
    public static DemoUIContent instance;

    [SerializeField] private WaheedDynamicScrollRect.ScrollContent _content = null;

    [SerializeField] public int _itemCount = 50;

    //public DynamicScrollRect.DynamicScrollRect rect;
    
    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 30;

        //_itemCount = rect.avatarData.Count;

        List<ScrollItemData> contentDatas = new List<ScrollItemData>();

        for (int i = 0; i < _itemCount; i++)
        {
            contentDatas.Add(new ScrollItemData(i));
        }
        
       // _content.InitScrollContent(contentDatas);
    }
}
