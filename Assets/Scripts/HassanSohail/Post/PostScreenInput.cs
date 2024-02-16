using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PostScreenInput : MonoBehaviour
{  
   [SerializeField] TMP_Text ShowText;
   [SerializeField] TMP_InputField inputField;
   [SerializeField] RectTransform bubbleImage;
   [SerializeField] Color placeHolderColor = new Vector4();
   [SerializeField] Color normalColor = new Vector4();
   [SerializeField]  ContentSizeFitter BubbleContentSizeFitter;
   int maxWidth = 270;
    int maxHeight = 100;
   float characterOffset = 5.0f;
   string placeHolderText = "Enter the text";
   TouchScreenKeyboard keyboard;
    public RectTransform bubbleParent;
    private void OnEnable()
    {
       ActiveInputFeild();
        bubbleParent.anchorMin = new Vector2(0.1090846f, 0.6209897f);
        bubbleParent.anchorMax = new Vector2(0.8273318f, 0.8013285f);
    }

    private void Start(){ 
        ShowText.text = placeHolderText;
        ShowText.color = placeHolderColor;
    }
    public void TextChange(){
        ShowText.text = "";
        if (inputField.text.Count()>0) // if the input field is not empty
        {
            ShowText.text = inputField.text;
            ShowText.color = normalColor;
        }
        else
        {
            ShowText.text = placeHolderText;
            ShowText.color = placeHolderColor;
        }
        if (bubbleImage.rect.width >= maxWidth)
        {
            BubbleContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        if (bubbleImage.rect.height >= maxHeight)
        {
            BubbleContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        if (ShowText.text.Count()<=35)
        {
            BubbleContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        if (ShowText.text.Count() <= 10)
        {
            bubbleParent.anchorMin = new Vector2(0.1090846f, 0.6209897f);
            bubbleParent.anchorMax = new Vector2(0.8273318f, 0.8013285f);
        }
      
    }

    public void ActiveInputFeild()
    {
        inputField.Select();
        inputField.ActivateInputField();
        //inputField.MoveToEndOfLine(shift: true, ctrl: false);
       // inputField.caretPosition = inputField.text.Length;
        BubbleContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        if (ShowText.text.Count() >= 100)
        {
            bubbleParent.anchorMin = new Vector2(0.1023894f, 0.5964248f);
            bubbleParent.anchorMax = new Vector2(0.8206365f, 0.7767635f);
        }
        if (ShowText.text.Count() >= 190)
        { 
            bubbleParent.anchorMin = new Vector2(0.09747515f, 0.5500709f);
            bubbleParent.anchorMax = new Vector2(0.8157223f, 0.7304096f);
        }
       
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
