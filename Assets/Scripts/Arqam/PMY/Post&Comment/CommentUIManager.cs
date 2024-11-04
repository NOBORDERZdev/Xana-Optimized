using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

public class CommentUIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI commentText;
    [SerializeField]
    private Button seeMoreButton;
    [SerializeField]
    private int previewLength = 100; // Character limit for preview
    [SerializeField]
    private TextMeshProUGUI likeText;
    [SerializeField]
    private TextMeshProUGUI dislikeText;
    [Space(5)][SerializeField]
    private ContentSizeFitter txtSizeFitter;
    [SerializeField]
    private ContentSizeFitter bodySizeFitter;
    [SerializeField]
    private ContentSizeFitter uiSizeFitter;
    //[SerializeField][TextArea]
    private string fullComment;
    private int totalLike = 0;
    private int totalDislike = 0;

    private void OnEnable()
    {
        commentText.text = likeText.text = dislikeText.text = "";
    }

    public void SetComment(string comment)
    {
        fullComment = comment;
        if (comment.Length > previewLength)
        {
            commentText.text = comment.Substring(0, previewLength) + "...";
            seeMoreButton.gameObject.SetActive(true);
        }
        else
        {
            commentText.text = comment;
            seeMoreButton.gameObject.SetActive(false);
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

    private void SetSizeFitter()
    {
        txtSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        bodySizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        uiSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
}
