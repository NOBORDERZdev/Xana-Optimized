using BetterJSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
public class PenpenzLpManager : MonoBehaviourPunCallbacks
{
    public int RaceID;
    public List<int> PlayerIDs = new List<int>();
    public List<int> WinnerPlayerIds = new List<int>();

    public GetRankPointsData[] RankPointsData;
    private RoundDataResponse roundDataResponse;

    [HideInInspector]
    public bool isLeaderboardShown = false;
    private bool IsRoundDataUpdated = false;
    private bool IsRoundDataFetched = false;


    private void Start()
    {
        StartCoroutine(GetPointsFromRank());
    }

    #region Get Rank Points

    private int page = 1;
    private int limit = 3;
    IEnumerator GetPointsFromRank()
    {
        string url = $"{ConstantsGod.API_BASEURL_Penpenz}{ConstantsGod.GetRankPoints_Penpenz}?page={page}&limit={limit}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}, HTTP Status Code: {webRequest.responseCode}");
                Debug.LogError("Response: " + webRequest.downloadHandler.text);
            }
            else
            {
                string response = webRequest.downloadHandler.text;
                Debug.Log("Response: " + response);

                GetPointsAPIResponse getPointsAPIResponse = JsonUtility.FromJson<GetPointsAPIResponse>(response);

                if (getPointsAPIResponse.success)
                {
                    RankPointsData = getPointsAPIResponse.data.rows;
                }
                else
                {
                    Debug.LogError("Error: " + getPointsAPIResponse.msg);
                }
            }
        }
    }

    [Serializable]
    public class GetPointsAPIResponse
    {
        public bool success;
        public GetPointsData data;
        public string msg;
    }
    [Serializable]
    public class GetPointsData
    {
        public int count;
        public GetRankPointsData[] rows;
    }

    [Serializable]
    public class GetRankPointsData
    {
        public int rank;
        public int points;
    }

    #endregion

    #region Start Race
    public IEnumerator SendingUsersIdsAtStartOfRace()
    {
        // Create a JSON object and add the user IDs
        JObject json = new JObject();
        json["user_ids"] = JArray.FromObject(PlayerIDs);

        // Convert the JSON object to a string
        string jsonString = json.ToString();


        using (UnityWebRequest webRequest = new UnityWebRequest(ConstantsGod.API_BASEURL_Penpenz + ConstantsGod.StartRace_Penpenz, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Response: " + webRequest.downloadHandler.text);
                JObject response = JObject.Parse(webRequest.downloadHandler.text);
                RaceID = response["data"]["race_id"].ToObject<int>();
                GamificationComponentData.instance.GetComponent<PhotonView>().RPC("StartGameRPC", RpcTarget.All, RaceID);
                GamificationComponentData.instance.isRaceStarted = true;
            }
        }
    }
    #endregion

    #region Print Leaderboard
    //To print the leaderboard
    public IEnumerator PrintLeaderboard()
    {
        if (isLeaderboardShown)
        {
            yield return null;
        }
        isLeaderboardShown = true;

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(UpdateRoundData());

            while (!IsRoundDataUpdated)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        yield return new WaitForSeconds(2f);

        StartCoroutine(GetRoundData());
        //var playerRanks = GetPlayerRanks();
        while (!IsRoundDataFetched)
        {
            yield return new WaitForSeconds(0.1f);
        }

        foreach (var player in roundDataResponse.data)
        {
            if (player.user_id == int.Parse(ConstantsHolder.userId))
            {
                GamePlayUIHandler.inst.MyRankText.text = player.rank.ToString();
                GamePlayUIHandler.inst.MyPointsText.text = player.points.ToString();
            }
            GameObject obj = Instantiate(GamePlayUIHandler.inst.PlayerLeaderboardStatsPrefab, GamePlayUIHandler.inst.PlayerLeaderboardStatsContainer.transform);
            obj.GetComponent<PlayerLeaderboardStats>().PlayerRank.text = player.rank.ToString();
            obj.GetComponent<PlayerLeaderboardStats>().PlayerName.text = player.name;
            obj.GetComponent<PlayerLeaderboardStats>().PlayerPoints.text = player.points.ToString();
            obj.gameObject.SetActive(true);
        }


        GamePlayUIHandler.inst.LeaderboardPanel.SetActive(true);
        ResetGame();

        if (XANAPartyManager.Instance.GameIndex >= XANAPartyManager.Instance.GamesToVisitInCurrentRound.Count)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(EndRace());
            }
            GamePlayUIHandler.inst.MoveToLobbyBtn.SetActive(true);
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(GamificationComponentData.instance.MovePlayersToNextGame());
            }
        }
    }
    #endregion

    #region Update Round Data

    [Serializable]
    public class PointsData
    {
        public Dictionary<int, int> points = new Dictionary<int, int>();
    }

    public IEnumerator UpdateRoundData()
    {
        PointsData pointsData = new PointsData();

        if (WinnerPlayerIds.Count > 0)
        {
            pointsData.points.Add(WinnerPlayerIds[0], RankPointsData[0].points);
        }

        if (WinnerPlayerIds.Count > 1)
        {
            pointsData.points.Add(WinnerPlayerIds[1], RankPointsData[1].points);
        }

        if (WinnerPlayerIds.Count > 2)
        {
            pointsData.points.Add(WinnerPlayerIds[2], RankPointsData[2].points);
        }

        string url = string.Format(ConstantsGod.API_BASEURL_Penpenz + "/races/" + RaceID + "/rounds/" + XANAPartyManager.Instance.GameIndex);

        // Manually create JSON string
        StringBuilder jsonBuilder = new StringBuilder();
        jsonBuilder.Append("{ \"points\": {");
        foreach (var point in pointsData.points)
        {
            jsonBuilder.AppendFormat("\"{0}\": {1},", point.Key, point.Value);
        }
        if (jsonBuilder[jsonBuilder.Length - 1] == ',')
        {
            jsonBuilder.Length--; // Remove the trailing comma
        }
        jsonBuilder.Append("} }");

        string jsonData = jsonBuilder.ToString();

        UnityWebRequest request = new UnityWebRequest(url, "PUT")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            IsRoundDataUpdated = true;
            string response = request.downloadHandler.text;
            Debug.Log("Response: " + response);
        }
    }

    #endregion

    #region Get Round Data

    public IEnumerator GetRoundData()
    {
        string requestUrl = string.Format(ConstantsGod.API_BASEURL_Penpenz + "/races/" + RaceID + "/rounds/" + XANAPartyManager.Instance.GameIndex);

        UnityWebRequest request = UnityWebRequest.Get(requestUrl);

        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string response = request.downloadHandler.text;

            roundDataResponse = JsonUtility.FromJson<RoundDataResponse>(response);
            IsRoundDataFetched = true;
            Debug.Log("Response: " + response);

        }
    }

    [Serializable]
    public class RoundDataResponse
    {
        public bool success;
        public RoundData[] data;
        public string msg;
    }

    [Serializable]
    public class RoundData
    {
        public int rank;
        public string name;
        public int round_points;
        public int user_id;
        public int points;
    }

    
    #endregion

    #region End Race
    public IEnumerator EndRace()
    {
        string url = string.Format(ConstantsGod.API_BASEURL_Penpenz + "/races/" + RaceID.ToString() + "/end");

        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(new byte[0]); // PUT request needs an upload handler

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            Debug.Log("Response: " + response);
        }
    }

    #endregion

    #region Reset Game
    public void ResetGame()
    {
        IsRoundDataUpdated = false;
        IsRoundDataFetched = false;
        WinnerPlayerIds.Clear();
        roundDataResponse = null;
    }
    #endregion
}



