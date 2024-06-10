using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteReactionUIHandler : MonoBehaviour
{
    public static Action ClearViewItemsReaction;
    public static Action< List<EmoteAnimationList>, EmoteReactionItemBtnHandler.ItemType> SetViewItemsEmote;
    public static Action< List<ReactionAnimationList>, EmoteReactionItemBtnHandler.ItemType> SetViewItemsReaction;

    public static Action<int, int> SetTabSelectedEmoteAction;
    public static Action<int, int> SetTabSelectedReactionAction;
    public static Action<EmoteReactionItemBtnHandler.ItemType, string, int> SetSeeAllTabSelectedReactionAction;

    public Transform DisplayDialogScrollView;
    public Transform DisplayContentScrollView;
    public List<Transform> ViewItemsInScrollView = new List<Transform>();
    public Transform ViewItemPrefab;
    protected EmoteReactionItemBtnHandler.ItemType SelectedAction;
    public Transform TabItemViewEmotes;
    public Transform TabItemViewReaction;
    public List<Transform> EmoteTabs = new List<Transform>();
    public List<Transform> ReactionTabs = new List<Transform>();
    private int _selectedTabEmote = 0;
    private int _selectedTabReaction = 0;
    public Color SelectedColorTab;
    public Color UnSelectedColorTab;

    public void SetTabSelectedEmote(int selectedTabEmote, int selectedTab)
    {
        _selectedTabEmote = selectedTab;
        for (int i = 0; i < EmoteTabs.Count; i++)
        {
            if(i == _selectedTabEmote)
            {
                EmoteTabs[i].GetComponent<Text>().color = SelectedColorTab;
            }
            else
            {
                 EmoteTabs[i].GetComponent<Text>().color = UnSelectedColorTab;
            }
        }
    }
    public void SetTabSelectedReaction(int selectedTabReaction, int selectedTab)
    {
        _selectedTabReaction = selectedTab;
        for (int i = 0; i < ReactionTabs.Count; i++)
        {
            if (i == _selectedTabReaction)
            {
                ReactionTabs[i].GetComponent<Text>().color = SelectedColorTab;
            }
            else
            {
                ReactionTabs[i].GetComponent<Text>().color = UnSelectedColorTab;
            }
        }
    }
    public void SetSeeAllTabSelectAction(EmoteReactionItemBtnHandler.ItemType actionType, string actionName, int TabIndex)
    {
        if (actionType == EmoteReactionItemBtnHandler.ItemType.Emote)
        {
            EmoteTabs[_selectedTabEmote].GetComponent<ActionHeaderTabHandler>().SetTabDetails(TabIndex, actionName);
        }
        else
        {
            ReactionTabs[_selectedTabReaction].GetComponent<ActionHeaderTabHandler>().SetTabDetails(TabIndex, actionName);
        }
    }
    private void Awake()
    {
        SetViewItemsEmote += PopulateViewItemsEmotes;
        SetViewItemsReaction += PopulateViewItemsReaction;
        ClearViewItemsReaction += ClearItemsInView;
        SetTabSelectedEmoteAction += SetTabSelectedEmote;
        SetTabSelectedReactionAction += SetTabSelectedReaction;
        SetSeeAllTabSelectedReactionAction += SetSeeAllTabSelectAction;
    }
    private void OnDisable()
    {
        SetViewItemsEmote -= PopulateViewItemsEmotes;
        SetViewItemsReaction -= PopulateViewItemsReaction;
        ClearViewItemsReaction -= ClearItemsInView;
        SetTabSelectedEmoteAction -= SetTabSelectedEmote;
        SetTabSelectedReactionAction -= SetTabSelectedReaction;
        SetSeeAllTabSelectedReactionAction -= SetSeeAllTabSelectAction;
    }
    private void SetTabOfItem()
    {
        if(SelectedAction == EmoteReactionItemBtnHandler.ItemType.Emote)
        {
            TabItemViewEmotes.gameObject.SetActive(true);
            TabItemViewReaction.gameObject.SetActive(false);
        }
        else
        {
            TabItemViewEmotes.gameObject.SetActive(false);
            TabItemViewReaction.gameObject.SetActive(true);
        }
    }
    private void ClearItemsInView()
    {
        foreach (Transform viewItem in ViewItemsInScrollView)
        {
            Destroy(viewItem.gameObject);
        }
        ViewItemsInScrollView.Clear();
    }
    public void PopulateViewItemsEmotes(List<EmoteAnimationList> items, EmoteReactionItemBtnHandler.ItemType _selectedAction )
    {
  
        SelectedAction = _selectedAction;
        SetTabOfItem();
        DisplayDialogScrollView.gameObject.SetActive(true);
        DisableAllItemsInView();
        int itemscount = 0;
        foreach (Transform viewItem in ViewItemsInScrollView)
        {
            SetDataToViewItem(viewItem, items[itemscount].id, items[itemscount].name, 
                items[itemscount].thumbnail, items[itemscount].group, _selectedAction);

            viewItem.gameObject.SetActive(true);

            itemscount++;

            if (itemscount >= items.Count) { return; }
        }
        for (int i = itemscount; i < items.Count; i++)
        {
            Transform spawnItem = Instantiate(ViewItemPrefab.gameObject, DisplayContentScrollView).transform;
            SetDataToViewItem(spawnItem, items[itemscount].id, items[itemscount].name, 
                items[itemscount].thumbnail, items[itemscount].group, _selectedAction);

            spawnItem.gameObject.SetActive(true);

            itemscount++;
            ViewItemsInScrollView.Add( spawnItem );
        }
    }
    public void PopulateViewItemsReaction(List<ReactionAnimationList> items, EmoteReactionItemBtnHandler.ItemType _selectedAction)
    {

        SelectedAction = _selectedAction;
        SetTabOfItem();
        DisplayDialogScrollView.gameObject.SetActive(true);
        DisableAllItemsInView();
        int itemscount = 0;

        foreach (Transform viewItem in ViewItemsInScrollView)
        {
            SetDataToViewItem(viewItem, items[itemscount].id, items[itemscount].name, 
                items[itemscount].thumbnail, items[itemscount].group, _selectedAction);

            viewItem.gameObject.SetActive(true);

            itemscount++;
            if (itemscount >= items.Count) {  return; }
        }
        for (int i = itemscount; i < items.Count; i++)
        {
            Transform spawnItem = Instantiate(ViewItemPrefab.gameObject, DisplayContentScrollView).transform;
            SetDataToViewItem(spawnItem, items[itemscount].id, items[itemscount].name, 
                items[itemscount].thumbnail, items[itemscount].group, _selectedAction);

            spawnItem.gameObject.SetActive(true);

            itemscount++;
            ViewItemsInScrollView.Add(spawnItem);
        }
    
    }
    private void SetDataToViewItem(Transform populateItem, int id, string name, string thumbnail, string group, EmoteReactionItemBtnHandler.ItemType _selectedAction)
    {
        populateItem.GetComponent<EmoteReactionItemBtnHandler>().InitializeItem
            (_selectedAction, id, name, thumbnail, group);
    }
    private void DisableAllItemsInView()
    {
        foreach (Transform viewItem in ViewItemsInScrollView)
        {
            viewItem.gameObject.SetActive(false);
        }
    }
    public virtual void CloseActionDisplayDialogScroll()
    {
        DisplayDialogScrollView.gameObject.SetActive(false);
        TabItemViewEmotes.gameObject.SetActive(false);
        TabItemViewReaction.gameObject.SetActive(false);
    }
}
