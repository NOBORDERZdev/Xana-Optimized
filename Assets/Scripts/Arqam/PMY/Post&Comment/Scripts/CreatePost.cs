using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

public class CreatePost : MonoBehaviour
{
    [System.Serializable]
    public class OutPutData
    {
        public bool success;
        public CommentData data;
        public string msg;
    }
    [System.Serializable]
    public class CommentData
    {
        public int id;
        public int exhibitId;
        public string commentText;
        public string deviceId;
        public string updatedAt;
        public string createdAt;
    }
    public OutPutData outPutData;
    [Space(5)]
    public List<CommentReplyManager> replyManager;
    public GameObject CommentUI;
    [SerializeField]
    private TMP_InputField inputField;

    private string createCommentApi = "https://api-test.xana.net/pmyWorlds/create-new-comment";
    private class PostData
    {
        public int exhibitId;
        public string commentText;
        public string deviceId;
    }
    private PostData postData;


    public void Send_PostOrComment()
    {
        if (!inputField.text.IsNullOrEmpty())
        {
            StartPostRequest();
        }
        else
        {
            Debug.LogError("Please Enter something");
        }
    }

    private void StartPostRequest()
    {
        postData = new PostData
        {
            exhibitId = 1,
            commentText = inputField.text,
            deviceId = "editor123"
        };

        string jsonData = JsonUtility.ToJson(postData);
        StartCoroutine(PostRequest(createCommentApi, jsonData));
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
            outPutData = new OutPutData();
            outPutData = JsonConvert.DeserializeObject<OutPutData>(request.downloadHandler.text);

            SpawnComment("Xana PMY", outPutData.data.commentText, outPutData.data.createdAt, outPutData.data.id);
            inputField.text = "";
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    public void SpawnComment(string userName, string comment, string timeSpan, int id)
    {
        GameObject ui = Instantiate(CommentUI);
        ui.transform.SetParent(transform, false);
        ui.GetComponent<PostUIManager>().SetComment(userName, comment, timeSpan, id);
        //commentManager.Add(ui.GetComponent<CommentManager>());
        replyManager.Add(ui.GetComponent<CommentReplyManager>());
    }


}
