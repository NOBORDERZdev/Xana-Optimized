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
    public SearchworldRoot tempSearchworldRoot = new SearchworldRoot();
    public Transform SearchWorldParent;
    public ScrollRect scrollcontroller;
    public AdvancedInputField searchWorldInput;
    public GameObject FindWorldScreen;

    public static WorldSearchManager Instance;
    [SerializeField] GameObject XanaLobbySearchPrefab;
    bool isXanaLobbyFound = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
            eventPrefab = eventPrefabTab;
    }
    public void OnClickBackButton()
    {
        FindWorldScreen.SetActive(false);
        XanaLobbySearchPrefab.SetActive(false);
        SearchWorldParent.GetComponent<GridLayoutGroup>().padding.top=12;
        foreach (Transform world in SearchWorldParent)
        {
            Destroy(world.gameObject);
        }
    }
    /*private void Update()
    {
        verticalNormalizedPosition = scrollcontroller.verticalNormalizedPosition;
    }*/


    public void GetSearchBarStatus()
    {
        Debug.Log("GetSearchBarStatus => " + PremiumUsersDetails.Instance.CheckSpecificItem("WorldSearchFeature"));
        //if (!PremiumUsersDetails.Instance.CheckSpecificItem("WorldSearchFeature"))
        //{
        //    return;

        //}
        //else
        //{
        FindWorldScreen.SetActive(true);
        searchWorldInput.Select();
        searchWorldInput.Clear();
        //searchButton.gameObject.SetActive(false);
        //searchBar.readOnly = false;
        //searchBar.Select();
        if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
        {
            UIManager.Instance.FindScetion.constraintCount = 4;
            UIManager.Instance.FindScetion.padding.left = 12;
            UIManager.Instance.FindScetion.padding.right = 12;
            UIManager.Instance.FindScetion.cellSize = new Vector2(320, 320);
        }
        print("Horayyy you have Access");
        // }
    }

    public string searchdata;
    public IEnumerator IESearchWorld()
    {
        string searchWorldStr = searchWorldInput.Text;
        if (!string.IsNullOrEmpty(searchWorldStr))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(ConstantsGod.API_BASEURL + ConstantsGod.SearchWorldAPI + "/" + searchWorldStr + "/" + pageNumb + "/" + pageSize))
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
                                isXanaLobbyFound= true;
                                print("IN IF ");
                                XanaLobbySearchPrefab.SetActive(true);
                                SearchWorldParent.GetComponent<GridLayoutGroup>().padding.top=310;
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
                            _event.DownloadAndLoadFeed();
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
    public void OnScrollSearchWorlds()
    {
        if (scrollcontroller.verticalNormalizedPosition < 0.01f && dataLoaded && !searchWorldInput.Selected)
        {
            if (SearchWorldScrollCoroutine == null)
            {
                SearchWorldScrollCoroutine = StartCoroutine(IEOnScrollSearchWorlds());
            }
        }
    }
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
    public void SearchWorld()
    {
        foreach (Transform world in SearchWorldParent)
        {
            Destroy(world.gameObject);
        }
        XanaLobbySearchPrefab.SetActive(false);
        SearchWorldParent.GetComponent<GridLayoutGroup>().padding.top=12;
        Debug.Log("SearchWorld");
        pageNumb = 1;
        pageSize = 15;
        if (searchworldCoroutine == null)
        {
            Debug.Log("SearchWorld coroutine");
            //StopCoroutine(IESearchWorld());
            searchWorldSTR = searchWorldInput.Text;
            searchworldCoroutine = StartCoroutine(IESearchWorld());
        }
        /*string searchWorldStr = searchWorldInput.Text;
        if (string.IsNullOrEmpty(searchWorldStr))
        {
            return;
        }
        TempWorldsbyName = AllWorldsInfoList.FindAll(o => o.name.StartsWith(searchWorldStr, StringComparison.InvariantCultureIgnoreCase));
        TempWorldsbyCreator = AllWorldsInfoList.FindAll(o => o.user.name.StartsWith(searchWorldStr, StringComparison.InvariantCultureIgnoreCase));
        List<RowList> TempWorlds = new List<RowList>();

        for (int i = 0; i < TempWorldsbyName.Count; i++)
        {
            if (!TempWorlds.Contains(TempWorldsbyName[i]))
            {
                TempWorlds.Add(TempWorldsbyName[i]);
            }
        }
        for (int i = 0; i < TempWorldsbyCreator.Count; i++)
        {
            if (!TempWorlds.Contains(TempWorldsbyCreator[i]))
            {
                TempWorlds.Add(TempWorldsbyCreator[i]);
            }
        }

        for (int i = 0; i < TempWorlds.Count; i++)
        {
            GameObject TempObject = Instantiate(eventPrefab);
            TempObject.transform.SetParent(SearchWorldParent);
            TempObject.transform.localScale = Vector3.one;
            FeedEventPrefab _event = TempObject.GetComponent<FeedEventPrefab>();
            _event.idOfObject = TempWorlds[i].id;
            _event.m_EnvironmentName = TempWorlds[i].name;
            try
            {
                _event.m_ThumbnailDownloadURL = TempWorlds[i].thumbnail.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");
            }
            catch (Exception e)
            {
                _event.m_ThumbnailDownloadURL = TempWorlds[i].thumbnail;
            }
            _event.m_BannerLink = TempWorlds[i].thumbnail;
            _event.m_WorldDescription = TempWorlds[i].description;
            _event.entityType = TempWorlds[i].entityType;
            _event.m_PressedIndex = int.Parse(TempWorlds[i].id);
            _event.updatedAt = TempWorlds[i].updatedAt;
            _event.createdAt = TempWorlds[i].createdAt;
            if (TempWorlds[i].entityType == WorldType.USER_WORLD.ToString())
            {
                _event.creatorName = TempWorlds[i].user.name;
                _event.userAvatarURL = TempWorlds[i].user.avatar;
                _event.userLimit = "10";
            }
            else
            {
                _event.creatorName = "XANA";
                _event.userLimit = TempWorlds[i].user_limit;
            }
            _event.Init();
            _event.DownloadAndLoadFeed();
            _event.DownloadPrefabSprite();
        }*/
    }
}
