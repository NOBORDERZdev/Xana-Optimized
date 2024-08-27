using Newtonsoft.Json.Linq;
using Photon.Pun;
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
    public bool EnableXANAPartyGuest;
    [SerializeField] List<GameData> TotalGamesToVisit = new List<GameData>(); // List of games to play 
    [SerializeField] List<GameData> RemainingGamesToVisit = new List<GameData>(); // List of remaining games to visit
    public List<GameData> GamesToVisitInCurrentRound = new List<GameData>(); // List of games to visit in the current round
    public int GamesToVisitInCurrentRoundCount = 5; // Number of games to visit in the current round
    public int GameIndex = 0; // Index of the game to visit

    [SerializeField] bool debugMode = false; // To test a specific game
    [SerializeField] int debugGameId = 0; // Index of the game to test
    private Random random = new Random();

    public int ActivePlayerInCurrentLevel = 0;
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
            PhotonNetwork.AutomaticallySyncScene = true;
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

    

    public void RandomizeAndUpdateGameData()
    {
        if(RemainingGamesToVisit.Count == 0)      // If all games are visited
        {
            RemainingGamesToVisit = new List<GameData>(TotalGamesToVisit);
        }

        if (RemainingGamesToVisit.Count < 5)     // If remaining games are less than 5
        {
            GamesToVisitInCurrentRoundCount = RemainingGamesToVisit.Count;
        }
        else
        {
            GamesToVisitInCurrentRoundCount = 5;
        }


        GamesToVisitInCurrentRound.Clear();

        for (int i = 0; i < GamesToVisitInCurrentRoundCount; i++)     
        {
            int randIndex = random.Next(RemainingGamesToVisit.Count);
            GamesToVisitInCurrentRound.Add(RemainingGamesToVisit[randIndex]);  
            RemainingGamesToVisit.RemoveAt(randIndex);
        }
    }

    public GameData GetGameToVisitNow()
    {
        if (GameIndex >= GamesToVisitInCurrentRound.Count) // If all games in the current round are visited
        {
            GameIndex = 0;
            RandomizeAndUpdateGameData();
        }

        if (debugMode)
        {
            return RemainingGamesToVisit[debugGameId];
        }
        else
        {
            return GamesToVisitInCurrentRound[GameIndex];
        }
    }
        
    IEnumerator FetchXanaPartyGames()
    {
        if (TotalGamesToVisit.Count != 0)
        {
            if (GameIndex >= GamesToVisitInCurrentRound.Count)
            {
                GameIndex = 0;
                RandomizeAndUpdateGameData();
            }
            StartCoroutine(LoadXanaPartyGame(true));
        }
        else
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
                        print("DATA " + data.ToString());
                        var rows = data["data"]["rows"];
                        foreach (var row in rows)
                        {
                            TotalGamesToVisit.Add(new GameData((int)row["id"], row["name"].ToString()));
                            RemainingGamesToVisit.Add(new GameData((int)row["id"], row["name"].ToString()));
                        }
                        RandomizeAndUpdateGameData();
                        StartCoroutine(LoadXanaPartyGame(true));
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

    public IEnumerator LoadXanaPartyGame(bool isJoiningLobby)
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        ConstantsHolder.xanaConstants.userLimit = ConstantsHolder.XanaPartyMaxPlayers.ToString();//"3"; // update the user limit for xana party

        if (isJoiningLobby)
        {
            GameIndex = 0;
            GetComponent<PenpenzLpManager>().RaceStartWithPlayers = 0;
            GetComponent<PenpenzLpManager>().PlayerIDs.Clear();
            GetComponent<PenpenzLpManager>().WinnerPlayerIds.Clear();
            GetComponent<PenpenzLpManager>().RaceFinishTime.Clear();

            ConstantsHolder.xanaConstants.XanaPartyGameName = "RoofTopParty"; // Setting world name to join XANA PARTY LOBBY
            if (APIBasepointManager.instance.IsXanaLive)
            {
                ConstantsHolder.xanaConstants.MuseumID = ""; // Main net Id
            }
            else
            {
                ConstantsHolder.xanaConstants.MuseumID = "2492"; // test net Id
            }
            yield return new WaitForSeconds(1);
        }
        else
        {
            MutiplayerController.CurrLobbyName = ConstantsHolder.xanaConstants.XanaPartyGameName;

            if (!ConstantsHolder.xanaConstants.isMasterOfGame) // is not master client
            {
                print("not master ");
                yield return new WaitForSeconds(20);
            }
            else
            {
                print("master entering a GAME!");
                yield return new WaitForSeconds(2);
            }
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