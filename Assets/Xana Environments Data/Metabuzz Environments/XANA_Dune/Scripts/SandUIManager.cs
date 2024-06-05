using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using EnumCheck;


public class SandUIManager : MonoBehaviour
{
    [SerializeField] List<Sprite> startInformImagesJp;
    [SerializeField] List<Sprite> sandGameImagesJp;
    [SerializeField] List<Sprite> startInformImagesEn;
    [SerializeField] List<Sprite> sandGameImagesEn;
    [SerializeField] List<Sprite> timerImages;

    [SerializeField] Image description;
    [SerializeField] Image result;
    [SerializeField] Image rankingBoard;
    [SerializeField] Image timer;
    [SerializeField] Image joyStick;
    [SerializeField] Image Point;

    [SerializeField] Button nxtBtn;
    [SerializeField] Button camelBtn;
    [SerializeField] Button camel2Btn;

    Dictionary<Des, List<Sprite>> desImgs = new Dictionary<Des, List<Sprite>>();
    Dictionary<Des, UnityAction> desCallback = new Dictionary<Des, UnityAction>();

    List<Sprite> currentDes;
    int currentIndex = 0;

    Des currentKey;

    void Start()
    {
        nxtBtn.onClick.AddListener(OnNextBtnClick);
        if (SandGameManager.Instance.local == Localiztion.Jp)
        {
            desImgs.Add(Des.StartInform, startInformImagesJp);
            desImgs.Add(Des.SandInform, sandGameImagesJp);
        }
        else if (SandGameManager.Instance.local == Localiztion.En)
        {
            desImgs.Add(Des.StartInform, startInformImagesEn);
            desImgs.Add(Des.SandInform, sandGameImagesEn);
        }
        joyStick.gameObject.SetActive(false);
        camelBtn.onClick.AddListener(() => DescriptionStart(Des.SandInform));
        camel2Btn.onClick.AddListener(() => SandGameManager.Instance.ResetPlayerPos());

        DescriptionStart(Des.StartInform);
    }

    public void AddCallback(Des des, UnityAction callback)
    {
        desCallback[des] = callback;
    }

    public void DescriptionStart(Des des)
    {
        currentDes = desImgs[des];
        currentKey = des;
        description.gameObject.SetActive(true);

        description.sprite = currentDes[currentIndex];
    }

    void OnNextBtnClick()
    {
        if (currentDes.Count - 1 <= currentIndex)
        {
            description.gameObject.SetActive(false);
            currentIndex = 0;
            description.sprite = null;
            currentDes = null;

            if (desCallback.ContainsKey(currentKey))
            {
                desCallback[currentKey].Invoke();
            }
        }
        else
        {
            currentIndex++;

            description.sprite = currentDes[currentIndex];
        }
    }

    public void TimerStart()
    {
        timer.gameObject.SetActive(true);
        joyStick.gameObject.SetActive(true);
        Point.gameObject.SetActive(false);

        StartCoroutine(TimeStart());
    }

    IEnumerator TimeStart()
    {
        int i = 0;
        float currTime = 0;

        timer.sprite = timerImages[i];
        Image effect = timer.transform.GetChild(0).GetComponent<Image>();

        while (i < 3)
        {
            currTime += Time.deltaTime;
            
            if (currTime >= 1)
            {
                currTime = 0;
                i++;
                if (i != 3)
                    timer.sprite = timerImages[i];
            }
            effect.fillAmount = currTime;
            yield return null;
            //yield return new WaitForSeconds(1);
        }

        timer.gameObject.SetActive(false);
    }

    public void OpenPointUI(){
        Point.gameObject.SetActive(true);
    }

    public void SetPointUI(string point){        
        Point.transform.GetChild(0).GetComponent<Text>().text = point;
    }

    public void ShowResult(string playRecord,string bestRecord, string rank, string reward)
    {
        joyStick.gameObject.SetActive(false);

        result.gameObject.SetActive(true);
        result.transform.GetChild(1).GetComponent<Text>().text = playRecord;
        result.transform.GetChild(2).GetComponent<Text>().text = bestRecord;
        result.transform.GetChild(3).GetComponent<Text>().text = rank;
        result.transform.GetChild(4).GetComponent<Text>().text = reward;

        StartCoroutine(ResultTween(500f));
    }

    IEnumerator ResultTween(float tweenSpeed)
    {
        float width = result.rectTransform.rect.width;

        Vector3 origin = result.transform.localPosition;
        result.transform.localPosition += new Vector3(width, 0, 0);
        while (true)
        {
            result.transform.position -= new Vector3(tweenSpeed * Time.deltaTime, 0, 0);

            if (result.rectTransform.localPosition.x - origin.x <= 0)
            {
                result.rectTransform.localPosition = origin;
                break;
            }

            yield return null;
        }
    }

    public void SetRankingBoard(Dictionary<string, string[]> rankingList)
    {
        foreach (KeyValuePair<string, string[]> item in rankingList)
        {
            Transform rank = rankingBoard.transform.GetChild(int.Parse(item.Value[2]) - 1);

            rank.GetChild(0).GetComponent<Text>().text = item.Key;
            rank.GetChild(1).GetComponent<Text>().text = item.Value[1];
        }

        //for (int i = 0; i < rankingList.Count; i++)
        //{
        //    Transform rank = rankingBoard.transform.GetChild(i);

        //    rank.GetChild(0).GetComponent<Text>() = rankingList.
        //}
    }
}


