using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MainMenuWorldManager : MonoBehaviour
{
    public bool dataIsFatched = false;
    public Transform WorldCategoryHolder, WorldCategoryHolderSpawnParent;
    int State = 0;
    IEnumerator  Start()
    {
        while(ConstantsGod.AUTH_TOKEN == "AUTH_TOKEN")
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }
        LoadingHandler.Instance.worldLoadingScreen.SetActive(true);
        Debug.LogError("Start");
        foreach(string url in PrepareApiURL())
        {
            Debug.LogError(GetCategoryName(State) + " ----  URL   ---- "+url);
            GetWorldsDetailFromServer(url, GetCategoryName(State));
            dataIsFatched = true;
            State++;
            while (dataIsFatched)
            {
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
        LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
    }
    List<string> PrepareApiURL()
    {
        List<string> WorldUrls = new List<string>();
        WorldUrls.Add( ConstantsGod.API_BASEURL + ConstantsGod.MUSEUMENVBUILDERWORLDSCOMBINED + 1 + "/" + 14);
        WorldUrls.Add(ConstantsGod.API_BASEURL + ConstantsGod.WORLDSBYCATEGORY + 1 + "/" + 14 + "/" + "Publish" + "/GAME");
        WorldUrls.Add(ConstantsGod.API_BASEURL + ConstantsGod.ALLBUILDERWORLDS + "Publish" + "/" + 1 + "/" + 14);
        WorldUrls.Add(ConstantsGod.API_BASEURL + ConstantsGod.WORLDSBYCATEGORY + 1 + "/" + 14 + "/" + "Publish" + "/EVENT");
        WorldUrls.Add(ConstantsGod.API_BASEURL + ConstantsGod.WORLDSBYCATEGORY + 1 + "/" + 14 + "/" + "Publish" + "/TEST");
        WorldUrls.Add(ConstantsGod.API_BASEURL + ConstantsGod.MYBUILDERWORLDS + "Publish" + "/" + 1 + "/" + 14);
        return WorldUrls;
    }
    int CallBackCheck = 0;
    public void GetWorldsDetailFromServer(string url,string category)
    {
        StartCoroutine(FetchUserMapFromServer(url, category, (isSucess) =>
        {
            if (!isSucess)
            {
                if (++CallBackCheck > 25)
                {
                    LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
                    CallBackCheck = 0;
                    return;
                }
                GetWorldsDetailFromServer(url, category);
            }
        }));
    }
    IEnumerator FetchUserMapFromServer(string apiURL, string category, Action<bool> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(apiURL))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                callback(false);
            }
            else
            {
                WorldsInfo _worldInfo;
                _worldInfo = JsonUtility.FromJson<WorldsInfo>(www.downloadHandler.text);
                InstantiateWorlds(_worldInfo, category);
                dataIsFatched = false;
                callback(true);
            }
            www.Dispose();
        }
    }
    void InstantiateWorlds(WorldsInfo _worldInfo, string category)
    {
       Transform categorySpawned = Instantiate(WorldCategoryHolder.gameObject, WorldCategoryHolderSpawnParent).transform;
        categorySpawned.gameObject.SetActive(true);
        categorySpawned.GetComponent<WorldCategoryUIHandler>().Init(category, _worldInfo.data.rows.Count);
        if(_worldInfo.data.rows.Count>6)
        {
            GetComponent<WorldManager>().AllWorldTabReference.SetCategorySize(2);
        }
        else
        {
            GetComponent<WorldManager>().AllWorldTabReference.SetCategorySize(1);
        }
        Debug.LogError("World Count " + _worldInfo.data.rows.Count);
        for (int i = 0; i < _worldInfo.data.rows.Count; i++)
        {
            WorldItemDetail _event;
            _event = new WorldItemDetail();
            _event.IdOfWorld = _worldInfo.data.rows[i].id;
            _event.EnvironmentName = _worldInfo.data.rows[i].name;
            try
            {
                if (_worldInfo.data.rows[i].entityType != null)
                {
                    _event.ThumbnailDownloadURL = _worldInfo.data.rows[i].thumbnail.Replace("https://cdn.xana.net/xanaprod", "https://aydvewoyxq.cloudimg.io/_xanaprod_/xanaprod");
                  //  if (!_event.EnvironmentName.Contains("XANA Lobby"))
                        _event.ThumbnailDownloadURL = _event.ThumbnailDownloadURL + "?width=" + 256 + "&height=" + 256;
                  //  _event.ThumbnailDownloadURLHigh = _event.ThumbnailDownloadURL + "?width=" + 512 + "&height=" + 512;
                }
            }
            catch
            {
                _event.ThumbnailDownloadURL = _worldInfo.data.rows[i].thumbnail;
            }
            _event.BannerLink = _worldInfo.data.rows[i].banner;
            _event.WorldDescription = _worldInfo.data.rows[i].description;
            _event.EntityType = _worldInfo.data.rows[i].entityType;
            _event.PressedIndex = int.Parse(_worldInfo.data.rows[i].id);
            _event.UpdatedAt = _worldInfo.data.rows[i].updatedAt;
            _event.CreatedAt = _worldInfo.data.rows[i].createdAt;
            if (_worldInfo.data.rows[i].tags != null)
                _event.WorldTags = _worldInfo.data.rows[i].tags;

            if (_worldInfo.data.rows[i].entityType == WorldType.USER_WORLD.ToString())
            {
                _event.CreatorName = _worldInfo.data.rows[i].user.name;
                _event.UserAvatarURL = _worldInfo.data.rows[i].user.avatar;
                _event.UserLimit = "10";
            }
            else
            {
                if (!string.IsNullOrEmpty(_worldInfo.data.rows[i].creator))
                    _event.CreatorName = _worldInfo.data.rows[i].creator;
                else
                    _event.CreatorName = "XANA";
                _event.UserLimit = _worldInfo.data.rows[i].user_limit;
            }
            categorySpawned.GetComponent<WorldCategoryUIHandler>().AddWorldElementToUI(_event);
        }
    }
    string GetCategoryName(int index)
    {
        switch(index)
        {
            case 0:return "Hot";
            case 1:return "Game";
            case 2:return "New";
            case 3:return "Event";
            case 4:return "Test";
            case 5: return "My World";
            default: return APIURL.Hot.ToString();
        }
    }
}