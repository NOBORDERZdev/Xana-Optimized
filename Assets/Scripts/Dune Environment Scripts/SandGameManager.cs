using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumCheck;
using UnityEngine.Networking;
using Unity.VisualScripting;
using Cinemachine;
using UnityEngine.UI;

public class SandGameManager : MonoBehaviour
{
    public Localiztion local = Localiztion.En;

    private static SandGameManager instance = null;
    [SerializeField]
    private GameObject _skateBoardPrefab;

    [SerializeField]
    private GameObject _markPrefab;
    [SerializeField] private CrabSpawner crabSpr;
    [SerializeField] private SandUIManager uiMgr;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform resetPoint;
    private Transform player;
    private Transform mark;
    private Transform board;

    [SerializeField] private ParticleSystem finishParticle1;
    [SerializeField] private ParticleSystem finishParticle2;

    [SerializeField] private AudioSource rewardSound;
    [SerializeField] private Button resetBtn;

    private InputManager input;

    private Vector3 _player34InitialPos;
    private bool _isSkatingControllerOn = false;
    private string url = "https://7cjaa2ckmj.execute-api.ap-northeast-1.amazonaws.com/default/Lambda-XANA";
    private Dictionary<string, string[]> rankList = new Dictionary<string, string[]>();
    private const int startTime = 3;
    private bool isStart = false;
    private float timer = 0;
    private float record = 0;
    private string id = "88";
    private Animator Animator34
    {
        get
        {
            return ReferencesForGamePlay.instance.m_34player.GetComponent<Animator>();
        }
    }
    public float Timer
    {
        get
        {
            return Mathf.Floor(timer);
        }

        set
        {
            timer = value;
        }
    }
    public Transform Player
    {
        get
        {
            return player;
        }
    }
    public static SandGameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        ConstantsHolder.xanaConstants.comingFrom = ConstantsHolder.ComingFrom.Dune;

        player = ReferencesForGamePlay.instance.MainPlayerParent.transform;
        player.AddComponent<XanaDuneControllerHandler>();

        if (LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
        {
            local = Localiztion.Jp;
        }
        else
        {
            local = Localiztion.En;
        }
        if (ConstantsHolder.userId != null)
        {
            id = ConstantsHolder.userId;
        }
        Debug.Log("User ID: " + id);
    }

    public void EnableSkating()
    {
        StartCoroutine(InitRoutine());
        ReferencesForGamePlay.instance.MainPlayerParent.transform.rotation = Quaternion.Euler(0, 90, 0);
    }
    public void DisableSkating()
    {
        SetBoardOff();
        uiMgr.TimerOn(false);

        isStart = false;
        timer = 0;
        uiMgr.SetTimerText("00.00");
        resetBtn.gameObject.SetActive(false);
        crabSpr.OnCrabStop();
    }

    IEnumerator InitRoutine()
    {
        yield return new WaitUntil(() => ReferencesForGamePlay.instance.m_34player);

        board = Instantiate(_skateBoardPrefab, ReferencesForGamePlay.instance.MainPlayerParent.transform, false).transform;
        ReferencesForGamePlay.instance.spawnedSkateBoard = board.gameObject;
        mark = Instantiate(_markPrefab, ReferencesForGamePlay.instance.MainPlayerParent.transform, false).transform;

        input = board.GetComponent<InputManager>();
        uiMgr.AddCallback(Des.SandInform, () => { StartBoarding(); });
        resetBtn.onClick.AddListener(() => { ResetPlayer(); });
        resetBtn.gameObject.SetActive(false);

        StartCoroutine(CheckPoint());
        SetRanking();
        StopCoroutine(InitRoutine());
    }

    IEnumerator CheckPoint()
    {
        WWWForm form = new WWWForm();
        form.AddField("command", "getPoint");
        form.AddField("id", id);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        string point = www.downloadHandler.text;
        if (point == "There is no player Data") point = "0";
        Debug.Log("Sohaib API Response getPoint : " + point);
        if (point == "Register complete") point = "0";
        uiMgr.SetPointUI(point);

        www.Dispose();
    }

    public void TeleportSand()
    {
        uiMgr.DescriptionStart(Des.StartInform);
    }

    public void StartBoarding()
    {
        _isSkatingControllerOn = true;
        SwitchToSkatingController();
        isStart = true;
        Animator34.SetBool("IsStart", true);
        SetPlayerStartPos();
        resetBtn.gameObject.SetActive(true);
        uiMgr.TimerStart();
        uiMgr.TimerOn(true);
        StartCoroutine(StartBoardingControl());
    }

    public void SwitchToSkatingController()
    {
        if (_isSkatingControllerOn)
        {
            //disable XANA controller and enable skating controller
            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<CharacterController>().enabled = false;
            ReferencesForGamePlay.instance.m_34player.GetComponent<CharacterController>().enabled = false;
            foreach (CapsuleCollider child in ReferencesForGamePlay.instance.m_34player.GetComponents<CapsuleCollider>())
            {
                child.enabled = false;
            }
            _player34InitialPos = ReferencesForGamePlay.instance.m_34player.transform.localPosition;
            ReferencesForGamePlay.instance.m_34player.transform.localPosition = new Vector3(_player34InitialPos.x, 0.014f, _player34InitialPos.z);
            Rigidbody playerRb = player.AddComponent<Rigidbody>();
            playerRb.mass = 0.1f;
            playerRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            board.GetComponent<FixedJoint>().connectedBody = playerRb;
            player.GetComponent<XanaDuneControllerHandler>().EnableSkating();
            GameplayEntityLoader.instance.PlayerCamera.m_XAxis.Value = -88f;
            GameplayEntityLoader.instance.PlayerCamera.m_YAxis.Value = 1f;
            PlayerCameraController.instance.lockRotation = true;
            PlayerCameraController.instance.gameObject.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Radius = 2.33f;
            PlayerCameraController.instance.gameObject.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Height = 2.57f;
        }
        else
        {
            //enable XANA controller and disable skating controller
            player.GetComponent<XanaDuneControllerHandler>().DisableSkating();
            Destroy(player.GetComponent<Rigidbody>());
            foreach (CapsuleCollider child in ReferencesForGamePlay.instance.m_34player.GetComponents<CapsuleCollider>())
            {
                child.enabled = true;
            }
            ReferencesForGamePlay.instance.m_34player.transform.localPosition = _player34InitialPos;
            player.GetComponent<CharacterController>().enabled = true;
            ReferencesForGamePlay.instance.m_34player.GetComponent<CharacterController>().enabled = true;
            player.GetComponent<PlayerController>().enabled = true;
            PlayerCameraController.instance.lockRotation = false;
            PlayerCameraController.instance.gameObject.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Radius = 1.75f;
            PlayerCameraController.instance.gameObject.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Height = 2.47f;
        }
    }

    IEnumerator StartBoardingControl()
    {
        yield return new WaitForSeconds(0.1f);
        uiMgr.SetTimerText("00.00");
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.mass = 0.1f;
        rb.freezeRotation = false;
        board.gameObject.SetActive(true);
        input.enabled = true;
        input.force = 250;
        input.StopMove();
        int i = 0;
        Debug.Log("Boarding ready set");

        while (i < startTime)
        {
            i++;

            yield return new WaitForSeconds(1);
        }

        input.canRotate = true;
        input.force = input.startForce;
        Animator34.SetTrigger("BoardOn");
        Debug.Log("Boarding Start");
        crabSpr.OnCrabStart();

        while (isStart)
        {
            timer += Time.deltaTime;
            string timerFormat = string.Format("{0:00.00}", timer);
            uiMgr.SetTimerText(timerFormat);
            yield return null;
        }

        StopCoroutine(StartBoardingControl());
    }

    public void GameOver()
    {
        if (!isStart) return;
        record = timer;
        timer = 0;
        isStart = false;
        crabSpr.OnCrabStop();
        //print(record);
        input.canRotate = false;
        rewardSound.Play();
        finishParticle1.Play();
        finishParticle2.Play();
        uiMgr.TimerOn(false);
        resetBtn.gameObject.SetActive(false);
        StartCoroutine(OnGameOver());
    }

    IEnumerator OnGameOver()
    {
        string recordString = string.Format("{0:00.00}", record);
        string playRecordString = recordString;

        WWWForm formSave = new WWWForm();
        formSave.AddField("command", "saveRecord");
        formSave.AddField("id", id);
        formSave.AddField("record", recordString);
        UnityWebRequest wwwSave = UnityWebRequest.Post(url, formSave);
        wwwSave.SendWebRequest();
        yield return new WaitUntil(() => wwwSave.isDone);
        Debug.Log("Sohaib API Response saveRecord : " + wwwSave.downloadHandler.text);

        WWWForm formPersonalRank = new WWWForm();
        formPersonalRank.AddField("command", "getPersonalRank");
        formPersonalRank.AddField("id", id);

        UnityWebRequest wwwPersonalRank = UnityWebRequest.Post(url, formPersonalRank);
        wwwPersonalRank.SendWebRequest();
        yield return new WaitUntil(() => wwwPersonalRank.isDone);

        string personalRank = wwwPersonalRank.downloadHandler.text;
        Debug.Log("personalRank : " + personalRank);

        string unit = "";   

        int rankReward = 0;
        int playReward = 0;
        float personalRankFloat = 0;

        if (float.TryParse(personalRank,out float result))
        {
            personalRankFloat = result;
        }
        else
        {
            Debug.Log("Error in parsing personal rank : " + personalRank);
        }

        switch ((int)personalRankFloat)
        {
            case 1:
                rankReward = 100;
                unit = "st";
                break;
            case 2:
                rankReward = 70;
                unit = "nd";
                break;
            case 3:
                rankReward = 50;
                unit = "rd";
                break;
            default:
                rankReward = 0;
                unit = "th";
                break;
        }

        if (record < 14)
        {
            playReward = 150;
        }
        else if (record < 20)
        {
            playReward = 120;
        }
        else if (record < 30)
        {
            playReward = 100;
        }
        else if (record >= 30)
        {
            playReward = 10;
        }

        string rewardString = rankReward != 0 ? $"{playReward}(+{rankReward})" : $"{playReward}";
        int totalReward = rankReward == 0 ? playReward : playReward + rankReward;
        WWWForm formReward = new WWWForm();
        formReward.AddField("command", "savePoint");
        formReward.AddField("id", id);
        formReward.AddField("point", totalReward);

        UnityWebRequest wwwReward = UnityWebRequest.Post(url, formReward);
        wwwReward.SendWebRequest();
        yield return new WaitUntil(() => wwwReward.isDone);
        Debug.Log("Sohaib API Response savePoint : " + wwwReward.downloadHandler.text);
        StartCoroutine(CheckPoint());
        if ((int)personalRankFloat <= 10)
        {
            SetRanking();
        }
        wwwSave.Dispose();
        wwwPersonalRank.Dispose();
        wwwReward.Dispose();


        uiMgr.ShowResult(playRecordString, personalRank + unit, rewardString);
    }

    private void SetRanking()
    {
        StartCoroutine(SetRankingData());
    }

    IEnumerator SetRankingData()
    {
        WWWForm form = new WWWForm();
        form.AddField("command", "getRank");

        using UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            StartCoroutine(SetRankingData());
            Debug.Log("ConnectionError getRank");

        }
        else
        {
            string rank = www.downloadHandler.text;
            Debug.Log(rank);

            string[] ranks = rank.Split("\n");
            for (int i = 0; i < ranks.Length; i++)
            {
                string[] _ranks = ranks[i].Split(",");

                string[] _rank = { _ranks[1], _ranks[3], (i + 1).ToString() };
                rankList.Add(_ranks[0], _rank);
            }

            uiMgr.SetRankingBoard(rankList);
            rankList.Clear();
        }
        //www.Dispose();
    }

    void SetPlayerStartPos()
    {
        Quaternion currentRot = ReferencesForGamePlay.instance.randerCamera.transform.rotation;
        float offset = 30;

        float y = currentRot.eulerAngles.x;
        if (y > 180)
        {
            Debug.Log(y);
            Debug.Log(offset);
            y -= 360;
        }
        SetPlayerPosition(startPoint.position);
    }

    void SetPlayerPosition(Vector3 pos)
    {
        Player.position = pos;
        Player.rotation = Quaternion.Euler(0, 30, 0);
    }
    private bool toResetWithDelay = true;
    public void ResetPlayer()
    {
        if (toResetWithDelay)
        {
            StartCoroutine(OnResetPlayer());
        }
    }

    IEnumerator OnResetPlayer()
    {
        toResetWithDelay = false;
        input.force = 0;
        input.StopMove();
        Vector3 currPos = player.position;
        Vector3 newPos = currPos + new Vector3(0, 1f, 0);
        Quaternion initRot = Quaternion.Euler(20, 30, 6);
        player.position = newPos;
        player.rotation = initRot;

        yield return new WaitForSeconds(1);
        input.force = input.startForce;

        yield return new WaitForSeconds(4);
        toResetWithDelay = true;
    }
    public void ResetPlayerPos()
    {
        StartCoroutine(ReturnPlayer());
    }

    IEnumerator ReturnPlayer()
    {
        SetPlayerPosition(resetPoint.position);

        yield return new WaitForSeconds(0.1f);

        //playerInput.enabled = true;
    }
    public void SetBoardOff()
    {
        board.gameObject.SetActive(false);
        Animator34.SetBool("IsStart", false);
        _isSkatingControllerOn = false;
        SwitchToSkatingController();

        uiMgr.OpenPointUI();
    }

    public void MarkOn()
    {
        StartCoroutine(Mark());
    }

    // caution before claw up
    IEnumerator Mark()
    {
        mark.gameObject.SetActive(true);

        yield return new WaitForSeconds(1);

        mark.gameObject.SetActive(false);
    }
}

