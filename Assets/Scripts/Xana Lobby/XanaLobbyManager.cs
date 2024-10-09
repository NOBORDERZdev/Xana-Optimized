using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class XanaLobbyManager : MonoBehaviour
{
    public static XanaLobbyManager Instance { get; private set; }
    [NonReorderable]
    public List<XanaLobbyWorldInfo> worldsInfo;
    public List<XanaLobbyData> worldsData;
    public List<GameObject> placedWorlds;

   
    int ratioId;
    JjRatio _Ratio;
    string _Title;
    string _Aurthor;
    string _Des;
    Texture2D _image;
    MediaType _Type;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitXanaLobbyWorlds();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public async void InitXanaLobbyWorlds()
    {
        StringBuilder apiUrl = new StringBuilder();
        apiUrl.Append(ConstantsGod.API_BASEURL + ConstantsGod.GetXanaLobbyWorlds);

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl.ToString()))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("<color=red>" + request.error + " </color>");
            }
            else
            {
                print("xana lobby data" + request.downloadHandler.text);
                XanaLobbyJson json = JsonConvert.DeserializeObject<XanaLobbyJson>(request.downloadHandler.text);
               // XanaLobbyJson json = JsonUtility.FromJson<XanaLobbyJson>(request.downloadHandler.text);

                //print("jason data"+json.data.rows.Count);
                worldsData = json.data.rows;
                StartCoroutine(InitData(json,placedWorlds));
            }
        }
        
    }
    public IEnumerator InitData(XanaLobbyJson data,List<GameObject> placedWorldsList)
    {
        XLWorldInfo xlWorldInfo;
        int placedWorldsCount = 0;
        for (int i = 0; i < placedWorldsList.Count; i++)
        {
            xlWorldInfo = placedWorldsList[i].GetComponent<XLWorldInfo>();
            for (int j = 0; j < worldsData.Count; j++)
            {
                if (i == worldsData[j].index - 1)
                {
                    //if (worldsData[i].ratio == "1:1")
                    //{
                    //    worldsInfo[i].JjRatio = JjRatio.OneXOneWithDes;
                    //}
                    placedWorldsList[i].SetActive(true);
                    if (!worldsData[j].thumbnail.IsNullOrEmpty())
                    {
                        //worldsInfo[i].Type = MediaType.Image;
                        xlWorldInfo.InitData(placedWorldsCount, worldsData[j].thumbnail, JjRatio.OneXOneWithDes, MediaType.Image);
                        placedWorldsCount++;
                        xlWorldInfo.worldChanger.WorldName = worldsData[j].world_name;
                        if(APIBasepointManager.instance.IsXanaLive)
                            xlWorldInfo.worldChanger.MainNet = worldsData[j].world_id;
                        else
                            xlWorldInfo.worldChanger.testNet = worldsData[j].world_id;
                        if (worldsData[j].entity_type == EntityType.USER_WORLD.ToString())
                        {
                            xlWorldInfo.worldChanger.isBuilderWorld = true;
                            xlWorldInfo.worldChanger.isMusuem = false;
                        }else if (worldsData[j].entity_type == EntityType.MUSEUMS.ToString())
                        {
                            xlWorldInfo.worldChanger.isBuilderWorld = false;
                            xlWorldInfo.worldChanger.isMusuem = true;
                        }else
                        {
                            xlWorldInfo.worldChanger.isBuilderWorld = false;
                            xlWorldInfo.worldChanger.isMusuem = false;
                        }
                        //worldsInfo[i].Title = worldsData[i].title;
                        //worldsInfo[i].Aurthor = worldsData[i].authorName;
                        //worldsInfo[i].Des = worldsData[i].description;
                    }
                    break;
                }
                else
                {
                    placedWorldsList[i].SetActive(false);
                }
            }
        }
        yield return null;
    }
    public void SetInfo(JjRatio ratio, string title, string aurthur, string des, Texture2D image, MediaType type)
    {
        _Ratio = ratio;
        _Title = title;
        _Aurthor = aurthur;
        _Des = des;
        _image = image;
        _Type = type;
        ratioId = ((int)ratio);

        title = title.IsNullOrEmpty() ? "" : title;
        aurthur = aurthur.IsNullOrEmpty() ? "" : aurthur;
        des = des.IsNullOrEmpty() ? "" : des;

        XLRatios.instance.renderTexture.Release();
        // Setting Landscape 
        XLRatios.instance.ratioReferences[ratioId].l_image.gameObject.SetActive(true);
        XLRatios.instance.ratioReferences[ratioId].p_image.gameObject.SetActive(true);
        XLRatios.instance.ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(true);
        XLRatios.instance.ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(true);
        XLRatios.instance.ratioReferences[ratioId].l_Title.text = title;
        XLRatios.instance.ratioReferences[ratioId].l_Aurthur.text = aurthur;
        XLRatios.instance.ratioReferences[ratioId].l_Description.text = des;
        if (type == MediaType.Image)
        {
            XLRatios.instance.ratioReferences[ratioId].l_image.texture = image;
            XLRatios.instance.ratioReferences[ratioId].l_videoPlayer.gameObject.SetActive(false);
        }
        else
        {
            XLRatios.instance.ratioReferences[ratioId].l_image.gameObject.SetActive(false);
            //XLRatios.instance.ratioReferences[ratioId].l_videoPlayer.url = videoLink;
        }
        // Setting Portrait
        XLRatios.instance.ratioReferences[ratioId].p_Title.text = title;
        XLRatios.instance.ratioReferences[ratioId].p_Aurthur.text = aurthur;
        XLRatios.instance.ratioReferences[ratioId].p_Description.text = des;
        XLRatios.instance.ratioReferences[ratioId].p_image.texture = image;
        if (type == MediaType.Image)
        {
            //JjInfoManager.Instance.ratioReferences[ratioId].p_image.texture = image;
            XLRatios.instance.ratioReferences[ratioId].p_videoPlayer.gameObject.SetActive(false);
        }
        else
        {
            XLRatios.instance.ratioReferences[ratioId].p_image.gameObject.SetActive(false);
            //ratioReferences[ratioId].p_videoPlayer.url = videoLink;
        }
        if (!ScreenOrientationManager._instance.isPotrait) // for Landscape
        {
            XLRatios.instance.LandscapeObj.SetActive(true);
            XLRatios.instance.PotraiteObj.SetActive(false);
            XLRatios.instance.ratioReferences[ratioId].l_obj.SetActive(true);
            XLRatios.instance.ratioReferences[ratioId].p_obj.SetActive(false);
            // Landscape Data

        }
        else // Portrait Data
        {
            XLRatios.instance.LandscapeObj.SetActive(false);
            XLRatios.instance.PotraiteObj.SetActive(true);
            XLRatios.instance.ratioReferences[ratioId].l_obj.SetActive(false);
            XLRatios.instance.ratioReferences[ratioId].p_obj.SetActive(true);
            
        }
        if (GamePlayUIHandler.inst.gameObject.activeInHierarchy)
        {
            GamePlayUIHandler.inst.gamePlayUIParent.SetActive(false);
        }
    }
}
[Serializable]
public class XanaLobbyWorldInfo
{
    public List<String> Title;
    public List<String> Aurthor;
    public List<String> Des;
    public MediaType Type;
    public Sprite WorldImage;
    public JjRatio JjRatio;
}
public enum MediaType
{
    Image
}
public enum EntityType
{
    MUSEUMS,ENVIRONMENTS,USER_WORLD
}
public class XanaLobbyJson
{
    public bool success;
    public DataContainer data;
    public string msg;
}
public class DataContainer
{
    public int count;
    public List<XanaLobbyData> rows;
}
[Serializable]
public class XanaLobbyData
{
    public int id;
    public int world_id;
    public int index;
    //public List<String> authorName;
    //public List<String> description;
    //public List<String> title;
    //public string ratio;
    public string world_name;
    public string description;
    public string thumbnail;
    //public string media_type;
    public string entity_type;
    public string creator_type;
    public DateTime createdAt;
    public DateTime updatedAt;
    public Users users;
}
[Serializable]
public class Users
{
    public string name;
}