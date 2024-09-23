using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatIOSNotch : MonoBehaviour
{
    public VerticalLayoutGroup verticalLayoutGroup;
    int leftPadding = 30;
    void Start()
    {
#if UNITY_IOS
        if (verticalLayoutGroup != null)
        {
            verticalLayoutGroup.padding.left = leftPadding;
            verticalLayoutGroup.SetLayoutHorizontal(); // Update the layout
        }
#endif
    }
}
