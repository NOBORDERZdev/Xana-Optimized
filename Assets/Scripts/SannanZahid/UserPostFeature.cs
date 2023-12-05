using System.Collections;
using UnityEditor;
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
        string moodToSend = GameManager.Instance.userAnimationPostFeature.MoodSelected;
        if(GameManager.Instance.userAnimationPostFeature.MoodSelected == "")
        {
            moodToSend = "null";
        }
        Debug.LogError("GameManager.MoodSelected ----> " + moodToSend);
        if (_postInputField.text == "")
        {
            StartCoroutine(SendPostDataToServer("null", moodToSend));
            Bubble.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(SendPostDataToServer(_postInputField.text, moodToSend));
            Bubble.gameObject.SetActive(true);
        }
        
        if(GameManager.Instance.moodManager.PostMood)
        {
            GameManager.Instance.moodManager.PostMood = false;
            Debug.LogError("PostMood ----> " + GameManager.Instance.moodManager.LastMoodSelected);

            bool flagg = GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == GameManager.Instance.moodManager.LastMoodSelected).IdleAnimationFlag;
            GameManager.Instance.moodManager.SetMoodPosted(GameManager.Instance.moodManager.LastMoodSelected, flagg);
            GameManager.Instance.moodManager.LastMoodSelected = "";
        }
        else
        {
            GameManager.Instance.moodManager.SetMoodPosted("Fun Happy", false);
        }
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
    IEnumerator SendPostDataToServer(string postMessage, string mood)
    {
        OnUpdatePostText.Invoke(postMessage);
        string FinalUrl = PrepareApiURL("Send");
       // Debug.LogError("Prepared URL ----> " + FinalUrl);
        WWWForm form = new WWWForm();
        form.AddField("text_post", postMessage);
        form.AddField("text_mood", mood);
        using (UnityWebRequest www = UnityWebRequest.Post(FinalUrl, form))
        {
           // Debug.LogError("Token ----> " + ConstantsGod.AUTH_TOKEN);
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return new WaitForSecondsRealtime(Time.deltaTime);

            // while (!www.isDone)
            //     yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                Debug.LogError("SendPostDataToServer ----> " + postMessage);
                Debug.LogError("Error Post --->  "+www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Posted ---->  "+www.downloadHandler.text);
            }
            www.Dispose();
        }
    }
    IEnumerator GetLatestPostsentToServer(TMPro.TMP_Text textElement)
    {
        string FinalUrl = PrepareApiURL("Receive");
       // Debug.LogError("Prepared URL ----> " + FinalUrl);
        using (UnityWebRequest www = UnityWebRequest.Get(FinalUrl))
        {
            while(ConstantsGod.AUTH_TOKEN == "AUTH_TOKEN")
                yield return new WaitForSeconds(0.5f);
        //    Debug.LogError("Token ----> " + ConstantsGod.AUTH_TOKEN);
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return new WaitForSecondsRealtime(Time.deltaTime);
            // while (!www.isDone)
            //      yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                Debug.LogError("Error Post --->  " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Posted ---->  " + www.downloadHandler.text);
                RetrievedPost = JsonUtility.FromJson<PostInfo>(www.downloadHandler.text);
                if (RetrievedPost.data !=null)
                {
                    Bubble.gameObject.SetActive(true);
                }
                else
                {
                    Bubble.gameObject.SetActive(false);
                }
                Debug.LogError("Message --->> " + RetrievedPost.data.text_post);
                if (RetrievedPost.data.text_post == "null")
                {
                    Bubble.gameObject.SetActive(false);
                }
                else
                    textElement.text = RetrievedPost.data.text_post;
                if(RetrievedPost.data.text_mood != "null" && RetrievedPost.data.text_mood!=null && RetrievedPost.data.text_mood != "")
                {
                    Debug.LogError("Last Mood Posted ---->  " + RetrievedPost.data.text_mood);
                    bool flagg = GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood).IdleAnimationFlag;
                    GameManager.Instance.moodManager.SetMoodPosted(RetrievedPost.data.text_mood, flagg);
                    Debug.LogError("Behaviour Assign ---->   "+GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood).Name);
                    GameManager.Instance.mainCharacter.GetComponent<Actor>().SetNewBehaviour(GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood));
                }
                else
                {
                    GameManager.Instance.moodManager.SetMoodPosted("Fun Happy", false);
                }
            }
            www.Dispose();
        }
    }
    [System.Serializable]
    public class PostInfo
    {
        public bool success;
        public PostRowList data;
    }
    [System.Serializable]
    public class PostRowList
    {
        public string text_post;
        public string text_mood;
    }
}
