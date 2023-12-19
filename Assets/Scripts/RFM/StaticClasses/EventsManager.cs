using System;
using System.Collections;
using System.Collections.Generic;
using RFM.Character;
using UnityEngine;

namespace RFM
{
    public static class EventsManager
    {
        public static event Action onCalculateScores;

        public static event Action onCountdownStart, onTakePositionTimeStart, onGameStart, onGameTimeup,
            onShowScores, onHideCanvasElements, onToggleHelpPanel, onToggleSetLayoutPanel;

        public static void TakePositionTime() => onTakePositionTimeStart?.Invoke();
        public static void StartGame() => onGameStart?.Invoke();
        public static void StartCountdown() => onCountdownStart?.Invoke();

        public static void CalculateScores() => onCalculateScores?.Invoke();

        public static void GameOver() => onGameTimeup?.Invoke();
        public static void ShowScores() => onShowScores?.Invoke();
        public static void OnHideCanvasElements() => onHideCanvasElements?.Invoke();
        public static void OnToggleHelpPanel() => onToggleHelpPanel?.Invoke();
        public static void onSetLayoutPanelActivate() => onToggleSetLayoutPanel?.Invoke();
    }
}
