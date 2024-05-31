using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/SummitDataContainer", fileName = "ScriptableObjects/SummitDataContainer")]
public class XANASummitDataContainer : ScriptableObject
{
    string[] s ={ "ZONE-X", "ZONE X Musuem", "Xana Lobby", "XANA Festival Stage", "Xana Festival", "THE RHETORIC STAR", "ROCK’N ROLL CIRCUS", "MASAMI TANAKA", "Koto-ku Virtual Exhibition", "JJ MUSEUM", "HOKUSAI KATSUSHIKA", "Green Screen Studio", "GOZANIMATOR HARUNA GOUZU GALLERY 2021", "Genesis ART Metaverse Museum", "FIVE ELEMENTS", "DEEMO THE MOVIE Metaverse Museum", "D_Infinity_Labo", "BreakingDown Arena", "Astroboy x Tottori Metaverse Museum" };

    public List<Data> summitData=new List<Data>();

    [System.Serializable]
    public class Data
    {
        public int domeId;
        public string sceneName;
    }

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

}
