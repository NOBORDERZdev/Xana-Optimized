using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.Networking;

public class WorldSpacesHomeScreen : MonoBehaviour
{
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
    public WorldManager worldManager;
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
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.Hot, 10);
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
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.GameWorld, 10);
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
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.AllWorld, 10);
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
        string finalAPIURL = worldManager.PrepareApiURL(APIURL.MyWorld, 10);
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


    void GetUsersMostVisitedTags(Action CallBack)
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
                else
                {
                    for (int i = 0; i < userTagInfo.data.rows.Count; i++)
                    {   
                        if (mostVisitedTagList.Count < 5)
                            mostVisitedTagList.Add(userTagInfo.data.rows[i].tagName);
                        else
                            break;
                    }
                    CallBack();
                }
                StartCoroutine(InstantiateUsersTag(userTagContent, userTagInfo));
            }
        }));
    }

    void Category1Loading()
    {
        WorldManager.instance.SearchKey = mostVisitedTagList[0];
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
                }
                StartCoroutine(SetContentItem(category1, worldInfo));
            }
        }));
    }

    void Category2Loading()
    {
        WorldManager.instance.SearchKey = mostVisitedTagList[1];
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
                }
                StartCoroutine(SetContentItem(category2, worldInfo));
            }
        }));
    }

    void Category3Loading()
    {
        WorldManager.instance.SearchKey = mostVisitedTagList[2];
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
                }
                StartCoroutine(SetContentItem(category3, worldInfo));
            }
        }));
    }

    void Category4Loading()
    {
        WorldManager.instance.SearchKey = mostVisitedTagList[3];
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
                }
                StartCoroutine(SetContentItem(category4, worldInfo));
            }
        }));
    }

    IEnumerator SetContentItem(GameObject spaceContent, WorldsInfo _WorldInfo)
    {
        for (int i = 0; i < _WorldInfo.data.rows.Count; i++)
        {
            if (i > (spaceContent.transform.childCount-1))
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

                        //_event.ThumbnailDownloadURL = IThumbnailDownloadURL;

                        if (!_event.EnvironmentName.Contains("XANA Lobby"))
                        {
                            _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                            //_event.ThumbnailDownloadURLHigh = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                        }
                        else
                        {
                            _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                        }

                    }
                    else
                    {
                        IThumbnailDownloadURL = _WorldInfo.data.rows[i].thumbnail.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");
                        // Test-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://cdn.xana.net/apitestxana/Defaults", "https://aydvewoyxq.cloudimg.io/_apitestxana_/apitestxana/Defaults");
                        // Main-net
                        IThumbnailDownloadURL = IThumbnailDownloadURL.Replace("https://ik.imagekit.io/xanalia/xanaprod/Defaults", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod/Defaults");

                        //_event.ThumbnailDownloadURL = IThumbnailDownloadURL;

                        if (!_event.EnvironmentName.Contains("XANA Lobby"))
                        {
                            _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                            //_event.ThumbnailDownloadURLHigh = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360;
                        }
                        else
                        {
                            _event.ThumbnailDownloadURL = IThumbnailDownloadURL + "?width=" + 640 + "&height=" + 360; ;
                        }
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
            if (_WorldInfo.data.rows[i].tags != null)
                _event.WorldTags = _WorldInfo.data.rows[i].tags;

            if (_WorldInfo.data.rows[i].creatorDetails != null)
            {
                _event.Creator_Name = _WorldInfo.data.rows[i].creatorDetails.userName;
                _event.CreatorDescription = _WorldInfo.data.rows[i].creatorDetails.description;
                _event.CreatorAvatarURL = _WorldInfo.data.rows[i].creatorDetails.avatar;
            }

            if (_WorldInfo.data.rows[i].entityType == WorldType.USER_WORLD.ToString())
            {
                _event.CreatorName = _WorldInfo.data.rows[i].user.name;
                _event.UserAvatarURL = _WorldInfo.data.rows[i].user.avatar;
                _event.UserLimit = "15";
            }
            else
            {
                if (!string.IsNullOrEmpty(_WorldInfo.data.rows[i].creator))
                    _event.CreatorName = _WorldInfo.data.rows[i].creator;
                else
                    _event.CreatorName = "XANA";
                _event.UserLimit = _WorldInfo.data.rows[i].user_limit;
            }
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
                callback(true, www.downloadHandler.text);
            }
            www.Dispose();
        }
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
}

