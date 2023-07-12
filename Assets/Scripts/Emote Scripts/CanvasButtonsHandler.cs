using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CanvasButtonsHandler : MonoBehaviour
{
    static CanvasButtonsHandler _inst;
    public static CanvasButtonsHandler inst
    {
        get
        {
            if (_inst == null) _inst = FindObjectOfType<CanvasButtonsHandler>();
            return _inst;
        }
    }

    [Header("GamePlay ui")]
    public GameObject gamePlayUIParent;

    public GameObject actionsContainer;
    public Transform actionToggleImg;
    public ActionSelectionPanelHandler ActionSelectionPanel;
    public GameObject AnimationBtnClose;
    public Button rotateOrientationLand;

    [Header("FPS Button Reference")]
    public GameObject fPSButton;

    public bool stopCurrentPlayingAnim = false;

    public LoadEmoteAnimations ref_LoadEmoteAnimations;

    public GameObject portraitJoystick;

    public GameObject jumpBtn;
    private void Start()
    {
        if (rotateOrientationLand)
            rotateOrientationLand.onClick.AddListener(ChangeOrientation);
    }

    private void OnEnable()
    {

    }
    void ChangeOrientation()
    {
        ChangeOrientation_waqas._instance.ChangeOrientation_editor();
    }

    public void OnGotoAnotherWorldClick()
    {
        GamePlayButtonEvents.inst.OnGotoAnotherWorldClick();
    }

    public void OnWordrobeClick()
    {
        GamePlayButtonEvents.inst.OnWordrobeClick();
    }

    public void OnHelpButtonClick(bool isOn)
    {
        gamePlayUIParent.SetActive(!isOn);//rik.......
        GamePlayButtonEvents.inst.UpdateHelpObjects(isOn);
    }

    public void OnSettingButtonClick()
    {
        GamePlayButtonEvents.inst.OnSettingButtonClick();
    }

    public void OnExitButtonClick()
    {
        GamePlayButtonEvents.inst.OnExitButtonClick();
    }

    public void OnPeopeClick()
    {
        GamePlayButtonEvents.inst.OnPeopeClick();
    }

    public void OnAnnouncementClick()
    {
        GamePlayButtonEvents.inst.OnAnnouncementClick();
    }

    public void OnInviteClick()
    {
        GamePlayButtonEvents.inst.OnInviteClick();
    }

    public void OnSwitchCameraClick()
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("fp_camera"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        GamePlayButtonEvents.inst.OnSwitchCameraClick();
    }

    public void OnChangehighlightedFPSbutton(bool isSelected)
    {
        fPSButton.GetComponent<Image>().enabled = isSelected;
    }

    public void OnSelfiBtnClick()
    {
        GamePlayButtonEvents.inst.OnSelfieClick();
    }

    public void OnOpenAnimationPanel()
    {
        ;
        ref_LoadEmoteAnimations.OpenAnimationSelectionPanel();
        Debug.Log("call hua times 3===" + GamePlayButtonEvents.inst.selectionPanelOpen);
        GamePlayButtonEvents.inst.selectionPanelOpen = true;
        GamePlayButtonEvents.inst.OpenAllAnims();
    }

    public void CloseEmoteSelectionPanel()
    {

        EmoteAnimationPlay.Instance.isEmoteActive = false;      // AH working

        if (stopCurrentPlayingAnim)                            // AH working
        {

            stopCurrentPlayingAnim = false;
            EmoteAnimationPlay.Instance.StopAnimation();
        }

        ref_LoadEmoteAnimations.CloseAnimationSelectionPanel();
        GamePlayButtonEvents.inst.CloseEmoteSelectionPanel();

        if (ReactionFilterManager.Instance && ReactionFilterManager.Instance.gameObject.activeInHierarchy)            // AH working
            ReactionFilterManager.Instance.HideReactionPanel();


        ReactScreen.Instance.HideEmoteScreen();
        // GamePlayButtonEvents.inst.CloseEmoteSelectionPanel();
        GamePlayButtonEvents.inst.selectionPanelOpen = false;

    }

    public void OnJumpBtnUp()
    {
        GamePlayButtonEvents.inst.OnJumpBtnUp();
    }

    public void OnJumpBtnDown()
    {
        GamePlayButtonEvents.inst.OnJumpBtnDown();
    }

    bool isActionShowing;
    public void OnActionsToggleClicked()
    {
        if (ChangeOrientation_waqas._instance.isPotrait)
        {
            if (ChangeOrientation_waqas._instance.joystickInitPosY == 0)
                ChangeOrientation_waqas._instance.joystickInitPosY = portraitJoystick.transform.localPosition.y;
        }
        if (!PremiumUsersDetails.Instance.CheckSpecificItem("env_actions"))
        {
            print("Please Upgrade to Premium account");
            return;
        }
        else
        {
            print("Horayyy you have Access");
        }

        isActionShowing = !isActionShowing;
        actionsContainer.SetActive(isActionShowing);
        Vector3 rot = new Vector3(0f, 0f, (isActionShowing) ? 0f : 180f);
        actionToggleImg.rotation = Quaternion.Euler(rot);
        if (jumpBtn)
            jumpBtn.transform.DOLocalMoveX((isActionShowing) ? 277f : 372.6f, 0.1f);
    }
}
