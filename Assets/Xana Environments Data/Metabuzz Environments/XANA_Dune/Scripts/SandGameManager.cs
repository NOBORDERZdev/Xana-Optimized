using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumCheck;
using UnityEngine.Networking;

public class SandGameManager : MonoBehaviour
{
    private static SandGameManager instance = null;

    [SerializeField] CrabSpawner crabSpr;
    [SerializeField] SandUIManager uiMgr;

    [SerializeField] Transform startPoint;
    [SerializeField] Transform resetPoint;
    [SerializeField] Transform player;
    [SerializeField] Transform mark;
    [SerializeField] Transform board;

    [SerializeField] ParticleSystem finishParticle1;
    [SerializeField] ParticleSystem finishParticle2;

    InputManager input;

    Animator animator;

    //vThirdPersonInput playerInput;
    Transform playerCamera;

    private string url = "https://7cjaa2ckmj.execute-api.ap-northeast-1.amazonaws.com/default/Lambda-XANA";

    Dictionary<string, string[]> rankList = new Dictionary<string, string[]>();
    public Transform Player
    {
        get
        {
            return player;
        }
    }

    const int startTime = 3;
    bool isStart = false;
    float timer = 0;
    float limitTime = 60;
    float record = 0;
    
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

    public string id = "88";

    public Localiztion local = Localiztion.Jp;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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

    IEnumerator Test()
    {
        while (true)
        {
            //playerCamera.RotateCamera(0, 5);

            yield return new WaitForSeconds(1);
        }
    }


    void Start()
    {
        //playerInput = player.GetComponent<vThirdPersonInput>();
        //playerCamera = Camera.main.GetComponent<vThirdPersonCamera>();
        input = board.GetComponent<InputManager>();
        animator = player.GetComponent<Animator>();

        uiMgr.AddCallback(Des.SandInform, () => { StartBoarding(); });

        StartCoroutine(CheckPoint());
        //StartCoroutine(Test());
    }

    IEnumerator CheckPoint(){
        WWWForm form = new WWWForm();
        form.AddField("command", "getPoint");
        form.AddField("id", id);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        string point = www.downloadHandler.text;
        uiMgr.SetPointUI(point);

        www.Dispose();
    }

    public void TeleportSand()
    {
        uiMgr.DescriptionStart(Des.StartInform);
    }

    public void StartBoarding()
    {
        isStart = true;
        animator.SetBool("IsStart", true);
        SetPlayerStartPos();

        uiMgr.TimerStart();
        StartCoroutine(StartBoardingControl());
    }

    IEnumerator StartBoardingControl()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.mass = 0.1f;
        rb.freezeRotation = false;
        
        input.enabled = true;
        input.force = 250;

        board.gameObject.SetActive(true);
        int i = 0;
        Debug.Log("Boarding ready set");

        while (i < startTime)
        {
            i++;

            yield return new WaitForSeconds(1);
        }

        input.canRotate = true;
        input.force = 600;
        animator.SetTrigger("BoardOn");
        Debug.Log("Boarding Start");
        crabSpr.OnCrabStart();

        while (isStart)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void GameOver()
    {
        if (!isStart) return;
        record = timer;
        timer = 0;
        isStart = false;
        crabSpr.OnCrabStop();
        print(record);
        input.canRotate = false;

        finishParticle1.Play();
        finishParticle2.Play();

        StartCoroutine(OnGameOver());
    }

    IEnumerator OnGameOver()
    {
        string recordString = string.Format("{0:00.00}", record);
        string playRecordString = recordString;

        WWWForm form = new WWWForm();
        form.AddField("command", "getRank");
        form.AddField("id", id);
        form.AddField("record", timer.ToString());

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        string rank = www.downloadHandler.text;
        Debug.Log(rank);

        string[] ranks = rank.Split("\n");
        for (int i = 0; i < ranks.Length; i ++)
        {
            string[] _ranks = ranks[i].Split(",");

            string[] _rank = { _ranks[1], _ranks[2], (i + 1).ToString() };
            rankList.Add(_ranks[0], _rank);
        }

        int rankReward = 0;
        
        if (float.Parse(rankList[id][1]) > record)
        {

            int prevRank = int.Parse(rankList[id][2]);

            WWWForm formSave = new WWWForm();
            formSave.AddField("command", "saveRecord");
            formSave.AddField("id", id);
            formSave.AddField("record", recordString);

            UnityWebRequest wwwSave = UnityWebRequest.Post(url, formSave);
            yield return wwwSave.SendWebRequest();

            UnityWebRequest wwwRank = UnityWebRequest.Post(url, form);
            yield return wwwRank.SendWebRequest();
            rank = wwwRank.downloadHandler.text;
            Debug.Log(rank);
            ranks = rank.Split("\n");
            for (int i = 0; i < ranks.Length; i++)
            {
                string[] _ranks = ranks[i].Split(",");

                string[] _rank = { _ranks[1], _ranks[2], (i + 1).ToString() };
                rankList[_ranks[0]] = _rank;
            }

            int curRank = int.Parse(rankList[id][2]);

            if (curRank < prevRank)
            {
                switch (int.Parse(rankList[id][2]))
                {
                    case 1:
                        rankReward = 500;
                        break;
                    case 2:
                        rankReward = 200;
                        break;
                    case 3:
                        rankReward = 100;
                        break;
                    case 4:
                        rankReward = 70;
                        break;
                    case 5:
                        rankReward = 70;
                        break;
                    case 6:
                    case 7:
                    case 8:
                        rankReward = 60;
                        break;
                    case 9:
                    case 10:
                        rankReward = 50;
                        break;
                    default:
                        break;
                }
            }


            wwwRank.Dispose();
            wwwSave.Dispose();
        }
        else if (float.Parse(rankList[id][1]) != 0)
        {
            recordString = rankList[id][1];

        }
        else
        {
            WWWForm formSave = new WWWForm();
            formSave.AddField("command", "saveRecord");
            formSave.AddField("id", id);
            formSave.AddField("record", recordString);

            UnityWebRequest wwwSave = UnityWebRequest.Post(url, formSave);
            yield return wwwSave.SendWebRequest();

            UnityWebRequest wwwRank = UnityWebRequest.Post(url, form);
            yield return wwwRank.SendWebRequest();
            rank = wwwRank.downloadHandler.text;
            Debug.Log(rank);
            ranks = rank.Split("\n");
            for (int i = 0; i < ranks.Length; i++)
            {
                string[] _ranks = ranks[i].Split(",");

                string[] _rank = { _ranks[1], _ranks[2], (i + 1).ToString() };
                rankList[_ranks[0]] = _rank;
            }
            wwwRank.Dispose();
            wwwSave.Dispose();
        }

        int reward = 0;

        int playReward = 0;

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

        string rewardString = rankReward != 0 ? $"{playReward}(+{rankReward})" : $"{playReward}";
        WWWForm formReward = new WWWForm();
        formReward.AddField("command", "savePoint");
        formReward.AddField("id", id);
        formReward.AddField("point", rewardString);

        UnityWebRequest wwwReward = UnityWebRequest.Post(url, formReward);
        yield return wwwReward.SendWebRequest();
        Debug.Log(wwwReward.downloadHandler.text);          
        StartCoroutine(CheckPoint());

        wwwReward.Dispose();
        www.Dispose();
        
        uiMgr.SetRankingBoard(rankList);
        uiMgr.ShowResult(playRecordString, recordString, rankList[id][2] + "nd", rewardString);
        yield return null;

        rankList.Clear();
    }

    void SetPlayerStartPos()
    {
        //Quaternion currentRot = playerInput.cameraMain.transform.rotation;
        //float offset = 30;

        //float y = currentRot.eulerAngles.x;
        //if (y > 180)
        //{
        //    Debug.Log(y);
        //    Debug.Log(offset);
        //    y -= 360;
        //}
        //playerCamera.RotateCamera(-currentRot.eulerAngles.y / 3, (y - offset) / 3);

        SetPlayerPosition(startPoint.position);
    }

    void SetPlayerPosition(Vector3 pos)
    {
        //playerInput.enabled = false;
        Player.position = pos;
        Player.rotation = Quaternion.Euler(0, 30, 0);
    }

    public void ResetPlayerPos()
    {
        StartCoroutine(ResetPlayer());
    }

    IEnumerator ResetPlayer()
    {
        SetPlayerPosition(resetPoint.position);

        yield return new WaitForSeconds(1);

        //playerInput.enabled = true;
    }

    public void SetBoardOff()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        rb.mass = 50f;
        rb.freezeRotation = true;

        board.gameObject.SetActive(false);
        //playerInput.enabled = true;
        animator.SetBool("IsStart", false);
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

