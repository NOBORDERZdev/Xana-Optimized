using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

namespace RFM.Managers
{
    public class RFMMissionsManager : MonoBehaviour
    {
        // public TextMeshProUGUI showMoney;
        // public MMScaleShaker moneyScaleShaker;

        // public int Money { get; private set; } = 0;

        // private void OnEnable()
        // {
        //     EventsManager.onGameStart += OnGameStarted;
        //     EventsManager.onGameTimeup += OnGameOver;
        //     EventsManager.onPlayerCaught += OnPlayerCaught;
        // }
        //
        // private void OnDisable()
        // {
        //     EventsManager.onGameStart -= OnGameStarted;
        //     EventsManager.onGameTimeup -= OnGameOver;
        //     EventsManager.onPlayerCaught -= OnPlayerCaught;
        // }
        //
        // private void Start()
        // {
        //     showMoney.text = "00";
        //     showMoney.gameObject.SetActive(false);
        // }
        //
        // private void OnGameStarted()
        // {
        //     if (Globals.IsLocalPlayerHunter) return;
        //     
        //     Money = 0;
        //     // _scores = new Dictionary<string, int>();
        //     showMoney.gameObject.SetActive(true);
        //
        //     InvokeRepeating(nameof(AddMoney), RFMManager.Instance.CurrentGameConfiguration.GainingMoneyTimeInterval,
        //         RFMManager.Instance.CurrentGameConfiguration.GainingMoneyTimeInterval);
        // }
        //
        // // private void RestartingGame()
        // // {
        // //     showMoney.gameObject.SetActive(false);
        // // }
        //
        // private void OnPlayerCaught(RFM.Character.NPCHunter catcher)
        // {
        //     OnGameOver();
        // }
        //
        // private void OnGameOver()
        // {
        //     showMoney.gameObject.SetActive(false);
        //     CancelInvoke(nameof(AddMoney));
        // }
        //
        // private void AddMoney()
        // {
        //     Money += RFMManager.Instance.CurrentGameConfiguration.MoneyPerInterval;
        //     showMoney.text = Money.ToString("F0") + "";
        //     moneyScaleShaker.Play();
        // }
    }
}