using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem.OnScreen;
using Climbing;

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

    public bool isSpiritInUse;
    public float spirit;
    public float maxSpirit;
    public Image spiritFillImg;
    public TextMeshProUGUI spiritText;
    public Image overlay;
    public Button runIcon;
    public InputCharacterController RFMInputController;
    public GameObject slideBtn;
    public GameObject runBtn;
    public GameObject favouriteBtn;

    [Header("FPS Button Reference")]
    public GameObject fPSButton;

    public bool stopCurrentPlayingAnim = false;

    public LoadEmoteAnimations ref_LoadEmoteAnimations;

    public GameObject portraitJoystick;

    public GameObject jumpBtn;
    private void Start()
    {
        spirit = maxSpirit;
        if (rotateOrientationLand)
            rotateOrientationLand.onClick.AddListener(ChangeOrientation);
        bool RFMUI = false;
        if (FeedEventPrefab.m_EnvName == "RFMDummy")
        {
            RFMUI = true;
        }
        slideBtn.gameObject.SetActive(RFMUI);
        runBtn.gameObject.SetActive(RFMUI);
        favouriteBtn.gameObject.SetActive(!RFMUI);
    }

    private void OnEnable()
    {

    }

    public void OnSpiritButtonDown()
    {
        if (isSpiritInUse)
        {
            RFMInputController.run = runIcon.interactable = isSpiritInUse = false;
            if (useSpiritCoroutine != null)
            {
                StopCoroutine(useSpiritCoroutine);
            }
            addSpiritCoroutine = StartCoroutine(IEAddSpirit());
        }
        else
        {
            RFMInputController.run = runIcon.interactable = isSpiritInUse = true;
            if (addSpiritCoroutine != null)
            {
                StopCoroutine(addSpiritCoroutine);
            }
            useSpiritCoroutine = StartCoroutine(IEUseSpirit());
        }
    }
    public Coroutine useSpiritCoroutine;
    public Coroutine addSpiritCoroutine;
    public IEnumerator IEUseSpirit()
    {
        Debug.LogError("IEUseSpirit"+ ((spirit / maxSpirit * 100) > 0));
        while ((spirit / maxSpirit * 100) > 0)
        {
            spirit -= Time.deltaTime;
            spiritFillImg.fillAmount = spirit / maxSpirit;
            spiritText.text = ((spirit / maxSpirit) * 100).ToString("00") + "%";
            yield return new WaitForUpdate();
        }
        overlay.gameObject.SetActive(true);
        RFMInputController.run = runIcon.interactable = isSpiritInUse = false;
        addSpiritCoroutine = StartCoroutine(IEAddSpirit());
    }

    public IEnumerator IEAddSpirit()
    {
        Debug.LogError("IEAddSpirit: "+((spirit / maxSpirit * 100) < 100));
        while ((spirit / maxSpirit * 100) < 100)
        {
            if ((spirit / maxSpirit * 100) > 30)
            {
                overlay.gameObject.SetActive(false);
            }
            spirit += Time.deltaTime;
            spiritFillImg.fillAmount = spirit / maxSpirit;
            spiritText.text = ((spirit / maxSpirit) * 100).ToString("00") + "%";
            yield return new WaitForUpdate();
        }
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
        if (RFM.Globals.IsRFMWorld) // Muneeb
        {
            RFM.EventsManager.OnToggleHelpPanel();
            return;
        }
        
        Debug.LogError("3");
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
