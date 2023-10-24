using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Photon.Voice.Unity.Demos;

public class WorldCategoryUIHandler : MonoBehaviour
{
    [SerializeField]
    TMP_Text CategoryName;
    [SerializeField]
    Transform WorldElement,SpawnWorldParent;
    string _categoryType;
    float addHeight = 430f, addWidth = 321f;
    public void Init(string categoryName,int totalWorlds)
    {
        CategoryName.text = categoryName;
        CalculateAndSetContentSize(totalWorlds);
    }
    public void AddWorldElementToUI(WorldItemDetail _event)
    {
        Transform worldItem = Instantiate(WorldElement.gameObject, SpawnWorldParent).transform;
        worldItem.gameObject.SetActive(true);
        worldItem.GetComponent<WorldItemView>().InitItem(0, Vector2.zero, _event);
    }
    public void CalculateAndSetContentSize(int totalWorlds)
    {
        if (totalWorlds > 0 && totalWorlds <= 6)
        {
            SpawnWorldParent.GetComponent<RectTransform>().sizeDelta = new Vector2(addWidth * totalWorlds, addHeight + 30f);
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1077f, addHeight + 30f);
        }
        else
        {
            float sizess = totalWorlds / 2;
            SpawnWorldParent.GetComponent<RectTransform>().sizeDelta = new Vector2(addWidth * sizess, addHeight * 2f);
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1077f, addHeight * 2f);
        }
    }
}
