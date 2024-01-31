using EnhancedUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FeedController : MonoBehaviour
{
    [SerializeField] private FeedUIController feedUIController;
    //[SerializeField] private GameObject feedPostPrefab;
    [SerializeField] private Transform feedContentParent;
    int feedPageNumber = 1;
    int feedPageSize = 1000;
    List<FeedResponseRow> FeedAPIData = new List<FeedResponseRow>();
    List<FeedData> feedList = new List<FeedData>();
    bool isFeedInitialized = false;
    [SerializeField]
    FeedScrollerController scrollerController;
    private void OnEnable()
    {
        SocketController.instance.updateFeedLike += UpdateFeedLike;
        if (feedUIController == null)
        feedUIController = FeedUIController.Instance;
        if (!isFeedInitialized)
        {
           Invoke(nameof( IntFeedPage),0.01f);  
        }
    }

    /// <summary>
    /// To Initialize the Feed Page with the data from API
    /// </summary>
    async void IntFeedPage()
    {
        scrollerController.IntFeedScroller();
        await GetFeedData(APIManager.Instance.userId);
    }

    async Task GetFeedData(int userId)
    {
        string url = ConstantsGod.API_BASEURL +ConstantsGod.FeedGetAllByUserId+userId+"/"+feedPageNumber +"/"+ feedPageSize;
        UnityWebRequest response = UnityWebRequest.Get(url);
        try
        {   await response.SendWebRequest();
            if (response.isNetworkError)
            {
                Debug.Log(response.error);
            }
            else
            {
                FeedResponse feedResponseData = JsonUtility.FromJson<FeedResponse>(response.downloadHandler.text.ToString());
               // FeedAPIData.Add(feedResponseData);
                isFeedInitialized = true;
                foreach (var item in feedResponseData.data.rows)
                {
                    if (!String.IsNullOrEmpty( item.text_post) && !item.text_post.Equals("null") )
                    {
                        FeedAPIData.Add(item);
                        scrollerController._data.Add(item);
                        //GameObject temp =  Instantiate(feedPostPrefab);
                        //temp.transform.SetParent(feedPostParent);
                        //temp.transform.localScale = Vector3.one;
                        //temp.GetComponent<FeedData>().SetFeedPrefab(item);
                        //feedList.Add(temp.GetComponent<FeedData>());
                    }
                   
                }
                Invoke(nameof(InovkeScrollReload), 2f);
                // feedUIController.IntFeedScroller(feedResponseData);
            }

        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
        response.Dispose();
    }


    void InovkeScrollReload()
    {
        gameObject.GetComponent<ScrollRect>().content.SetParent(feedContentParent);
        scrollerController.scroller.ReloadData();
    }
    /// <summary>
    /// To Update Feed Like Count from Socket
    /// </summary>
    /// <param name="feedId"></param>
    /// <param name="isLiked"></param>
    public void UpdateFeedLike(FeedLikeSocket feedLikeSocket)
    {
        foreach (var item in feedList)
        {
            if (item.GetFeedId() == feedLikeSocket.textPostId)
            {
              item.UpdateLikeCount(feedLikeSocket.likeCount);
            }
        }
    }

    private void OnDisable()
    {
        SocketController.instance.updateFeedLike -= UpdateFeedLike;

    }
}

[System.Serializable]
public class FeedResponse
{
    public bool success;
    public FeedResponseData data;
    public string msg;
}

[System.Serializable]
public class FeedResponseData
{
    public bool count;
    public List<FeedResponseRow> rows;
}

[System.Serializable]
public class FeedResponseRow
{
    public int id;
    public int user_id;
    public string text_post;
    public string text_mood;
    public int like_count;
    public string createdAt;
    public string updatedAt;
    public bool isLikedByUser;
    public FeedIResponseUser user;
}

[System.Serializable]
public class FeedIResponseUser
{
    public int id;
    public string name;
    public string avatar;
}

