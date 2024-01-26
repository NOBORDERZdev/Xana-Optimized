using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FeedController : MonoBehaviour
{
    [SerializeField] private FeedUIController feedUIController;
    [SerializeField] private GameObject feedPostPrefab;
    [SerializeField] private Transform feedPostParent;
    int feedPageNumber = 1;
    int feedPageSize = 10;
    List<FeedResponseRow> FeedAPIData = new List<FeedResponseRow>();
    bool isFeedInitialized = false;
    private void OnEnable()
    {
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
                        Instantiate(feedPostPrefab, feedPostParent).GetComponent<FeedData>().SetFeedPrefab(item);
                    }
                   
                }
                //feedUIController.InitializeFeedPage(feedResponseData);
            }

        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
        response.Dispose();
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

