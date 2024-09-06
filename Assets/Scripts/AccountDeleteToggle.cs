using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountDeleteToggle : MonoBehaviour
{
    public Toggle Toggle;
    public Button Button;
    // public TextMeshProUGUI DescriptionText;
    public float TopAlignment;

    public void Awake()
    {
        if (LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
        {
            Vector3 localPos = transform.localPosition;
            localPos.y = TopAlignment; // Set your desired local Y position
            transform.localPosition = localPos;
        }
    }
    public void ChangeButtonOnToggle()
   {
        if (Toggle.isOn)
        {
            Button.interactable = true;
           // DescriptionText.color = new Color32(0, 0, 0, 255);
        }
        else
        {
            Button.interactable = false;
           // DescriptionText.color = new Color32(65, 65, 65, 255);
        }

   }
}
