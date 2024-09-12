using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectResetter : MonoBehaviour
{
    private void OnEnable()
    {
        var scrollRect = GetComponent<ScrollRect>();
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }
}
