using UnityEngine;

namespace RFM.Managers
{
    public class RFMEconomyManager : MonoBehaviour
    {
        private void OnEnable()
        {
            RFM.EventsManager.onGameStart += OnGameStarted;
            RFM.EventsManager.onCalculateScores += OnCalculateScores;
        }

        private void OnDisable()
        {
            RFM.EventsManager.onGameStart -= OnGameStarted;
            RFM.EventsManager.onCalculateScores -= OnCalculateScores;
        }

        private void OnGameStarted()
        {

        }

        private void OnCalculateScores()
        {
            //foreach (var runner in FindObjectsOfType<RFM.Character.Runner>())
            //{
            //    Debug.LogError($"Runner {runner.nickName} has {runner.money}");
            //}


            //foreach (var hunter in FindObjectsOfType<RFM.Character.Hunter>())
            //{
            //    Debug.LogError($"Hunter {hunter.nickName} has {hunter.rewardMultiplier} x {hunter.participationAmount}");
            //}
        }


        #region Static
        public static int money = 150;

        public static void RemoveMoney(int amount)
        {
            money -= amount;
        }


        public static bool CanAfford(int amount)
        {
            return money >= amount;
        }

        public static bool PayToPlayRFM(int amount)
        {
            if (CanAfford(amount))
            {
                RemoveMoney(amount);
                return true;
            }

            return false;
        }
        #endregion
    }
}
