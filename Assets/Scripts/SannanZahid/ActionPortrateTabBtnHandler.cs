using System.Collections.Generic;
using UnityEngine;

public class ActionPortrateTabBtnHandler : MonoBehaviour
{
    [SerializeField] private List<Transform> _tabBtnEmote = new List<Transform>();
    [SerializeField] private List<Transform> _tabBtnReaction = new List<Transform>();

    private void Start()
    {
        _tabBtnEmote[0].gameObject.SetActive(true);
        _tabBtnReaction[0].gameObject.SetActive(true);
    }

    public void SetSelectedSeeAllBtnEmote(int i)
    {
        for (int item = 0; item < _tabBtnEmote.Count; item++)
        {
            _tabBtnEmote[item].gameObject.SetActive(false);
        }

        _tabBtnEmote[i].gameObject.SetActive(true);
    }

    public void SetSelectedSeeAllBtnReaction(int i)
    {
        for (int item = 0; item < _tabBtnReaction.Count; item++)
        {
            _tabBtnReaction[item].gameObject.SetActive(false);
        }

        _tabBtnReaction[i].gameObject.SetActive(true);
    }
}
