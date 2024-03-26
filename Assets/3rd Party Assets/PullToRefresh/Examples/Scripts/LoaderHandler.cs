using System.Collections;
using PullToRefresh;
using UnityEngine;

public class LoaderHandler : MonoBehaviour
{
    public static LoaderHandler Instance;
    
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
        // Register callback
        // This registration is possible even from Inspector.       
        //m_UIRefreshControl.OnRefresh.AddListener(RefreshItems);
    }

    public void RefreshItems()
    {
        if (m_UIRefreshControl.loaderObj.activeSelf)
        {
           Debug.Log("RefreshItems Name:" + this.gameObject.name);
            loaderBGImage.SetActive(true);
            m_UIRefreshControl.loaderObj.transform.GetChild(0).GetComponent<CustomLoader>().isRotate = true;

            //FeedsManager.Instance.allFeedCurrentpage = 1;
            //FeedsManager.Instance.followingUserCurrentpage = 1;

            if (FeedsManager.Instance.feedUiScreen.activeSelf)
            {
                for (int i = 0; i < FeedsManager.Instance.allFeedPanel.Length; i++)
                {
                    if (i == 1 && FeedsManager.Instance.allFeedPanel[1].gameObject.activeSelf)
                    {
                        SNS_APIResponseManager.Instance.RequestGetFeedsByFollowingUser(1, 10, "PullRefresh");

                    }
                    else if (FeedsManager.Instance.allFeedPanel[i].gameObject.activeSelf)
                    {
                        //Riken
                        //SNS_APIResponseManager.Instance.RequestGetAllUsersWithFeeds(1, 10, "PullRefresh");
                        SNS_APIResponseManager.Instance.RequestGetAllUsersWithFeeds(/*FeedsManager.Instance.allFeedCurrentpage +*/ 1, 10, "PullRefresh");
                        //Debug.Log("Refresh GetAllUsersWithFeeds Api");
                    }
                }
            }
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