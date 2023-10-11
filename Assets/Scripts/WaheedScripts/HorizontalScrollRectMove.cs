using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalScrollRectMove : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public Button nextButton;
    public Button prevButton;
    //public Button addToTailButton; // Move 0 index to last
    //public Button addToHeadButton; // Move last index to 0
    //public Button deleteFromHeadButton; // Delete item at 0 index
    //public Button deleteFromTailButton; // Delete last item
    public float scrollSpeed = 1.0f;

    private int currentIndex = 0;
    private float targetHorizontalPosition = 0.0f;
    private bool isScrolling = false;

    private void Start()
    {
        nextButton.onClick.AddListener(ScrollNext);
        prevButton.onClick.AddListener(ScrollPrevious);
        //addToTailButton.onClick.AddListener(MoveFirstToLast);
        //addToHeadButton.onClick.AddListener(MoveLastToFirst);
        //deleteFromHeadButton.onClick.AddListener(DeleteFromHead);
        //deleteFromTailButton.onClick.AddListener(DeleteFromTail);
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
        currentIndex++;
        currentIndex = Mathf.Clamp(currentIndex, 0, content.childCount - 1);
        targetHorizontalPosition = -currentIndex * scrollRect.viewport.rect.width;
        MoveFirstToLast();
       // DeleteFromHead();
        isScrolling = true;
    }

    private void ScrollPrevious()
    {
        currentIndex--;
        currentIndex = Mathf.Clamp(currentIndex, 0, content.childCount - 1);
        targetHorizontalPosition = -currentIndex * scrollRect.viewport.rect.width;
        MoveLastToFirst();
       // DeleteFromTail();
        isScrolling = true;
    }

    private void MoveFirstToLast()
    {
        // Move the 0 index item to the last position
        if (currentIndex > 0 && content.childCount > 1)
        {
            RectTransform firstItem = content.GetChild(0) as RectTransform;
            RectTransform lastItem = content.GetChild(content.childCount - 1) as RectTransform;

            Vector2 tempAnchoredPos = firstItem.anchoredPosition;
            firstItem.anchoredPosition = lastItem.anchoredPosition;
            lastItem.anchoredPosition = tempAnchoredPos;

            firstItem.SetSiblingIndex(content.childCount - 1);
            currentIndex = content.childCount - 1;
            targetHorizontalPosition = -currentIndex * scrollRect.viewport.rect.width;
            isScrolling = true;
        }
    }

    private void MoveLastToFirst()
    {
        // Move the last index item to the first position
        if (currentIndex <= content.childCount - 1 && content.childCount > 1)
        {
            RectTransform firstItem = content.GetChild(0) as RectTransform;
            RectTransform lastItem = content.GetChild(content.childCount - 1) as RectTransform;

            Vector2 tempAnchoredPos = firstItem.anchoredPosition;
            firstItem.anchoredPosition = lastItem.anchoredPosition;
            lastItem.anchoredPosition = tempAnchoredPos;

            lastItem.SetSiblingIndex(0);
            //currentIndex = 0;
            targetHorizontalPosition = -currentIndex * scrollRect.viewport.rect.width;
            isScrolling = true;
        }
    }

    private void DeleteFromHead()
    {
        // Delete the item at 0 index
        if (content.childCount > 0)
        {
            Destroy(content.GetChild(0).gameObject);
            currentIndex = Mathf.Clamp(currentIndex - 1, 0, content.childCount - 1);
            targetHorizontalPosition = -currentIndex * scrollRect.viewport.rect.width;
            isScrolling = true;
        }
    }

    private void DeleteFromTail()
    {
        // Delete the last item
        if (content.childCount > 0)
        {
            Destroy(content.GetChild(content.childCount - 1).gameObject);
            currentIndex = Mathf.Clamp(currentIndex, 0, content.childCount - 2);
            targetHorizontalPosition = -currentIndex * scrollRect.viewport.rect.width;
            isScrolling = true;
        }
    }
}
