using BetterJSON;
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
    public List<string> playerIDs = new List<string>();
    public bool IsPlayerIdsSaved = false;

    private int page = 1;
    private int limit = 10;
    public bool isLeaderboardShown = false;
    public int MyRankInOverallGames = 0;
    public int MyPointsInOverallGames = 0;
    public void SaveCurrentRoomPlayerIds()  // Save the current room's player IDs; a player's ID will remain in the list even if they leave the room
    {
        if (!IsPlayerIdsSaved)
        {
            playerIDs.Clear();
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                playerIDs.Add(player.UserId);
            }
            IsPlayerIdsSaved = true;
        }
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
            StartCoroutine(GetPointsFromRank(MyRankInCurrentRace));
            //MyPointsInCurrentRace += CalculatePointsFromRank(MyRankInCurrentRace);

            UpdateUserPoints(MyPointsInCurrentRace);
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
    IEnumerator GetPointsFromRank(int rank)
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
                    GetPointsRankData[] rows = getPointsAPIResponse.data.rows;
                    foreach (GetPointsRankData row in rows)
                    {
                        if (row.rank == rank)
                        {
                            MyPointsInCurrentRace += row.points;
                            break;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Error: " + getPointsAPIResponse.msg);
                }
            }
        }
    }

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

    private List<(string playerId, int points, int rank)> GetPlayerRanks()
    {
        var playerPoints = GetPlayerPoints();

        // Sort by points in ascending order
        var sortedPlayerPoints = playerPoints.OrderByDescending(player => player.points).ToList();

        // Assign ranks
        List<(string playerId, int points, int rank)> playerRanks = new List<(string playerId, int points, int rank)>();

        for (int i = 0; i < sortedPlayerPoints.Count; i++)
        {
            playerRanks.Add((sortedPlayerPoints[i].playerId, sortedPlayerPoints[i].points, i + 1));
            if (sortedPlayerPoints[i].playerId == PhotonNetwork.LocalPlayer.UserId)
            {
                MyRankInOverallGames = i + 1;
                MyPointsInOverallGames = sortedPlayerPoints[i].points;
            }
        }

        return playerRanks;
    }

    private List<(string playerId, int points)> GetPlayerPoints()
    {
        List<(string playerId, int points)> playerPoints = new List<(string playerId, int points)>();

        foreach (string pId in playerIDs)
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
        var data = new
        {
            user_id = ConstantsHolder.userId,
            points = _points
        };
        string jsonData = JsonUtility.ToJson(data);

        byte[] bodyData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(ConstantsGod.API_BASEURL_Penpenz+ ConstantsGod.UpdateUserPoints_Penpenz, "PUT"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            webRequest.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return webRequest.SendWebRequest();

            if(webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string response = webRequest.downloadHandler.text;
                Debug.Log("Response: " + response);
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
        public GetPointsRankData[] rows;
    }

    [Serializable]
    public class GetPointsRankData
    {
        public int rank;
        public int points;
    }
}



