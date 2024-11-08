using Newtonsoft.Json;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GetAllCommentAndReplies : MonoBehaviour
{
    [SerializeField]
    private CreatePost createPost;

    [System.Serializable]
    public class AllComments
    {
        public bool success;
        public CommentList data;
        public string msg;
    }
    [System.Serializable]
    public class CommentList
    {
        public int count;
        public CommentContent[] data;
    }
    [System.Serializable]
    public class CommentContent
    {
        public int id;
        public int exhibitId;
        public string commentText;
        public string deviceId;
        public string createdAt;
        public string updatedAt;
        public string likecount;
        public bool isLike;
    }
    public AllComments data;

    private string getCommentApi = "https://api-test.xana.net/pmyWorlds/get-comments-by-exhibitId";
    private class SendData
    {
        public int exhibitId;
        public int pageNumber;
        public int pageSize;
        public string deviceId;
    }
    private SendData sendData;

    /// <summary>
    /// Comment Reply Data
    /// </summary>
    [System.Serializable]
    public class AllReply
    {
        public bool success;
        public ReplyList data;
        public string msg;
    }
    [System.Serializable]
    public class ReplyList
    {
        public int count;
        public ReplyContent[] data;
    }
    [System.Serializable]
    public class ReplyContent
    {
        public int id;
        public int commentId;
        public string replyText;
        public string deviceId;
        public string createdAt;
        public string updatedAt;
        public string likecount;
        public bool isLike;
    }
    public AllReply replyData;

    private string getCommentReplyApi = "https://api-test.xana.net/pmyWorlds/get-replies-by-commentId";
    private class SendReplyData
    {
        public int commentId;
        public int pageNumber;
        public int pageSize;
        public string deviceId;
    }
    private SendReplyData sendReplyData;
    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        GetAllComments();
    }

    private void GetAllComments()
    {
        sendData = new SendData
        {
            exhibitId = 1,
            pageNumber = 1,
            pageSize = 100,
            deviceId = "editor123"
        };

        string jsonData = JsonUtility.ToJson(sendData);
        StartCoroutine(PostRequest(getCommentApi, jsonData));
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
            data = new AllComments();
            data = JsonConvert.DeserializeObject<AllComments>(request.downloadHandler.text);

            for(int i=0; i<data.data.count; i++)
            {
                createPost.SpawnComment("XanaPMY", data.data.data[i].commentText, data.data.data[i].createdAt, data.data.data[i].id);
            }
            GetAllReplies(); // Fetch all replies
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    private void GetAllReplies()
    {
            sendReplyData = new SendReplyData
            {
                commentId = createPost.commentManager[counter].commentId,
                pageNumber = 1,
                pageSize = 100,
                deviceId = "editor123"
            };
        string jsonData = JsonUtility.ToJson(sendReplyData);
            StartCoroutine(PostReplyRequest(getCommentReplyApi, jsonData));
    }

    private IEnumerator PostReplyRequest(string url, string jsonData)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            replyData = new AllReply();
            replyData = JsonConvert.DeserializeObject<AllReply>(request.downloadHandler.text);

            for (int i = 0; i < replyData.data.count; i++)
            {
                createPost.replyManager[counter].SpawnReply("XanaPMY", replyData.data.data[i].replyText, replyData.data.data[i].createdAt, replyData.data.data[i].id);
            }

            counter++;
            if (counter >= createPost.replyManager.Count)
                yield break;
            else
                GetAllReplies();
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

}
