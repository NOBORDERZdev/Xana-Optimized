using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoodTaghandler : MonoBehaviour
{
    public List<Transform> MoodText = new List<Transform>();
    public List<Transform> MoodButton = new List<Transform>();
    public Color SelectedTxtColor, UnSelectedTxtColor;
    public HorizontalLayoutGroup TagHodler;
    private void OnEnable()
    {
        if (LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
        {
            TagHodler.spacing = -203;
        }
        else
        {
            TagHodler.spacing = -340;
        }
    }
    private void Start()
    {
        ActivateSelectedTag(0);
    }
    public void ActivateSelectedTag(int index)
    {
        for (int i = 0; i < MoodText.Count; i++)
        {
            MoodText[i].GetComponent<TMPro.TMP_Text>().color = UnSelectedTxtColor;
            MoodButton[i].GetComponent<Button>().interactable = true;
        }
        MoodText[index].GetComponent<TMPro.TMP_Text>().color = SelectedTxtColor;
        MoodButton[index].GetComponent<Button>().interactable = false;

    }

}
