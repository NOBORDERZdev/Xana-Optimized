using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public GameObject GetWorldPointerUI() { return worldSpacePointer; }
    [SerializeField] GameObject inGameUI;
    [SerializeField] List<Sprite> buttonImages;
    [SerializeField] List<Sprite> infoImages;
    private List<Sprite> infos;
    [SerializeField] List<Sprite> duneInfoImages;
    private List<Sprite> duneInfos;
    [SerializeField] List<Sprite> daisenInfoImages;
    private List<Sprite> daisenInfos;
    [SerializeField] List<Sprite> worshipImages;
    private List<Sprite> worshipInfos;
    [SerializeField] List<Sprite> worshipRejectImages;
    private List<Sprite> worshipFails;
    [SerializeField] List<Sprite> worshipAnimIconImages;
    [SerializeField] List<Sprite> omikuziImages;
    [SerializeField] Transform pointUI;

    // UI Configs
    private Transform informationUI;
    private Transform daisenInfoUI;
    private Transform duneInfoUI;
    private Transform emikageUI;
    private Transform worshipInfoUI;
    private Transform worshipFailUI;
    private Transform worshipGameUI;
    public Transform GetWorshipGameUI() { return worshipGameUI; }
    private Transform omikuziUI;
    public Transform GetOmikuziUI() { return omikuziUI; }
    private Transform ohudouUI;
    private GameLanguage gameLanguage;

    // Index Configs
    private int infoNum = 0;
    private int daisenNum = 0;
    private int duneNum = 0;
    private int worshipNum = 0;
    private int worshipFailNum = 0;
    private int worshipAnimNum = 0;
    // private string padletUrl = "https://padlet.com/metabuzz2021/padlet-pm18k4b03ik18lby";

    // Models
    [SerializeField] public GameObject gourd;
    [SerializeField] private GameObject DataManager_Shrine;
    [SerializeField] private Transform mizuya;

    //private GameObject player;
    private Animator _playerAnimator;
    private string _lang = "";

    private void Awake()
    {
        Enable_DisableObjects.Instance.DisableScreenRotaionButton();
        ReferencesForGamePlay.instance.minimap.SetActive(false);

        informationUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.Information);
        daisenInfoUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.DaisenGuide);
        duneInfoUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.DuneGuide);
        emikageUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.Emikage);
        worshipInfoUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.Worship);
        worshipFailUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.Worship_fail);
        ohudouUI = gameSpaceUI.transform.GetChild((int)GameSpaceUIChild.Omikuzi);
        worshipGameUI = inGameUI.transform.GetChild((int)InGameUIChild.Worship);
        omikuziUI = inGameUI.transform.GetChild((int)InGameUIChild.Omikuzi);
    }

    void Start()
    {
        infos = new List<Sprite>();
        daisenInfos = new List<Sprite>();
        duneInfos = new List<Sprite>();
        worshipInfos = new List<Sprite>();
        worshipFails = new List<Sprite>();

        //Init Localized Image
        if (GameManager.currentLanguage.Contains("en") && !LocalizationManager.forceJapanese)
        {
            _lang = "Eng";
        }
        else
        {
            _lang = "Jpn";
        }

        for (int i = 0; i < infoImages.Count; i++)
        {
            if (infoImages[i].name.Contains(_lang))
            {
                infos.Add(infoImages[i]);
            }
        }

        for (int i = 0; i < daisenInfoImages.Count; i++)
        {
            if (daisenInfoImages[i].name.Contains(_lang))
            {
                daisenInfos.Add(daisenInfoImages[i]);
            }
        }

        for (int i = 0; i < duneInfoImages.Count; i++)
        {
            if (duneInfoImages[i].name.Contains(_lang))
            {
                duneInfos.Add(duneInfoImages[i]);
            }
        }

        for (int i = 0; i < worshipImages.Count; i++)
        {
            if (worshipImages[i].name.Contains(_lang))
            {
                worshipInfos.Add(worshipImages[i]);
            }
        }

        for (int i = 0; i < worshipRejectImages.Count; i++)
        {
            if (worshipRejectImages[i].name.Contains(_lang))
            {
                worshipFails.Add(worshipRejectImages[i]);
            }
        }

        //Init Images
        informationUI.GetComponent<Image>().sprite = infos[infoNum];
        daisenInfoUI.GetComponent<Image>().sprite = daisenInfos[daisenNum];
        daisenInfoUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[_lang == "Eng" ? 1 : 0];
        duneInfoUI.GetComponent<Image>().sprite = duneInfos[duneNum];
        worshipInfoUI.GetComponent<Image>().sprite = worshipInfos[worshipNum];
        worshipFailUI.GetComponent<Image>().sprite = worshipFails[worshipFailNum];
        daisenInfoUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[_lang == "Eng" ? 1 : 0];
        worshipGameUI.GetComponent<RawImage>().texture = worshipAnimIconImages[worshipAnimNum].texture;

        //Button Interaction
        informationUI.GetComponentInChildren<Button>().onClick.AddListener(onInfoNextPage);
        daisenInfoUI.GetComponentInChildren<Button>().onClick.AddListener(onDaisenNextPage);
        duneInfoUI.GetComponentInChildren<Button>().onClick.AddListener(onDuneNextPage);
        worshipInfoUI.GetComponentInChildren<Button>().onClick.AddListener(onWorshipNextPage);
        worshipFailUI.GetComponentInChildren<Button>().onClick.AddListener(onWorshipFailNextPage);
        worshipGameUI.GetComponent<Button>().onClick.AddListener(playWorshipAnimation);
        omikuziUI.GetComponent<Button>().onClick.AddListener(getRandomOmikuzi);
        ohudouUI.GetComponentInChildren<Button>().onClick.AddListener(closeOhudouUI);

        Button[] pointerArray = worldSpacePointer.GetComponentsInChildren<Button>();
        for (int i = 0; i < pointerArray.Length; i++)
        {
            int index = i;
            pointerArray[i].onClick.AddListener(() => openPopUp(pointerArray[index]));
            pointerArray[i].gameObject.SetActive(false);
        }
        //player = GameObject.FindGameObjectWithTag("PhotonLocalPlayer");
    }

    public void SetPointUI(string point)
    {
        pointUI.GetChild(0).GetComponent<Text>().text = point;
    }

    private void onInfoNextPage()
    {
        infoNum++;
        if (infoNum > infos.Count - 1)
        {
            //close information Popup
            infoNum = 0;
            informationUI.gameObject.SetActive(false);
        }
        else if (infoNum == infos.Count - 1)
        {
            informationUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[0];
        }
        informationUI.GetComponent<Image>().sprite = infos[infoNum];
    }

    private void closeUIPage(Transform uiObject)
    {
        uiObject.gameObject.SetActive(false);
    }

    private void onDaisenNextPage()
    {
        daisenNum++;
        if (daisenNum > daisenInfos.Count - 1)
        {
            //close information Popup
            daisenNum = 0;
            daisenInfoUI.gameObject.SetActive(false);
            daisenInfoUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[_lang == "Eng" ? 1 : 0];
        }
        else if (daisenNum == daisenInfos.Count - 1)
        {
            daisenInfoUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[0];
        }
        daisenInfoUI.GetComponent<Image>().sprite = daisenInfos[daisenNum];
    }

    private void onDuneNextPage()
    {
        duneNum++;
        if (duneNum > duneInfos.Count - 1)
        {
            //close information Popup
            duneNum = 0;
            duneInfoUI.gameObject.SetActive(false);
            duneInfoUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[1];
        }
        else if (duneNum == duneInfos.Count - 1)
        {
            duneInfoUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[0];
        }
        duneInfoUI.GetComponent<Image>().sprite = duneInfos[duneNum];
    }

    private void onWorshipNextPage()
    {
        worshipNum++;
        if (worshipNum > worshipInfos.Count - 1)
        {
            //close information Popup
            worshipNum = 0;
            worshipInfoUI.gameObject.SetActive(false);
            worshipInfoUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[1];
            DataManager_Shrine.GetComponent<DataManager_Shrine>().GetPlayerData();
        }
        else if (worshipNum == worshipInfos.Count - 1)
        {
            worshipInfoUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[2];
        }
        worshipInfoUI.GetComponent<Image>().sprite = worshipInfos[worshipNum];
    }

    private void onWorshipFailNextPage()
    {
        worshipFailNum++;
        if (worshipFailNum > worshipFails.Count - 1)
        {
            worshipFailNum = 0;
            SetWorshipFailUI(false);
            ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = true;
            PlayerController.PlayerIsIdle?.Invoke();
            Enable_DisableObjects.Instance.DisableDashButton(true);
            worshipFailUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[_lang == "Eng" ? 1 : 0];
        }
        else if (worshipFailNum == worshipFails.Count - 1)
        {
            worshipFailUI.GetChild(0).GetComponent<Image>().sprite = buttonImages[0];
        }
        worshipFailUI.GetComponent<Image>().sprite = worshipFails[worshipFailNum];
    }

    public void SetWorshipFailUI(bool active)
    {
        worshipFailUI.gameObject.SetActive(active);
    }

    private void openPopUp(Button button)
    {

        int idx = button.transform.GetSiblingIndex() >= (int)PointerUIChild.Emikage1 ? (int)PointerUIChild.Emikage1 : button.transform.GetSiblingIndex();
        if (idx == (int)PointerUIChild.Temizuya)
        {
            if (_playerAnimator == null)
            {
                _playerAnimator = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().animator;

            }
            IKMuseum iKMuseum = _playerAnimator.gameObject.GetComponent<IKMuseum>();
            iKMuseum.uIController_Shine = this;
            iKMuseum.RPCForKanzakiGourdEnable();

            //Disabling gameplay UI intereactions
            PlayerController.PlayerIsWalking?.Invoke();
            ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = false;
            Enable_DisableObjects.Instance.DisableDashButton(false);

            _playerAnimator.SetTrigger("KanzakiWorld");
            _playerAnimator.SetTrigger("Washing"); //animation ending funchionality is in IKMuseum.cs from animation end events

            Transform player = ReferencesForGamePlay.instance.MainPlayerParent.transform;
            Vector3 directionToMizuya = (mizuya.position - player.transform.position).normalized;
            Vector3 newPosition = mizuya.position - directionToMizuya * 1.0f;
            player.transform.position = newPosition;

            Vector3 lookDirection = mizuya.position - player.transform.position;
            float angleY = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
            player.transform.rotation = Quaternion.Euler(0, angleY, 0);


        }
        else
        {
            int gamespaceIdx = -1;
            switch ((PointerUIChild)idx)
            {
                case PointerUIChild.Worship:
                    gamespaceIdx = (int)GameSpaceUIChild.Worship;
                    //Disabling gameplay UI intereactions
                    PlayerController.PlayerIsWalking?.Invoke();
                    ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = false;
                    Enable_DisableObjects.Instance.DisableDashButton(false);
                    break;
                case PointerUIChild.DuneGuide:
                    gamespaceIdx = (int)GameSpaceUIChild.DuneGuide;
                    break;
                case PointerUIChild.DaisenGuide:
                    gamespaceIdx = (int)GameSpaceUIChild.DaisenGuide;
                    break;
                case PointerUIChild.Emikage1:
                    gamespaceIdx = (int)GameSpaceUIChild.Emikage;
                    Enable_DisableObjects.Instance.EnableDisableUIObjects(false);
                    GamePlayUIHandler.inst.gamePlayUIParent.SetActive(false);
                    PlayerController.PlayerIsWalking?.Invoke();
                    ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = false;
                    break;
                default:
                    break;
            }
            if (gamespaceIdx != -1)
                gameSpaceUI.transform.GetChild(gamespaceIdx).gameObject.SetActive(true);
            //if (gamespaceIdx == 3)
            //{
            //    StartCoroutine(ScrollSizeControl());
            //}
        }
        button.gameObject.SetActive(false);
    }

    IEnumerator ScrollSizeControl()
    {
        yield return new WaitForSeconds(0.5f);
        emikageUI.GetChild(0).GetChild(0).GetChild(1).GetComponent<Scrollbar>().size = 0;
    }


    void FindChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name == "RightHand")
            {
                Instantiate(gourd, child);
                break;
            }
            FindChildren(child);
        }
    }

    private void playWorshipAnimation()
    {
        if (_playerAnimator == null)
        {
            _playerAnimator = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().animator;
            _playerAnimator.gameObject.GetComponent<IKMuseum>().uIController_Shine = this;
            Enable_DisableObjects.Instance.DisableDashButton(false);
        }
        _playerAnimator.SetTrigger("KanzakiWorld"); //animation ending funchionality is in IKMuseum.cs from animation end events

        if (worshipAnimNum == 0)
        {

            _playerAnimator.SetTrigger("BowTwice");

        }
        else if (worshipAnimNum == 1)
        {

            _playerAnimator.SetTrigger("Clap");

        }
        else if (worshipAnimNum == 2)
        {

            _playerAnimator.SetTrigger("Pray");

        }
        else if (worshipAnimNum == 3)
        {
            _playerAnimator.SetTrigger("BowOnce");
        }
        worshipAnimNum++;
        Debug.Log(worshipAnimNum);
        Debug.Log(worshipGameUI);
        worshipGameUI.gameObject.SetActive(false);
        if (worshipAnimNum > 3)
        {
            worshipAnimNum = 0;
        }
        worshipGameUI.GetComponent<RawImage>().texture = worshipAnimIconImages[worshipAnimNum].texture;
    }

    private void getRandomOmikuzi()
    {
        omikuziUI.gameObject.SetActive(false);
        ohudouUI.gameObject.SetActive(true);

        ohudouUI.GetChild(1).GetComponent<Image>().sprite = omikuziImages[UnityEngine.Random.Range(0, omikuziImages.Count)];
    }

    private void closeOhudouUI()
    {
        ohudouUI.gameObject.SetActive(false);
        ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = true;
        PlayerController.PlayerIsIdle?.Invoke();
        Enable_DisableObjects.Instance.DisableDashButton(true);
    }
}