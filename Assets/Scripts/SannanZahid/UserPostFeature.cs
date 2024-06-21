using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class UserPostFeature : MonoBehaviour
{
    public Transform Bubble;
    [SerializeField]
    public TMPro.TMP_InputField _postInputField;
    [SerializeField]
    public Transform _postScreen;
    [SerializeField]
    PostInfo RetrievedPostPlayer, RetrievedPostFriend;
    bool _postBubbleFlag = false;
    public delegate void UpdatePostText(string txt);
    public UpdatePostText OnUpdatePostText;
    public static event Action OnPostButtonPressed;
    public void ActivatePostButtbleHome(bool flag)
    {
        if (_postBubbleFlag) Bubble.parent.gameObject.SetActive(flag);
    }
    public void SendPost()
    {
        if (_postInputField.text == "" && GameManager.Instance.userAnimationPostFeature.MoodSelected == "")
        {
            SNSNotificationHandler.Instance.ShowNotificationMsg("Enter Text/Mood To Post");
            return;
        }
        GameManager.Instance.UiManager.SwitchToPostScreen(false);
        string moodToSend = GameManager.Instance.userAnimationPostFeature.MoodSelected;
        if (GameManager.Instance.userAnimationPostFeature.MoodSelected == "")
        {
            moodToSend = "null";
        }
        if (_postInputField.text == "")
        {
            StartCoroutine(SendPostDataToServer("null", moodToSend));
            _postBubbleFlag = false;
            Bubble.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(SendPostDataToServer(_postInputField.text, moodToSend));
            _postBubbleFlag = true;
            Bubble.gameObject.SetActive(true);
        }
        if (GameManager.Instance.moodManager.PostMood)
        {
            RetrievedPostPlayer.data.text_mood = moodToSend;
            GameManager.Instance.moodManager.PostMood = false;
            // Debug.LogError("---> "+moodToSend+"   --->"+ GameManager.Instance.moodManager.LastMoodSelected);
            bool flagg = GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == GameManager.Instance.moodManager.LastMoodSelected).IdleAnimationFlag;
            GameManager.Instance.moodManager.SetMoodPosted(GameManager.Instance.moodManager.LastMoodSelected, flagg, GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController, GameManager.Instance.mainCharacter.transform.GetComponent<Animator>());
            GameManager.Instance.mainCharacter.GetComponent<Actor>().SetNewBehaviour(GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == GameManager.Instance.moodManager.LastMoodSelected));
            GameManager.Instance.moodManager.LastMoodSelected = "";
        }
        else
        {
            AssignRandomAnimationIfUserNotPosted(GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController, GameManager.Instance.mainCharacter.transform.GetComponent<Animator>());
        }

        if(OnPostButtonPressed!=null)
        {
            OnPostButtonPressed.Invoke();
        }
    }

    private void AssignRandomAnimationIfUserNotPosted(AnimatorOverrideController animatorOverrideController, Animator animator)
    {
        Actor actor=animator.transform.GetComponent<Actor>();
        if (!Actor.RandAnimKeys.TryGetValue(actor.ActorId, out string randomAnimKey) || string.IsNullOrEmpty(randomAnimKey))
        {
            string randomAnimName = GameManager.Instance.ActorManager.actorBehaviour[GameManager.Instance.ActorManager.GetPostRandomDefaultAnim()].Name;

            Actor.RandAnimKeys[actor.ActorId] = randomAnimName;
        }
        GameManager.Instance.moodManager.SetMoodPosted(Actor.RandAnimKeys[actor.ActorId], true, animatorOverrideController, animator);
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
        WWWForm form = new WWWForm();
        form.AddField("text_post", postMessage);
        form.AddField("text_mood", mood);
        using (UnityWebRequest www = UnityWebRequest.Post(FinalUrl, form))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return new WaitForSecondsRealtime(Time.deltaTime);

            if (www.error != null)
            {
                Debug.Log("UnityWebRequest.error:" + www.error);
            }
            else
            {
                Debug.Log("Response:" + www.downloadHandler.text);
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

        while (ConstantsHolder.userId.IsNullOrEmpty())
            yield return new WaitForSeconds(0.5f);

        string FinalUrl = PrepareApiURL("Receive") + ConstantsHolder.userId;
        using (UnityWebRequest www = UnityWebRequest.Get(FinalUrl))
        {

            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return new WaitForSecondsRealtime(Time.deltaTime);
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                // Debug.LogError("Error Post -------- Response --->  " + www.downloadHandler.text);
            }
            else
            {
                RetrievedPostPlayer = JsonUtility.FromJson<PostInfo>(www.downloadHandler.text);
                if (RetrievedPostPlayer.data != null)
                {
                    if (string.IsNullOrEmpty(RetrievedPostPlayer.data.text_post) || RetrievedPostPlayer.data.text_post == "null")
                    {
                        RetrievedPostPlayer.data.text_post = "";
                        RetrievedPostPlayer.success = false;
                        _postBubbleFlag = false;
                        Bubble.gameObject.SetActive(false);
                    }
                    else
                    {
                        RetrievedPostPlayer.success = true;
                        _postBubbleFlag = true;
                        Bubble.gameObject.SetActive(true);
                    }
                }
                else
                {
                    _postBubbleFlag = false;
                    Bubble.gameObject.SetActive(false);
                }
                if (!RetrievedPostPlayer.data.text_post.IsNullOrEmpty())
                {
                    textElement.text = RetrievedPostPlayer.data.text_post;
                    InsertNewlines(textElement);
                }

                if (RetrievedPostPlayer.data.text_mood != "null" && RetrievedPostPlayer.data.text_mood != null && RetrievedPostPlayer.data.text_mood != "")
                {
                    RetrievedPostPlayer.success = true;
                    bool flagg = GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPostPlayer.data.text_mood).IdleAnimationFlag;
                    GameManager.Instance.moodManager.SetMoodPosted(RetrievedPostPlayer.data.text_mood, flagg, GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController, GameManager.Instance.mainCharacter.transform.GetComponent<Animator>());
                    GameManager.Instance.mainCharacter.GetComponent<Actor>().SetNewBehaviour(GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPostPlayer.data.text_mood));
                }
                else
                {
                    AssignRandomAnimationIfUserNotPosted(GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController, GameManager.Instance.mainCharacter.transform.GetComponent<Animator>());
                }
            }
            www.Dispose();
        }
    }
    public void SetLastPostToPlayer()
    {
        if (!string.IsNullOrEmpty(Bubble.transform.GetComponentInChildren<TMPro.TMP_Text>().text))
        {
            _postBubbleFlag = true;
            Bubble.gameObject.SetActive(true);
        }
        else
        {
            _postBubbleFlag = false;
            Bubble.gameObject.SetActive(false);
        }
        if (RetrievedPostPlayer.data.text_post == "null")
        {
            _postBubbleFlag = false;
            Bubble.gameObject.SetActive(false);
        }
        //else if(_previousTextElement!=null)
        //    _previousTextElement.text = RetrievedPostPlayer.data.text_post;
        if (RetrievedPostPlayer.data.text_mood != "null" && RetrievedPostPlayer.data.text_mood != null && RetrievedPostPlayer.data.text_mood != "")
        {
            bool flagg = GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPostPlayer.data.text_mood).IdleAnimationFlag;
            GameManager.Instance.moodManager.SetMoodPosted(RetrievedPostPlayer.data.text_mood, flagg, GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController, GameManager.Instance.mainCharacter.transform.GetComponent<Animator>());
            GameManager.Instance.mainCharacter.GetComponent<Actor>().SetNewBehaviour(GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPostPlayer.data.text_mood));
        }
        else
        {
            AssignRandomAnimationIfUserNotPosted(GameManager.Instance.mainCharacter.GetComponent<Actor>().overrideController, GameManager.Instance.mainCharacter.transform.GetComponent<Animator>());
        }
    }


    public void GetLatestPostOfFriend(int friend_id, PlayerPostBubbleHandler friendBubbleRef, Actor friendActor)
    {
        StartCoroutine(GetLatestPostOfFriendFromServer(friend_id, friendBubbleRef, friendActor));
    }

    IEnumerator GetLatestPostOfFriendFromServer(int friend_id, PlayerPostBubbleHandler friendBubbleRef, Actor friendActor)
    {
        friendActor.ActorId = friend_id;
        string FinalUrl = PrepareApiURL("Receive") + friend_id;
        using (UnityWebRequest www = UnityWebRequest.Get(FinalUrl))
        {
            while (ConstantsGod.AUTH_TOKEN == "AUTH_TOKEN")
                yield return new WaitForSeconds(0.5f);
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while (!www.isDone)
                yield return new WaitForSecondsRealtime(Time.deltaTime);

            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                // Debug.LogError("Error Post --->  " + www.downloadHandler.text);
            }
            else
            {
                RetrievedPostFriend = JsonUtility.FromJson<PostInfo>(www.downloadHandler.text);
                if (RetrievedPostFriend.data != null)
                {
                    if (string.IsNullOrEmpty(RetrievedPostFriend.data.text_post))
                    {
                        friendBubbleRef.ActivatePostFirendBubble(false);
                    }
                    else
                    {
                        friendBubbleRef.ActivatePostFirendBubble(true);
                    }
                }
                else
                {
                    friendBubbleRef.ActivatePostFirendBubble(false);
                }
                if (RetrievedPostFriend.data.text_post == "null")
                {
                    friendBubbleRef.ActivatePostFirendBubble(false);
                }
                else
                    friendBubbleRef.UpdateText(RetrievedPostFriend.data.text_post);
                if (RetrievedPostFriend.data.text_mood != "null" && RetrievedPostFriend.data.text_mood != null && RetrievedPostFriend.data.text_mood != "")
                {
                    ActorBehaviour tempBehav = GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPostFriend.data.text_mood);
                    if (tempBehav != null)
                    {
                        bool flagg = tempBehav.IdleAnimationFlag;
                        GameManager.Instance.moodManager.SetMoodPosted(RetrievedPostFriend.data.text_mood, flagg, friendActor.overrideController, friendActor.transform.GetComponent<Animator>());
                        friendActor.SetNewBehaviour(GameManager.Instance.ActorManager.actorBehaviour.Find(x => x.Name == RetrievedPostFriend.data.text_mood));
                    }
                    else
                    {
                        AssignRandomAnimationIfUserNotPosted(friendActor.overrideController, friendActor.transform.GetComponent<Animator>());
                    }
                }
                else
                {
                    AssignRandomAnimationIfUserNotPosted(friendActor.overrideController, friendActor.transform.GetComponent<Animator>());
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

    //public string InsertNewlines(string input)
    //{
    //    StringBuilder stringBuilder = new StringBuilder();

    //    for (int i = 0; i < input.Length; i++)
    //    {
    //        stringBuilder.Append(input[i]);

    //        if ((i + 1) % 20 == 0)
    //        {
    //            stringBuilder.Append("\n");
    //        }
    //    }

    //    return stringBuilder.ToString();
    //}

    public void InsertNewlines(TMP_Text input)
    {
        StartCoroutine(ArrangeBubbleTxt(input));
    }

    private IEnumerator ArrangeBubbleTxt(TMP_Text tmpText)
    {
        ContentSizeFitter contentSizeFitter = tmpText.transform.parent.parent.GetComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        yield return new WaitForEndOfFrame();

        string str = tmpText.text;
        tmpText.text = "";
        for (int i = 0; i < str.Length; i++)
        {
            tmpText.text += str[i];
            tmpText.ForceMeshUpdate();

            var preferredWidth = tmpText.GetPreferredValues().x;
            if (preferredWidth > 260)
            {
                yield return new WaitForEndOfFrame();
                contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                yield return new WaitForEndOfFrame();
                contentSizeFitter.gameObject.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(300f, contentSizeFitter.gameObject.GetComponent<RectTransform>().sizeDelta.y);

                tmpText.text = str;
                yield break;
            }
        }
    }
}
