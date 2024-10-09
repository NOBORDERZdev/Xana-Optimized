using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpButtonComponentResizer : MonoBehaviour
{
    Vector3 pos;
    public Transform target;
    public Camera cam;
    public bool isAlwaysOn = true;
    public TextMeshProUGUI titleText, contentText;
    public ScrollRect scrollView;
    public GameObject scrollbar;
    public Button helpUIDownTextbtn;
    public string msg;
    bool isAgainCollided;
    int textCharCount = 0;
    float val;
    float letterDelay = 0.1f;
    float infopopuptotalHeight;
    float singleLineHeight;
    bool isInfoTextWritten;

    private RectTransform viewportRectT;
    private int rightPosition = 23;
    private int bottomPosition = -9;
    internal void Init()
    {
        isInfoTextWritten = false;
        contentText.text = msg;
        Invoke(nameof(InfoPopupUILinesCount), 0.1f);
        //StartCoroutine(StoryNarration());
    }

    void InfoPopupUILinesCount()
    {
        contentText.rectTransform.parent.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        scrollView.enabled = false;
        scrollbar.SetActive(false);


        infopopuptotalHeight = contentText.rectTransform.rect.height;

        // Get the number of lines in the text.
        int numberOfLines = contentText.textInfo.lineCount;
        // Calculate the single line height by dividing the total height by the number of lines.
        singleLineHeight = infopopuptotalHeight / numberOfLines;

        helpUIDownTextbtn.interactable = !isInfoTextWritten;

    }

    IEnumerator StoryNarration()
    {
        //string msg = this.msg;
        #region
        isAgainCollided = true;
        yield return new WaitForSeconds(0.2f);
        isAgainCollided = false;
        #endregion
        while (textCharCount < msg.Length && !isAgainCollided)
        {
            contentText.text += msg[textCharCount];
            //if (GamificationComponentData.instance.arialFont)
            //    contentText.font = GamificationComponentData.instance.arialFont;
            textCharCount++;

            yield return new WaitForSeconds(letterDelay);
            //StartCoroutine(WaitForScrollingOption());
        }
        isInfoTextWritten = false;
        InfoPopupUILinesCount();
    }

    private void OnEnable()
    {
        viewportRectT = scrollView.viewport.GetComponent<RectTransform>();
        StartCoroutine(CheckJapaneseRoutine());
    }

    private IEnumerator CheckJapaneseRoutine()
    {
        yield return new WaitForSeconds(0.15f); //Wait for the text to be set
        switch (LocalizationManager._instance.IsJapanese(contentText.text))
        {
            case false:
                viewportRectT.offsetMin = new Vector2(0, 0);
                viewportRectT.offsetMax = new Vector2(0, 0);
                break;
            case true:
                viewportRectT.offsetMin = new Vector2(viewportRectT.offsetMin.x, bottomPosition);
                viewportRectT.offsetMax = new Vector2(-rightPosition, viewportRectT.offsetMax.y);
                break;
        }
        StopCoroutine(CheckJapaneseRoutine());
    }

    public void DisplayDownText()
    {
        if (scrollView.content.anchoredPosition.y + singleLineHeight * 4 <= infopopuptotalHeight)
        {
            scrollView.content.anchoredPosition += new Vector2(0, singleLineHeight);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        cam = null;
        StopCoroutine(StoryNarration());
        textCharCount = 0;
    }

    void Update()
    {
        if (GamificationComponentData.instance.playerControllerNew == null)
            return;

        if (!isAlwaysOn)
        {
            if (target == null)
                return;
            if (cam == null)
                cam = GamificationComponentData.instance.playerControllerNew.ActiveCamera.GetComponent<Camera>();

            pos = transform.position;
            pos.x = cam.WorldToScreenPoint(target.position).x;
            float distance = Vector3.Distance(GamificationComponentData.instance.playerControllerNew.transform.position, GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform.position);
            if (distance > 3.5f)
            {
                val = 1 - (0.5f / 27) * (distance - 3.5f);
                val = Mathf.Clamp(val, 0, 1);
                transform.localScale = Vector3.one * val;
            }
            else
                transform.localScale = Vector3.one;
            transform.position = pos;
        }
        else
        {
            transform.LookAt(GamificationComponentData.instance.playerControllerNew.ActiveCamera.transform);
            transform.Rotate(new Vector3(0, 1, 0), 180f);
        }

    }
}