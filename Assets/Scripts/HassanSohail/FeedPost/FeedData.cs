using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FeedData : MonoBehaviour
{
   [SerializeField] Sprite defaultProfileImage;
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
    public FeedResponseRow _data;
    bool isLiked = false;
    bool isEnable = false;
    int timeUpdateInterval = 1;
    FeedScroller scrollerController;
    public void SetFeedPrefab(FeedResponseRow data, bool isFeed = true ){
        if (gameObject.activeInHierarchy)
        {
            _data = data;
            DisplayName.text = data.user.name;
            PostText.text = data.text_post;
            isEnable= true;
            //Likes.text = data.like_count.ToString();
            UpdateLikeCount(data.like_count);
            timeUpdateInterval=1;
            if (isEnable)
            {
                Date.text = CalculateTimeDifference(Convert.ToDateTime(data.createdAt)).ToString();
            }
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
            if (isFeed)
            {
                Invoke(nameof(HieghtListUpdateWithDelay),0.1f);
            }
        }
    }
   
    void HieghtListUpdateWithDelay(){ 
       scrollerController.AddInHeightList(_data.id, gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().CalculateHeight());
       gameObject.GetComponent<LayoutElement>().minHeight = gameObject.transform.GetChild(0).gameObject.GetComponent<RectTransform>().CalculateHeight();
      // scrollerController.scroller.ReloadData();
     }
    public string CalculateTimeDifference(DateTime postTime)
   {
        if (isEnable && gameObject.activeInHierarchy)
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeDifference = currentTime - postTime;
            StartCoroutine(ReCallingTimeDifference(postTime));
            if (timeDifference.TotalMinutes < 1){
                timeUpdateInterval =1;
                return $"{Math.Floor(timeDifference.TotalSeconds)} s";
            }
            else if (timeDifference.TotalMinutes < 60)
            {
                timeUpdateInterval =60;
                return $"{Math.Floor(timeDifference.TotalMinutes)} m";
            }
            else if (timeDifference.TotalHours < 24)
            {
                timeUpdateInterval =3600;
                return $"{Math.Floor(timeDifference.TotalHours)} h";
            }
            else if (timeDifference.TotalDays < 30){
                timeUpdateInterval =86400;
                return $"{Math.Floor(timeDifference.TotalDays)} d"; 
             }
            else if (timeDifference.TotalDays < 365)
            {
                 timeUpdateInterval =86400;
                return $"{Math.Floor(timeDifference.TotalDays / 30)} mo";
            }
            else
            {
                timeUpdateInterval =86400;
                return $"{Math.Floor(timeDifference.TotalDays / 365)} y";
            }
        }else
        {
            return "";
        }
   }

    IEnumerator ReCallingTimeDifference(DateTime postTime){
        yield return new WaitForSeconds(timeUpdateInterval);
        Date.text = CalculateTimeDifference(postTime).ToString();
    }
    IEnumerator GetProfileImage(string url)
    {
        string newUrl = url+"?width=256&height=256";
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (ProfileImage != null && www.texture!= null)
            {
                ProfileImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
            }
            www.Dispose();
        }
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
                scrollerController.updateLikeCount(feedId,likeResponse.data.likeCount,isLiked);
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

    public void SetFeedUiController(FeedScroller controller){ 
        scrollerController = controller;    
    }

    private void OnDisable()
    {
        ProfileImage.sprite= defaultProfileImage;
        DisplayName.text = "";
        PostText.text = "";
        Date.text = "";
        Likes.text = "";
        Likes.color = UnLikedColor;
        Heart.sprite = UnLikedHeart;
        timeUpdateInterval =1;
        isLiked = false;
        isEnable = false;
        gameObject.GetComponent<FeedData>().StopAllCoroutines();
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

