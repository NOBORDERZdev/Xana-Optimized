using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

public class MultiplayerXanaParty : MonoBehaviour
{
    [SerializeField] List<GameData> GameIds = new List<GameData>();
    private List<GameData> copyList;
    private Random random = new Random();

    private void Start()
    {
        if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            StartCoroutine(FetchXanaPartyGames());
        }   
    }


    public GameData GetRandomAndRemove()
    {
         print("!!!!!!!!!");
        if (copyList.Count == 0 || copyList != null)
        {
            copyList = new List<GameData>(GameIds);
        }

        int index = random.Next(copyList.Count);
        GameData rand = copyList[index];
        copyList.RemoveAt(index);
       
        return rand;
    }
        
    IEnumerator FetchXanaPartyGames()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GetXanaPartyWorlds;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Error on XANA PARTY WORLD FETCH : <color=red>{www.error}</color>");
            }
            else
            {
                try
                {
                    var data = JObject.Parse(www.downloadHandler.text);
                    var rows = data["data"]["rows"];
                    foreach (var row in rows)
                    {
                        GameIds.Add(new GameData( (int)row["id"],row["name"].ToString()));
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"Error on XANA PARTY WORLD Parse : <color=red>{e.Message}</color>");
                    throw;
                }
            }
        }
    }
}

[Serializable]
public class  GameData
{
    public int Id;
    public string WorldName;

    public GameData(int id, string worldName)
    {
        Id = id;
        WorldName = worldName;
    }

}