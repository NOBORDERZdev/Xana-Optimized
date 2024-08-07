using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateFontRuntime : MonoBehaviour
{
    public TMP_Text text;
    public float FontSize;
    public float CharacterSpacing;
    public float LineSpacing;
    public float newLeftMargin;
    public float newTopMargin;
    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        FontLoadOnRuntime();
    }

    // Update is called once per frame
    public void FontLoadOnRuntime()
    {
        if (LocalizationManager.forceJapanese)
        {
            TMP_FontAsset fontAsset = Resources.Load<TMP_FontAsset>("Fonts/JP R");
            text.font = fontAsset;
            text.fontSize = FontSize;
            text.characterSpacing = CharacterSpacing;
            text.lineSpacing = LineSpacing;

            // Modify the left margin
            Vector4 currentMargin = text.margin;
            currentMargin.x = newLeftMargin;
            text.margin = currentMargin;
            // Modify the Top margin
            currentMargin.y = newTopMargin;
            text.margin = currentMargin;
            


        }
    }
}
