using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class PostUIManager : MonoBehaviour
{
    //[HideInInspector]
    public int commentId;
    //[HideInInspector]
    public int replyId;
    [SerializeField]
    private TextMeshProUGUI UserNameTxt;
    [SerializeField]
    private TMP_InputField commentText;
    [SerializeField]
    private Button seeMoreButton;
    //[SerializeField]
    //private int previewLength = 100; // Character limit for preview
    [SerializeField]
    private TextMeshProUGUI likeText;
    [SerializeField]
    private TextMeshProUGUI dislikeText;
    [SerializeField]
    private TextMeshProUGUI timeText;
    //[Space(5)]
    //[SerializeField]
    //private ContentSizeFitter txtSizeFitter;
    //[SerializeField]
    //private ContentSizeFitter bodySizeFitter;
    //[SerializeField]
    //private ContentSizeFitter uiSizeFitter;
    private string fullComment;
    private int totalLike = 0;
    private int totalDislike = 0;

    private void OnEnable()
    {
         commentText.text = UserNameTxt.text = timeText.text = likeText.text = dislikeText.text = "";
    }

    public void SetComment(string userName, string comment, string creatAt, int commentNum)
    {
        this.commentId = commentNum;
        UpdatePostData(userName, comment, creatAt);
    }
    public void SetCommentReply(string userName, string comment, string creatAt, int replyNum)
    {
        this.replyId = replyNum;
        UpdatePostData(userName, comment, creatAt);
    }

    public void ShowFullComment()
    {
        commentText.text = fullComment;
        seeMoreButton.gameObject.SetActive(false);
        StartCoroutine(EnableDisableSizeFitter());
    }

    public void OnClickLike()
    {
        totalLike = totalLike + 1;
        likeText.text = totalLike.ToString();
    }
    public void OnClickDislike()
    {
        totalDislike = totalDislike + 1;
        dislikeText.text = totalDislike.ToString();
    }

    //private void SetSizeFitterComponent()
    //{
    //    txtSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    //    bodySizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    //    uiSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    //}

    private void UpdatePostData(string userName, string comment, string creatAt)
    {
        UserNameTxt.text = userName;
        commentText.text = comment;
        timeText.text = ExtractTime(creatAt);
        fullComment = comment;

        commentText.textComponent.ForceMeshUpdate();
        int lineCount = commentText.textComponent.textInfo.lineCount;
        //Debug.LogError("lineCount: " + lineCount);
        if (lineCount > 2)
        {
            int characterCount = commentText.textComponent.textInfo.lineInfo[0].characterCount + (commentText.textComponent.textInfo.lineInfo[1].characterCount - 15);
            //Debug.LogError("characterCount2: " + characterCount);
            commentText.text = comment.Substring(0, characterCount) + "...";
            seeMoreButton.gameObject.SetActive(true);
        }
    }

    IEnumerator EnableDisableSizeFitter()
    {
        GetComponent<ContentSizeFitter>().enabled = false;
        //SetSizeFitterComponent();
        yield return new WaitForSeconds(0.1f);
        GetComponent<ContentSizeFitter>().enabled = true;
    }

    private string ExtractTime(string createdTime)
    {
        DateTime dateTime = DateTime.Parse(createdTime);

        // Extract date, add "at", and extract time in HH:mm:ss format
        //string formattedDate = dateTime.ToString("yyyy-MM-dd");
        //string formattedTime = dateTime.ToString("HH:mm:ss");

        //return $"{formattedDate} at {formattedTime}";

        return dateTime.ToString("yyyy-MM-dd");
    }

}
