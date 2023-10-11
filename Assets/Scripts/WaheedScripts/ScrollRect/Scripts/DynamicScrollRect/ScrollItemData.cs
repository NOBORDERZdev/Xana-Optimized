
using UnityEngine;
namespace DynamicScrollRect
{
    public class ScrollItemData
    {
        public int Index { get; }

        public ScrollItemData(int index)
        {
            Index = index;
            //Debug.Log("INDEX IS "+ Index);
        }
    }
}
