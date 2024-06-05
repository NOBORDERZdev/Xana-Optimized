using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(menuName = "ScriptableObjects/SummitDataContainer", fileName = "ScriptableObjects/SummitDataContainer")]
public class XANASummitDataContainer : ScriptableObject
{
    string[] s ={ "ZONE-X", "ZONE X Musuem", "Xana Lobby", "XANA Festival Stage", "Xana Festival", "THE RHETORIC STAR", "ROCK’N ROLL CIRCUS", "MASAMI TANAKA", "Koto-ku Virtual Exhibition", "JJ MUSEUM", "HOKUSAI KATSUSHIKA", "Green Screen Studio", "GOZANIMATOR HARUNA GOUZU GALLERY 2021", "Genesis ART Metaverse Museum", "FIVE ELEMENTS", "DEEMO THE MOVIE Metaverse Museum", "D_Infinity_Labo", "BreakingDown Arena", "Astroboy x Tottori Metaverse Museum" };

    public List<Data> summitData=new List<Data>();

    public AIData aiData=new AIData();

    //private void OnEnable()
    //{
    //    Debug.LogError("a");
    //    for (int i = 0; i < 100; i++)
    //    {
    //        Data data = new Data();
    //        data.domeId = i;
    //        data.sceneName = s[Random.Range(0, s.Length)];

    //        summitData.Add(data);
    //    }
    //}

    public async void GetAIData(string domeId, string npctype)
    {
        string url = ConstantsGod.BASE_URL + ConstantsGod.GETDOMENPCINFO + domeId + "/" + 1;
        string result=await GetAsyncRequest(url);
        aiData = JsonUtility.FromJson<AIData>(result);
    }

    async Task<string> GetAsyncRequest(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest();
        while(!www.isDone)
            await System.Threading.Tasks.Task.Yield();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            return www.error;
        }
        else
            return www.downloadHandler.text;
    }

    #region DomeInfo
    [System.Serializable]
    public class Data
    {
        public int domeId;
        public string sceneName;
    }
    #endregion



    #region AINPC Data Classes
    [System.Serializable]
    public class AIData
    {
        public List<AINPCInfo> data; 
    }

    [System.Serializable]
    public class AINPCInfo
    {
        public int id;
        public int domeId;
        public string language;
        public string avatarCategory;
        public string name;
        public int[] spawnPosition;
        public string personalityURL;
    }
    #endregion

}
