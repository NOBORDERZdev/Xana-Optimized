using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : MonoBehaviour
{
    List<WorldItemDetail> Worlds = new List<WorldItemDetail>();
    public Transform WorldContentHolder;
    [SerializeField] private DynamicScrollRect.ScrollContent _content = null;
    [SerializeField] private int _itemCount = 50;
    public void AddWorld(WorldItemDetail _world)
    {
        Worlds.Add(_world);
    }
    public void DisplayWorlds()
    {
       // Application.targetFrameRate = 60;
       //List<ScrollItemDefault> contentDatas = new List<ScrollItemDefault>();
        _content.TotalItems = Worlds.Count;
       // for (int i = 0; i < _itemCount; i++)
       // {
       //     contentDatas.Add(new ScrollItemDefault(i));
       // }
        _content.InitScrollContent(Worlds);
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
