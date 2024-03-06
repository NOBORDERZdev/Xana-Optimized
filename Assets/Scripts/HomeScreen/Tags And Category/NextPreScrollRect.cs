using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextPreScrollRect : MonoBehaviour
{

    public ScrollRect scrollRect;
    public Button nextButton;
    public Button prevButton;

    private float scrollableWidth;
    private float currentScrollPos;

    private void Start()
    {
        // Get the width of the content in the ScrollRect.
        scrollableWidth = scrollRect.content.rect.width - scrollRect.viewport.rect.width;
        // Initially, hide the buttons if content fits within the view.
        UpdatePaginationButtons();

        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        nextButton.onClick.AddListener(OnNextButtonClick);
        prevButton.onClick.AddListener(OnPreButtonClick);
    }

    //private void Update()
    //{
    //    // Get the current horizontal scroll position.
    //    currentScrollPos = scrollRect.normalizedPosition.x;

    //    // Update the buttons based on the current scroll position.
    //    UpdatePaginationButtons();
    //}

    private void OnScrollValueChanged(Vector2 normalizedPosition)
    {
        // Get the current horizontal scroll position.
        currentScrollPos = scrollRect.normalizedPosition.x;
        if (scrollRect.content.rect.width > scrollRect.viewport.rect.width) //if tags are less than the width of the scroll view disable arrow buttons
        {
            UpdatePaginationButtons();

        }
        else
        {
            nextButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(false);
        }

        // Update the buttons based on the horizontal normalized position.
        //UpdatePaginationButtons();
    }

    private void UpdatePaginationButtons()
    {
        // Enable/disable the buttons based on the scroll position.
        nextButton.interactable = currentScrollPos < 1f;
        prevButton.interactable = currentScrollPos > 0f;

        // Auto-hide the buttons when not needed.
        nextButton.gameObject.SetActive(currentScrollPos < 1f);
        prevButton.gameObject.SetActive(currentScrollPos > 0f);
    }

    void OnNextButtonClick()
    {
        float x = 0;
        while(x < 1)
        {
            scrollRect.horizontalNormalizedPosition += .01f;
            x += .02f;
        }
    }

    void OnPreButtonClick()
    {
        float x = 1;
        while (x > 0)
        {
            scrollRect.horizontalNormalizedPosition -= .01f;
            x -= .02f;
        }
    }
}
