using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

[CreateAssetMenu(menuName = "ScriptableObjects/SummitDataContainer", fileName = "ScriptableObjects/SummitDataContainer")]
public class XANASummitDataContainer : ScriptableObject
{

    public GameObject maleAIAvatar;
    public GameObject femaleAIAvatar;
    public string[] avatarJson;
    public DomeData summitData=new DomeData();
    public AIData aiData=new AIData();
    public static string FixedAvatarJson;
    public static Stack<StackInfoWorld> LoadedScenesInfo = new Stack<StackInfoWorld>();
    public static List<GameObject> SceneTeleportingObjects = new List<GameObject>();
    public static bool Penpenz=false;
    string[] s ={ "ZONE-X", "ZONE X Musuem", "Xana Lobby", "XANA Festival Stage", "Xana Festival", "THE RHETORIC STAR", "ROCK?N ROLL CIRCUS", "MASAMI TANAKA", "Koto-ku Virtual Exhibition", "JJ MUSEUM", "HOKUSAI KATSUSHIKA", "Green Screen Studio", "GOZANIMATOR HARUNA GOUZU GALLERY 2021", "Genesis ART Metaverse Museum", "FIVE ELEMENTS", "DEEMO THE MOVIE Metaverse Museum", "D_Infinity_Labo", "BreakingDown Arena", "Astroboy x Tottori Metaverse Museum" };

    //private void OnEnable()
    //{
    //    for (int i = 0; i < 128; i++)
    //    {
    //        DomeGeneralData domeData = new DomeGeneralData();
    //        domeData.id = i;
    //        string sceneName= s[Random.Range(0, s.Length)];
    //        domeData.name = sceneName;
    //        domeData.world = sceneName;
    //        summitData.domes.Add(domeData);
    //        //summitData.root[i].world = summitData.root[i].name;
    //    }

    //}

    public async void GetAllDomesData()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GETALLDOMES;
        string result = await GetAsyncRequest(url);
        summitData=JsonUtility.FromJson<DomeData>(result);

        // Activate Map
        ReferencesForGamePlay.instance.FullScreenMapStatus(true);
    }

    public async Task<bool> GetAIData(int domeId)
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GETDOMENPCINFO + domeId + "/" + 1;
        string result=await GetAsyncRequest(url);
        aiData = JsonUtility.FromJson<AIData>(result);

        return aiData.npcData.Count > 0;
    }


    async void GetSingleDomeData()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GETSINGLEDOME + "1";
        string result = await GetAsyncRequest(url);

        Debug.LogError(result);
        JsonUtility.FromJson<DomeGeneralData>(result);

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


    public string GetAudioFile(int domeId)
    {
        for (int i = 0;i<summitData.domes.Count;i++)
        {
            if (domeId == summitData.domes[i].id)
            {
                return summitData.domes[i].bgm;
            }
        }

        return string.Empty;
    }

    public string[] GetDomeImage(int DomeId)
    {
        for (int i = 0; i < summitData.domes.Count; i++)
        {
            if (DomeId == summitData.domes[i].id)
            {
                return new[] { summitData.domes[i].world360Image, summitData.domes[i].name ,summitData.domes[i].companyLogo};
            }
        }

        return new[] { string.Empty, string.Empty,string.Empty };
    }

    #region DomeInfo

    [System.Serializable]
    public class DomeData
    {
        public List<DomeGeneralData> domes;
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
        public bool worldType;
        public int worldId;
        public string world;
        public string experienceType;
        public int builderWorldId;
        public bool IsPenguin;
        public bool Ishumanoid;
        public int AvatarIndex;
        public string Avatarjson;
        public string world360Image;
        public string companyLogo;
        public int maxPlayer;
        public bool Penpenz;
        public List<SubWorldInfo> SubWorlds;
        public bool isSubWorld;
    }

    [System.Serializable]
    public class SubWorldInfo
    {
        public bool builderWorld;
        public bool officialWorld;
        public OfficialWorldDetails selectWorld;
        public string builderSubWorldId;
    }

    [System.Serializable]
    public class OfficialWorldDetails
    {
        public int id;
        public string label;
        public string icon;
        public int userLimit;
        public string subWorldType;
        public string subWorldCategory;
        public string creatorName;
        public string description;
    }


    [System.Serializable]
    public class StackInfoWorld
    {
        public string id;
        public string name;
        public int user_limit;
        public string thumbnail;
        public string banner;
        public string thumbnail_new;
        public string description;
        public string creator;
        public bool isBuilderWorld;
        public int domeId;
        public bool haveSubWorlds;
        public bool isFromSummitWorld;
        public Vector3[] playerTrasnform;
    }
    #endregion



    #region AINPC Data Classes
    [System.Serializable]
    public class AIData
    {
        public List<AINPCInfo> npcData; 
    }

    [System.Serializable]
    public class AINPCInfo
    {
        public int id;
        public int domeId;
        public string language;
        public string name;
        public int avatarId;
        public string personalityURL;
        public int[] spawnPositionArray;
    }
    #endregion

}
