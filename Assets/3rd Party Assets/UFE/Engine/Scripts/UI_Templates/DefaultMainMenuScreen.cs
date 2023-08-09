using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UFE3D;

public class DefaultMainMenuScreen : MainMenuScreen
{
    #region public instance fields
    public AudioClip onLoadSound;
    public AudioClip music;
    public AudioClip selectSound;
    public AudioClip cancelSound;
    public AudioClip moveCursorSound;
    public AudioClip gameMusic; //kush
    public GameObject characterVarient;
    public bool stopPreviousSoundEffectsOnLoad = false;
    public float delayBeforePlayingMusic = 0.1f;

    public Button buttonNetwork;
    public Button buttonBluetooth;
    #endregion

    #region public override methods
    public override void DoFixedUpdate(
        IDictionary<InputReferences, InputEvents> player1PreviousInputs,
        IDictionary<InputReferences, InputEvents> player1CurrentInputs,
        IDictionary<InputReferences, InputEvents> player2PreviousInputs,
        IDictionary<InputReferences, InputEvents> player2CurrentInputs
    )
    {
        base.DoFixedUpdate(player1PreviousInputs, player1CurrentInputs, player2PreviousInputs, player2CurrentInputs);

        this.DefaultNavigationSystem(
            player1PreviousInputs,
            player1CurrentInputs,
            player2PreviousInputs,
            player2CurrentInputs,
            this.moveCursorSound,
            this.selectSound,
            this.cancelSound
        );
    }

    public override void OnShow()
    {
        base.OnShow();
        this.HighlightOption(this.FindFirstSelectable());

        if (this.music != null)
        {
            UFE.DelayLocalAction(delegate () { UFE.PlayMusic(this.music); }, this.delayBeforePlayingMusic);
        }

        if (this.stopPreviousSoundEffectsOnLoad)
        {
            UFE.StopSounds();
        }

        if (this.onLoadSound != null)
        {
            UFE.DelayLocalAction(delegate () { UFE.PlaySound(this.onLoadSound); }, this.delayBeforePlayingMusic);
        }

        if (buttonNetwork != null)
        {
            buttonNetwork.interactable = UFE.isNetworkAddonInstalled || UFE.isBluetoothAddonInstalled;
        }

        if (buttonBluetooth != null)
        {
            buttonBluetooth.interactable = UFE.isBluetoothAddonInstalled;
        }
    }

    private void Start()
    {
        InitXanaAvatar();
        UFE.config.selectedStage.music = gameMusic; //kush
        Camera.main.GetComponent<AudioListener>().enabled = false;
        Camera.main.GetComponent<AudioListener>().enabled = true;
    }

    public void InitXanaAvatar()
    {
        GameObject g = Instantiate(FightingModuleManager.Instance.myAvatar, characterVarient.gameObject.transform.position, FightingModuleManager.Instance.myAvatar.transform.rotation,transform);
        g.transform.localRotation = Quaternion.Euler(Vector3.up*270);
        g.transform.localScale = (Vector3.one * 10);
        g.SetActive(true);
        characterVarient.SetActive(false);
        g.GetComponent<Animator>().runtimeAnimatorController = FightingModuleManager.Instance.fightingModuleAnimator;
    }
    #endregion
}
