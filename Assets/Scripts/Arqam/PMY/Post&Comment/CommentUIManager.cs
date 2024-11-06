using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CommentUIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI UserNameTxt;
    [SerializeField]
    private TMP_InputField commentText;
    [SerializeField]
    private Button seeMoreButton;
    [SerializeField]
    private int previewLength = 100; // Character limit for preview
    [SerializeField]
    private TextMeshProUGUI likeText;
    [SerializeField]
    private TextMeshProUGUI dislikeText;
    [Space(5)]
    [SerializeField]
    private ContentSizeFitter txtSizeFitter;
    [SerializeField]
    private ContentSizeFitter bodySizeFitter;
    [SerializeField]
    private ContentSizeFitter uiSizeFitter;
    [SerializeField]
    private GameObject replySection;
    [SerializeField]
    private TMP_InputField replyInputField;
    [SerializeField]
    private GameObject childCommentUI;

    private string fullComment;
    private int totalLike = 0;
    private int totalDislike = 0;

    private void OnEnable()
    {
         commentText.text = UserNameTxt.text = likeText.text = dislikeText.text = "";
    }

    public void SetComment(string userName, string comment)
    {
        UserNameTxt.text = userName;
        commentText.text = comment;
        fullComment = comment;

        commentText.textComponent.ForceMeshUpdate();
        int lineCount = commentText.textComponent.textInfo.lineCount;
        Debug.LogError("lineCount: " + lineCount);
        if (lineCount > 2)
        {
            int characterCount = commentText.textComponent.textInfo.lineInfo[0].characterCount + (commentText.textComponent.textInfo.lineInfo[1].characterCount - 15);
            Debug.LogError("characterCount2: " + characterCount);
            commentText.text = comment.Substring(0, characterCount) + "...";
            seeMoreButton.gameObject.SetActive(true);
        }
    }


    public void ShowFullComment()
    {
        commentText.text = fullComment;
        seeMoreButton.gameObject.SetActive(false);
        StartCoroutine(EnableDisableSizeFitter());
    }

    IEnumerator EnableDisableSizeFitter()
    {
        GetComponent<ContentSizeFitter>().enabled = false;
        SetSizeFitter();
        yield return new WaitForSeconds(0.1f);
        GetComponent<ContentSizeFitter>().enabled = true;
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

    public void ClickOnReply()
    {
        replySection.SetActive(true);
    }

    public void ClickOnCancel()
    {
        replySection.SetActive(false);
    }

    public void SendReply()
    {
        if (!replyInputField.text.IsNullOrEmpty())
        {
            replySection.SetActive(false);
            GameObject child = Instantiate(childCommentUI);
            child.transform.SetParent(transform, false);
            child.GetComponent<CommentUIManager>().SetComment("Xana PMY", replyInputField.text);
            replyInputField.text = "";
        }
        else
        {
            Debug.LogError("Please Enter something");
        }
    }

    private void SetSizeFitter()
    {
        txtSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        bodySizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        uiSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }


}
