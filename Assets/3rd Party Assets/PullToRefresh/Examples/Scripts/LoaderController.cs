using System.Collections;
using PullToRefresh;
using UnityEngine;

public class LoaderController : MonoBehaviour
{
    public static LoaderController Instance;

    public UIRefreshControl m_UIRefreshControl;

    public GameObject loaderBGImage;

    public bool isLoaderGetApiResponce = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        
    }

    public void RefreshItems()
    {
        if (m_UIRefreshControl.loaderObj.activeSelf)
        {
            Debug.Log("RefreshItems Name:" + this.gameObject.name);
            loaderBGImage.SetActive(true);
            m_UIRefreshControl.loaderObj.transform.GetChild(0).GetComponent<CustomLoader>().isRotate = true;

            //if (FeedUIController.Instance.feedUiScreen.activeSelf)
            //{
            //    var allFeedPanel = FeedUIController.Instance.allFeedPanel;
            //    for (int i = 0; i < allFeedPanel.Length; i++)
            //    {
            //        if (allFeedPanel[i].gameObject.activeSelf)
            //        {
            //            if (i == 1)
            //            {
            //                //SNS_APIManager.Instance.RequestGetFeedsByFollowingUser(1, 10, "PullRefresh");
            //            }
            //            else
            //            {
            //                SNS_APIManager.Instance.RequestGetAllUsersWithFeeds(1, 10, "PullRefresh");
            //            }
            //        }
            //    }
            //}
            Debug.Log("Refresh Current Screen Api");

            StartCoroutine(FetchDataDemo());
        }
    }

    private IEnumerator FetchDataDemo()
    {
        // Instead of data acquisition.
        yield return new WaitForSeconds(1f);

        while (!isLoaderGetApiResponce)
        {
            //Debug.Log("1:" + isLoaderGetApiResponce);
            yield return true;
        }
        // Call EndRefreshing() when refresh is over.
        loaderBGImage.SetActive(false);
        m_UIRefreshControl.EndRefreshing();
    }

    // Register the callback you want to call to OnRefresh when refresh starts.
    public void OnRefreshCallback()
    {
        Debug.Log("OnRefresh called.");
    }
}