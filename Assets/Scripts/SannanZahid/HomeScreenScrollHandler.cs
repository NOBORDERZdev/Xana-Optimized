using UnityEngine;
using UnityEngine.UI;

public class HomeScreenScrollHandler : ScrollRect
{
    [SerializeField]
    public DynamicScrollRect.DynamicScrollRect DynamicGrid;
    protected override void Start()
    {
        DynamicGrid.ParentSliderFlag = false;
    }
}
