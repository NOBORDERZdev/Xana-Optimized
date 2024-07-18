using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
            MyPointsInCurrentRace += CalculatePointsFromRank(MyRankInCurrentRace);

            UpdateRoomCustomPropertiesForPoints(localPlayer.UserId, MyPointsInCurrentRace);
        }
    }

    // Calculates LP based on the assigned rank
    private int CalculatePointsFromRank(int rank)
    {
        switch (rank)
        {
            case 1: return 100;
            case 2: return 80;
            case 3: return 60;
            default: return 0; // Consider if this default is appropriate
        }
    }

    // Updates the room custom properties with the player's points
    private void UpdateRoomCustomPropertiesForPoints(string userId, int points)
    {
        var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        roomProperties[userId + "_Points"] = points;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }


    //To print the leaderboard
    public void PrintLeaderboard()
    {
        var playerRanks = GetPlayerRanks();
       

        GamePlayUIHandler.inst.MyRankText.text = MyRankInCurrentRace.ToString();
        GamePlayUIHandler.inst.MyPointsText.text = MyPointsInCurrentRace.ToString();
        foreach (var playerInfo in playerRanks)
        {
            GameObject obj = Instantiate(GamePlayUIHandler.inst.PlayerLeaderboardStatsPrefab, GamePlayUIHandler.inst.PlayerLeaderboardStatsContainer.transform);
            obj.GetComponent<PlayerLeaderboardStats>().PlayerRank.text = playerInfo.rank.ToString();
            obj.GetComponent<PlayerLeaderboardStats>().PlayerName.text = playerInfo.playerId;
            obj.GetComponent<PlayerLeaderboardStats>().PlayerPoints.text = playerInfo.points.ToString();
            Debug.Log($"Player ID: {playerInfo.playerId}, Rank: {playerInfo.rank}, LP: {playerInfo.points}");
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

}
