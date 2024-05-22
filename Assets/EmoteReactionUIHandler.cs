using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteReactionUIHandler : MonoBehaviour
{
    public static Action< List<EmoteAnimationList>, EmoteReactionItemBtnHandler.ItemType> SetViewItemsEmote;
    public static Action< List<ReactionAnimationList>, EmoteReactionItemBtnHandler.ItemType> SetViewItemsReaction;
    public Transform DisplayDialogScrollView;
    public Transform DisplayContentScrollView;
    public List<Transform> ViewItemsInScrollView = new List<Transform>();
    public Transform ViewItemPrefab;
    public EmoteReactionItemBtnHandler.ItemType SelectedAction;
    public Transform TabItemViewEmotes;
    public Transform TabItemViewReaction;

    private void Awake()
    {
        SetViewItemsEmote += PopulateViewItemsEmotes;
        SetViewItemsReaction += PopulateViewItemsReaction;
    }
    private void OnDisable()
    {
        SetViewItemsEmote -= PopulateViewItemsEmotes;
        SetViewItemsReaction -= PopulateViewItemsReaction;
    }
    void SetTabOfItem()
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
                items[itemscount].thumbnail, items[itemscount].group);
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
                items[itemscount].thumbnail, items[itemscount].group);

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
                items[itemscount].thumbnail, items[itemscount].group);

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
                items[itemscount].thumbnail, items[itemscount].group);

            spawnItem.gameObject.SetActive(true);

            itemscount++;
            ViewItemsInScrollView.Add(spawnItem);
        }
    
    }
    void SetDataToViewItem(Transform populateItem, int id, string name, string thumbnail, string group)
    {
        populateItem.GetComponent<EmoteReactionItemBtnHandler>().InitializeItem
            (SelectedAction, id, name, thumbnail, group);
    }
    void DisableAllItemsInView()
    {
        foreach (Transform viewItem in ViewItemsInScrollView)
        {
            viewItem.gameObject.SetActive(false);
        }
    }
}
