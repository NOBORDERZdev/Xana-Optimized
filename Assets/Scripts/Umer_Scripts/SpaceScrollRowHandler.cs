using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedScrollerDemos.NestedScrollers;

public class SpaceScrollRowHandler
{
    // This value will store the position of the detail scroller to be used 
    // when the scroller's cell view is recycled
    public float normalizedScrollPosition;

    public AllWorldManage _allWorldManageRef;

    public string categoryTitle;

    public List<WorldItemDetail> childData;
}
