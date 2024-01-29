using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FeedData : MonoBehaviour
{
   [SerializeField] Image ProfileImage;
   [SerializeField] TMP_Text DisplayName;
   [SerializeField] TMP_Text PostText;
   [SerializeField] TMP_Text Date;
   [SerializeField] TMP_Text Likes;
   [SerializeField] Image Heart;
   [SerializeField] Sprite LikedHeart;
   [SerializeField] Sprite UnLikedHeart;
   [SerializeField] Color LikedColor;
   [SerializeField] Color UnLikedColor;
    FeedResponseRow _data;
    bool isLiked = false;
   public void SetFeedPrefab(FeedResponseRow data){ 
        _data = data;
        DisplayName.text = data.user.name;
        PostText.text = data.text_post;
        //Likes.text = data.like_count.ToString();
        UpdateLikeCount(data.like_count);
        Date.text = CalculateTimeDifference(Convert.ToDateTime(data.createdAt)).ToString();
        if (data.isLikedByUser)
        {
            isLiked = true;
            Likes.color = LikedColor;
        }
        UpdateHeart();
        if (!String.IsNullOrEmpty(data.user.avatar) &&  !data.user.avatar.Equals("null") )
        {
            StartCoroutine(GetProfileImage(data.user.avatar));
        }
   }

   public double CalculateTimeDifference(DateTime postTime)
   {
        DateTime currentTime = DateTime.Now;
        TimeSpan timeDifference = currentTime - postTime;
        return timeDifference.Hours;
   }
    IEnumerator GetProfileImage(string url)
    {
        string newUrl = url+"?width=256&height=256";
        WWW www = new WWW(url);
        yield return www;
        ProfileImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
    }

    public void LikeUnlikePost()
    {
       StartCoroutine(LikeUnLike());
    }

    IEnumerator LikeUnLike()
    {
        string url = ConstantsGod.API_BASEURL +ConstantsGod.FeedLikeDislikePost;
        int feedId = _data.id;
        WWWForm form = new WWWForm();
        form.AddField("textPostId", feedId);
        using (UnityWebRequest www = UnityWebRequest.Post(url,form))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            www.SendWebRequest();
            while(!www.isDone)
            {
                yield return null;
            }
            if (www.isNetworkError || www.isHttpError)
            {
                StartCoroutine(LikeUnLike());
            }
            else
            {  
                LikeResponse likeResponse = JsonUtility.FromJson<LikeResponse>(www.downloadHandler.text);
                UpdateLikeCount(likeResponse.data.likeCount);
                //Likes.text =  likeResponse.data.likeCount.ToString();
                isLiked = !isLiked;
                UpdateHeart();
            }
        }
    }

    void UpdateHeart()
    {
        if (isLiked)
        {
            Heart.sprite = LikedHeart;
            Likes.color = LikedColor;
        }
        else
        {
            Heart.sprite = UnLikedHeart;
            Likes.color = UnLikedColor;
        }
    }

    public void UnPoolPrefab(){ 
        _data=null;
        isLiked = false;
        UpdateHeart();
    }

    public int GetFeedId(){
        return _data.id;
    }

    public void UpdateLikeCount(int count){ 
        Likes.text = count.ToString();
    }
}

[Serializable]
public class LikeResponse  { 
    public bool success;
    public likeCountClass data;
    public string msg;
}

[Serializable]
public class likeCountClass  { 
    public int likeCount;
}

