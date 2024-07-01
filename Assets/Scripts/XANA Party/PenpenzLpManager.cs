using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;

public class PenpenzLpManager : MonoBehaviour
{
    // Initializes room properties for rank management
    public void Initialize()
    {
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("lastRank"))
        {
            var initialProps = new ExitGames.Client.Photon.Hashtable
            {
                { "lastRank", 0 } // Start with 0 so the first player gets rank 1
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(initialProps);
        }
    }

    // Assigns the next rank to the local player and updates room properties
    private int AssignNextRank()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("lastRank", out object lastRankObj))
        {
            int lastRank = (int)lastRankObj;
            int newRank = lastRank + 1;

            var playerProps = new ExitGames.Client.Photon.Hashtable { { "Rank", newRank } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

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

    // Updates the player's rank and LP based on the newly assigned rank
    public void UpdatePlayerRankAndLP()
    {
        int rank = AssignNextRank();
        if (rank == 0) return; // Early exit if rank assignment failed

        Player localPlayer = PhotonNetwork.LocalPlayer;
        int lp = CalculateLPFromRank(rank);

        var playerProperties = new ExitGames.Client.Photon.Hashtable { { "LP", lp }, { "Rank", rank } };
        localPlayer.SetCustomProperties(playerProperties);

        UpdateRoomCustomPropertiesForRank(localPlayer.UserId, rank);

        Debug.Log($"Updated Rank to: {rank}, LP to: {lp}");
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
    public (int lp, int rank) GetPlayerLpAndRank()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        int lp = localPlayer.CustomProperties.TryGetValue("LP", out object lpValue) ? (int)lpValue : 0;
        int rank = localPlayer.CustomProperties.TryGetValue("Rank", out object rankValue) ? (int)rankValue : 0;

        // Print the LP and Rank to the console
        Debug.Log($"Player LP: {lp}, Rank: {rank}");

        return (lp, rank);
    }

    //To print the leaderboard
    public void PrintLeaderboard()
    {
        // Retrieve all players in the room
        Player[] players = PhotonNetwork.PlayerList;

        // Convert players to a list of tuples containing relevant information
        var playerInfoList = players.Select(player => new
        {
            PlayerId = player.UserId,
            Rank = player.CustomProperties.TryGetValue("Rank", out object rankValue) ? (int)rankValue : int.MaxValue, // Use int.MaxValue for unranked players to sort them at the end
            LP = player.CustomProperties.TryGetValue("LP", out object lpValue) ? (int)lpValue : 0
        })
        .OrderBy(playerInfo => playerInfo.Rank) // Sort by rank
        .ToList();

        // Print the sorted leaderboard
        Debug.Log("Leaderboard:");
        foreach (var playerInfo in playerInfoList)
        {
            Debug.Log($"Player ID: {playerInfo.PlayerId}, Rank: {playerInfo.Rank}, LP: {playerInfo.LP}");
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
                { "LP", 0 }, // Reset LP to 0 or any default value
                { "Rank", 0 } // Reset Rank to 0 or any default value
            };
            player.SetCustomProperties(playerProps);
        }

        Debug.Log("Game and player properties have been reset for a new game.");
    }

}
