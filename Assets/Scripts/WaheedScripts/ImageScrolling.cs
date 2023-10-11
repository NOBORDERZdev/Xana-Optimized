using UnityEngine;
using UnityEngine.UI;

public class ImageScrolling : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public Button nextButton;
    public Button prevButton;
    public float scrollSpeed = 1.0f;

    private int currentIndex = 0;
    private float targetHorizontalPosition = 0.0f;
    private bool isScrolling = false;

    private void Start()
    {
        nextButton.onClick.AddListener(ScrollNext);
        prevButton.onClick.AddListener(ScrollPrevious);
    }

    private void Update()
    {
        if (isScrolling)
        {
            float currentHorizontalPosition = content.anchoredPosition.x;
            float newPosition = Mathf.Lerp(currentHorizontalPosition, targetHorizontalPosition, Time.deltaTime * scrollSpeed);
            content.anchoredPosition = new Vector2(newPosition, content.anchoredPosition.y);

            if (Mathf.Abs(newPosition - targetHorizontalPosition) < 0.1f)
            {
                isScrolling = false;
            }
        }
    }

    private void ScrollNext()
    {
        //if (!isScrolling)
        //{
            currentIndex++;
            currentIndex = Mathf.Clamp(currentIndex, 0, content.childCount - 1);
            targetHorizontalPosition = -currentIndex * scrollRect.viewport.rect.width;
            isScrolling = true;
        //}
    }

    private void ScrollPrevious()
    {
        //if (!isScrolling)
        //{
            currentIndex--;
            currentIndex = Mathf.Clamp(currentIndex, 0, content.childCount - 1);
            targetHorizontalPosition = -currentIndex * scrollRect.viewport.rect.width;
            isScrolling = true;
        //}
    }
}