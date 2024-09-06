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
    public ContentSizeFitter ContentFitterRef;
    private void OnEnable()
    {
        if (LocalizationManager.forceJapanese || GameManager.currentLanguage == "ja")
        {
            TagHodler.spacing = 11f;
            TagHodler.padding.left = -112;
        }
        else
        {
            TagHodler.spacing = 10f;
            TagHodler.padding.left = -277;
        }
        Invoke(nameof(SetComponentState), 0.01f);
    }
    private void Start()
    {
        ActivateSelectedTag(0);
    }

    public void SetComponentState()
    {
        if (!ContentFitterRef.enabled)
        {
            ContentFitterRef.enabled = true;
        }
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
