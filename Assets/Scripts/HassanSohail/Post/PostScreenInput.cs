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
   float characterOffset = 5.0f;
   string placeHolderText = "Enter the text";
   TouchScreenKeyboard keyboard;
    private void OnEnable()
    {
       ActiveInputFeild();
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
        print("~~~~~~~~~~ "+ShowText.text.Count()+"~~~~~~~~~~ ");
        if (bubbleImage.rect.width >= maxWidth)
        {
            BubbleContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        if (ShowText.text.Count()<=40)
        {
            print("!!!! else call~~~~~~~~~~ ");
            BubbleContentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
   }

    public void ActiveInputFeild()
    {
        inputField.Select();
        inputField.ActivateInputField();
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
