using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
//using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.UI;


enum PointerUIChild
{
    Worship,
    DuneGuide,
    DaisenGuide,
    Temizuya,
    Emikage1,
    Emikage2,
}

enum InGameUIChild
{
    Worship,
    Omikuzi,
}

enum GameSpaceUIChild
{
    Information,
    DuneGuide,
    DaisenGuide,
    Emikage,
    Worship,
    Worship_fail,
    Omikuzi
}

public class UIController_Shine : MonoBehaviour
{
    //UI GameObjects
    [SerializeField] GameObject gameSpaceUI;
    [SerializeField] GameObject worldSpacePointer;
    public GameObject GetWorldPointerUI() {return worldSpacePointer;}
    [SerializeField] GameObject inGameUI;
    [SerializeField] List<Sprite> buttonImages;
    [SerializeField] List<Sprite> infoImages;    
    [SerializeField] List<Sprite> duneInfoImages;
    [SerializeField] List<Sprite> worshipImages;
    [SerializeField] List<Sprite> worshipAnimIconImages;
    [SerializeField] List<Sprite> omikuziImages;
    [SerializeField] Transform pointUI;

    // UI Configs
    private Transform informationUI;
    private Transform daisenInfoUI;
    private Transform duneInfoUI;
    private Transform emikageUI;
    private Transform worshipInfoUI;
    private Transform worshipGameUI;
    public Transform GetWorshipGameUI() {return worshipGameUI;}    
    private Transform omikuziUI;
    public Transform GetOmikuziUI() {return omikuziUI;}
    private Transform ohudouUI;

    // Index Configs
    private int infoNum = 0;
    private int duneNum = 0;
    private int worshipNum = 0;
    private int worshipAnimNum = 0;
    private string padletUrl = "https://padlet.com/metabuzz2021/padlet-pm18k4b03ik18lby";

    // Models
    [SerializeField] private GameObject gourd;
    [HideInInspector] public GameObject SpawnedGourd;
    [SerializeField] private GameObject DataManager_Shrine;

    //player references
    public Animator _playerAnimator;

    private void Awake() {
        informationUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.Information);
        daisenInfoUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.DaisenGuide);
        duneInfoUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.DuneGuide);
        emikageUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.Emikage);
        worshipInfoUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.Worship);
        ohudouUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.Omikuzi);
        worshipGameUI = inGameUI.transform.GetChild((int)InGameUIChild.Worship);
        omikuziUI = inGameUI.transform.GetChild((int)InGameUIChild.Omikuzi);
    }

    void Start()
    {
        //Init Images
        informationUI.GetComponent<Image>().sprite = infoImages[infoNum];
        duneInfoUI.GetComponent<Image>().sprite = duneInfoImages[duneNum];
        worshipInfoUI.GetComponent<Image>().sprite = worshipImages[worshipNum];
        worshipGameUI.GetComponent<RawImage>().texture = worshipAnimIconImages[worshipAnimNum].texture;

        //Button Interaction
        informationUI.GetComponentInChildren<Button>().onClick.AddListener(onInfoNextPage);
        daisenInfoUI.GetComponentInChildren<Button>().onClick.AddListener(() => closeUIPage(daisenInfoUI));
        duneInfoUI.GetComponentInChildren<Button>().onClick.AddListener(onDuneNextPage);
        worshipInfoUI.GetComponentInChildren<Button>().onClick.AddListener(onWorshipNextPage);
        worshipGameUI.GetComponent<Button>().onClick.AddListener(playWorshipAnimation);
        omikuziUI.GetComponent<Button>().onClick.AddListener(getRandomOmikuzi);
        ohudouUI.GetComponentInChildren<Button>().onClick.AddListener(closeOhudouUI);

        Button[] pointerArray = worldSpacePointer.GetComponentsInChildren<Button>();
        for (int i = 0; i < pointerArray.Length; i++) {
            int index = i;
            pointerArray[i].onClick.AddListener(() => openPopUp(pointerArray[index]));
            pointerArray[i].gameObject.SetActive(false);
        }
    }

    public void SetPointUI(string point) {
        pointUI.GetChild(0).GetComponent<Text>().text = point;
    }

    private void onInfoNextPage() {
        infoNum++;
        if (infoNum > infoImages.Count - 1) {
            //close information Popup
            infoNum = 0;
            informationUI.gameObject.SetActive(false);
        } else if (infoNum == infoImages.Count - 1) {        
            informationUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[0];
        }
        informationUI.GetComponent<Image>().sprite = infoImages[infoNum];
    }

    private void closeUIPage(Transform uiObject) {
        uiObject.gameObject.SetActive(false);
    }

    private void onDuneNextPage() {
        duneNum++;
        if (duneNum > duneInfoImages.Count - 1) {
            //close information Popup
            duneNum = 0;
            duneInfoUI.gameObject.SetActive(false);
        } else if (duneNum == duneInfoImages.Count - 1) {        
            duneInfoUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[0];
        }
        duneInfoUI.GetComponent<Image>().sprite = duneInfoImages[duneNum];
    }

    private void onWorshipNextPage() {
        worshipNum++;
        if (worshipNum > worshipImages.Count - 1) {
            //close information Popup
            worshipNum = 0;
            worshipInfoUI.gameObject.SetActive(false);
            DataManager_Shrine.GetComponent<DataManager_Shrine>().getPlayerData();
        } else if (worshipNum == worshipImages.Count - 1) {        
            worshipInfoUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[2];
        }
        worshipInfoUI.GetComponent<Image>().sprite = duneInfoImages[worshipNum];
    }

    private void openPopUp(Button button) {

        int idx = button.transform.GetSiblingIndex() >= (int)PointerUIChild.Emikage1 ? (int)PointerUIChild.Emikage1 : button.transform.GetSiblingIndex();
        if (idx == (int)PointerUIChild.Temizuya) {
            if (_playerAnimator == null)
            {
                _playerAnimator = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().animator;
                _playerAnimator.gameObject.GetComponent<IKMuseum>().uIController_Shine = this;

            }
            Transform rightHand = _playerAnimator.gameObject.GetComponent<IKMuseum>().RightHand.transform;
            SpawnedGourd = Instantiate(gourd, rightHand);

            //Disabling gameplay UI intereactions
            PlayerController.PlayerIsWalking?.Invoke();
            ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = false;

            _playerAnimator.SetTrigger("KanzakiWorld");
            _playerAnimator.SetTrigger("Washing");
            
        } else {
            int gamespaceIdx = -1;
            switch ((PointerUIChild)idx)
            {
                case PointerUIChild.Worship:
                    gamespaceIdx = (int)GameSpaceUIChild.Worship;
                    break;
                case PointerUIChild.DuneGuide:
                    gamespaceIdx = (int)GameSpaceUIChild.DuneGuide;
                    break;
                case PointerUIChild.DaisenGuide:
                    gamespaceIdx = (int)GameSpaceUIChild.DaisenGuide;
                    break;
                case PointerUIChild.Emikage1:
                    gamespaceIdx = (int)GameSpaceUIChild.Emikage;
                    break;
                default:
                    break;
            }
            if (gamespaceIdx != -1)
                gameSpaceUI.transform.GetChild(gamespaceIdx).gameObject.SetActive(true);
                if (gamespaceIdx == 3) {
                    StartCoroutine(ScrollSizeControl());
                }
        }
        button.gameObject.SetActive(false);           
    }

    IEnumerator ScrollSizeControl(){
        yield return new WaitForSeconds(0.5f);
        emikageUI.GetChild(0).GetChild(0).GetChild(1).GetComponent<Scrollbar>().size = 0;                    
    }


    private void playWorshipAnimation() {
        if (_playerAnimator == null)
        {
            _playerAnimator = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().animator;
            _playerAnimator.gameObject.GetComponent<IKMuseum>().uIController_Shine = this;
        }

        _playerAnimator.SetTrigger("KanzakiWorld");

        //Disabling gameplay UI intereactions
        PlayerController.PlayerIsWalking?.Invoke();
        ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = false;

        if (worshipAnimNum == 0) {            
            _playerAnimator.SetTrigger("BowTwice");            
        } else if (worshipAnimNum == 1) {
            _playerAnimator.SetTrigger("Clap");            
        } else if (worshipAnimNum == 2) {
            _playerAnimator.SetTrigger("Pray");            
        } else if (worshipAnimNum == 3) {
            _playerAnimator.SetTrigger("BowOnce");
        }
        worshipAnimNum++;
        Debug.Log(worshipAnimNum);
        Debug.Log(worshipGameUI);
        worshipGameUI.gameObject.SetActive(false);  
        if (worshipAnimNum > 3) {
            worshipAnimNum = 0;
        }
        worshipGameUI.GetComponent<RawImage>().texture = worshipAnimIconImages[worshipAnimNum].texture;
    }

    private void getRandomOmikuzi() {
        omikuziUI.gameObject.SetActive(false);
        ohudouUI.gameObject.SetActive(true);
        
        ohudouUI.GetChild(1).GetComponent<Image>().sprite = omikuziImages[UnityEngine.Random.Range(0, omikuziImages.Count)];
    }

    private void closeOhudouUI() {
        ohudouUI.gameObject.SetActive(false);
        ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = true;
        PlayerController.PlayerIsIdle?.Invoke();
    }
}