using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

    enum BoardChild
    {
        WriteBtn,
        WrtieBoard,
    }
    enum WriteBoardChild
{  
        InputTitle,
        InputContent,
        CloseBtn,
        PostBtn,
        ColorSelectBtn,
        ColorPanel,
        CancelBtn
    }
public class Emikage : MonoBehaviour
{

    [SerializeField] GameObject writeBoard;
    [SerializeField] GameObject writeBoardMask;
    [SerializeField] Button createPost;
    [SerializeField] Button closePost;
    [SerializeField] GameObject colorSelectBtn;
    [SerializeField] GameObject colorPanel;
    [SerializeField] GameObject GridContent;
    [SerializeField] GameObject BoardContentPrefab;
    [SerializeField] Image HandleImage;
    List<EmikageBoardContent> boards;
    List<Included> Includeds;
    TMP_InputField InputTitle;
    TMP_InputField InputContent;

    [SerializeField] Color[] colors;
    private string pdltUrl = "https://api.padlet.dev/v1/boards/pm18k4b03ik18lby";
    private string api_Key = "pdltp_a442152fc8402d1207d54c7bc89c9b0542c3bc70207dc4ce825ef6f945755ef5bea535";
    void Awake()
    {
        boards = new List<EmikageBoardContent>(GridContent.transform.childCount);
        Includeds = new List<Included>();
        createPost.onClick.AddListener(onCreatePost);
        closePost.onClick.AddListener(() => { 
            gameObject.SetActive(false);
            PlayerController.PlayerIsIdle?.Invoke();
            ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = true;
            GamePlayUIHandler.inst.gamePlayUIParent.SetActive(true);
        });
        writeBoard.transform.GetChild((int)WriteBoardChild.CloseBtn).GetComponent<Button>().onClick.AddListener(closeBtnClick);
        writeBoard.transform.GetChild((int)WriteBoardChild.PostBtn).GetComponent<Button>().onClick.AddListener(onPostOnBoard);
        writeBoard.transform.GetChild((int)WriteBoardChild.CancelBtn).GetComponent<Button>().onClick.AddListener(cancelPost);
        writeBoardMask.GetComponent<Button>().onClick.AddListener(closeBtnClick);
        InputTitle = writeBoard.transform.GetChild((int)WriteBoardChild.InputTitle).GetComponent<TMP_InputField>();
        InputContent = writeBoard.transform.GetChild((int)WriteBoardChild.InputContent).GetComponent<TMP_InputField>();

        colorSelectBtn.GetComponent<Button>().onClick.AddListener(onColorPanelOn);
        Button[] colorBtns = colorPanel.GetComponentsInChildren<Button>();
        colorBtns[0].onClick.AddListener(onColorPanelOff);
        for (int i = 1; i < colorBtns.Length; i++)
        {
            int index = i;
            colorBtns[i].onClick.AddListener(() => selectColor(colorBtns[index]));
        }

        colorPanel.SetActive(false);
        writeBoard.SetActive(false);
        writeBoardMask.SetActive(false);
        HandleImage.enabled = false;


        for (int i = 0; i< GridContent.transform.childCount ; i++)
        {
            boards.Add(GridContent.transform.GetChild(i).GetComponent<EmikageBoardContent>());
        }
        StartCoroutine(GetRequst());

        //gameObject.SetActive(false);

    }

    private void cancelPost() {
        writeBoard.SetActive(false);
        InputTitle.text = "";
        InputContent.text = "";
    }

    private void OnEnable()
    {
        HandleImage.enabled = false;
        StartCoroutine(GetRequst());
    }


    private void onCreatePost()
    {
        writeBoard.SetActive(true);
        writeBoardMask.SetActive(true);
    }
    private void closeBtnClick()
    {
        writeBoard.SetActive(false);
        writeBoardMask.SetActive(false);
    }
    private void onColorPanelOn()
    {
        colorPanel.SetActive(true);
    }
    private void onColorPanelOff()
    {
        colorPanel.SetActive(false);
    }
    private void selectColor(Button colorBtn)
    {
        colorSelectBtn.transform.GetChild(0).GetComponent<RawImage>().color = colorBtn.transform.GetChild(0).GetComponent<RawImage>().color;
        colorSelectBtn.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = colorBtn.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text;
        colorPanel.SetActive(false);
    }

    private void onPostOnBoard()
    {
        closeBtnClick();
        string title = InputTitle.text;
        string content = InputContent.text;
        string color = colorSelectBtn.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text.ToLower();
        StartCoroutine(PostRequest(title, content, color));

        InputTitle.text = "";
        InputContent.text = "";
    }
    IEnumerator PostRequest(string title, string content, string color)
    {
        // 요청 데이터 생성
        string jsonBody = color == "white" ? $"{{\"data\":{{\"type\":\"post\",\"attributes\":{{\"content\":{{\"subject\":\"{title}\",\"body\":\"{content}\"}}}}}}}}" : $"{{\"data\":{{\"type\":\"post\",\"attributes\":{{\"content\":{{\"subject\":\"{title}\",\"body\":\"{content}\"}},\"color\":\"{color}\"}}}}}}";

        //UnityWebRequest 생성
        using (UnityWebRequest www = new UnityWebRequest(pdltUrl + "/posts", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            //헤더추가
            www.SetRequestHeader("accept", "application/vnd.api+json");
            www.SetRequestHeader("X-Api-Key", api_Key);
            www.SetRequestHeader("Content-Type", "application/vnd.api+json");

            //요청 보내기
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
            }

        }
        StartCoroutine(GetRequst());
    }

    IEnumerator GetRequst()
    {
        string finalUrl = pdltUrl + "?include=posts";
        using (UnityWebRequest www = new UnityWebRequest(finalUrl, "GET"))
        {

            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("accept", "application/vnd.api+json");
            www.SetRequestHeader("X-Api-Key", api_Key);
            www.SetRequestHeader("Content-Type", "application/vnd.api+json");

            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                // JSON 문자열 파싱
                SetBoardContent(www.downloadHandler.text);
            }
        }

        if (!HandleImage.enabled)
        {
            HandleImage.enabled = true;
        }

    }

    void SetBoardContent(string reultText)
    {
        RootObject rootObject = JsonUtility.FromJson<RootObject>(reultText);

        Includeds.Clear();

        foreach (var include in rootObject.included)
        {
            if (include.attributes.status == "approved")
            {
                Includeds.Add(include);
            }
        }


        if (boards.Count < Includeds.Count)
        {
            for (int i = boards.Count; i < Includeds.Count; i++)
            {
                EmikageBoardContent bc = Instantiate(BoardContentPrefab, GridContent.transform).GetComponent<EmikageBoardContent>();
                boards.Add(bc);
            }
        }

        for (int i = 0; i < rootObject.included.Length; i++)
        {
            Color clr = GetColor(Includeds[i].attributes.color);
            boards[i].Init(Includeds[i].attributes.content.subject, Includeds[i].attributes.content.bodyHtml,
                clr);
            boards[i].gameObject.SetActive(true);
        }
        for (int i = rootObject.included.Length; i < boards.Count; i++)
        {
            boards[i].gameObject.SetActive(false);
        }
        // 추출한 데이터 출력
        //foreach (var included in rootObject.included)
        //{
        //    Debug.Log("Subject: " + included.attributes.content.subject);
        //    Debug.Log("BodyHtml: " + included.attributes.content.bodyHtml);
        //    Debug.Log("Color: " + included.attributes.color);
        //}
    }

    Color GetColor(string color)
    {

        if (color == "red")
        {
            return colors[1];
        }
        else if (color == "orange")
        {
            return colors[2];
        }
        else if (color == "green")
        {
            return colors[3];
        }
        else if (color == "blue")
        {
            return colors[4];
        }
        else if (color == "purple")
        {
            return colors[5];
        }
        return colors[0];

    }
}
[Serializable]
public class RootObject
{
    public Included[] included;
}
[Serializable]

public class Included
{
    public string id;
    public string type;
    public Attributes attributes;
}

[Serializable]
public class Attributes
{
    public Author author;
    public string color;
    public string status;
    public Content content;
}
[Serializable]
public class Author
{
    public string username;
    public string shortName;
    public string fullName;
    public string avatarUrl;
}

[Serializable]
public class Content
{
    public string subject;
    public string bodyHtml;
}