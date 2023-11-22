using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UserPostFeature : MonoBehaviour
{
    public Transform Bubble;
    [SerializeField]
    public TMPro.TMP_InputField _postInputField;
    [SerializeField]
    public Transform _postScreen;
    [SerializeField]
    PostInfo RetrievedPost;

    public delegate void UpdatePostText(string txt);
    public UpdatePostText OnUpdatePostText;
    public void SendPost()
    {
        StartCoroutine(SendPostDataToServer(_postInputField.text));
    }
    public void GetLatestPost(TMPro.TMP_Text textElement)
    {
        StartCoroutine(GetLatestPostsentToServer(textElement));
    }
    string PrepareApiURL(string urlType)
    {
        switch (urlType)
        {
            case "Send":
                return ConstantsGod.API_BASEURL + ConstantsGod.SendPostToServer;
            case "Receive":
                return ConstantsGod.API_BASEURL + ConstantsGod.GetPostSentToServer;
            default:
                return "";
        }
    }
    IEnumerator SendPostDataToServer(string postMessage)
    {
        string FinalUrl = PrepareApiURL("Send");
        Debug.LogError("Prepared URL ----> " + FinalUrl);
        WWWForm form = new WWWForm();
        form.AddField("text_post", postMessage);
        using (UnityWebRequest www = UnityWebRequest.Post(FinalUrl, form))
        {
            Debug.LogError("Token ----> " + ConstantsGod.AUTH_TOKEN);
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                Debug.LogError("Error Post --->  "+www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Posted ---->  "+www.downloadHandler.text);
                Bubble.gameObject.SetActive(true);
                OnUpdatePostText.Invoke(postMessage);
            }
            www.Dispose();
        }
    }
    IEnumerator GetLatestPostsentToServer(TMPro.TMP_Text textElement)
    {
        string FinalUrl = PrepareApiURL("Receive");
        Debug.LogError("Prepared URL ----> " + FinalUrl);
        using (UnityWebRequest www = UnityWebRequest.Get(FinalUrl))
        {
            while(ConstantsGod.AUTH_TOKEN == "AUTH_TOKEN")
                yield return new WaitForSeconds(0.5f);
            Debug.LogError("Token ----> " + ConstantsGod.AUTH_TOKEN);
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                Debug.LogError("Error Post --->  " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Posted ---->  " + www.downloadHandler.text);
                RetrievedPost = JsonUtility.FromJson<PostInfo>(www.downloadHandler.text);
                if(RetrievedPost.data.rows.Count>0)
                {
                    Bubble.gameObject.SetActive(true);
                }
                else
                {
                    Bubble.gameObject.SetActive(false);
                }
                for (int i = RetrievedPost.data.rows.Count-1; i >0; i--)
                {
                    Debug.LogError("Message --->> " + RetrievedPost.data.rows[i].text_post);
                    textElement.text = RetrievedPost.data.rows[i].text_post;
                    break;
                }
            }
            www.Dispose();
        }
    }
    [System.Serializable]
    public class PostInfo
    {
        public bool success;
        public PostClass data;
        public string msg;
    }
    [System.Serializable]
    public class PostClass
    {
        public int count;
        public List<PostRowList> rows;
    }
    [System.Serializable]
    public class PostRowList
    {
        public string id;
        public string user_id;
        public string text_post;

    }
}
