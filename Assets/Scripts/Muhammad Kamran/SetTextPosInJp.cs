using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTextPosInJp : MonoBehaviour
{
    public RectTransform SignUpText1;
    public RectTransform SignUpText2;
    public RectTransform PrivacyText1;
    public RectTransform PrivacyText2;
    private void Start()
    {
        if (CustomLocalization.forceJapanese || GameManager.currentLanguage == "ja")
        {
            SignUpText1.localPosition = new Vector2(41, 12);
            SignUpText2.localPosition = new Vector2(315, 12);
            PrivacyText1.localPosition = new Vector2(74, 0);
            PrivacyText2.localPosition = new Vector2(74, 0);
        }
    }
}
