using System;
using System.Collections;
using System.Collections.Generic;
using RFM.Character;
using UnityEngine;

namespace RFM
{
    public static class EventsManager
    {
        public static event Action onCountdownStart, onTakePositionTimeStart, onGameStart, onGameTimeup,
            onRestarting, onHideCanvasElements, onToggleHelpPanel;
        public static event Action<NPCHunter> onPlayerCaught;
        public static event Action<PlayerHunter> onPlayerCaughtByPlayer;

        public static void PlayerCaught(NPCHunter catcher) => onPlayerCaught?.Invoke(catcher);
        public static void PlayerCaughtByPlayer(PlayerHunter catcher) => onPlayerCaughtByPlayer?.Invoke(catcher);
        public static void TakePositionTime() => onTakePositionTimeStart?.Invoke();
        public static void StartGame() => onGameStart?.Invoke();
        public static void StartCountdown() => onCountdownStart?.Invoke();
        public static void GameOver() => onGameTimeup?.Invoke();
        public static void GameRestarting() => onRestarting?.Invoke();
        public static void OnHideCanvasElements() => onHideCanvasElements?.Invoke();
        public static void OnToggleHelpPanel() => onToggleHelpPanel?.Invoke();
    }
}
