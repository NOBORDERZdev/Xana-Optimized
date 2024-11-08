using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using Unity.IO.LowLevel.Unsafe;

public class LikeUnlikeComment : MonoBehaviour
{
    private bool isCommentLiked = false;
    private int numOfLike = 0;

    [System.Serializable]
    public class LikeResponse
    {
        public bool success;
        public LikeData data;
        public string msg;
    }
    [System.Serializable]
    public class LikeData
    {
        public int id;
        public int commentId;
        public string deviceId;
        public string updatedAt;
        public string createdAt;
    }
    public LikeResponse likeResponse;

    [System.Serializable]
    public class UnLikeResponse
    {
        public bool success;
        public int data;
        public string msg;
    }
    public UnLikeResponse unLikeResponse;

    [SerializeField]
    private TextMeshProUGUI likeText;
    [SerializeField]
    private TextMeshProUGUI dislikeText;

    private string likeUnlikeApi = "https://api-test.xana.net/pmyWorlds/like-unlike-comment";
    private class PostLikeData
    {
        public int commentId;
        public string deviceId;
    }
    private PostLikeData postlikeData;

    void Start()
    {
        
    }

    public void OnClickLikeUnlikeBtn()
    {
        StartPostRequest();
    }

    private void StartPostRequest()
    {
        postlikeData = new PostLikeData
        {
            commentId = GetComponent<PostUIManager>().commentId,
            deviceId = "editor123"
        };

        string jsonData = JsonUtility.ToJson(postlikeData);
        StartCoroutine(PostRequest(likeUnlikeApi, jsonData));
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
            if (!isCommentLiked)
            {
                likeResponse = new LikeResponse();
                likeResponse = JsonConvert.DeserializeObject<LikeResponse>(request.downloadHandler.text);
                isCommentLiked = true;
                numOfLike++;
            }
            else
            {
                unLikeResponse = new UnLikeResponse();
                unLikeResponse = JsonConvert.DeserializeObject<UnLikeResponse>(request.downloadHandler.text);
                isCommentLiked = false;
                numOfLike--;
            }
            UpdateLike(isCommentLiked, numOfLike.ToString());
            //Debug.LogError("Like: " + likeResponse);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    public void UpdateLike(bool likeStatus, string likeCount)
    {
        isCommentLiked = likeStatus;
        numOfLike = int.Parse(likeCount);
        likeText.text = likeCount;
    }

}
