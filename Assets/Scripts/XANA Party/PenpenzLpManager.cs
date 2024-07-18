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
    public int MyRank = 0;
    public int MyPoints = 0;
    public bool ShowLeaderboard = false;
    private List<string> playerIDs = new List<string>();
    public bool IsPlayerIdsSaved = false;
    // Initializes room properties for rank management
    //public void Initialize()
    //{
    //    if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("lastRank"))
    //    {
    //        var initialProps = new ExitGames.Client.Photon.Hashtable
    //        {
    //            { "lastRank", 0 } // Start with 0 so the first player gets rank 1
    //        };
    //        PhotonNetwork.CurrentRoom.SetCustomProperties(initialProps);
    //    }
    //}

    public void SaveCurrentRoomPlayerIds()
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



    //public void UpdateLastRank()
    //{
    //    // Get the current lastRank
    //    int lastRank = (int)PhotonNetwork.CurrentRoom.CustomProperties["lastRank"];

    //    // Increment the lastRank
    //    int newRank = lastRank + 1;

    //    //Update the room property
    //    Hashtable customProperties = new Hashtable
    //    {
    //        { "lastRank", newRank }
    //    };
    //    PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
    //}

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Debug.Log("Calling Current lastRank: ");
        if (propertiesThatChanged.ContainsKey("lastRank"))
        {
            // Handle the updated property
            CurrentUpdatedRank = (int)propertiesThatChanged["lastRank"];
            Debug.Log("Updated lastRank: " + CurrentUpdatedRank);

            if (NeedToUpdateMyRank)
            {
                AssignMyRank();
                //UpdatePlayerRankAndLP();
            }
        }
        if(propertiesThatChanged.ContainsKey(PhotonNetwork.LocalPlayer.UserId+ "_Points") && ShowLeaderboard)
        {
            ShowLeaderboard = false;
            Invoke(nameof(PrintLeaderboard), 3f);
        }

    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //if(changedProps.ContainsKey("MyRank") && changedProps.ContainsKey("MyPoints") && ShowLeaderboard)
        //{
        //    ShowLeaderboard = false;
        //    Invoke(nameof(PrintLeaderboard), 3f);
        //}

        //if (ShowLeaderboard)// && targetPlayer == PhotonNetwork.LocalPlayer)
        //{
        //    ShowLeaderboard = false;
        //    PrintLeaderboard();
        //}
    }


    public void AssignMyRank()
    {
        if (NeedToUpdateMyRank)
        {
            Player localPlayer = PhotonNetwork.LocalPlayer;
            NeedToUpdateMyRank = false;
            MyRank = CurrentUpdatedRank;
            MyPoints += CalculateLPFromRank(MyRank);
            Debug.Log("MyRank" + MyRank);
            var playerProps = new ExitGames.Client.Photon.Hashtable { { "MyRank", MyRank }, { "MyPoints", MyPoints } };
            localPlayer.SetCustomProperties(playerProps);
            Debug.Log($"Updated Rank to: {MyRank}, MyPoints to: {MyPoints}");

            UpdateRoomCustomPropertiesForPoints(localPlayer.UserId, MyPoints);
        }
    }




    // Assigns the next rank to the local player and updates room properties
    public int AssignNextRank()
    {
        Debug.Log("Assigning next rank to player...enter");
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("lastRank", out object lastRankObj))
        {
            Debug.Log("Assigning next rank to player..." + (int)lastRankObj);
            int lastRank = (int)lastRankObj;
            int newRank = lastRank + 1;

            

            var roomProps = new ExitGames.Client.Photon.Hashtable { { "lastRank", newRank } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

            Debug.Log($"Assigned new rank to player: {newRank}");
            return newRank;
        }
        else
        {
            Debug.LogError("Failed to retrieve lastRank from room properties.");
            return 0; // Consider how to handle this error case in your game logic
        }
    }
    private void UpdateRoomCustomPropertiesForPoints(string userId, int points)
    {
        var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        roomProperties[userId + "_Points"] = points;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }


    // Updates the player's rank and LP based on the newly assigned rank
    public void UpdatePlayerRankAndLP()
    {
        //int rank = AssignNextRank();
        //if (rank == 0) return; // Early exit if rank assignment failed

        //Player localPlayer = PhotonNetwork.LocalPlayer;
        //MyPoints = CalculateLPFromRank(MyRank);

        //var playerProperties = new ExitGames.Client.Photon.Hashtable { { "MyPoints", MyPoints }, { "MyRank", MyRank } };
        //localPlayer.SetCustomProperties(playerProperties);

        //UpdateRoomCustomPropertiesForRank(localPlayer.UserId, MyRank);

        Debug.Log($"Updated Rank to: {MyRank}, MyPoints to: {MyPoints}");
    }

    // Calculates LP based on the assigned rank
    private int CalculateLPFromRank(int rank)
    {
        switch (rank)
        {
            case 1: return 100;
            case 2: return 80;
            case 3: return 60;
            default: return 0; // Consider if this default is appropriate
        }
    }

    // Updates room custom properties to include the player's rank
    private void UpdateRoomCustomPropertiesForRank(string userId, int rank)
    {
        var roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        roomProperties[userId + "_Rank"] = rank;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
    }

    // Retrieves the player's LP and Rank from custom properties
    //public (int lp, int rank) GetPlayerLpAndRank()
    //{
    //    Player localPlayer = PhotonNetwork.LocalPlayer;

    //    int lp = localPlayer.CustomProperties.TryGetValue("MyPoints", out object lpValue) ? (int)lpValue : 0;
    //    int rank = localPlayer.CustomProperties.TryGetValue("MyRank", out object rankValue) ? (int)rankValue : 0;

    //    // Print the LP and Rank to the console
    //    Debug.Log($"Player LP: {lp}, Rank: {rank}");

    //    return (lp, rank);
    //}

    //To print the leaderboard
    public void PrintLeaderboard()
    {
        // Retrieve all players in the room
        Player[] players = PhotonNetwork.PlayerList;

        // Convert players to a list of tuples containing relevant information
        var playerInfoList = players.Select(player => new
        {
            PlayerId = player.UserId,
            Rank = player.CustomProperties.TryGetValue("MyRank", out object rankValue) ? (int)rankValue : int.MaxValue, // Use int.MaxValue for unranked players to sort them at the end
            MyPoints = player.CustomProperties.TryGetValue("MyPoints", out object myPointsValue) ? (int)myPointsValue : 0
        })
        .OrderBy(playerInfo => playerInfo.Rank) // Sort by rank
        .ToList();

        // Print the sorted leaderboard
        Debug.Log("Leaderboard:");

        GamePlayUIHandler.inst.MyRankText.text = MyRank.ToString();
        GamePlayUIHandler.inst.MyPointsText.text = MyPoints.ToString();
        foreach (var playerInfo in playerInfoList)
        {
            GameObject obj = Instantiate(GamePlayUIHandler.inst.PlayerLeaderboardStatsPrefab, GamePlayUIHandler.inst.PlayerLeaderboardStatsContainer.transform);
            obj.GetComponent<PlayerLeaderboardStats>().PlayerRank.text = playerInfo.Rank.ToString();
            obj.GetComponent<PlayerLeaderboardStats>().PlayerName.text = playerInfo.PlayerId;
            obj.GetComponent<PlayerLeaderboardStats>().PlayerPoints.text = playerInfo.MyPoints.ToString();
            Debug.Log($"Player ID: {playerInfo.PlayerId}, Rank: {playerInfo.Rank}, LP: {playerInfo.MyPoints}");
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


    // Resets the game and player properties for a new game
    public void ResetGame()
    {
        // Reset the lastRank in the room custom properties
        var roomProps = new ExitGames.Client.Photon.Hashtable { { "lastRank", 0 } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);

        // Optionally, reset player-specific properties if needed
        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerProps = new ExitGames.Client.Photon.Hashtable
            {
                { "MyPoints", 0 }, // Reset LP to 0 or any default value
                { "MyRank", 0 } // Reset Rank to 0 or any default value
            };
            player.SetCustomProperties(playerProps);
        }

        Debug.Log("Game and player properties have been reset for a new game.");
    }

}
