using Newtonsoft.Json.Linq;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Random = System.Random;

public class XANAPartyManager : MonoBehaviour
{
    public static XANAPartyManager Instance;

    [SerializeField] List<GameData> GameIds = new List<GameData>(); // List of games to play 
    [SerializeField] bool debugMode = false; // To test a specific game
    [SerializeField] int debugGameId = 0; // Index of the game to test

    //private List<GameData> copyList /*= new List<GameData>()*/;
    private Random random = new Random();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }

    public void EnablingXANAParty()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        if (ShouldFetchXanaPartyGames())
        {
            StartCoroutine(FetchXanaPartyGames()); // Fetching XANA PARTY GAMES and Joining XANA PARTY LOBBY
        }
        else
        {
           StartCoroutine( LoadXanaPartyGame(false) ); // Joining XANA PARTY GAME
        }   
    }

    private bool ShouldFetchXanaPartyGames()
    {
        return ConstantsHolder.xanaConstants.isXanaPartyWorld && !ConstantsHolder.xanaConstants.JjWorldSceneChange && !ConstantsHolder.xanaConstants.isJoinigXanaPartyGame;
    }


    public GameData GetRandomAndRemove()
    {
         print("!!!!!!!!!");
        //Debug.LogError("GetRandomAndRemove---- >  1");
        //Debug.LogError("GetRandomAndRemove---- >  1.1 "+copyList.Count);

        //if (copyList.Count == 0 || copyList != null)
        //{
        //Debug.LogError("GetRandomAndRemove---- >  2");
         
        //}
        //Debug.LogError("GetRandomAndRemove---- >  3");
        //   copyList = new List<GameData>(GameIds);
        //Debug.LogError("GetRandomAndRemove---- >  4");

        //int index = random.Next(copyList.Count);
        //Debug.LogError("GetRandomAndRemove---- >  5");

        //GameData rand = copyList[index];
        //Debug.LogError("GetRandomAndRemove---- >  6");

        //copyList.RemoveAt(index);

         int index = random.Next(GameIds.Count);
        Debug.LogError("GetRandomAndRemove---- >  5");

        GameData rand = GameIds[index];
        Debug.LogError("GetRandomAndRemove---- >  6");

        GameIds.RemoveAt(index);

        if (debugMode)
        {
            return  GameIds[debugGameId];//rand;

        }
        else
        {
            return rand;
        }
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
                   StartCoroutine( LoadXanaPartyGame(true));
                }
                catch (Exception e)
                {
                    Debug.Log($"Error on XANA PARTY WORLD Parse : <color=red>{e.Message}</color>");
                    throw;
                }
            }
        }
    }

    IEnumerator
    LoadXanaPartyGame(bool isJoiningLobby)
    {
        ConstantsHolder.xanaConstants.userLimit = "15"; // update the user limit for xana party

        if (isJoiningLobby)
        {
            ConstantsHolder.xanaConstants.XanaPartyGameName = "RoofTopParty"; // Setting world name to join XANA PARTY LOBBY
            if (APIBasepointManager.instance.IsXanaLive)
            {
                ConstantsHolder.xanaConstants.MuseumID = ""; // Main net Id
            }
            else
            {
                ConstantsHolder.xanaConstants.MuseumID = "2492"; // test net Id
            }
            yield return new WaitForSeconds(3);
        }
        else
        {
            if (!ConstantsHolder.xanaConstants.isMasterOfGame) // is not master client
            {
                print("not master ");
                yield return new WaitForSeconds(10);
            }
            else
            {
                print("master entering a GAME!");
                yield return new WaitForSeconds(2);
            }
            
            MutiplayerController.CurrLobbyName = ConstantsHolder.xanaConstants.XanaPartyGameName;
        }

        HideLoadingScreens();
        ConstantsHolder.xanaConstants.EnviornmentName = ConstantsHolder.xanaConstants.XanaPartyGameName;
        WorldItemView.m_EnvName = ConstantsHolder.xanaConstants.XanaPartyGameName;
        WorldManager.instance.PlayWorld();
    }

    void HideLoadingScreens()
    {
        LoadingHandler.Instance.characterLoading.SetActive(false);
        LoadingHandler.Instance.presetCharacterLoading.SetActive(false);
        LoadingHandler.Instance.worldLoadingScreen.SetActive(false);
        LoadingHandler.Instance.loadingPanel.SetActive(false);
        LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
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