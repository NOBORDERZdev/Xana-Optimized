using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedScrollerDemos.NestedScrollers;

    public class SpaceScrollRowHandler
    {
        // This value will store the position of the detail scroller to be used 
        // when the scroller's cell view is recycled
        public float normalizedScrollPosition;

        public List<WorldItemDetail> childData;
    }
