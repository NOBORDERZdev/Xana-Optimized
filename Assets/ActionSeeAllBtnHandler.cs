using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSeeAllBtnHandler : MonoBehaviour
{
    public List<Transform> _seeAllBtnEmote = new List<Transform>();
    public List<Transform> _seeAllBtnReaction = new List<Transform>();
    public Color SelectedColor = Color.white;
    public Color UnSelectedColor = Color.white;

    public void SetSelectedSeeAllBtnEmote(int i)
    {
        for(int item = 0; item < _seeAllBtnEmote.Count; item++)
        {
            _seeAllBtnEmote[item].GetComponent<Text>().color = UnSelectedColor;
        }

        _seeAllBtnEmote[i].GetComponent<Text>().color = SelectedColor;
    }

    public void SetSelectedSeeAllBtnReaction(int i)
    {
        for (int item = 0; item < _seeAllBtnReaction.Count; item++)
        {
            _seeAllBtnReaction[item].GetComponent<Text>().color = UnSelectedColor;
        }

        _seeAllBtnReaction[i].GetComponent<Text>().color = SelectedColor;
    }
}
