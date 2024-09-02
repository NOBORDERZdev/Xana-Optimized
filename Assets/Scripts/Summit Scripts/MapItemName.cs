using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MapItemName : MonoBehaviour
{
    int ItemIndex;
    public string ItemName = "Pavilion: ";
    public TMP_Text ItemNameText;
    public GameObject highlighter;
    
    [HideInInspector]
    public MoveMaptoCenter manager;



    void Start()
    {
        gameObject.AddComponent<Button>().onClick.AddListener(BtnClicked);
    }

    public void SetItemName(int ind)
    {
        ItemIndex = ind;
        ItemNameText.text = ItemName + ItemIndex;
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