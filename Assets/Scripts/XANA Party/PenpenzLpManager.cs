using BetterJSON;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PenpenzLpManager : MonoBehaviourPunCallbacks
{
    public int CurrentUpdatedRank = 0;
    public bool NeedToUpdateMyRank = false;
    public int MyRankInCurrentRace = 0;
    public int MyPointsInCurrentRace = 0;
    public bool ShowLeaderboard = false;
    public List<int> PlayerIDs = new List<int>();
    public bool IsPlayerIdsSaved = false;

    private int page = 1;
    private int limit = 10;
    public bool isLeaderboardShown = false;
    public int MyRankInOverallGames = 0;
    public int MyPointsInOverallGames = 0;

    private const string PlayerIDsKey = "PlayerIDs";
    public int RaceID;
    public List<int> WinnerPlayerIds = new List<int>(3);



    //public void SaveCurrentRoomPlayerIds()  // Save the current room's player IDs; a player's ID will remain in the list even if they leave the room
    //{
    //    if (!IsPlayerIdsSaved)
    //    {
    //        PlayerIDs.Clear();
    //        foreach (Player player in PhotonNetwork.PlayerList)
    //        {
    //            PlayerIDs.Add(player.UserId);
    //        }
    //        IsPlayerIdsSaved = true;
    //    }
    //}

    private void Start()
    {
        StartCoroutine(GetPointsFromRank());
    }

    public int UpdateLastRank()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("lastRank", out object lastRankObj))
        {
            int lastRank = (int)lastRankObj;
            int newRank = lastRank + 1;

            var roomProps = new Hashtable { { "lastRank", newRank } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
            return newRank;
        }
        else
        {
            Debug.LogError("Failed to retrieve lastRank from room properties.");
            return 0;
        }
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("lastRank"))
        {
            CurrentUpdatedRank = (int)propertiesThatChanged["lastRank"];
            if (NeedToUpdateMyRank)
            {
                UpdateMyRankAndPoints();
            }
        }

        if(propertiesThatChanged.ContainsKey(PhotonNetwork.LocalPlayer.UserId+ "_Points") && ShowLeaderboard)
        {
            ShowLeaderboard = false;
            Invoke(nameof(PrintLeaderboard), 3f);
        }
    }


    public void UpdateMyRankAndPoints() 
    {
        if (NeedToUpdateMyRank)
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;
            NeedToUpdateMyRank = false;
            MyRankInCurrentRace = CurrentUpdatedRank;
            //StartCoroutine(GetPointsFromRank(MyRankInCurrentRace));
            //MyPointsInCurrentRace += CalculatePointsFromRank(MyRankInCurrentRace);

            //UpdateRoomCustomPropertiesForPoints(localPlayer.UserId, MyPointsInCurrentRace);
        }
    }

    // Calculates LP based on the assigned rank
    //private int CalculatePointsFromRank(int rank)
    //{
    //    switch (rank)
    //    {
    //        case 1: return 100;
    //        case 2: return 80;
    //        case 3: return 60;
    //        default: return 0; // Consider if this default is appropriate
    //    }
    //}
    

    // Updates the room custom properties with the player's points
    private void UpdateRoomCustomPropertiesForPoints(string userId, int points)
    {
        var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        roomProperties[userId + "_Points"] = points;
        roomProperties[userId + "_Name"] = PhotonNetwork.LocalPlayer.NickName;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }


    //To print the leaderboard
    public void PrintLeaderboard()
    {
        if(isLeaderboardShown)
        {
            return;
        }
        isLeaderboardShown = true;

        StartCoroutine(UpdateRoundData());

        var playerRanks = GetPlayerRanks();



        GamePlayUIHandler.inst.MyRankText.text = MyRankInOverallGames.ToString();
        GamePlayUIHandler.inst.MyPointsText.text = MyPointsInOverallGames.ToString();

        
        foreach (var playerInfo in playerRanks)
        {
            GameObject obj = Instantiate(GamePlayUIHandler.inst.PlayerLeaderboardStatsPrefab, GamePlayUIHandler.inst.PlayerLeaderboardStatsContainer.transform);
            obj.GetComponent<PlayerLeaderboardStats>().PlayerRank.text = playerInfo.rank.ToString();
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(playerInfo.playerId + "_Name", out object userName))
            {
                obj.GetComponent<PlayerLeaderboardStats>().PlayerName.text = userName.ToString();
            }
            obj.GetComponent<PlayerLeaderboardStats>().PlayerPoints.text = playerInfo.points.ToString();
            //Debug.Log($"Player ID: {playerInfo.playerId}, Rank: {playerInfo.rank}, LP: {playerInfo.points}");
        }
        GamePlayUIHandler.inst.LeaderboardPanel.SetActive(true);


        if (XANAPartyManager.Instance.GameIndex >= XANAPartyManager.Instance.GamesToVisitInCurrentRound.Count)
        {
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

    private List<(int playerId, int points, int rank)> GetPlayerRanks()
    {
        var playerPoints = GetPlayerPoints();

        // Sort by points in ascending order
        var sortedPlayerPoints = playerPoints.OrderByDescending(player => player.points).ToList();

        // Assign ranks
        List<(int playerId, int points, int rank)> playerRanks = new List<(int playerId, int points, int rank)>();

        for (int i = 0; i < sortedPlayerPoints.Count; i++)
        {
            playerRanks.Add((sortedPlayerPoints[i].playerId, sortedPlayerPoints[i].points, i + 1));
            if (sortedPlayerPoints[i].playerId == int.Parse(PhotonNetwork.LocalPlayer.UserId))
            {
                MyRankInOverallGames = i + 1;
                MyPointsInOverallGames = sortedPlayerPoints[i].points;
            }
        }

        return playerRanks;
    }

    private List<(int playerId, int points)> GetPlayerPoints()
    {
        List<(int playerId, int points)> playerPoints = new List<(int playerId, int points)>();

        foreach (int pId in PlayerIDs)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(pId + "_Points", out object userPoints))
            {
                playerPoints.Add((pId, (int)userPoints));
            }
            else
            {
                playerPoints.Add((pId, 0)); // Default to 0 if no points are found
            }
        }

        return playerPoints;
    }


    public void ResetGame()
    {
        // Reset the lastRank in the room custom properties
        var roomProps = new Hashtable { { "lastRank", 0 } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    }


    IEnumerator UpdateUserPoints(int _points) // Update the user's points in the database
    {
        UserData userData = new UserData { user_id = 14188, points = 100 };
        string jsonBody = JsonUtility.ToJson(userData);

        UnityWebRequest request = new UnityWebRequest(ConstantsGod.API_BASEURL_Penpenz + ConstantsGod.UpdateUserPoints_Penpenz, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            UpdateUserPointsResponseData response = JsonUtility.FromJson<UpdateUserPointsResponseData>(request.downloadHandler.text);
            Debug.Log("Response: " + request.downloadHandler.text);
            // Handle the response data as needed
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
        //var data = new
        //{
        //    user_id = 14188,  // int.Parse(ConstantsHolder.userId),
        //    points = 100//_points
        //};
        //string jsonData = JsonUtility.ToJson(data);

        //byte[] bodyData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        //using (UnityWebRequest webRequest = new UnityWebRequest(ConstantsGod.API_BASEURL_Penpenz+ ConstantsGod.UpdateUserPoints_Penpenz, "POST"))
        //{
        //    webRequest.uploadHandler = new UploadHandlerRaw(bodyData);
        //    webRequest.downloadHandler = new DownloadHandlerBuffer();
        //    webRequest.SetRequestHeader("Content-Type", "application/json");

        //    webRequest.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        //    yield return webRequest.SendWebRequest();

        //    if(webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        //    {
        //        Debug.LogError("Error: " + webRequest.error);
        //    }
        //    else
        //    {
        //        string response = webRequest.downloadHandler.text;
        //        Debug.Log("Response: " + response);

        //        // Deserialize the JSON response
        //        UpdateUserPointsResponseData responseData = JsonUtility.FromJson<UpdateUserPointsResponseData>(response);

        //        if (responseData.success)
        //        {
        //            Debug.Log("User points updated successfully");
        //            foreach (var user in responseData.data)
        //            {
        //                Debug.Log($"ID: {user.id}, User ID: {user.user_id}, User Name: {user.user_name}, Points: {user.points}, Created At: {user.createdAt}, Updated At: {user.updatedAt}");
        //            }
        //        }
        //        else
        //        {
        //            Debug.LogError("Failed to update user points");
        //        }
        //    }
        //}
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
        public GetPointsRankData[] rows;
    }

    [Serializable]
    public class GetPointsRankData
    {
        public int rank;
        public int points;
    }


    [System.Serializable]
    public class UserData
    {
        public int user_id;
        public int points;
    }

    
    [System.Serializable]
    public class UpdateUserPointsResponseData
    {
        public bool success;
        public UserData[] data;
        public string msg;
    }

    [System.Serializable]
    public class User
    {
        public int id;
        public int user_id;
        public string user_name;
        public int points;
        public string createdAt;
        public string updatedAt;
    }

    #region Start Race
    public IEnumerator SendingUsersIdsAtStartOfRace()
    {
        // Create a form and add the user IDs
        WWWForm form = new WWWForm();
        for (int i = 0; i < PlayerIDs.Count; i++)
        {
            form.AddField("user_ids", PlayerIDs[i]);
        }


        using (UnityWebRequest webRequest = UnityWebRequest.Post(ConstantsGod.API_BASEURL_Penpenz + ConstantsGod.StartRace_Penpenz, form))
        {
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
            }
        }
    }
    #endregion

    #region Update Round Data

    public IEnumerator UpdateRoundData()
    {
        Dictionary<int, int> points = new Dictionary<int, int>();

        if (WinnerPlayerIds.Count > 0)
        {
            points.Add(WinnerPlayerIds[0], PointsRankData[0].points);
        }
        if (WinnerPlayerIds.Count > 1)
        {
            points.Add(WinnerPlayerIds[1], PointsRankData[1].points);
        }
        if (WinnerPlayerIds.Count > 2)
        {
            points.Add(WinnerPlayerIds[2], PointsRankData[2].points);
        }


        string url = string.Format(ConstantsGod.API_BASEURL_Penpenz + ConstantsGod.UpdateRoundData_Penpenz, RaceID, XANAPartyManager.Instance.GameIndex,points);

        var data = new
        {
            points = points
        };
        string jsonData = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
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

    #region Get Rank Points

    public GetPointsRankData[] PointsRankData; 
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
                    PointsRankData = getPointsAPIResponse.data.rows;
                    //foreach (GetPointsRankData row in PointsRankData)
                    //{
                    //    if (row.rank == rank)
                    //    {
                    //        MyPointsInCurrentRace += row.points;
                    //        StartCoroutine(UpdateUserPoints(MyPointsInCurrentRace));
                    //        break;
                    //    }
                    //}
                }
                else
                {
                    Debug.LogError("Error: " + getPointsAPIResponse.msg);
                }
            }
        }
    }
    #endregion


    #region Get Round Data

    [Serializable]
    public class RoundData
    {
        public int rank;
        public string name;
        public int round_points;
        public int user_id;
        public int points;
    }


    public IEnumerator GetRoundData()
    {
        string requestUrl = string.Format(ConstantsGod.API_BASEURL_Penpenz + ConstantsGod.UpdateRoundData_Penpenz, RaceID, XANAPartyManager.Instance.GameIndex);

        UnityWebRequest request = UnityWebRequest.Get(requestUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            RoundData data = JsonUtility.FromJson<RoundData>(response);
            Debug.Log("Response: " + response + " data: "+ data);

        }
    }
    #endregion
}



