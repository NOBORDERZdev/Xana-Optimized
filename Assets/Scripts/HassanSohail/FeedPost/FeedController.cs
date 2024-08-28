using AdvancedInputFieldPlugin;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FeedController : MonoBehaviour
{
    [SerializeField] private FeedUIController feedUIController;
    [SerializeField] private GameObject feedPostPrefab;
    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private Transform feedContentParent;
    int feedPageNumber = 1;
    int feedPageSize = 100;
    List<FeedResponseRow> FeedAPIData = new List<FeedResponseRow>();
    List<FeedData> feedList = new List<FeedData>();
    bool isFeedInitialized = false;
    [SerializeField]
    FeedScroller scrollerController;
    [SerializeField] private Transform noFeedsScreen;
    [SerializeField] private Transform noFeedSerach;
    [SerializeField] private TMP_Text noFeedText;
    [SerializeField] GameObject SerachPanel;
    [SerializeField] GameObject SearchContentPanel;
    [SerializeField] GameObject SerchBarObj;
    [SerializeField] AdvancedInputField searchInputField;
    [SerializeField] RectTransform feedTabsContainer;
    [SerializeField] GameObject FeedLoader;
    public HeightMngrAccToDevice ParentHeightAdjuster;

    public FeedResponse feedResponseData;

    public bool noResultinFeedSearch = true;

    private void OnEnable()
    {
        SerachPanel.SetActive(false);
        feedContentParent.gameObject.SetActive(true);
        SerchBarObj.SetActive(false);
        searchInputField.Text = "";
        HomeScoketHandler.instance.updateFeedLike += UpdateFeedLike;
        if (feedUIController == null)
        feedUIController = FeedUIController.Instance;
        if (!isFeedInitialized)
        {
           Invoke(nameof( IntFeedPage),0.01f);
        }
        else
        {
            PullNewPlayerPost();
        }
    }

    /// <summary>
    /// To Initialize the Feed Page with the data from API
    /// </summary>
    async void IntFeedPage()
    {
         feedUIController.feedUiScreen.SetActive(true);
         noFeedsScreen.gameObject.SetActive(false);
         feedContentParent.gameObject.SetActive(true);
        //FeedUIController.Instance.ShowLoader(true);
        FeedLoader.SetActive(true);
        scrollerController.IntFeedScroller();
        if (SNS_APIManager.Instance.userId == 0)
        {
           SNS_APIManager.Instance.userId=int.Parse(PlayerPrefs.GetString("UserName"));
        }
        await GetFeedData(SNS_APIManager.Instance.userId);
    }

    async Task GetFeedData(int userId)
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.FeedGetAllByUserId + userId + "/" + feedPageNumber + "/" + feedPageSize;
        UnityWebRequest response = UnityWebRequest.Get(url);
        try
        {
            await response.SendWebRequest();
            if (response.isNetworkError)
            {
                Debug.Log(response.error);
                FeedLoader.SetActive(false);
                noFeedText.text = "";
                noFeedsScreen.gameObject.SetActive(true);
            }
            else
            {
               
                print("~~~~~ "+ response.downloadHandler.text);
                noFeedsScreen.gameObject.SetActive(false);
                feedResponseData = JsonUtility.FromJson<FeedResponse>(response.downloadHandler.text.ToString());
                // FeedAPIData.Add(feedResponseData);
                if (feedResponseData.data.rows.Count>0)
                {
                     isFeedInitialized = true;
                    foreach (var item in feedResponseData.data.rows)
                    {
                        if (!String.IsNullOrEmpty(item.text_post) && !item.text_post.Equals("null"))
                        {
                            if (!FeedAPIData.Any(list1 => list1.id == item.id))
                            {
                                FeedAPIData.Add(item);
                                scrollerController._data.Add(item);
                            }
                        }

                    }
                    Invoke(nameof(InovkeScrollReload), 2f);
                }
                else
                {
                    FeedDataFromAPICountCheck();
                    FeedLoader.SetActive(false);
                    feedContentParent.gameObject.SetActive(false);
                }
               
            }

        }
        catch (System.Exception ex)
        {
            FeedLoader.SetActive(false);
            Debug.Log(ex.Message);
        }
        response.Dispose();
    }

    public void FeedDataFromAPICountCheck()
    {
        if (feedResponseData.data.rows.Count <= 0)
        {
            //noFeedText.text = "";
            noFeedsScreen.gameObject.SetActive(true);
        }
    }

    public void PullNewPlayerPost(){ 
        FeedLoader.SetActive(true);
        GetPlayerNewPosts(SNS_APIManager.Instance.userId);
    }

    async void GetPlayerNewPosts(int userId){ 
         string url = ConstantsGod.API_BASEURL + ConstantsGod.FeedGetAllByUserId + userId + "/" + 1 + "/" + FeedAPIData.Count;
         UnityWebRequest response = UnityWebRequest.Get(url);
        try
        {
            await response.SendWebRequest();
            if (response.isNetworkError)
            {
               
                Debug.Log(response.error);
            }
            else
            {
                noFeedsScreen.gameObject.SetActive(false);
                FeedResponse feedResponseData = JsonUtility.FromJson<FeedResponse>(response.downloadHandler.text.ToString());
                //FeedUIController.Instance.ShowLoader(false);
                List<FeedResponseRow> tempData = new List<FeedResponseRow>();
                bool _isNameChanged = false;
                foreach (var item1 in feedResponseData.data.rows)
                {
                    if (!String.IsNullOrEmpty(item1.text_post) && !item1.text_post.Equals("null"))
                    {
                        if (!FeedAPIData.Any(list1 => list1.id == item1.id)){ 
                            tempData.Add(item1);
                        }
                        var indexes = FeedAPIData.Select((number, index) => new { Number = number, Index = index })
                             .Where(item => (item.Number.user.id == item1.user_id && item.Number.user.name != item1.user.name))
                             .Select(item => item.Index)
                             .ToList();
                        if (indexes.Count > 0)
                        {
                            _isNameChanged = true;
                            foreach (var index in indexes)
                            {
                                FeedAPIData[index].user.name = item1.user.name;
                            }
                        }
                    }
                }
                if (tempData.Count>0){
                    FeedAPIData.InsertRange(0,tempData);
                    AddDataToTopScroller(FeedAPIData);
                }
                else{
                    if (_isNameChanged)
                    {
                        AddDataToTopScroller(FeedAPIData);
                        _isNameChanged = false;
                    }
                }
            }
            Invoke(nameof(turnoffLoaderForReload),1f);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
       
        response.Dispose();
    }

    void turnoffLoaderForReload(){ 
       FeedLoader.SetActive(false);
    }

    public void AddDataToTopScroller(List<FeedResponseRow> tempData)
    {
        for (int i = tempData.Count - 1; i >= 0; i--)
        {
             scrollerController._data.Insert(tempData[i],0);
        }
        gameObject.GetComponent<EnhancedScroller>().ReloadData();
        FeedLoader.SetActive(false);
    }

    /// <summary>
    /// To get next page player post
    /// </summary>
    public void GetPlayerNextPostPage(){ 
        GetFeedDataByPage(SNS_APIManager.Instance.userId);
    }

    async void GetFeedDataByPage(int userId)
    {
        feedPageNumber+=1;
        string url = ConstantsGod.API_BASEURL + ConstantsGod.FeedGetAllByUserId + userId + "/" + feedPageNumber + "/" + feedPageSize;
        UnityWebRequest response = UnityWebRequest.Get(url);
        try
        {
            await response.SendWebRequest();
            if (response.isNetworkError)
            {
                Debug.Log(response.error);
            }
            else
            {
                noFeedsScreen.gameObject.SetActive(false);
                FeedResponse feedResponseData = JsonUtility.FromJson<FeedResponse>(response.downloadHandler.text.ToString());
                 FeedLoader.SetActive(false);
                foreach (var item in feedResponseData.data.rows)
                {
                    if (!String.IsNullOrEmpty(item.text_post) && !item.text_post.Equals("null"))
                    {
                         if (!FeedAPIData.Any(list1 => list1.id == item.id)){ 
                            FeedAPIData.Add(item);
                            scrollerController._data.Add(item);
                        }
                    }
                }
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
        FeedLoader.SetActive(false);
        ParentHeightAdjuster.AdjustParentHeight();
    }
    /// <summary>
    /// To Update Feed Like Count from Socket
    /// </summary>
    /// <param name="feedId"></param>
    /// <param name="isLiked"></param>
    public void UpdateFeedLike(FeedLikeSocket feedLikeSocket)
    {
        for (int i = 0; i < scrollerController._data.Count; i++)
        {
            if (scrollerController._data[i].id == feedLikeSocket.textPostId)
            {
                scrollerController.updateLikeCount(feedLikeSocket.textPostId, feedLikeSocket.likeCount);
                //scrollerController.scroller.ReloadData();
                foreach (Transform item in feedContentParent.GetChild(0).transform )
                {
                    if (item.GetComponent<FeedData>() && item.GetComponent<FeedData>().GetFeedId() == feedLikeSocket.textPostId)
                    {
                         item.GetComponent<FeedData>().UpdateLikeCount(feedLikeSocket.likeCount);
                    }
                }
                break;
            }
        }
    }

    public void OnClickSerachBtn(){
        if (SerchBarObj.activeInHierarchy) // serach is active 
        {
            if (noResultinFeedSearch)
            {
                noFeedSerach.gameObject.SetActive(false);
                noResultinFeedSearch = false;
                FeedDataFromAPICountCheck();
            }
            feedContentParent.gameObject.SetActive(true);
            SerchBarObj.SetActive(false);
            SerachPanel.SetActive(false);
            SearchContentPanel.SetActive(false);
            EmptySearchPanel();
            searchInputField.Text = "";
            feedTabsContainer.sizeDelta = new Vector2(feedTabsContainer.rect.width, 80);
        }
        else// serach is not active 
        {
            SerchBarObj.SetActive(true);
            SerachPanel.SetActive(true);
            SearchContentPanel.SetActive(true);
            feedTabsContainer.sizeDelta = new Vector2(feedTabsContainer.rect.width, 60);
        }
    } 

    public void SearchFeed(){
        noFeedSerach. gameObject.SetActive(false);
        FeedLoader.SetActive(true);
        EmptySearchPanel();   
        if (searchInputField.Text.Length > 0)       
        {
            StartCoroutine(FeedSearch(searchInputField.Text));
        }
        else
        {
            FeedLoader.SetActive(false);
            feedContentParent.gameObject.SetActive(true);
            scrollerController.scroller.ReloadData();
        }
    }

    IEnumerator FeedSearch(string input)
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.FeedSearch +"/"+SNS_APIManager.Instance.userId +"/"+ input +"/1/20";
        UnityWebRequest response = UnityWebRequest.Get(url);
        yield return response.SendWebRequest();
        if (response.isNetworkError)
        {
            Debug.Log(response.error);
            noFeedSerach. gameObject.SetActive(true);
            noFeedsScreen.gameObject.SetActive(false);
            feedContentParent.gameObject.SetActive(true);
        }
        else
        {
            noFeedSerach. gameObject.SetActive(false);
            noFeedsScreen.gameObject.SetActive(false);
            SearchContentPanel.SetActive(true);
            SerachPanel.SetActive(true);

            // Clear Old Data
            EmptySearchPanel();

            feedContentParent.gameObject.SetActive(false);
            print("~~~~~~~~~ FEED Search "+response.downloadHandler.text);
            FeedResponse feedResponseData = JsonUtility.FromJson<FeedResponse>(response.downloadHandler.text.ToString());
            if (feedResponseData!=null && feedResponseData.data.rows.Count>0)
            {
                foreach (var item in feedResponseData.data.rows)
                {
                    if (!String.IsNullOrEmpty( item.text_post) && !item.text_post.Equals("null") )
                    {
                        GameObject temp = Instantiate(feedPostPrefab);
                        temp.transform.SetParent(SearchContentPanel.transform);
                        temp.transform.localScale = Vector3.one;
                        temp.GetComponent<FeedData>().SetFeedPrefab(item,false);
                        temp.GetComponent<FeedData>().isProfileScene = true;
                        temp.GetComponent<FeedData>().SetFeedUiController(scrollerController);
                    }
                }
                GameObject emptySerach = Instantiate(emptyPrefab);
                emptySerach.transform.SetParent(SearchContentPanel.transform);
                emptySerach.transform.localScale = Vector3.one;
            }
            else
            {
                if (GameManager.currentLanguage == "en" && !LocalizationManager.forceJapanese) // for English 
                {
                    noFeedText.text = "We couldn’t find a match for “ "+
                                       SerchStringToEllipsis( input)
                                        +"”.\r\nPlease try another search.";
                }
                else if(GameManager.currentLanguage == "ja" || LocalizationManager.forceJapanese)   // for Jp 
                {
                    noFeedText.text = SerchStringToEllipsis( input) + "に一致するものが見つかりませんでした。\r\n" +
                                        "別のキーワードで試してみてください。";
                                        
                }
                noFeedSerach. gameObject.SetActive(true);
                noResultinFeedSearch = true;
            }
             FeedLoader.SetActive(false);
        }
        response.Dispose();
    }


    string SerchStringToEllipsis(string input)
    {
        if (input.Length > 5)
        {
            input = input.Substring(0, 5);
            input = input + "...";
        }
        return input;
    }

    void EmptySearchPanel(){
        foreach (Transform item in SearchContentPanel.transform)
        {
            Destroy(item.gameObject);
        }
    }


    public void BackToHome(){
        if (SerchBarObj.activeInHierarchy) // serach is active 
        {
            SerchBarObj.SetActive(false);
            SerachPanel.SetActive(false);
            SearchContentPanel.SetActive(false);
            EmptySearchPanel();
            feedContentParent.gameObject.SetActive(true);
            searchInputField.Text = "";
            feedTabsContainer.sizeDelta = new Vector2(feedTabsContainer.rect.width, 80);
            if (!isFeedInitialized)
            {
                Invoke(nameof(IntFeedPage), 0.01f);
            }
            else
            {
                PullNewPlayerPost();
            }
        }
        else
        {
            EmptySearchPanel();
            noFeedSerach.gameObject.SetActive(false);
            noFeedsScreen.gameObject.SetActive(false);
            FeedLoader.gameObject.SetActive(false);
            feedUIController.footerCan.GetComponent<HomeFooterHandler>().OnClickHomeButton();
        }
     }
    private void OnDisable()
    {
        HomeScoketHandler.instance.updateFeedLike -= UpdateFeedLike;
        ResetFeedController();
    }

    /// <summary>
    /// To reset the feed controller on signout
    /// </summary>
    public void ResetFeedController(){ 
        SerachPanel.SetActive(false);
        feedContentParent.gameObject.SetActive(true);
        SerchBarObj.SetActive(false);
        searchInputField.Text = "";
        isFeedInitialized = false;
        FeedAPIData.Clear();
        if (scrollerController._data !=null && scrollerController._data.Count>0)
        {
            scrollerController._data.Clear();
        }
        if (scrollerController.feedHeight != null && scrollerController.feedHeight.Count>0)
        {
            scrollerController.feedHeight.Clear();
        }
        scrollerController.scroller.ClearAll();
//        scrollerController.scroller.ReloadData();
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
    public int count;
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

