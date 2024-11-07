using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.UI;

public class CommentReplyManager : MonoBehaviour
{
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
    private GameObject parent;
    [SerializeField]
    private GameObject replySection;
    [SerializeField]
    private TMP_InputField replyInputField;
    [SerializeField]
    private GameObject bottomBar;
    [SerializeField]
    private GameObject viewMoreReplyBtn;
    [Space(5)]
    [SerializeField]
    private GameObject replyUI;

    private string commentReplyApi = "https://api-test.xana.net/pmyWorlds/add-reply-on-comment";
    private class CommentInfo
    {
        public int commentId;
        public string replyText;
        public string deviceId;
    }
    private CommentInfo commentInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartPostRequest()
    {
        commentInfo = new CommentInfo
        {
            commentId = GetComponent<CommentUIManager>().commentId,
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
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    public void SpawnReply(string user, string replyData, string createdAt, int replyId)
    {
        GameObject replyPrefab = Instantiate(replyUI);
        replyPrefab.transform.SetParent(parent.transform, false);
        replyPrefab.GetComponent<CommentUIManager>().SetComment(user, replyData, createdAt, replyId);
        StartCoroutine(EnableDisableSizeFitter());
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

    IEnumerator EnableDisableSizeFitter()
    {
        parent.GetComponent<ContentSizeFitter>().enabled = false;
        //SetSizeFitterComponent();
        yield return new WaitForSeconds(0.1f);
        parent.GetComponent<ContentSizeFitter>().enabled = true;
    }


}
