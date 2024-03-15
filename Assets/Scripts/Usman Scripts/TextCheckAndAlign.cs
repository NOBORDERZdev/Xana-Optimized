using UnityEngine;
using System.Text.RegularExpressions; // For basic check (optional)
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class TextCheckAndAlign : MonoBehaviour
{
    public GameObject textComponentGameObject;
    public TextMeshProUGUI textComponent;
    public float X,Y;

    void Start()
    {
      //  textComponent = textComponentGameObject.GetComponent<TextMeshProUGUI>();
        if (IsJapaneseRegex(textComponent.text))
        {
            textComponentGameObject.GetComponent<RectTransform>().localPosition = new Vector3(X,Y,0f);
        }
    }

    // Basic check using Regular Expressions (optional)
    public static bool IsJapaneseRegex(string text)
    {
        string japaneseRegex = @"[\u3040-\u309F]|[\u30A0-\u30FF]|[\u4E00-\u9FFF]";
        return Regex.IsMatch(text, japaneseRegex);
    }

   
}