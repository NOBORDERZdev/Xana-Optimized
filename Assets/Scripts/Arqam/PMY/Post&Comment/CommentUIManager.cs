using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
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
    [SerializeField][TextArea]
    private string fullComment;

    private void OnEnable()
    {
        commentText.text = likeText.text = dislikeText.text = "";
        SetComment(fullComment, 10, 2);
    }

    public void SetComment(string comment, int totalLike, int totalDislike)
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
        likeText.text = totalLike.ToString();
        dislikeText.text = totalDislike.ToString();
    }

    public void ShowFullComment()
    {
        commentText.text = fullComment;
        seeMoreButton.gameObject.SetActive(false);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(commentText.rectTransform);
        StartCoroutine(EnableDisableSizeFitter());
    }

    IEnumerator EnableDisableSizeFitter()
    {
        GetComponent<ContentSizeFitter>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        GetComponent<ContentSizeFitter>().enabled = true;
    }

    public void OnClickLike()
    {

    }
    public void OnClickDislike()
    {

    }
}
