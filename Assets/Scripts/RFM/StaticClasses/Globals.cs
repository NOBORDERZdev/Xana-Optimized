using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFM
{
    public static class Globals
    {
        public enum GameState { InLobby, Countdown, TakePosition, Gameplay, GameOver }
        public static GameState gameState;

        public static /*GameObject*/Climbing.ThirdPersonController player;

        public static string LOCAL_PLAYER_TAG = "PhotonLocalPlayer";
        public static string PLAYER_TAG = "Player";
        public static string CANVAS_TAG = "NewCanvas";
        public static string MAIN_CAMERA_TAG = "MainCamera";

        public static float npcCameraFollowSpeed = 3;

        public static int gameRestartWait = 5000; // In milli-seconds
        public static int countDownTime = 10;
        public static int takePositionTime = 10;
        public static int gameplayTime = 300;
        public static int minNumberOfPlayer = 2;
        public static int numOfAIHunters = 0;

        public static readonly int GainingMoneyTimeInterval = 1;
        public static readonly int MoneyPerInterval = 5;

        public static bool IsRFMWorld = true;
        public static bool IsLocalPlayerHunter;
    }
}
