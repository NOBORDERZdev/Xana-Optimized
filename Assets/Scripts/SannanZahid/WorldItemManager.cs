using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour
{
    [SerializeField]
    Dictionary<string , List<WorldItemDetail>> Worlds = new Dictionary<string , List<WorldItemDetail>>();
    public Transform WorldContentHolder;
    [SerializeField] private DynamicScrollRect.ScrollContent _content = null;
    [SerializeField] private int _itemCount = 50;
    public void AddWorld(string key, WorldItemDetail _world)
    {
        if(Worlds.ContainsKey(key))
        {
            foreach(WorldItemDetail searchWorld in Worlds[key])
                if (searchWorld.IdOfWorld.Equals(_world.IdOfWorld))
                    return;
            Worlds[key].Add(_world);
        }
        else
        {
            Worlds.Add(key,new List<WorldItemDetail>());
            Worlds[key].Add(_world);
        }
    }
    public List<WorldItemDetail> Get6WorldsForTutorial()
    {
        List<WorldItemDetail> tutorialWorlds= new List<WorldItemDetail>();
        for (int i = 0; i < 6;i++)
        {
            tutorialWorlds.Add(Worlds["Hot"][i]);
        }
        return tutorialWorlds;
    }
    public void DisplayWorlds(string key)
    {
        if (!Worlds.ContainsKey("Temp"))
        {
            Worlds.Add("Temp", new List<WorldItemDetail>());
        }
        if (!Worlds.ContainsKey("SearchWorld"))
        {
            Worlds.Add("SearchWorld",new List<WorldItemDetail>());
        }
        _content.TotalItems = Worlds[key].Count;
        _content.InitScrollContent(key,Worlds[key]);
    }
    public void WorldPageStateHandler(bool _checkCheck)
    {
        _content.DynamicScrollRect.StateBlock = _checkCheck;
        _content.DynamicScrollRect.TopScroller.vertical = !_checkCheck;
        _content.DynamicScrollRect.content.anchoredPosition = Vector2.zero;
    }
    public void WorldScrollReset()
    {
        _content.DynamicScrollRect.content.anchoredPosition = Vector2.zero;
        _content.DynamicScrollRect.TopScroller.verticalNormalizedPosition = 1f;
        _content.ResetContent();
        _content.DynamicScrollRect.velocity = Vector2.zero;
    }
    public void ClearWorldScrollWorlds()
    {
        _content.DynamicScrollRect.TopScroller.verticalNormalizedPosition = 1f;
        _content.DynamicScrollRect.content.anchoredPosition = Vector2.zero;
        _content.ClearContent();
        if(!Worlds.ContainsKey("SearchWorld"))
        {
            _content.InitScrollContent("SearchWorld", new List<WorldItemDetail>());
        }
    }
    public int GetWorldCountPresentInMemory(string _key)
    {
        if (Worlds.ContainsKey(_key))
        {
            return Worlds[_key].Count;
        }
        else return 0;
    }
    public void ClearListInDictionary(string _key)
    {
        if(Worlds.ContainsKey(_key))
        {
            Worlds[_key].Clear();
        }
    }
}
[Serializable]
public class WorldItemDetail
{
    public string IdOfWorld;
    public string EnvironmentName;
    public string WorldDescription;
    public string ThumbnailDownloadURL;
    public string ThumbnailDownloadURLHigh;
    public string CreatorName;
    public string CreatedAt;
    public string UserLimit;
    public string UserAvatarURL;
    public string UpdatedAt = "00";
    public string EntityType = "None";
    public string BannerLink;
    public int PressedIndex;
    public string[] WorldTags;
    public string Creator_Name;
    public string CreatorAvatarURL;
    public string CreatorDescription;
}
