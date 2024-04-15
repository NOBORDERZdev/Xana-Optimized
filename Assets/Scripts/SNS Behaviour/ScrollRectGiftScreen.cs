using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScrollRectGiftScreen : MonoBehaviour
{
    public RectTransform[] tabRectTransform;
    //[Space(5)]
    //public GridLayoutGroup[] gridGroup;

    public static ScrollRectGiftScreen Instance;

    [Tooltip("Set starting page index - starting from 0")]
    public int startingPage = 0;
    [Tooltip("Threshold time for fast swipe in seconds")]
    public float fastSwipeThresholdTime = 0.3f;
    [Tooltip("Threshold time for fast swipe in (unscaled) pixels")]
    public int fastSwipeThresholdDistance = 100;
    [Tooltip("How fast will page lerp to target position")]
    public float decelerationRate = 10f;
    [Tooltip("Button to go to the previous page (optional)")]
    public GameObject prevButton;
    [Tooltip("Button to go to the next page (optional)")]
    public GameObject nextButton;
    [Tooltip("Sprite for unselected page (optional)")]
    public Sprite unselectedPage;
    [Tooltip("Sprite for selected page (optional)")]
    public Sprite selectedPage;
    [Tooltip("Container with page images (optional)")]
    public Transform pageSelectionIcons;

    public Transform mainContainer;


    // fast swipes should be fast and short. If too long, then it is not fast swipe
    private int _fastSwipeThresholdMaxLimit;

    private ScrollRect _scrollRectComponent;
    private RectTransform _scrollRectRect;
    private RectTransform _container;

    private bool _horizontal;

    // number of pages in container
    private int _pageCount;
    public int _currentPage;

    // whether lerping is in progress and target lerp position
    private bool _lerp;
    private Vector2 _lerpTo;

    // target position of every page
    private List<Vector2> _pagePositions = new List<Vector2>();

    // in draggging, when dragging started and where it started
    private bool _dragging;
    private float _timeStamp;
    private Vector2 _startPosition;

    // for showing small page icons
    private bool _showPageSelection;
    private int _previousPageSelectionIndex;
    // container with Image components - one Image for each page
    private List<Image> _pageSelectionImages;

    //------------------------------------------------------------------------
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        // SetHeightWidth();
        //Coomented for now as profile scroll not working properly---------UMER
        //StartScrollSnap();

    }
    //public void SetHeightWidth()
    //{
    //    for (int i = 0; i < mainContainer.childCount; i++)
    //    {
    //        // Debug.LogError("width " + this.GetComponent<RectTransform>().rect.width +  "height  "+ this.GetComponent<RectTransform>().rect.height);
    //        mainContainer.GetChild(i).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().rect.width, this.GetComponent<RectTransform>().rect.height);
    //    }
    //}


    public void StartScrollSnap()
    {
        _scrollRectComponent = GetComponent<ScrollRect>();
        _scrollRectRect = GetComponent<RectTransform>();
        _container = _scrollRectComponent.content;
        _pageCount = _container.childCount;

        // is it horizontal or vertical scrollrect
        if (_scrollRectComponent.horizontal && !_scrollRectComponent.vertical)
        {
            _horizontal = true;
        }
        else if (!_scrollRectComponent.horizontal && _scrollRectComponent.vertical)
        {
            _horizontal = false;
        }
        else
        {
            Debug.LogWarning("Confusing setting of horizontal/vertical direction. Default set to horizontal.");
            _horizontal = true;
        }

        _lerp = false;

        // init
        SetPagePositions();
        SetPage(startingPage);
        InitPageSelection();
        SetPageSelection(startingPage);

        // prev and next buttons
        if (nextButton)
            nextButton.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });

        if (prevButton)
            prevButton.GetComponent<Button>().onClick.AddListener(() => { PreviousScreen(); });
    }

    //------------------------------------------------------------------------
    void Update()
    {
        // if moving to target position
        //Falsed for now as profile scroll not working properly---------UMER
        if (/*_lerp*/false)
        {
            // prevent overshooting with values greater than 1
            float decelerate = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
            // _container.anchoredPosition = Vector2.Lerp(_container.anchoredPosition, _lerpTo, decelerate);
            _container.localPosition = Vector2.Lerp(_container.localPosition, _lerpTo, decelerate);
            // time to stop lerping?
            //   Debug.Log(_container.localPosition.x-_lerpTo.x);
            if ((_container.localPosition.x - _lerpTo.x) < 0.25f)    // _container.anchoredPosition - _lerpTo
            {
                // snap to target and stop lerping
                //_container.anchoredPosition = _lerpTo;

                Vector3 pos = new Vector3(_lerpTo.x, _container.localPosition.y, _container.localPosition.z);
                _container.localPosition = pos;

                _lerp = false;
                // clear also any scrollrect move that may interfere with our lerping
                _scrollRectComponent.velocity = Vector2.zero;
            }

            // switches selection icon exactly to correct page
            if (_showPageSelection)
            {
                SetPageSelection(GetNearestPage());
            }
        }
    }

    //------------------------------------------------------------------------
    private void SetPagePositions()
    {
        int width = 0;
        int height = 0;
        int offsetX = 0;
        int offsetY = 0;
        int containerWidth = 0;
        int containerHeight = 0;

        if (_horizontal)
        {
            // screen width in pixels of scrollrect window
            width = (int)_scrollRectRect.rect.width;
            // center position of all pages
            offsetX = width / 2;
            // total width
            containerWidth = width * _pageCount;
            // limit fast swipe length - beyond this length it is fast swipe no more
            _fastSwipeThresholdMaxLimit = width;
        }
        else
        {
            height = (int)_scrollRectRect.rect.height;
            offsetY = height / 2;
            containerHeight = height * _pageCount;
            _fastSwipeThresholdMaxLimit = height;
        }

        //// set width of container
        //Vector2 newSize = new Vector2(containerWidth, containerHeight);
        //_container.sizeDelta = newSize;
        //Vector2 newPosition = new Vector2(containerWidth / 2, containerHeight / 2);
        _container.anchoredPosition = tabRectTransform[_currentPage].anchoredPosition; //newPosition;

        //if (ConstantsHolder.xanaConstants.screenType.Equals(ConstantsHolder.ScreenType.MobileScreen))
        //    gridGroup[_currentPage].cellSize = new Vector2(345, 350);
        //else if (ConstantsHolder.xanaConstants.screenType.Equals(ConstantsHolder.ScreenType.TabScreen))
        //gridGroup[_currentPage].cellSize = new Vector2(370, 400);
        //Debug.Log("<color=red>Set Grid Layout Cell size</color>");

        // delete any previous settings
        _pagePositions.Clear();

        // iterate through all container childern and set their positions
        for (int i = 0; i < _pageCount; i++)
        {
            //RectTransform child = _container.GetChild(i).GetComponent<RectTransform>();
            Vector2 childPosition;
            if (_horizontal)
            {
                childPosition = new Vector2(i * width - containerWidth / 2 + offsetX, 0f);
            }
            else
            {
                childPosition = new Vector2(0f, -(i * height - containerHeight / 2 + offsetY));
            }
            //child.anchoredPosition = childPosition;
            Debug.Log("<color=blue>Page Position: " + -childPosition + "</color>");
            _pagePositions.Add(-childPosition);
        }
    }

    //------------------------------------------------------------------------
    public void SetPage(int aPageIndex)
    {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
        Debug.Log("<color=red>Page Position: " + tabRectTransform[aPageIndex].localPosition + "</color>");
        _container.localPosition = tabRectTransform[aPageIndex].localPosition;

        // _container.anchoredPosition = tabRectTransform[aPageIndex].anchoredPosition;  //_pagePositions[aPageIndex];
        _currentPage = aPageIndex;
    }

    //------------------------------------------------------------------------
    public void LerpToPage(int aPageIndex)
    {
       
        //if (ConstantsHolder.xanaConstants.screenType.Equals(ConstantsHolder.ScreenType.MobileScreen))
        //    gridGroup[aPageIndex].cellSize = new Vector2(345, 350);
        //else if (ConstantsHolder.xanaConstants.screenType.Equals(ConstantsHolder.ScreenType.TabScreen))
        //gridGroup[aPageIndex].cellSize = new Vector2(370, 400);
        //Debug.Log("<color=red>Set Grid Layout Cell size</color>");

        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
        //_lerpTo = tabRectTransform[aPageIndex].anchoredPosition;  // _pagePositions[aPageIndex];
        if(aPageIndex> 0 && aPageIndex < tabRectTransform.Length)
            _lerpTo = tabRectTransform[aPageIndex].localPosition;
        //Debug.Log("<color=blue>_lerpTo: " + tabRectTransform[aPageIndex].localPosition + "</color>");
        //Debug.Log("<color=blue>_lerpFrom: " + _container.localPosition + "</color>");
        _lerp = true;
        _currentPage = aPageIndex;
    }
    //------------------------------------------------------------------------
    private void InitPageSelection()
    {
        // page selection - only if defined sprites for selection icons
        _showPageSelection = unselectedPage != null && selectedPage != null;
        if (_showPageSelection)
        {
            // also container with selection images must be defined and must have exatly the same amount of items as pages container
            if (pageSelectionIcons == null || pageSelectionIcons.childCount != _pageCount)
            {
                Debug.LogWarning("Different count of pages and selection icons - will not show page selection");
                _showPageSelection = false;
            }
            else
            {
                _previousPageSelectionIndex = -1;
                _pageSelectionImages = new List<Image>();

                // cache all Image components into list
                for (int i = 0; i < pageSelectionIcons.childCount; i++)
                {
                    Image image = pageSelectionIcons.GetChild(i).GetComponent<Image>();
                    if (image == null)
                    {
                        Debug.LogWarning("Page selection icon at position " + i + " is missing Image component");
                    }
                    _pageSelectionImages.Add(image);
                }
            }
        }
    }

    //------------------------------------------------------------------------
    private void SetPageSelection(int aPageIndex)
    {
        // nothing to change
        if (_previousPageSelectionIndex == aPageIndex)
        {
            return;
        }
    }

    //------------------------------------------------------------------------
    public void NextScreen()
    {
        LerpToPage(_currentPage + 1);
    }

    //------------------------------------------------------------------------
    public void PreviousScreen()
    {
        LerpToPage(_currentPage - 1);
    }

    public void OnCLickTabButton(int pageIndex)
    {
        LerpToPage(pageIndex);
    }

    //------------------------------------------------------------------------
    private int GetNearestPage()
    {
        // based on distance from current position, find nearest page
        Vector2 currentPosition = _container.anchoredPosition;

        float distance = float.MaxValue;
        int nearestPage = _currentPage;

        for (int i = 0; i < _pagePositions.Count; i++)
        {
            float testDist = Vector2.SqrMagnitude(currentPosition - _pagePositions[i]);
            if (testDist < distance)
            {
                distance = testDist;
                nearestPage = i;
            }
        }
        Debug.Log("<color=blue> Get Nearest Page </color>");
        return nearestPage;
    }
}