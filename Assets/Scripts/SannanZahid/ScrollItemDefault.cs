using TMPro;
using UnityEngine;

public class ScrollItemDefault : MonoBehaviour
{
    [SerializeField] private DynamicScrollRect.DynamicScrollRect _dynamicScroll = null;
    [SerializeField] private TextMeshProUGUI _text = null;
    public int Index;
    public Vector2 GridIndex { get; protected set; }
    public RectTransform RectTransform => transform as RectTransform;
    public void Activated()
    {
        gameObject.SetActive(true);
    }
    public void Deactivated()
    {
        gameObject.SetActive(false);
    }
    public ScrollItemDefault(int index)
    {
        Index = index;
    }
    public void InitItem(int index, Vector2 gridPos)
    {
        Index = index;
        _text.SetText(Index.ToString());
        GridIndex = gridPos;
    }
}