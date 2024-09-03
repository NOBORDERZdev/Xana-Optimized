using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MapItemName : MonoBehaviour
{
    public TMP_Text ItemNameText;
    public GameObject highlighter;

    [HideInInspector]
    public MoveMaptoCenter manager;

    int ItemIndex;
    string ItemName = "";

    void Start()
    {
        gameObject.AddComponent<Button>().onClick.AddListener(BtnClicked);
    }

    public void SetItemName(string namePrefix, int ind, bool canUsePrefix = true)
    {
        ItemIndex = ind;
        if (canUsePrefix)
            ItemName = namePrefix + ItemIndex;
        else
            ItemName = namePrefix ;

        ItemNameText.text = ItemName;
    }

    void BtnClicked()
    {
        manager.ItemClicked(ItemIndex);
        highlighter.SetActive(true);
    }

    private void OnDisable()
    {
        highlighter.SetActive(false);
    }
}