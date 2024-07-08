using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using EnhancedScrollerDemos.NestedScrollers;
using UnityEngine.UI;

public class SpaceScrollInitializer : MonoBehaviour, IEnhancedScrollerDelegate
{
    /// <summary>
    /// Internal representation of our data. Note that the scroller will never see
    /// this, so it separates the data from the layout using MVC principles.
    /// </summary>
    public List<SpaceScrollRowHandler> _data = new List<SpaceScrollRowHandler>();

    /// <summary>
    /// This is our scroller we will be a delegate for. The master scroller contains mastercellviews which in turn
    /// contain EnhancedScrollers
    /// </summary>
    public EnhancedScroller masterScroller;

    /// <summary>
    /// This will be the prefab of each cell in our scroller. Note that you can use more
    /// than one kind of cell, but this example just has the one type.
    /// </summary>
    public EnhancedScrollerCellView masterCellViewPrefab;

    /// <summary>
    /// Used to determine if the scroller is already loading new data.
    /// If so, then we don't want to call again to avoid an infinite loop.
    /// </summary>
    public bool _loadingNew;

    //Custome Variables
    public bool initializeCategoryRow = false;
    public AllWorldManage allWorldManageRef;
    public SNSAPILoaderController paginationLoaderRef;
    SpaceScrollRowHandler masterData;
    public List<Sprite> categIcons = new List<Sprite>();
    public float scrollPosition;
    int instanChildCount = 0;

    /// <summary>
    /// Be sure to set up your references to the scroller after the Awake function. The 
    /// scroller does some internal configuration in its own Awake function. If you need to
    /// do this in the Awake function, you can set up the script order through the Unity editor.
    /// In this case, be sure to set the EnhancedScroller's script before your delegate.
    /// 
    /// In this example, we are calling our initializations in the delegate's Start function,
    /// but it could have been done later, perhaps in the Update function.
    /// </summary>
    void Start()
    {
        allWorldManageRef = GetComponent<AllWorldManage>();
        // set the application frame rate.
        // this improves smoothness on some devices
            Application.targetFrameRate = 30;

        // tell the scroller that this script will be its delegate
        masterScroller.Delegate = this;
        masterScroller.scrollerScrolled = ScrollerScrolled;

        // load in a large set of data
        //LoadData();
    }

    /// <summary>
    /// Populates the data with a lot of records
    /// </summary>
    public void AddRowToScroller(WorldItemDetail _singleWorldItem, int _dataCount, string _categTitle, List<TagsCategoryData> _tagsCategData = null, List<string> _categTitles = null, bool _isTagCateg = false)
    {
        //Debug.Log("Function called this much time: " + _dataCount);
        // set up some simple data. This will be a two-dimensional array,
        // specifically a list within a list.
        if (_isTagCateg)
        {
            for (int i = 0; i < _tagsCategData.Count; i++)
            {
                initializeCategoryRow = true;
                for (int j = 0; j < _tagsCategData[i]._tagAsCategoryData.Count; j++)
                {
                    MasterScrollRowInit(initializeCategoryRow, _categTitles[i], _tagsCategData[i]._tagAsCategoryData[j]);
                }
            }
            Debug.Log("Added log message here in order to get loader turned off as without it loader wont get turned off");//UMER
            LoadDataInPool();
            Debug.Log("Added log message here in order to get loader turned off as without it loader wont get turned off");//UMER
            paginationLoaderRef.ShowApiLoader(false);
        }
        else
        {
            MasterScrollRowInit(initializeCategoryRow, _categTitle, _singleWorldItem);
            instanChildCount++;

            if (instanChildCount.Equals(_dataCount))
            {
                LoadDataInPool();
            }
        }
    }

    public void MasterScrollRowInit(bool _initCategRow, string _categRowTitle, WorldItemDetail _masterRowChildData)
    {
        if (_initCategRow)
        {
            initializeCategoryRow = !_initCategRow;
            instanChildCount = 0;
            Debug.Log("Master row Initialized" + _categRowTitle);

            masterData = new SpaceScrollRowHandler()
            {
                normalizedScrollPosition = 0,
                _allWorldManageRef = allWorldManageRef,
                categoryTitle = _categRowTitle,
                childData = new List<WorldItemDetail>()
            };

            _data.Add(masterData);
        }
        masterData.childData.Add(_masterRowChildData);
    }

    public void LoadDataInPool()
    {
        scrollPosition = masterScroller.ScrollPosition;
        if (!masterScroller.GetComponent<ScrollRect>().enabled)
            masterScroller.GetComponent<ScrollRect>().enabled = true;
        // tell the scroller to reload now that we have the data
        masterScroller.ReloadData();
        masterScroller.ScrollPosition = scrollPosition;

        _loadingNew = false;
    }

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
        // in this example, our master cells are 100 pixels tall
        return 363f;
    }

    /// <summary>
    /// Gets the cell to be displayed. You can have numerous cell types, allowing variety in your list.
    /// Some examples of this would be headers, footers, and other grouping cells.
    /// </summary>
    /// <param name="scroller">The scroller requesting the cell</param>
    /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
    /// <param name="cellIndex">The index of the list. This will likely be different from the dataIndex if the scroller is 4ing</param>
    /// <returns>The cell for the scroller to use</returns>
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        // first, we get a cell from the scroller by passing a prefab.
        // if the scroller finds one it can recycle it will do so, otherwise
        // it will create a new cell.
        SpaceScrollRowController masterCellView = scroller.GetCellView(masterCellViewPrefab) as SpaceScrollRowController;

        // set the name of the game object to the cell's data index.
        // this is optional, but it helps up debug the objects in 
        // the scene hierarchy.
        masterCellView.name = "Master Cell " + dataIndex.ToString();
        masterCellView.gameObject.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = 
            new Vector2(masterCellView.gameObject.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x, GetCellViewSize(scroller, dataIndex));
        if (dataIndex >= 0 && dataIndex < categIcons.Count)
        {
            masterCellView.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = categIcons[dataIndex];
            SetCategIconState(5, -5, true, masterCellView.transform.GetChild(0).GetComponent<HorizontalLayoutGroup>(), masterCellView.transform.GetChild(0).GetChild(0).gameObject);
        }
        else
        {
            SetCategIconState(0, -9, false, masterCellView.transform.GetChild(0).GetComponent<HorizontalLayoutGroup>(), masterCellView.transform.GetChild(0).GetChild(0).gameObject);
        }

        //Setting child data loopable if child data count is greater than 3
        //Debug.Log("This Master cell view name: " + masterCellView.name + " child data size is: " + _data[dataIndex].childData.Count);
        //if (_data[dataIndex].childData.Count >= 3)
        //{
        //    masterCellView.gameObject.transform.GetChild(1).GetComponent<EnhancedScroller>().Loop = true;
        //}

        // in this example, we just pass the data to our cell's view which will update its UI
        masterCellView.SetData(_data[dataIndex]);

        // return the cell to the scroller
        return masterCellView;
    }

    void SetCategIconState(int _leftPaddingHLG, int _topPaddingHLG, bool _cateIconState, HorizontalLayoutGroup _categTitleHLG, GameObject _categIconRef)
    {
        _categTitleHLG.padding.left = _leftPaddingHLG;
        _categTitleHLG.padding.top = _topPaddingHLG;
        _categIconRef.SetActive(_cateIconState);
    }

    /// <summary>
    /// This is called when the scroller fires a scrolled event
    /// </summary>
    /// <param name="scroller">the scroller that fired the event</param>
    /// <param name="val">scroll amount</param>
    /// <param name="scrollPosition">new scroll position</param>
    private void ScrollerScrolled(EnhancedScroller scroller, Vector2 val, float scrollPosition)
    {
        // if the scroller is at the end of the list and not already loading
        if (scroller.NormalizedScrollPosition >= 1f && !_loadingNew)
        {
            WorldManager.instance.worldSpaceHomeScreenRef.tagAsCategoryData.Clear();
            WorldManager.instance.worldSpaceHomeScreenRef.CategorytagNames.Clear();
            if (!(WorldManager.instance.worldSpaceHomeScreenRef._tagsTraversedCount >= WorldSpacesHomeScreen.mostVisitedTagList.Count - 1))
            {
                masterScroller.GetComponent<ScrollRect>().enabled = false;

                // toggle on loading so that we don't get stuck in a loading loop
                _loadingNew = true;

                Invoke(nameof(LoadCategoryTagsWithDelay), 0.5f);
                //Debug.Log("Scroller Scrolled Registered");

            }
            //    // for this example, we fake a delay that would simulate getting new data in a real application.
            //    // normally you would just call LoadData(_data.Count) directly here, instead of adding the fake
            //    // 1 second delay.

            //    //StartCoroutine(FakeDelay());
        }
    }

    public void LoadCategoryTagsWithDelay()
    {
        Debug.LogError("hereerererer");
        paginationLoaderRef.ShowApiLoader(true);
        WorldManager.instance.worldSpaceHomeScreenRef.GetUsersMostVisitedTags(5);
    }

        #endregion
    }
