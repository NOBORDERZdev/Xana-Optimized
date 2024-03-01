using UnityEngine;
using UnityEngine.UI;

public class ScrollDefaultPosition : MonoBehaviour
{
    public ScrollRect scrollRect;
    private void OnEnable()
    {
        scrollRect.content.anchoredPosition = new Vector2(0, 0);
    }
}
