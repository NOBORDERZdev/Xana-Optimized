using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public static Action<string> ActivateHeighlightOfPanelBtn;
    public Transform DisplayDialogScrollView;
    public Transform DisplayContentScrollView;
    public List<Transform> ViewItemsInScrollView = new List<Transform>();
    public Transform ViewItemPrefab;
    public Transform TabItemViewEmotes;
    public Transform TabItemViewReaction;
    public List<Transform> EmoteTabs = new List<Transform>();
    public List<Transform> ReactionTabs = new List<Transform>();
    public Color SelectedColorTab;
    public Color UnSelectedColorTab;
    public Transform CommingSoonTxt;

    protected EmoteReactionItemBtnHandler.ItemType SelectedAction;

    private int _selectedTabEmote = 0;
    private int _selectedTabReaction = 0;

    protected virtual void Awake()
    {
        SetViewItemsEmote += PopulateViewItemsEmotes;
        SetViewItemsReaction += PopulateViewItemsReaction;
        ClearViewItemsReaction += ClearItemsInView;
        SetTabSelectedEmoteAction += SetTabSelectedEmote;
        SetTabSelectedReactionAction += SetTabSelectedReaction;
        SetSeeAllTabSelectedReactionAction += SetSeeAllTabSelectAction;
        ActivateHeighlightOfPanelBtn += ActivateHeighlightOfEmoteReactionItem;
    }
    protected virtual void OnDestroy()
    {
        SetViewItemsEmote -= PopulateViewItemsEmotes;
        SetViewItemsReaction -= PopulateViewItemsReaction;
        ClearViewItemsReaction -= ClearItemsInView;
        SetTabSelectedEmoteAction -= SetTabSelectedEmote;
        SetTabSelectedReactionAction -= SetTabSelectedReaction;
        SetSeeAllTabSelectedReactionAction -= SetSeeAllTabSelectAction;
        ActivateHeighlightOfPanelBtn -= ActivateHeighlightOfEmoteReactionItem;
    }

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
        if (!this.gameObject.activeInHierarchy)
            return;

        if (actionType == EmoteReactionItemBtnHandler.ItemType.Emote)
        {
            EmoteTabs[_selectedTabEmote].GetComponent<ActionHeaderTabHandler>().SetTabDetails(TabIndex, actionName);
        }
        else
        {
            ReactionTabs[_selectedTabReaction].GetComponent<ActionHeaderTabHandler>().SetTabDetails(TabIndex, actionName);
        }
    }

    public void PopulateViewItemsEmotes(List<EmoteAnimationList> items, EmoteReactionItemBtnHandler.ItemType _selectedAction)
    {
        if(!this.gameObject.activeInHierarchy)
        {
            return;
        }

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

            if (itemscount >= items.Count) 
            { 
                return; 
            }
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

    public void PopulateViewItemsReaction(List<ReactionAnimationList> items, EmoteReactionItemBtnHandler.ItemType _selectedAction)
    {
        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        if (items.Count.Equals(0))
        {
            CommingSoonTxt.gameObject.SetActive(true);
            return;
        }
        else
        {
            CommingSoonTxt.gameObject.SetActive(false);
        }

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
            ViewItemsInScrollView.Add(spawnItem);
        }

    }

    public virtual void CloseActionDisplayDialogScroll()
    {
        DisplayDialogScrollView.gameObject.SetActive(false);
        TabItemViewEmotes.gameObject.SetActive(false);
        TabItemViewReaction.gameObject.SetActive(false);
    }

    public void ActivateHeighlightOfEmoteReactionItem(string actionName)
    {
        foreach (Transform item in ViewItemsInScrollView)
        {
            if (item.gameObject.activeInHierarchy)
            {
                if (item.GetComponent<EmoteReactionItemBtnHandler>().ActionName == actionName)
                {
                    item.GetComponent<EmoteReactionItemBtnHandler>().HeighlightObj.gameObject.SetActive(true);
                }
                else
                {
                    item.GetComponent<EmoteReactionItemBtnHandler>().HeighlightObj.gameObject.SetActive(false);
                }
            }
        }
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

    private void SetDataToViewItem(Transform populateItem, int id, string name, string thumbnail, string group, EmoteReactionItemBtnHandler.ItemType _selectedAction)
    {
        populateItem.GetComponent<EmoteReactionItemBtnHandler>().InitializeItem(_selectedAction, id, name, thumbnail, group);
    }

    private void DisableAllItemsInView()
    {
        foreach (Transform viewItem in ViewItemsInScrollView)
        {
            viewItem.gameObject.SetActive(false);
        }
    }
}
