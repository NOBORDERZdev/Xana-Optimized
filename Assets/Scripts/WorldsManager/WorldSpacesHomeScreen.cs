using SuperStar.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WorldSpacesHomeScreen : MonoBehaviour
{
    public Sprite defaultThumbnail;
    public GameObject userTagPrefab;
    public GameObject hotSpacesParent, hotGamesParent, followingParent, mySpaceParent, userTagParent, category1Parent, category2Parent, category3Parent, category4Parent;
    public GameObject hotSpacesContent;
    public GameObject hotGamesContent;
    public GameObject followingContent;
    public GameObject mySpaceContent;
    public GameObject userTagContent;
    public GameObject category1;
    public GameObject category2;
    public GameObject category3;
    public GameObject category4;
    public TMPro.TextMeshProUGUI category1Heading, category2Heading, category3Heading, category4Heading;

    /*private int apiHitCountC1 = 0, apiHitCountC2 = 0, apiHitCountC3 = 0, apiHitCountC4 = 0;*/

    public WorldManager worldManager;
    public ResponseHolder apiResponseHolder;
    public static List<string> mostVisitedTagList = new List<string>();
    private void OnEnable()
    {
        WorldManager.LoadHomeScreenWorlds += StartLoading;
    }

    private void OnDisable()
    {
        WorldManager.LoadHomeScreenWorlds -= StartLoading;
    }

    void StartLoading()
    {
        HotSpaceLoading();
        HotGamesLoading();
        FollowingSpaceLoading();
        MySpaceLoading();
        GetAllTags();
        GetUsersMostVisitedTags(() =>
        {
            Category1Loading();
            Category2Loading();
            Category3Loading();
            Category4Loading();
        });
    }


    void HotSpaceLoading()
    {
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.HotSpaces, 10);
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                WorldsInfo worldInfo = JsonUtility.FromJson<WorldsInfo>(response);
                if (worldInfo.data.rows.Count == 0)
                {
                    hotSpacesParent.SetActive(false);
                    FlexibleRect.OnAdjustSize?.Invoke(false);
                }
                StartCoroutine(SetContentItem(hotSpacesContent, worldInfo));
            }

        }));
    }

    void HotGamesLoading()
    {
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.HotGames, 10);
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                WorldsInfo worldInfo = JsonUtility.FromJson<WorldsInfo>(response);
                if (worldInfo.data.rows.Count == 0)
                {
                    hotGamesParent.SetActive(false);
                    FlexibleRect.OnAdjustSize?.Invoke(false);
                }
                StartCoroutine(SetContentItem(hotGamesContent, worldInfo));
            }
        }));
    }

    void FollowingSpaceLoading()
    {
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.FolloingSpace, 10);
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                WorldsInfo worldInfo = JsonUtility.FromJson<WorldsInfo>(response);
                if (worldInfo.data.rows.Count == 0)
                {
                    followingParent.SetActive(false);
                    FlexibleRect.OnAdjustSize?.Invoke(false);
                }
                StartCoroutine(SetContentItem(followingContent, worldInfo));
            }
        }));
    }

    void MySpaceLoading()
    {
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.MySpace, 10);
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                WorldsInfo worldInfo = JsonUtility.FromJson<WorldsInfo>(response);
                if (worldInfo.data.rows.Count == 0)
                {
                    mySpaceParent.SetActive(false);
                    FlexibleRect.OnAdjustSize?.Invoke(false);
                }
                StartCoroutine(SetContentItem(mySpaceContent, worldInfo));
            }
        }));
    }


    void GetAllTags()
    {
        string finalAPIURL = ConstantsGod.API_BASEURL + ConstantsGod.USERTAGS;
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                UserTagInfo userTagInfo = JsonUtility.FromJson<UserTagInfo>(response);
                if (userTagInfo.data.rows.Count == 0)
                {
                    userTagParent.SetActive(false);
                    FlexibleRect.OnAdjustSize?.Invoke(false);
                }
                StartCoroutine(InstantiateUsersTag(userTagContent, userTagInfo));
            }
        }));
    }

    void GetUsersMostVisitedTags(Action CallBack)
    {
        string finalAPIURL = ConstantsGod.API_BASEURL + ConstantsGod.MOSTVISITEDTAG + "1/10";
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                MostVisitedTag userTagInfo = JsonUtility.FromJson<MostVisitedTag>(response);
                if (userTagInfo.data.Count == 0)
                {
                    userTagParent.SetActive(false);
                    FlexibleRect.OnAdjustSize?.Invoke(false);
                }
                else
                {
                    for (int i = 0; i < userTagInfo.data.Count; i++)
                    {
                        if (mostVisitedTagList.Count < 5)
                            mostVisitedTagList.Add(userTagInfo.data[i].tagName);
                        else
                            break;
                    }
                    CallBack();
                }
            }
        }));
    }

    void Category1Loading()
    {
        //apiHitCountC1++;
        worldManager.SearchKey = mostVisitedTagList[0];
        category1Heading.text = mostVisitedTagList[0];
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.SearchWorldByTag, 10);
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                WorldsInfo worldInfo = JsonUtility.FromJson<WorldsInfo>(response);
                if (worldInfo.data.rows.Count == 0)
                {
                    category1Parent.SetActive(false);
                    FlexibleRect.OnAdjustSize?.Invoke(false);
                    /*if (apiHitCountC1 < 5)
                        Invoke("Category1Loading", 1);*/
                }
                /*else
                    apiResponseHolder.AddReponse(finalAPIURL, response);*/
                StartCoroutine(SetContentItem(category1, worldInfo));
            }
           /* else
            {
                if (apiHitCountC1 < 5)
                    Invoke("Category1Loading", 1);
            }*/
        }));
    }

    void Category2Loading()
    {
        //apiHitCountC2++;
        worldManager.SearchKey = mostVisitedTagList[1];
        category2Heading.text = mostVisitedTagList[1];
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.SearchWorldByTag, 10);
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                WorldsInfo worldInfo = JsonUtility.FromJson<WorldsInfo>(response);
                if (worldInfo.data.rows.Count == 0)
                {
                    category2Parent.SetActive(false);
                    FlexibleRect.OnAdjustSize?.Invoke(false);
                   /* if (apiHitCountC2 < 5)
                        Invoke("Category2Loading", 1);*/
                }
                /*else
                    apiResponseHolder.AddReponse(finalAPIURL, response);*/
                StartCoroutine(SetContentItem(category2, worldInfo));
            }
            /*else
            {
                if (apiHitCountC2 < 5)
                    Invoke("Category2Loading", 1);
            }*/
        }));
    }

    void Category3Loading()
    {
        //apiHitCountC3++;
        worldManager.SearchKey = mostVisitedTagList[2];
        category3Heading.text = mostVisitedTagList[2];
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.SearchWorldByTag, 10);
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                WorldsInfo worldInfo = JsonUtility.FromJson<WorldsInfo>(response);
                if (worldInfo.data.rows.Count == 0)
                {
                    category3Parent.SetActive(false);
                    FlexibleRect.OnAdjustSize?.Invoke(false);
                   /* if (apiHitCountC3 < 5)
                        Invoke("Category3Loading", 1);*/
                }
                /*else
                    apiResponseHolder.AddReponse(finalAPIURL, response);*/
                StartCoroutine(SetContentItem(category3, worldInfo));
            }
            /*else
            {
                if (apiHitCountC3 < 5)
                    Invoke("Category3Loading",1);
            }*/
        }));
    }

    void Category4Loading()
    {
        //apiHitCountC4++;
        worldManager.SearchKey = mostVisitedTagList[3];
        category4Heading.text = mostVisitedTagList[3];
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.SearchWorldByTag, 10);
        StartCoroutine(GetDataFromAPI(finalAPIURL, (isSucess, response) =>
        {
            if (isSucess)
            {
                WorldsInfo worldInfo = JsonUtility.FromJson<WorldsInfo>(response);
                if (worldInfo.data.rows.Count == 0)
                {
                    category4Parent.SetActive(false);
                    FlexibleRect.OnAdjustSize?.Invoke(false);
                    /*if (apiHitCountC4 < 5)
                        Invoke("Category4Loading", 1);*/
                }
                /*else
                    apiResponseHolder.AddReponse(finalAPIURL, response);*/
                StartCoroutine(SetContentItem(category4, worldInfo));
            }
            /*else
            {
                if (apiHitCountC4 < 5)
                    Invoke("Category4Loading", 1);
            }*/
        }));
    }

    IEnumerator SetContentItem(GameObject spaceContent, WorldsInfo _WorldInfo)
    {
        for (int i = 0; i < _WorldInfo.data.rows.Count; i++)
        {
            if (i > (spaceContent.transform.childCount - 1))
                yield break;
            WorldItemDetail _event;
            _event = new WorldItemDetail();
            _event.IdOfWorld = _WorldInfo.data.rows[i].id;
            _event.EnvironmentName = _WorldInfo.data.rows[i].name;
            try
            {
                if (_WorldInfo.data.rows[i].entityType != null)
                {
                    string IThumbnailDownloadURL = "";
                    //Modify Path for Thumbnail
                    if (!string.IsNullOrEmpty(_WorldInfo.data.rows[i].banner_new))
                    {
                        IThumbnailDownloadURL = _WorldInfo.data.rows[i].banner_new;
                        IThumbnailDownloadURL = _WorldInfo.data.rows[i].banner_new.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");
                        // Test-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://cdn.xana.net/apitestxana/Defaults", "https://aydvewoyxq.cloudimg.io/_apitestxana_/apitestxana/Defaults");
                        // Main-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://ik.imagekit.io/xanalia/xanaprod/Defaults", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod/Defaults");
                        _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                    }
                    else
                    {
                        IThumbnailDownloadURL = _WorldInfo.data.rows[i].thumbnail.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");
                        // Test-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://cdn.xana.net/apitestxana/Defaults", "https://aydvewoyxq.cloudimg.io/_apitestxana_/apitestxana/Defaults");
                        // Main-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://ik.imagekit.io/xanalia/xanaprod/Defaults", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod/Defaults");
                        _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                    }
                }
            }
            catch
            {
                Debug.LogError("Check Exception world thumbnail Image");
                _event.ThumbnailDownloadURL = _WorldInfo.data.rows[i].thumbnail;
            }
            _event.BannerLink = _WorldInfo.data.rows[i].banner;
            _event.WorldDescription = _WorldInfo.data.rows[i].description;
            _event.EntityType = _WorldInfo.data.rows[i].entityType;
            _event.PressedIndex = int.Parse(_WorldInfo.data.rows[i].id);
            _event.UpdatedAt = _WorldInfo.data.rows[i].updatedAt;
            _event.CreatedAt = _WorldInfo.data.rows[i].createdAt;
            _event.WorldVisitCount = _WorldInfo.data.rows[i].totalVisits;
            _event.isFavourite = _WorldInfo.data.rows[i].isFavourite;
            if (_WorldInfo.data.rows[i].tags != null)
                _event.WorldTags = _WorldInfo.data.rows[i].tags;

            //if (_WorldInfo.data.rows[i].creatorDetails != null)
            //{
            //    _event.Creator_Name = _WorldInfo.data.rows[i].creatorDetails.userName;
            //    _event.CreatorDescription = _WorldInfo.data.rows[i].creatorDetails.description;
            //    _event.CreatorAvatarURL = _WorldInfo.data.rows[i].creatorDetails.avatar;
            //}
            if (_WorldInfo.data.rows[i].user.userProfile != null)
            {
                if (!string.IsNullOrEmpty(_WorldInfo.data.rows[i].user.userProfile.bio))
                    _event.CreatorDescription = _WorldInfo.data.rows[i].user.userProfile.bio;

                _event.CreatorDescription = _WorldInfo.data.rows[i].user.userProfile.bio;

                if (_WorldInfo.data.rows[i].entityType == WorldType.USER_WORLD.ToString())
                {
                    _event.Creator_Name = _WorldInfo.data.rows[i].user.name;
                    _event.CreatorDescription = _WorldInfo.data.rows[i].creatorDetails.description;
                    _event.UserAvatarURL = _WorldInfo.data.rows[i].user.avatar;
                    _event.UserLimit = "15";
                }
                else
                {
                    if (!string.IsNullOrEmpty(_WorldInfo.data.rows[i].user.name))
                        _event.Creator_Name = _WorldInfo.data.rows[i].user.name;
                    else
                        _event.Creator_Name = "XANA";

                    if (!string.IsNullOrEmpty(_WorldInfo.data.rows[i].creator))
                        _event.Creator_Name = _WorldInfo.data.rows[i].creator;

                }
            }
            _event.UserLimit = _WorldInfo.data.rows[i].user_limit;
            spaceContent.transform.GetChild(i).gameObject.SetActive(true);
            spaceContent.transform.GetChild(i).GetComponent<WorldItemView>().InitItem(_event);
        }
    }

    IEnumerator InstantiateUsersTag(GameObject userTagParent, UserTagInfo userTagInfo)
    {
        for (int i = 0; i < userTagInfo.data.rows.Count; i++)
        {
            GameObject userTag = Instantiate(userTagPrefab, userTagParent.transform);
            userTag.GetComponent<TagPrefabInfo>().tagName.text = userTagInfo.data.rows[i].tagName;
            userTag.SetActive(true);
            if (mostVisitedTagList.Count < 5)
                mostVisitedTagList.Add(userTagInfo.data.rows[i].tagName);
            yield return new WaitForEndOfFrame();
        }
    }


    IEnumerator GetDataFromAPI(string apiURL, Action<bool, string> callback)
    {
        yield return new WaitForEndOfFrame();

        if (apiResponseHolder.CheckResponse(apiURL))
        {
            callback(true, apiResponseHolder.GetResponse(apiURL));
            yield break;
        }
        // Debug.LogError("API URL :- " + apiURL);
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                callback(false, null);
            }
            else
            {
                //_WorldInfo = JsonUtility.FromJson<WorldsInfo>(www.downloadHandler.text);
                //worldstr = www.downloadHandler.text;
                apiResponseHolder.AddReponse(apiURL, www.downloadHandler.text);
                callback(true, www.downloadHandler.text);
            }
            Debug.LogError(apiURL + "---" + www.downloadHandler.text);
            www.Dispose();
        }
    }

    public void RemoveThumbnailImages()
    {
        for (int i = 0; i < hotSpacesContent.transform.childCount; i++)
        {
            AssetCache.Instance.RemoveFromMemoryDelayCoroutine(hotSpacesContent.transform.GetChild(i).GetComponent<WorldItemView>().m_ThumbnailDownloadURL, true);
            //Destroy(hotSpacesContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite);
            hotSpacesContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = null;
            hotSpacesContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = defaultThumbnail;
        }

        for (int i = 0; i < hotGamesContent.transform.childCount; i++)
        {
            AssetCache.Instance.RemoveFromMemoryDelayCoroutine(hotGamesContent.transform.GetChild(i).GetComponent<WorldItemView>().m_ThumbnailDownloadURL, true);
            //Destroy(hotGamesContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite);
            hotGamesContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = null;
            hotGamesContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = defaultThumbnail;
        }

        for (int i = 0; i < followingContent.transform.childCount; i++)
        {
            AssetCache.Instance.RemoveFromMemoryDelayCoroutine(followingContent.transform.GetChild(i).GetComponent<WorldItemView>().m_ThumbnailDownloadURL, true);
            //Destroy(followingContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite);
            followingContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = null;
            followingContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = defaultThumbnail;
        }

        for (int i = 0; i < mySpaceContent.transform.childCount; i++)
        {
            AssetCache.Instance.RemoveFromMemoryDelayCoroutine(mySpaceContent.transform.GetChild(i).GetComponent<WorldItemView>().m_ThumbnailDownloadURL, true);
            //Destroy(mySpaceContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite);
            mySpaceContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = null;
            mySpaceContent.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = defaultThumbnail;
        }

        for (int i = 0; i < category1.transform.childCount; i++)
        {
            AssetCache.Instance.RemoveFromMemoryDelayCoroutine(category1.transform.GetChild(i).GetComponent<WorldItemView>().m_ThumbnailDownloadURL, true);
            //Destroy(category1.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite);
            category1.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = null;
            category1.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = defaultThumbnail;
        }

        for (int i = 0; i < category2.transform.childCount; i++)
        {
            AssetCache.Instance.RemoveFromMemoryDelayCoroutine(category2.transform.GetChild(i).GetComponent<WorldItemView>().m_ThumbnailDownloadURL, true);
            //Destroy(category2.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite);
            category2.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = null;
            category2.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = defaultThumbnail;
        }

        for (int i = 0; i < category3.transform.childCount; i++)
        {
            AssetCache.Instance.RemoveFromMemoryDelayCoroutine(category3.transform.GetChild(i).GetComponent<WorldItemView>().m_ThumbnailDownloadURL, true);
            //Destroy(category3.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite);
            category3.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = null;
            category3.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = defaultThumbnail;
        }

        for (int i = 0; i < category4.transform.childCount; i++)
        {
            AssetCache.Instance.RemoveFromMemoryDelayCoroutine(category4.transform.GetChild(i).GetComponent<WorldItemView>().m_ThumbnailDownloadURL, true);
            //Destroy(category4.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite);
            category4.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = null;
            category4.transform.GetChild(i).GetComponent<WorldItemView>().worldIcon.sprite = defaultThumbnail;
        }

        GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    [Serializable]
    public class Tag
    {
        public int id;
        public string tagName;
        public DateTime createdAt;
        public DateTime updatedAt;
    }
    [Serializable]
    public class Data
    {
        public int count;
        public List<Tag> rows;
    }
    [Serializable]
    public class UserTagInfo
    {
        public bool success;
        public Data data;
        public string msg;
    }

    [Serializable]
    public class MostVisitedTag
    {
        public bool success;
        public List<TagData> data;
        public string msg;
    }
    [Serializable]
    public class TagData
    {
        public string tagName;
        public int tagVisits;
    }
}

