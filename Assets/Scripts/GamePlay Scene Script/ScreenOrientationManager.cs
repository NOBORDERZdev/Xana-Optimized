using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Metaverse;

public class ScreenOrientationManager : MonoBehaviour
{
    public List<GameObject> landscapeObj;
    public List<GameObject> potraitObj;

    public bool isPotrait = false;
    public static ScreenOrientationManager _instance;
    public static Action switchOrientation;

    [HideInInspector]
    public float joystickInitPosY = 0;
    public GameObject JyosticksObject;
    CanvasGroup landscapeCanvas;
    CanvasGroup potraitCanvas;

    public AvatarSpawnerOnDisconnect ref_AvatarSpawnerOnDisconnect;
    public AvatarSpawnerOnDisconnect ref_AvatarSpawnerOnDisconnect_Portrait;

    private void Awake()
    {
        _instance = this;
        landscapeCanvas = landscapeObj[1].GetComponent<CanvasGroup>();
        potraitCanvas = potraitObj[1].GetComponent<CanvasGroup>();

        //Invoke("CheckOrienataionWhenComeFromLobby", 1f);
        CheckOrienataionWhenComeFromLobby();
    }


    void CheckOrienataionWhenComeFromLobby()
    {
        if (ConstantsHolder.xanaConstants.isFromXanaLobby && ConstantsHolder.xanaConstants.orientationchanged)
        {
            MyOrientationChangeCode(DeviceOrientation.Portrait);
        }
        else
        {
            ConstantsHolder.xanaConstants.orientationchanged = false;
        }

        isPotrait = ConstantsHolder.xanaConstants.orientationchanged;
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
        ConstantsHolder.xanaConstants.orientationchanged = isPotrait;
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
        AvatarSpawnerOnDisconnect.Instance = null;
        if (isPotrait)
        {
            AvatarSpawnerOnDisconnect.Instance = ref_AvatarSpawnerOnDisconnect_Portrait;
            potraitCanvas.DOFade(1, 0.5f);
            potraitCanvas.blocksRaycasts = true;
            potraitCanvas.interactable = true;
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else
        {
            AvatarSpawnerOnDisconnect.Instance = ref_AvatarSpawnerOnDisconnect;
            landscapeCanvas.DOFade(1, 0.5f);
            landscapeCanvas.blocksRaycasts = true;
            landscapeCanvas.interactable = true;
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        if (ArrowManager.Instance)
        {
            AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer = ArrowManager.Instance.gameObject;
            AvatarSpawnerOnDisconnect.Instance.Defaultanimator = AvatarSpawnerOnDisconnect.Instance.currentDummyPlayer.transform.GetComponent<Animator>().runtimeAnimatorController;
        }

        for (int i = 0; i < landscapeObj.Count; i++)
        {
            landscapeObj[i].SetActive(!isPotrait);
            potraitObj[i].SetActive(isPotrait);
        }

       ReferrencesForGameplay.instance.MainPlayerParent.GetComponent<PlayerController>().restJoyStick();
    }

    public void ChangeOrientation_editor()
    {
        isPotrait = !isPotrait;
        StartCoroutine(ChangeOrientation(isPotrait));
        if (switchOrientation != null)
            switchOrientation.Invoke();
    }

}