using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using System.Text.RegularExpressions;

public class PostScreenInput : MonoBehaviour
{  
   [SerializeField] TMP_Text ShowText;
   [SerializeField] TMP_InputField inputField;
   [SerializeField] RectTransform bubbleImage;
   [SerializeField] Color placeHolderColor = new Vector4();
   [SerializeField] Color normalColor = new Vector4();
   [SerializeField]  ContentSizeFitter BubbleContentSizeFitter;
   [SerializeField] TMP_Text ButtonTextRefrence;
    int maxWidth = 270;
    int maxHeight = 125;
    float characterOffset = 5.0f;

    string placeHolderText;
    TouchScreenKeyboard keyboard;
    public RectTransform bubbleParent;
    bool bubbleHeightCheck = false;
    string emojiRegexPattern = @"[\uD83C-\uDBFF\uDC00-\uDFFF]+";
    public bool ContainsEmoji(string _text)
    {
        try
        {
            Regex regex = new Regex(emojiRegexPattern);
            return regex.IsMatch(_text);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error in ContainsEmoji " + e);
            return false;
        }
    }
    private void OnEnable()
    {
        if (GameManager.currentLanguage.Equals("en"))
        {
            placeHolderText = "How are you feeling?";
        }
        else
        {
            placeHolderText = "今、どんな気分ですか？";
        }
        
        ActiveInputFeild();
        StartCoroutine(SetBubblePos());
        bubbleHeightCheck = false;
        bubbleImage.GetComponent<Image>().pixelsPerUnitMultiplier = 0.81f;
    }

    private void Start(){ 
        ShowText.text = placeHolderText;
        ShowText.color = placeHolderColor;
    }
    public void TextChange(){
        Debug.Log("Text Change " + bubbleImage.rect.height);
        ShowText.text = "";
        if (inputField.text.Count()>0) // if the input field is not empty
        {
            if (inputField.text.Count() > 0 || GameManager.Instance.userAnimationPostFeature.MoodSelected != "")
            {
                GameManager.Instance.userAnimationPostFeature.postButton.interactable = true;
                GameManager.Instance.userAnimationPostFeature.postButtonText.color=Color.white;
                ButtonTextRefrence.characterSpacing = 7.5f;
                ButtonTextRefrence.outlineWidth = 0.18f;
                ButtonTextRefrence.outlineColor = Color.white;
                
                
            }
            ShowText.text = inputField.text;
            ShowText.color = normalColor;

            // Check if the text contains an Emoji

            if (ContainsEmoji(ShowText.text))
            {
                ShowText.lineSpacing = 1.0f;
                //Debug.LogError("Emoji Found " + ShowText.lineSpacing);
            }
            else
            {
                ShowText.lineSpacing = 0.5f;
                //Debug.LogError("No Emoji Found " + ShowText.lineSpacing);
            }
        }
        else
        {
            if (GameManager.Instance.userAnimationPostFeature.MoodSelected == "")
            {
                GameManager.Instance.userAnimationPostFeature.postButton.interactable = false;
                GameManager.Instance.userAnimationPostFeature.postButtonText.color = placeHolderColor;
                ButtonTextRefrence.characterSpacing = 4f;
                ButtonTextRefrence.outlineWidth = 0f;
               
            }
            ShowText.text = placeHolderText;
            ShowText.color = placeHolderColor;
            if (GameManager.currentLanguage.Equals("en"))
            {
                if (ShowText.text == "How are you feeling?")
                {
                    BubbleContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    bubbleParent.anchorMin = new Vector2(0.1080481f, 0.6324353f);
                    bubbleParent.anchorMax = new Vector2(0.8262953f, 0.8127741f);
                }
            }
            else
            {
                if (ShowText.text == "今、どんな気分ですか？")
                {
                    BubbleContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    bubbleParent.anchorMin = new Vector2(0.1080481f, 0.6324353f);
                    bubbleParent.anchorMax = new Vector2(0.8262953f, 0.8127741f);
                }
            }
           
        }
        if (bubbleImage.rect.width >= maxWidth)
        {
            BubbleContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        if (bubbleImage.rect.height >= maxHeight && bubbleHeightCheck == false)
        {
            BubbleContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        if (ShowText.text.Count()<=35)
        {
            BubbleContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        if (ShowText.text.Count() <= 10)
        {
            bubbleParent.anchorMin = new Vector2(0.1080481f, 0.6324353f);
            bubbleParent.anchorMax = new Vector2(0.8262953f, 0.8127741f);
            bubbleImage.GetComponent<Image>().pixelsPerUnitMultiplier = 0.81f;
        }
        if (ShowText.text.Count() > 35)
        {
            bubbleImage.GetComponent<Image>().pixelsPerUnitMultiplier = 0.6f;
        }
        if (ShowText.text.Count() >= 70)
        {
            bubbleParent.anchorMin = new Vector2(0.1023894f, 0.5964248f);
            bubbleParent.anchorMax = new Vector2(0.8206365f, 0.7767635f);
        }
        if (ShowText.text.Count() >= 150)
        {
            bubbleParent.anchorMin = new Vector2(0.09747515f, 0.5200526f);
            bubbleParent.anchorMax = new Vector2(0.8157223f, 0.7003913f);
        }
       
    }

    public void ActiveInputFeild()
    {
        inputField.Select();
        inputField.ActivateInputField();
        BubbleContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        if (ShowText.text.Count() >= 70)
        {
            bubbleParent.anchorMin = new Vector2(0.1023894f, 0.5964248f);
            bubbleParent.anchorMax = new Vector2(0.8206365f, 0.7767635f);
            bubbleHeightCheck = true;
        }
        if (ShowText.text.Count() >= 150)
        {
            bubbleParent.anchorMin = new Vector2(0.09747515f, 0.5200526f);
            bubbleParent.anchorMax = new Vector2(0.8157223f, 0.7003913f);
        }
        //inputField.MoveToEndOfLine(shift: true, ctrl: false);
        // inputField.caretPosition = inputField.text.Length;


    }
    IEnumerator SetBubblePos()
    {
        yield return new WaitForEndOfFrame();
        ShowText.text = placeHolderText;
        ShowText.color = placeHolderColor;
        BubbleContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        bubbleParent.anchorMin = new Vector2(0.1080481f, 0.6324353f);
        bubbleParent.anchorMax = new Vector2(0.8262953f, 0.8127741f);

    }
    //private string FormatInput(string input)
    //{
    //    string formattedText = "";

    //    for (int i = 0; i < input.Length; i += 36)
    //    {
    //        int length = Mathf.Min(36, input.Length - i);
    //        formattedText += input.Substring(i, length);
    //        formattedText += "\n";

    //        // Check if the formattedText exceeds 110 characters
    //        if (formattedText.Length > 110)
    //        {
    //            formattedText = formattedText.Substring(0, 110);
    //            break;
    //        }
    //    }

    //    return formattedText;
    //}

    //    public void IncreaseImageWidth()
    //    {
    //        float currentWidth = bubbleImage.rect.width;
    //        float newWidth = currentWidth + characterOffset;
    //        bubbleImage.sizeDelta = new Vector2 (Mathf.Min(newWidth, maxWidth), bubbleImage.rect.height);
    //    }
    //    public void DecreaseImageWidth()
    //    {
    //        float currentWidth = bubbleImage.rect.width;
    //        float newWidth = currentWidth - characterOffset;
    //        bubbleImage.sizeDelta = new Vector2 (Mathf.Min(newWidth, maxWidth), bubbleImage.rect.height);
    //    }
}
