using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Metaverse;

public class ChangeOrientation_waqas : MonoBehaviour
{
    public List<GameObject> landscapeObj;
    public List<GameObject> potraitObj;

    public bool isPotrait = false;
    public static ChangeOrientation_waqas _instance;
    public static Action switchOrientation;

    [HideInInspector]
    public float joystickInitPosY = 0;
    public GameObject JyosticksObject;
    CanvasGroup landscapeCanvas;
    CanvasGroup potraitCanvas;

    public AvatarManager ref_avatarManager;
    public AvatarManager ref_avatarManager_Portrait;

    private void Awake()
    {
        _instance = this;
        landscapeCanvas = landscapeObj[1].GetComponent<CanvasGroup>();
        potraitCanvas = potraitObj[1].GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {

        ChangeOrientation_Main.OnOrientationChange += MyOrientationChangeCode;
    }

    private void OnDisable()
    {
        ChangeOrientation_Main.OnOrientationChange -= MyOrientationChangeCode;
    }

    public void MyOrientationChangeCode(DeviceOrientation orientation)
    {
        //FadeBothCanvas();
        print("Waqas Orientation Changed : " + orientation);

        switch (orientation)
        {
            case DeviceOrientation.LandscapeLeft:
                StartCoroutine(ChangeOrientation(false));
                break;
            case DeviceOrientation.Portrait:

                StartCoroutine(ChangeOrientation(true));
                break;
        }
    }


    IEnumerator ChangeOrientation(bool orientation)
    {
        isPotrait = orientation;
        XanaConstants.xanaConstants.orientationchanged = isPotrait;
        BuilderEventManager.BuilderSceneOrientationChange?.Invoke(orientation);
        landscapeCanvas.DOKill();
        landscapeCanvas.alpha = 0;
        landscapeCanvas.blocksRaycasts = false;
        landscapeCanvas.interactable = false;
        potraitCanvas.DOKill();
        potraitCanvas.alpha = 0;
        potraitCanvas.blocksRaycasts = false;
        potraitCanvas.interactable = false;
        yield return new WaitForSeconds(0.1f);
        AvatarManager.Instance = null;
        if (isPotrait)
        {
            AvatarManager.Instance = ref_avatarManager_Portrait;
            potraitCanvas.DOFade(1, 0.5f);
            potraitCanvas.blocksRaycasts = true;
            potraitCanvas.interactable = true;
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else
        {
            AvatarManager.Instance = ref_avatarManager;
            landscapeCanvas.DOFade(1, 0.5f);
            landscapeCanvas.blocksRaycasts = true;
            landscapeCanvas.interactable = true;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        if (ArrowManager.Instance)
        {
            AvatarManager.Instance.currentDummyPlayer = ArrowManager.Instance.gameObject;
            AvatarManager.Instance.Defaultanimator = AvatarManager.Instance.currentDummyPlayer.transform.GetComponent<Animator>().runtimeAnimatorController;
        }

        for (int i = 0; i < landscapeObj.Count; i++)
        {
            landscapeObj[i].SetActive(!isPotrait);
            potraitObj[i].SetActive(isPotrait);
        }

       ReferrencesForDynamicMuseum.instance.MainPlayerParent.GetComponent<PlayerControllerNew>().restJoyStick();
    }

    public void ChangeOrientation_editor()
    {
        isPotrait = !isPotrait;
        StartCoroutine(ChangeOrientation(isPotrait));
        if (switchOrientation != null)
            switchOrientation.Invoke();
    }

}