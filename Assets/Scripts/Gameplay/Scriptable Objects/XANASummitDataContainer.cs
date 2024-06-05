using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[CreateAssetMenu(menuName = "ScriptableObjects/SummitDataContainer", fileName = "ScriptableObjects/SummitDataContainer")]
public class XANASummitDataContainer : ScriptableObject
{
    string[] s ={ "ZONE-X", "ZONE X Musuem", "Xana Lobby", "XANA Festival Stage", "Xana Festival", "THE RHETORIC STAR", "ROCK’N ROLL CIRCUS", "MASAMI TANAKA", "Koto-ku Virtual Exhibition", "JJ MUSEUM", "HOKUSAI KATSUSHIKA", "Green Screen Studio", "GOZANIMATOR HARUNA GOUZU GALLERY 2021", "Genesis ART Metaverse Museum", "FIVE ELEMENTS", "DEEMO THE MOVIE Metaverse Museum", "D_Infinity_Labo", "BreakingDown Arena", "Astroboy x Tottori Metaverse Museum" };

    public DomeData summitData=new DomeData();

    public AIData aiData=new AIData();

    
    //private void OnEnable()
    //{
    //    for(int i=0;i<100;i++)
    //    {
    //        DomeGeneralData domeData = new DomeGeneralData();
    //        domeData.id = i;
    //        domeData.name = s[Random.Range(0, s.Length)];
    //        summitData.root.Add(domeData);
    //    }
    //}

    public async void GetAllDomesData()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GETALLDOMES;
        string result = await GetAsyncRequest(url);

        summitData=JsonUtility.FromJson<DomeData>(result);
    }

    public async Task<bool> GetAIData(string domeId)
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GETDOMENPCINFO + domeId + "/" + 1;
        string result=await GetAsyncRequest(url);

        Debug.LogError(result);
        aiData = JsonUtility.FromJson<AIData>(result);

        return aiData.root.Count > 0;
    }

    async Task<string> GetAsyncRequest(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        await www.SendWebRequest();
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
    public class DomeData
    {
        public List<DomeGeneralData> root;
    }

    [System.Serializable]
    public class DomeGeneralData
    {
        public int id;
        public string name;
        public string description;
        public string creatorName;
        public string bgm;
        public string thumbnail;
        public string worldType;
        public int worldId;
        public string experienceType;
        public string builderWorldId;
    }
    #endregion



    #region AINPC Data Classes
    [System.Serializable]
    public class AIData
    {
        public List<AINPCInfo> root; 
    }

    [System.Serializable]
    public class AINPCInfo
    {
        public string id;
        public int domeId;
        public string language;
        public string avatarCategory;
        public string name;
        public int[] spawnPosition;
        public string personalityURL;
    }
    #endregion

}
