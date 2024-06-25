using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSeeAllBtnHandler : MonoBehaviour
{
    public List<Transform> SeeAllBtnEmote = new List<Transform>();
    public List<Transform> SeeAllBtnReaction = new List<Transform>();
    public Color SelectedColor = Color.white;
    public Color UnSelectedColor = Color.white;

    public void SetSelectedSeeAllBtnEmote(int i)
    {
        for(int item = 0; item < SeeAllBtnEmote.Count; item++)
        {
            SeeAllBtnEmote[item].GetComponent<Text>().color = UnSelectedColor;
        }
        SeeAllBtnEmote[i].GetComponent<Text>().color = SelectedColor;
    }
    public void SetSelectedSeeAllBtnReaction(int i)
    {
        for (int item = 0; item < SeeAllBtnReaction.Count; item++)
        {
            SeeAllBtnReaction[item].GetComponent<Text>().color = UnSelectedColor;
        }
        SeeAllBtnReaction[i].GetComponent<Text>().color = SelectedColor;
    }
}
