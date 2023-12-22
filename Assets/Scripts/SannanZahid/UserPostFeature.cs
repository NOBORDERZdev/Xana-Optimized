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
    bool _postBubbleFlag = false;

    public delegate void UpdatePostText(string txt);
    public UpdatePostText OnUpdatePostText;


    public void ActivatePostButtbleHome(bool flag)
    {
        if(_postBubbleFlag)
        {
            Bubble.gameObject.SetActive(flag);
        }
    }
    public void SendPost()
    {
       // Debug.LogError("GameManager.MoodSelected ----> " + GameManager.Instance.userAnimationPostFeature.MoodSelected);
       // Debug.LogError("_postInputField.text ----> " + _postInputField.text);
        if(_postInputField.text == "" && GameManager.Instance.userAnimationPostFeature.MoodSelected=="")
        {
            SNSNotificationManager.Instance.ShowNotificationMsg("Enter Text/Mood To Post");
            return;
        }
        UIManager.Instance.SwitchToPostScreen(false);
        string moodToSend = GameManager.Instance.userAnimationPostFeature.MoodSelected;
        if(GameManager.Instance.userAnimationPostFeature.MoodSelected == "")
        {
            moodToSend = "null";
        }
       // Debug.LogError("GameManager.MoodSelected ----> " + moodToSend);
        if (_postInputField.text == "")
        {
            StartCoroutine(SendPostDataToServer("null", moodToSend));
            _postBubbleFlag= false;
            Bubble.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(SendPostDataToServer(_postInputField.text, moodToSend));
            _postBubbleFlag = true;
            Bubble.gameObject.SetActive(true);
        }
        
        if(GameManager.Instance.moodManager.PostMood)
        {
            GameManager.Instance.moodManager.PostMood = false;
           // Debug.LogError("PostMood ----> " + GameManager.Instance.moodManager.LastMoodSelected);

            bool flagg = GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == GameManager.Instance.moodManager.LastMoodSelected).IdleAnimationFlag;
            GameManager.Instance.moodManager.SetMoodPosted(GameManager.Instance.moodManager.LastMoodSelected, flagg, GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController);
            GameManager.Instance.mainCharacter.GetComponent<Actor>().SetNewBehaviour(GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == GameManager.Instance.moodManager.LastMoodSelected));

            GameManager.Instance.moodManager.LastMoodSelected = "";
        }
        else
        {
           // Debug.LogError("GameManager.Instance.moodManager.PostMood ----> " + "   Fun Happy");

            GameManager.Instance.moodManager.SetMoodPosted("Fun Happy", false, GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController);
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
            case "GetFriendPost":
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
              //  Debug.LogError("SendPostDataToServer ----> " + postMessage);
              //  Debug.LogError("Error Post --->  "+www.downloadHandler.text);
            }
            else
            {
               // Debug.LogError("Posted ---->  "+www.downloadHandler.text);
            }
            www.Dispose();
        }
    }
    TMPro.TMP_Text _previousTextElement;
    IEnumerator GetLatestPostsentToServer(TMPro.TMP_Text textElement)
    {
     
        _previousTextElement = textElement;
        while (ConstantsGod.AUTH_TOKEN == "AUTH_TOKEN")
            yield return new WaitForSeconds(0.5f);

        // WWWForm form = new WWWForm();
        // form.AddField("user_id", XanaConstants.xanaConstants.userId);
        Debug.LogError("----- AUTH_TOKEN GIVEN ----> " );

        while (PlayerPrefs.GetString("UserNameAndPassword") == "")
            yield return new WaitForSeconds(0.5f);

        Debug.LogError("----- User Loged In ----> ");

        string FinalUrl = PrepareApiURL("Receive") + XanaConstants.xanaConstants.userId;
        Debug.LogError("----- URL ----> " + FinalUrl + XanaConstants.xanaConstants.userId);

        using (UnityWebRequest www = UnityWebRequest.Get(FinalUrl))
        {
           
        //    Debug.LogError("Token ----> " + ConstantsGod.AUTH_TOKEN);
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return new WaitForSecondsRealtime(Time.deltaTime);
            // while (!www.isDone)
            //      yield return null;
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                Debug.LogError("Error Post -------- Response --->  " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Posted--------- Response ---->  " + www.downloadHandler.text);
                RetrievedPost = JsonUtility.FromJson<PostInfo>(www.downloadHandler.text);
                if (RetrievedPost.data !=null)
                {
                    Debug.LogError("Posted--------- Response ---->   is not null");
                    if (string.IsNullOrEmpty(RetrievedPost.data.text_post))
                    {
                        _postBubbleFlag = false;
                        Bubble.gameObject.SetActive(false);
                    }
                    else
                    {
                        _postBubbleFlag = true;
                        Bubble.gameObject.SetActive(true);
                    }
                }
                else
                {
                    Debug.LogError("Posted--------- Response ---->   is null");
                    _postBubbleFlag = false;
                    Bubble.gameObject.SetActive(false);
                }
          
                Debug.LogError("Message --->> " + RetrievedPost.data.text_post);
                if (RetrievedPost.data.text_post == "null")
                {
                    _postBubbleFlag = true;
                    Bubble.gameObject.SetActive(true);
                }
                else
                    textElement.text = RetrievedPost.data.text_post;
                if(RetrievedPost.data.text_mood != "null" && RetrievedPost.data.text_mood != null && RetrievedPost.data.text_mood != "")
                {
                  //  Debug.LogError("Last Mood Posted ---->  " + RetrievedPost.data.text_mood);
                    bool flagg = GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood).IdleAnimationFlag;
                    GameManager.Instance.moodManager.SetMoodPosted(RetrievedPost.data.text_mood, flagg, GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController);
                  //  Debug.LogError("Behaviour Assign ---->   "+GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood).Name);
                    GameManager.Instance.mainCharacter.GetComponent<Actor>().SetNewBehaviour(GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood));
                }
                else
                {
                  //  Debug.LogError("Last Mood Posted ELSE ---->  " + "   Fun Happy");
                    GameManager.Instance.moodManager.SetMoodPosted("Fun Happy", false, GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController);
                }
            }
            www.Dispose();
        }
    }
    public void SetLastPostToPlayer()
    {
     //   Debug.LogError("Reset to last post --->> ");

        if (RetrievedPost.data != null)
        {
            _postBubbleFlag = true;
            Bubble.gameObject.SetActive(true);
        }
        else
        {
            _postBubbleFlag = false;
            Bubble.gameObject.SetActive(false);
        }
      //  Debug.LogError("Message --->> " + RetrievedPost.data.text_post);
        if (RetrievedPost.data.text_post == "null")
        {
            _postBubbleFlag = false;
            Bubble.gameObject.SetActive(false);
        }
        else if(_previousTextElement!=null)
            _previousTextElement.text = RetrievedPost.data.text_post;
        if (RetrievedPost.data.text_mood != "null" && RetrievedPost.data.text_mood != null && RetrievedPost.data.text_mood != "")
        {
         //   Debug.LogError("Last Mood Posted ---->  " + RetrievedPost.data.text_mood);
            bool flagg = GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood).IdleAnimationFlag;
            GameManager.Instance.moodManager.SetMoodPosted(RetrievedPost.data.text_mood, flagg, GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController);
         //   Debug.LogError("Behaviour Assign ---->   " + GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood).Name);
            GameManager.Instance.mainCharacter.GetComponent<Actor>().SetNewBehaviour(GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood));
        }
        else
        {
          //  Debug.LogError("Last Mood Posted ELSE ---->  " + "   Fun Happy");
            GameManager.Instance.moodManager.SetMoodPosted("Fun Happy", false, GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController);
        }
    }


    public void GetLatestPostOfFriend(int friend_id, PlayerPostBubbleHandler friendBubbleRef, AnimatorOverrideController overrideController)
    {
        StartCoroutine(GetLatestPostOfFriendFromServer(friend_id, friendBubbleRef, overrideController));
    }

    IEnumerator GetLatestPostOfFriendFromServer(int friend_id, PlayerPostBubbleHandler friendBubbleRef,AnimatorOverrideController overrideController)
    {
        string FinalUrl = PrepareApiURL("Receive") + friend_id;
        // Debug.LogError("Prepared URL ----> " + FinalUrl);
        using (UnityWebRequest www = UnityWebRequest.Get(FinalUrl))
        {
            while (ConstantsGod.AUTH_TOKEN == "AUTH_TOKEN")
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
                if (RetrievedPost.data != null)
                {
                    friendBubbleRef.ActivatePostFirendBubble(true);
                }
                else
                {
                    friendBubbleRef.ActivatePostFirendBubble(false);
                }
                // Debug.LogError("Message --->> " + RetrievedPost.data.text_post);
                if (RetrievedPost.data.text_post == "null")
                {
                    friendBubbleRef.ActivatePostFirendBubble(false);
                }
                // else
                //    textElement.text = RetrievedPost.data.text_post;
                if (RetrievedPost.data.text_mood != "null" && RetrievedPost.data.text_mood != null && RetrievedPost.data.text_mood != "")
                {
                      Debug.LogError("Last Mood Posted ---->  " + RetrievedPost.data.text_mood);
                    ActorBehaviour tempBehav = GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood);
                    if(tempBehav!=null)
                    {
                        bool flagg = tempBehav.IdleAnimationFlag;
                        GameManager.Instance.moodManager.SetMoodPosted(RetrievedPost.data.text_mood, flagg, overrideController);
                        //  Debug.LogError("Behaviour Assign ---->   "+GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood).Name);
                        GameManager.Instance.mainCharacter.GetComponent<Actor>().SetNewBehaviour(GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPost.data.text_mood));
                    }
                    else
                    {
                        GameManager.Instance.moodManager.SetMoodPosted("Fun Happy", false, overrideController);
                    }
                }
                else
                {
                    //  Debug.LogError("Last Mood Posted ELSE ---->  " + "   Fun Happy");
                    GameManager.Instance.moodManager.SetMoodPosted("Fun Happy", false, overrideController);
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
