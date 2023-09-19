using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFM
{
    public static class EventsManager
    {
        public static event Action onCountdownStart, onGameStart, onGameTimeup,
            onRestarting, onHideCanvasElements, onToggleHelpPanel;
        public static event Action<NPC> onPlayerCaught;

        public static void PlayerCaught(NPC catcher) => onPlayerCaught?.Invoke(catcher);
        public static void StartGame() => onGameStart?.Invoke();
        public static void StartCountdown() => onCountdownStart?.Invoke();
        public static void GameTimeup() => onGameTimeup?.Invoke();
        public static void GameRestarting() => onRestarting?.Invoke();
        public static void OnHideCanvasElements() => onHideCanvasElements?.Invoke();
        public static void OnToggleHelpPanel() => onToggleHelpPanel?.Invoke();
    }
}
