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
    public CommentReplyManager replyManager;

    [SerializeField]
    private TextMeshProUGUI UserNameTxt;
    [SerializeField]
    private TMP_InputField commentText;
    [SerializeField]
    private Button seeMoreButton;
    [SerializeField]
    private TextMeshProUGUI timeText;
    private string fullComment;
    private int totalLike = 0;
    private int totalDislike = 0;

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

    private void UpdatePostData(string userName, string comment, string creatAt)
    {
        UserNameTxt.text = commentText.text = timeText.text = "";

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

    public void ViewMoreBtnPressed()
    {
        replyManager.ClickOnViewMore();
    }

}
