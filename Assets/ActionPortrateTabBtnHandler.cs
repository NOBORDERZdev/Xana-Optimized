using System.Collections.Generic;
using UnityEngine;

public class ActionPortrateTabBtnHandler : MonoBehaviour
{
    [SerializeField] private List<Transform> TabBtnEmote = new List<Transform>();
    [SerializeField] private List<Transform> TabBtnReaction = new List<Transform>();

    private void Start()
    {
        TabBtnEmote[0].gameObject.SetActive(true);
        TabBtnReaction[0].gameObject.SetActive(true);
    }
    public void SetSelectedSeeAllBtnEmote(int i)
    {
        for (int item = 0; item < TabBtnEmote.Count; item++)
        {
            TabBtnEmote[item].gameObject.SetActive(false);
        }
        TabBtnEmote[i].gameObject.SetActive(true);
    }
    public void SetSelectedSeeAllBtnReaction(int i)
    {
        for (int item = 0; item < TabBtnReaction.Count; item++)
        {
            TabBtnReaction[item].gameObject.SetActive(false);
        }
        TabBtnReaction[i].gameObject.SetActive(true);
    }
}
