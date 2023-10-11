using TMPro;
using UnityEngine;
using DynamicScrollRect;
using UnityEngine.UI;

public class ScrollItemDefault : ScrollItem<ScrollItemData>
{
    [SerializeField] private DynamicScrollRect.DynamicScrollRect _dynamicScroll = null;
    
    [SerializeField] private TextMeshProUGUI _text = null;

    private void Start()
    {
       
        
    }

    public void FocusOnItem()
    {
        _dynamicScroll.StartFocus(this);
    }
    
    protected override void InitItemData(ScrollItemData data)
    {
        //_text.SetText(data.Index.ToString());
        
        base.InitItemData(data);
    }

    
}
