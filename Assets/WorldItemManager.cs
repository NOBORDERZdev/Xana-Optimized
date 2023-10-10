using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour
{
    [SerializeField]
    Dictionary<string , List<WorldItemDetail>> Worlds= new Dictionary<string , List<WorldItemDetail>>();
   // List<WorldItemDetail> Worlds = new List<WorldItemDetail>();
    public Transform WorldContentHolder;
    [SerializeField] private DynamicScrollRect.ScrollContent _content = null;
    [SerializeField] private int _itemCount = 50;
    public void AddWorld(string key, WorldItemDetail _world)
    {
        if(Worlds.ContainsKey(key))
        {
            Worlds[key].Add(_world);
        }
        else
        {
            Debug.LogError("Generate key");
            Worlds.Add(key,new List<WorldItemDetail>());
            Worlds[key].Add(_world);
        }
    }
    public void DisplayWorlds(string key)
    {
        Debug.LogError("Target Frame Rate " + Application.targetFrameRate);
        Application.targetFrameRate = 120;
        Debug.LogError("Target Frame Rate " + Application.targetFrameRate);
        //List<ScrollItemDefault> contentDatas = new List<ScrollItemDefault>();
        _content.TotalItems = Worlds[key].Count;
        // for (int i = 0; i < _itemCount; i++)
        // {
        //     contentDatas.Add(new ScrollItemDefault(i));
        // }
        _content.InitScrollContent(key,Worlds[key]);
    }
}
[Serializable]
public class WorldItemDetail
{
    public string IdOfWorld;
    public string EnvironmentName;
    public string WorldDescription;
    public string ThumbnailDownloadURL;
    public string CreatorName;
    public string CreatedAt;
    public string UserLimit;
    public string UserAvatarURL;
    public string UpdatedAt = "00";
    public string EntityType = "None";
    public string BannerLink;
    public int PressedIndex;
    public string[] WorldTags;


}
