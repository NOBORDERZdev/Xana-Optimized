using TMPro;
using UnityEngine;
using WaheedDynamicScrollRect;
using UnityEngine.UI;

public class ScrollItemDefaultContent : ScrollItem<ScrollItemData>
{
    [SerializeField] private WaheedDynamicScrollRect.WaheedDynamicScrollRect _dynamicScroll = null;
    
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
