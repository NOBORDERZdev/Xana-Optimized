using UnityEngine;
using System.Text.RegularExpressions; // For basic check (optional)
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class TextCheckAndAlign : MonoBehaviour
{
    public GameObject textComponentGameObject;
    public TextMeshProUGUI textComponent;
    public float MarginBottom;
    public bool IsUseForChangeAllign;
    public bool IsUseForChangeMargin;
    public float X, Y;

    void Start()
    {
        if (textComponentGameObject != null)
        {
            if (IsJapaneseRegex(textComponent.text))
            {
                textComponentGameObject.GetComponent<RectTransform>().localPosition = new Vector3(X, Y, 0f);
                Invoke("ChangeCharacterSpacingForJP", 1f);
            }
        }
       
       
        
       
    }


    void ChangeCharacterSpacingForJP()
    {

        if (IsUseForChangeAllign)
        {
            if (IsJapaneseRegex(textComponent.text))
            {
                textComponent.characterSpacing = -15f;
            }
        }
        if (IsUseForChangeMargin)
        {
            if (IsJapaneseRegex(textComponent.text))
            {
                textComponent.margin = new Vector4(0, 0, 0, MarginBottom);
            }
        }
    }

    // Basic check using Regular Expressions (optional)
    public static bool IsJapaneseRegex(string text)
    {
        string japaneseRegex = @"[\u3040-\u309F]|[\u30A0-\u30FF]|[\u4E00-\u9FFF]";
        return Regex.IsMatch(text, japaneseRegex);
    }


}