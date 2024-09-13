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
    static GameObject _HighlightObj;

    int ItemIndex;
    string ItemName = "";

    void Start()
    {
        gameObject.AddComponent<Button>().onClick.AddListener(BtnClicked);
    }

    public void SetItemName(string namePrefix, int ind, bool canUsePrefix = true)
    {
        ItemIndex = (ind - 1);
        if (canUsePrefix)
            ItemName = namePrefix + (ItemIndex + 1);
        else
            ItemName = namePrefix ;

        ItemNameText.text = ItemName;
    }

    void BtnClicked()
    {
        if (_HighlightObj != null)
            _HighlightObj.SetActive(false);

        manager.ItemClicked(ItemIndex);
        highlighter.SetActive(true);
        _HighlightObj = highlighter;
    }

    //private void OnDisable()
    //{
    //    highlighter.SetActive(false);
    //}
}