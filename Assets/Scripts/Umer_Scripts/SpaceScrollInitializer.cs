using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;
using EnhancedScrollerDemos.NestedScrollers;

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

    //Custome Variables
    public bool initializeCategoryRow = false;
    public AllWorldManage allWorldManageRef;
    SpaceScrollRowHandler masterData;
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
        //Application.targetFrameRate = 60;

        // tell the scroller that this script will be its delegate
        masterScroller.Delegate = this;

        // load in a large set of data
        //LoadData();
    }

    /// <summary>
    /// Populates the data with a lot of records
    /// </summary>
    public void AddRowToScroller(WorldItemDetail _singleWorldItem, int _dataCount, string _categTitle)
    {
        Debug.Log("Function called this much time: ");
        // set up some simple data. This will be a two-dimensional array,
        // specifically a list within a list.

        if (initializeCategoryRow)
        {
            initializeCategoryRow = false;
            instanChildCount = 0;
            Debug.Log("Master row Initialized");
            //for (var i = 0; i < 10; i++)
            //{
            masterData = new SpaceScrollRowHandler()
            {
                normalizedScrollPosition = 0,
                _allWorldManageRef = allWorldManageRef,
                categoryTitle = _categTitle,
                childData = new List<WorldItemDetail>()
            };

            _data.Add(masterData);
        }
        masterData.childData.Add(_singleWorldItem);

        instanChildCount++;
        //}
        //}
        if (instanChildCount.Equals(_dataCount))
        {
            Debug.Log("Worked once only?");
            // tell the scroller to reload now that we have the data
            masterScroller.ReloadData();
        }
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
    /// <param name="cellIndex">The index of the list. This will likely be different from the dataIndex if the scroller is looping</param>
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

        // in this example, we just pass the data to our cell's view which will update its UI
        masterCellView.SetData(_data[dataIndex]);

        // return the cell to the scroller
        return masterCellView;
    }

    #endregion
}
