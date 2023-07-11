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
        // Register callback
        // This registration is possible even from Inspector.       
        //m_UIRefreshControl.OnRefresh.AddListener(RefreshItems);
    }

    public void RefreshItems()
    {
        if (m_UIRefreshControl.loaderObj.activeSelf)
        {
            Debug.LogError("RefreshItems Name:" + this.gameObject.name);
            loaderBGImage.SetActive(true);
            m_UIRefreshControl.loaderObj.transform.GetChild(0).GetComponent<CustomLoader>().isRotate = true;

            //FeedUIController.Instance.allFeedCurrentpage = 1;
            //FeedUIController.Instance.followingUserCurrentpage = 1;

            if (FeedUIController.Instance.feedUiScreen.activeSelf)
            {
                for (int i = 0; i < FeedUIController.Instance.allFeedPanel.Length; i++)
                {
                    if (i == 1 && FeedUIController.Instance.allFeedPanel[1].gameObject.activeSelf)
                    {
                        APIManager.Instance.RequestGetFeedsByFollowingUser(1, 10, "PullRefresh");

                    }
                    else if (FeedUIController.Instance.allFeedPanel[i].gameObject.activeSelf)
                    {
                        //Riken
                        //APIManager.Instance.RequestGetAllUsersWithFeeds(1, 10, "PullRefresh");
                        APIManager.Instance.RequestGetAllUsersWithFeeds(/*FeedUIController.Instance.allFeedCurrentpage +*/ 1, 10, "PullRefresh");
                        // Debug.LogError("Refresh GetAllUsersWithFeeds Api");
                    }
                }
            }
            Debug.LogError("Refresh Current Screen Api");

            StartCoroutine(FetchDataDemo());
        }
    }

    private IEnumerator FetchDataDemo()
    {
        // Instead of data acquisition.
        yield return new WaitForSeconds(1f);

        while (!isLoaderGetApiResponce)
        {
            //Debug.LogError("1:" + isLoaderGetApiResponce);
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