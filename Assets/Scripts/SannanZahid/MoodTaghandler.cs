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
