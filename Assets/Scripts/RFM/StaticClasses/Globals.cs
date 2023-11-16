using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFM
{
    public static class Globals
    {
        public enum GameState { InLobby, Countdown, TakePosition, Gameplay, GameOver }
        public static GameState gameState;

        public static GameObject player;

        public static string LOCAL_PLAYER_TAG = "PhotonLocalPlayer";
        public static string PLAYER_TAG = "Player";
        public static string HUNTER_NPC_TAG = "HunterNPC";
        public static string RUNNER_NPC_TAG = "RunnerNPC";
        public static string HUNTER_PLAYER_TAG = "HunterPLAYER";
        public static string CANVAS_TAG = "NewCanvas";
        public static string MAIN_CAMERA_TAG = "MainCamera";

        public static float npcCameraFollowSpeed = 3;

        // public static int gameRestartWait = 5000; // In milli-seconds
        // public static int countDownTime = 10;
        // public static int takePositionTime = 10;
        // public static int gameplayTime = 30;
        // public static int minNumberOfPlayer = 2;
        // public static int numOfAIHunters = 0;

        // public static readonly int GainingMoneyTimeInterval = 1;
        // public static readonly int MoneyPerInterval = 5;

        public static bool IsRFMWorld = false;
        public static bool IsLocalPlayerHunter;
    }
}
