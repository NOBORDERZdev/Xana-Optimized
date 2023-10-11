using AdvancedInputFieldPlugin;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WorldSearchManager : MonoBehaviour
{
    [Header("search World Component")]
    public GameObject eventPrefab;
    public GameObject eventPrefabTab;
    public float verticalNormalizedPosition;
    public bool dataLoaded;
    public int pageNumb = 1;
    public int pageSize = 15;
    public SearchworldRoot searchworldRoot = new SearchworldRoot();
   // public SearchworldRoot tempSearchworldRoot = new SearchworldRoot();
    public Transform SearchWorldParent;
    public ScrollRect scrollcontroller;
    public AdvancedInputField searchWorldInput;
    public GameObject FindWorldScreen;
   // public static Action<string> OpenSearchPanel;
    [SerializeField] GameObject XanaLobbySearchPrefab;
   // bool isXanaLobbyFound = false;

    private void OnEnable()
    {
        if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
            eventPrefab = eventPrefabTab;
       // OpenSearchPanel += OpenSearchPanelFromTag;
    }

    //private void OnDisable()
    //{
    //   // OpenSearchPanel -= OpenSearchPanelFromTag;
    //}
    public void OnClickBackButton()
    {
        FindWorldScreen.SetActive(false);
        XanaLobbySearchPrefab.SetActive(false);
       // SearchWorldParent.GetComponent<GridLayoutGroup>().padding.top=12;
        foreach (Transform world in SearchWorldParent)
        {
            Destroy(world.gameObject);
        }
    }

    public void GetSearchBarStatus()
    {
        Debug.Log("GetSearchBarStatus => " + PremiumUsersDetails.Instance.CheckSpecificItem("WorldSearchFeature"));
        FindWorldScreen.SetActive(true);
        searchWorldInput.Select();
        searchWorldInput.Clear();
  
        //if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
        //{
        //    UIManager.Instance.FindScetion.constraintCount = 4;
        //    UIManager.Instance.FindScetion.padding.left = 12;
        //    UIManager.Instance.FindScetion.padding.right = 12;
        //    UIManager.Instance.FindScetion.cellSize = new Vector2(320, 320);
        //}
    }

    //void OpenSearchPanelFromTag(string tagName)
    //{
    //    FindWorldScreen.SetActive(true);
    //    searchWorldInput.Clear();
    //    searchWorldInput.Text = tagName;
    //    searchWorldInput.ManualDeselect();
    //    searchWorldInput.ReadOnly=true;
    //    if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
    //    {
    //        UIManager.Instance.FindScetion.constraintCount = 4;
    //        UIManager.Instance.FindScetion.padding.left = 12;
    //        UIManager.Instance.FindScetion.padding.right = 12;
    //        UIManager.Instance.FindScetion.cellSize = new Vector2(320, 320);
    //    }

    //    SearchWorld(true);
    //}

    string PrePareURL(bool isFromTag,string searchKey)
    {
        Debug.LogError("Tagggggg  " + isFromTag);
        if(isFromTag)
        {
            return ConstantsGod.API_BASEURL + ConstantsGod.SEARCHWORLDBYTAG + "/" + searchKey + "/" + pageNumb + "/" + pageSize;
        }
        else
        {
            return ConstantsGod.API_BASEURL + ConstantsGod.SearchWorldAPI + "/" + searchKey + "/" + pageNumb + "/" + pageSize;
        }
    }

    public string searchdata;
    public IEnumerator IESearchWorld(bool isFromTag = false)
    {
        string searchWorldStr = searchWorldInput.Text;
        if (!string.IsNullOrEmpty(searchWorldStr))
        {
            string ApiURL = PrePareURL(isFromTag,searchWorldStr);
            using (UnityWebRequest www = UnityWebRequest.Get(ApiURL))
            {
                www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    Debug.Log("Error: " + www.error);
                }
                else
                {
                    searchdata = www.downloadHandler.text;
                    Debug.Log("searchdata :" + searchdata);

                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    searchworldRoot = JsonConvert.DeserializeObject<SearchworldRoot>(searchdata, settings);
                    if (searchworldRoot != null && searchworldRoot.data.rows.Count > 0)
                    {
                        for (int i = 0; i < searchworldRoot.data.rows.Count; i++)
                        {
                            GameObject TempObject;
                            if (searchworldRoot.data.rows[i].name.Contains("XANA Lobby"))
                            {
                                //isXanaLobbyFound= true;
                                print("IN IF ");
                                XanaLobbySearchPrefab.SetActive(true);
                                SearchWorldParent.GetComponent<GridLayoutGroup>().padding.top = 310;
                                TempObject = XanaLobbySearchPrefab;
                            }
                            else
                            {
                                TempObject = Instantiate(eventPrefab);
                                TempObject.transform.SetParent(SearchWorldParent);
                                TempObject.transform.localScale = Vector3.one;
                            }
                           
                            
                            FeedEventPrefab _event = TempObject.GetComponent<FeedEventPrefab>();
                            _event.idOfObject = searchworldRoot.data.rows[i].id.ToString();
                            _event.m_EnvironmentName = searchworldRoot.data.rows[i].name;
                            try
                            {
                                _event.m_ThumbnailDownloadURL = searchworldRoot.data.rows[i].thumbnail.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");

                                if (searchworldRoot.data.rows[i].entityType == WorldType.USER_WORLD.ToString())
                                {
                                    _event.m_ThumbnailDownloadURL = _event.m_ThumbnailDownloadURL + "?width=" + 512 + "&height=" + 512;
                                }
                            }
                            catch (Exception e)
                            {
                                _event.m_ThumbnailDownloadURL = searchworldRoot.data.rows[i].thumbnail;
                            }
                            _event.m_BannerLink = searchworldRoot.data.rows[i].banner;
                            _event.m_WorldDescription = searchworldRoot.data.rows[i].description;
                            _event.entityType = searchworldRoot.data.rows[i].entityType;
                            _event.m_PressedIndex = searchworldRoot.data.rows[i].id;
                            _event.updatedAt = searchworldRoot.data.rows[i].updatedAt.ToString();
                            _event.createdAt = searchworldRoot.data.rows[i].createdAt.ToString();

                            if (searchworldRoot.data.rows[i].tags != null)
                                _event.worldTags = searchworldRoot.data.rows[i].tags;

                            if (searchworldRoot.data.rows[i].entityType == WorldType.USER_WORLD.ToString())
                            {
                                _event.creatorName = searchworldRoot.data.rows[i].user.name;
                                _event.userAvatarURL = searchworldRoot.data.rows[i].user.avatar;
                                _event.userLimit = "10";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(searchworldRoot.data.rows[i].creator))
                                {
                                    _event.creatorName = searchworldRoot.data.rows[i].creator;
                                }
                                else
                                {
                                     _event.creatorName = "XANA";
                                }
                               
                                _event.userLimit = searchworldRoot.data.rows[i].user_limit.ToString();
                            }
                            _event.Init();
                            StartCoroutine(  _event.DownloadAndLoadFeed());
                            _event.DownloadPrefabSprite();
                        }
                        //yield return new WaitForSeconds(.5f);
                    }
                }
                dataLoaded = true;
                SearchWorldScrollCoroutine = searchworldCoroutine = null;
                if (searchWorldSTR != searchWorldInput.Text)
                {
                    SearchWorld();
                }
            }
        }
    }
    //public void OnScrollSearchWorlds()
    //{
    //    if (scrollcontroller.verticalNormalizedPosition < 0.01f && dataLoaded && !searchWorldInput.Selected)
    //    {
    //        if (SearchWorldScrollCoroutine == null)
    //        {
    //            SearchWorldScrollCoroutine = StartCoroutine(IEOnScrollSearchWorlds());
    //        }
    //    }
    //}
    public Coroutine SearchWorldScrollCoroutine;
    public IEnumerator IEOnScrollSearchWorlds()
    {
        dataLoaded = false;
        yield return new WaitForSeconds(.1f);
        pageNumb++;
        StartCoroutine(IESearchWorld());
    }
    public Coroutine searchworldCoroutine;
    public string searchWorldSTR;
    public void SearchWorld(bool isFromTag=false)
    {
        foreach (Transform world in SearchWorldParent)
        {
            Destroy(world.gameObject);
        }
        XanaLobbySearchPrefab.SetActive(false);
        SearchWorldParent.GetComponent<GridLayoutGroup>().padding.top=12;
        pageNumb = 1;
        pageSize = 15;
        if (searchworldCoroutine == null)
        {
            searchWorldSTR = searchWorldInput.Text;
            searchworldCoroutine = StartCoroutine(IESearchWorld(isFromTag));
        }
    }
}
