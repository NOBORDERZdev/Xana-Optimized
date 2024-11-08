using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CommentReplyManager : MonoBehaviour
{
    public CreatePost createPost;
    public int replyCounter = 0;
    [System.Serializable]
    public class ReplyData
    {
        public bool success;
        public ReplyInfo data;
        public string msg;
    }
    [System.Serializable]
    public class ReplyInfo
    {
        public int id;          // exhibit id
        public int commentId;
        public string replyText;
        public string deviceId;
        public string updatedAt;
        public string createdAt;
    }
    public ReplyData replyData;
    [SerializeField]
    private GameObject replySection;
    [SerializeField]
    private TMP_InputField replyInputField;
    [SerializeField]
    private GameObject bottomBar;
    [SerializeField]
    private GameObject viewMoreReplyBtn;
    [SerializeField]
    private TextMeshProUGUI viewMoreReplyTxt;
    [Space(5)]
    [SerializeField]
    private GameObject replyUI;
    [SerializeField]
    private List<GameObject> allReplyObj;

    private string commentReplyApi = "https://api-test.xana.net/pmyWorlds/add-reply-on-comment";
    private class CommentInfo
    {
        public int commentId;
        public string replyText;
        public string deviceId;
    }
    private CommentInfo commentInfo;
    public bool isShowReply = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartPostRequest()
    {
        commentInfo = new CommentInfo
        {
            commentId = GetComponent<PostUIManager>().commentId,
            replyText = replyInputField.text,
            deviceId = "editor123"
        };

        string jsonData = JsonUtility.ToJson(commentInfo);
        StartCoroutine(PostRequest(commentReplyApi, jsonData));
    }

    private IEnumerator PostRequest(string url, string jsonData)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            replyData = new ReplyData();
            replyData = JsonConvert.DeserializeObject<ReplyData>(request.downloadHandler.text);
            SpawnReply("Xana PMY", replyData.data.replyText, replyData.data.createdAt, replyData.data.id);
            replySection.SetActive(false);
            replyInputField.text = "";
            replyCounter++;
            EnableViewMoreReplyBtn();
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    public void SpawnReply(string user, string replyData, string createdAt, int replyId)
    {
        GameObject replyPrefab = Instantiate(replyUI);
        replyPrefab.transform.SetParent(this.transform, false);
        replyPrefab.GetComponent<PostUIManager>().SetCommentReply(user, replyData, createdAt, replyId);
        replyPrefab.GetComponent<PostUIManager>().replyManager = this;
        if (isShowReply)
        {
            replyPrefab.SetActive(true);
        }
        else
        {
            replyPrefab.SetActive(false);
        }
        allReplyObj.Add(replyPrefab);
    }

    public void ClickOnReply()
    {
        replySection.SetActive(true);
        StartCoroutine(EnableDisableSizeFitter());
    }

    public void ClickOnCancel()
    {
        replySection.SetActive(false);
        replyInputField.text = "";
        StartCoroutine(EnableDisableSizeFitter());
    }

    public void ClickOnViewMore()
    {
        isShowReply = isShowReply ? false : true;
        foreach (GameObject items in allReplyObj)
        {
            items.SetActive(isShowReply);
        }
        StartCoroutine(EnableDisableSizeFitter());
        StartCoroutine(AdjustUIPosition());
    }

    public void EnableViewMoreReplyBtn()
    {
        bottomBar.SetActive(true);
        viewMoreReplyBtn.SetActive(true);
        viewMoreReplyTxt.text = "View " + replyCounter + " Reply";
        StartCoroutine(EnableDisableSizeFitter());
    }

    IEnumerator EnableDisableSizeFitter()
    {
        this.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    IEnumerator AdjustUIPosition()
    {
        createPost.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.15f);
        createPost.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

}
