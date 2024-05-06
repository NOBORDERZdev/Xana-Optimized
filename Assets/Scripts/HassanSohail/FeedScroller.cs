using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class FeedScroller : MonoBehaviour, IEnhancedScrollerDelegate, IBeginDragHandler, IEndDragHandler
{
    /// <summary>
    /// Internal representation of our data. Note that the scroller will never see
    /// this, so it separates the data from the layout using MVC principles.
    /// </summary>
    public SmallList<FeedResponseRow> _data;
    public List<FeedHeightData> feedHeight;
    FeedController feedController;
    /// <summary>
    /// Whether the scroller is being dragged
    /// </summary>
    private bool _dragging = false;

    /// <summary>
    /// Whether we should refresh after releasing the drag
    /// </summary>
    private bool _pullToRefresh = false;

    /// <summary>
    /// This is our scroller we will be a delegate for
    /// </summary>
    public EnhancedScroller scroller;

    // <summary>
    /// If true, this will load cells before they are visible,
    /// buffering the data if you have a lookAhead value high enough
    /// to meet the download times
    /// </summary>
    bool preloadCells=true;

    /// <summary>
    /// This will be the prefab of each cell in our scroller. Note that you can use more
    /// than one kind of cell, but this example just has the one type.
    /// </summary>
    public EnhancedScrollerCellView cellViewPrefab;
    
    /// <summary>
    /// The higher the number here, the more we have to pull down to refresh
    /// </summary>
    public float pullDownThreshold;

    /// <summary>
    /// Some text to show that the user can pull down to refresh.
    /// Only shows up when near the top of the scroller in this example.
    /// </summary>
    public UnityEngine.UI.Text pullDownToRefreshText;

    /// <summary>
    /// Some text to show that the user can release to refresh.
    /// </summary>
    public UnityEngine.UI.Text releaseToRefreshText;

    /// <summary>
    /// Be sure to set up your references to the scroller after the Awake function. The 
    /// scroller does some internal configuration in its own Awake function. If you need to
    /// do this in the Awake function, you can set up the script order through the Unity editor.
    /// In this case, be sure to set the EnhancedScroller's script before your delegate.
    /// 
    /// In this example, we are calling our initializations in the delegate's Start function,
    /// but it could have been done later, perhaps in the Update function.
    /// </summary>


    private void Awake()
    {
        if (preloadCells)
        {
            scroller.lookAheadBefore = 2000f;
            scroller.lookAheadAfter = 2000f;
        }
    }
    public void IntFeedScroller(){
       // set the application frame rate.
       // this improves smoothness on some devices
       // Application.targetFrameRate = 60;

        // tell the scroller that this script will be its delegate
        scroller.Delegate = this;

        // tell our controller to monitor the scroller's scrolled event.
        scroller.scrollerScrolled = ScrollerScrolled;
        _data = new SmallList<FeedResponseRow>();
        feedHeight = new List<FeedHeightData>();
       
        // load in a large set of data
        //LoadLargeData();
     }

    ///// <summary>
    ///// Populates the data with a lot of records
    ///// </summary>
    //private void LoadLargeData()
    //{
    //    // set up some simple data
    //    _data = new SmallList<FeedResponseRow>();
    //    //for (var i = 0; i < 100; i++)
    //    //    _data.Add(new Data() { someText = "Cell Data Index " + i.ToString() });

    //    // tell the scroller to reload now that we have the data
    //    scroller.ReloadData();
    //}

    #region EnhancedScroller Handlers

    /// <summary>
    /// This tells the scroller the number of cells that should have room allocated. This should be the length of your data array.
    /// </summary>
    /// <param name="scroller">The scroller that is requesting the data size</param>
    /// <returns>The number of cells</returns>
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        // in this example, we just pass the number of our data elements
        return _data.Count;
    }

    /// <summary>
    /// This tells the scroller what the size of a given cell will be. Cells can be any size and do not have
    /// to be uniform. For vertical scrollers the cell size will be the height. For horizontal scrollers the
    /// cell size will be the width.
    /// </summary>
    /// <param name="scroller">The scroller requesting the cell size</param>
    /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
    /// <returns>The size of the cell</returns>
    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        // in this example, even numbered cells are 30 pixels tall, odd numbered cells are 100 pixels tall
        //return (dataIndex % 2 == 0 ? 30f : 100f);
        if (dataIndex < feedHeight.Count)
        {
             return feedHeight[dataIndex].height ;
        }
        else
        {
            return 256f;
        }
       
    }

    /// <summary>
    /// Gets the cell to be displayed. You can have numerous cell types, allowing variety in your list.
    /// Some examples of this would be headers, footers, and other grouping cells.
    /// </summary>
    /// <param name="scroller">The scroller requesting the cell</param>
    /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
    /// <param name="cellIndex">The index of the list. This will likely be different from the dataIndex if the scroller is looping</param>
    /// <returns>The cell for the scroller to use</returns>
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        // first, we get a cell from the scroller by passing a prefab.
        // if the scroller finds one it can recycle it will do so, otherwise
        // it will create a new cell.
        CellView cellView = scroller.GetCellView(cellViewPrefab) as CellView;

        // set the name of the game object to the cell's data index.
        // this is optional, but it helps up debug the objects in 
        // the scene hierarchy.
        cellView.name = "Feed " + dataIndex.ToString();

        // in this example, we just pass the data to our cell's view which will update its UI
        //cellView.SetData(_data[dataIndex]);
        cellView.GetComponent<FeedData>().SetFeedUiController(this.GetComponent<FeedScroller>());
        cellView.GetComponent<FeedData>().SetFeedPrefab(_data[dataIndex]);
        // return the cell to the scroller
        return cellView;
    }

    #endregion

    /// <summary>
    /// This delegate will fire when the scroller is scrolled
    /// </summary>
    /// <param name="scroller"></param>
    /// <param name="val"></param>
    /// <param name="scrollPosition"></param>
    private void ScrollerScrolled(EnhancedScroller scroller, Vector2 val, float scrollPosition)
    {
        var scrollMoved = (scroller.ScrollRect.content.anchoredPosition.y <= -pullDownThreshold);

        if (_dragging && scrollMoved)
        {
            // we are dragging and the scroll position is beyond the scroll threshold.
            // we should flag that a refresh is needed when the dragging is released.
            _pullToRefresh = true;

            // show the release text if the scroller is down beyond the threshold
            releaseToRefreshText.gameObject.SetActive(true);
        }

        // show the pull to refresh text if the scroller position is at the top
        pullDownToRefreshText.gameObject.SetActive(scrollPosition <= 0);
    }

    /// <summary>
    /// Fired by the ScrollRect
    /// </summary>
    /// <param name="data"></param>
    public void OnBeginDrag(PointerEventData data)
    {
        // we are now dragging.
        // we flag this so that refreshing won't occur if the scroller
        // is scrolling due to inertia. 
        // the user must drag manually in this example.
        _dragging = true;
    }

    /// <summary>
    /// Fired by the ScrollRect
    /// </summary>
    /// <param name="data"></param>
    public void OnEndDrag(PointerEventData data)
    {
        // no longer dragging
        _dragging = false;

        if (_pullToRefresh)
        {
            // we reached the scroll pull down threshold, so now we insert new data

            //for (var i = 0; i < 3; i++)
            //{
            //    _data.Insert(new Data() { someText = "Brand New Data " + i.ToString() + "!!!" }, 0);
            //}
            
        /// GET NEW DATA FROM API
             scroller.GetComponent<FeedController>().GetComponent<FeedController>().PullNewPlayerPost();
            // reload the scroller to show the new data
          //  scroller.ReloadData();

            // take off the refresh now that it is handled
            _pullToRefresh = false;

            // hide the release text if the scroller is down beyond the threshold
            releaseToRefreshText.gameObject.SetActive(false);
        }
    }

    public void AddInHeightList(int feedId, float height){
        if (!feedHeight.Any(item => item.feedId == feedId))
        {
            feedHeight.Add( new FeedHeightData(feedId,height));
        }
    }

    public void updateLikeCount(int feedId, int likeCount,bool likedBool= false){
        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].id == feedId)
            {
                _data[i].like_count = likeCount;
                _data[i].isLikedByUser = likedBool;
                break;
            }
        }
    }
}

[Serializable] 
public class FeedHeightData
{
    public int feedId;
    public float height;

    public FeedHeightData(int feedId, float height)
    {
        this.feedId = feedId;
        this.height = height;
    }
}