using UnityEngine;
using TMPro;

public class WorldCategoryUIHandler : MonoBehaviour
{
    [SerializeField]
    TMP_Text CategoryName;
    [SerializeField]
    Transform WorldElement,ScrollerWorlds,SpawnWorldParent;
    string _categoryType;
    float addHeight = 535f, addWidth = 351f;
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
        float Parentheight = transform.parent.GetComponent<RectTransform>().sizeDelta.y;
        if (totalWorlds > 0 && totalWorlds <= 6)
        {
            float height = addHeight + 35f;
            float width = 1077f;
            SpawnWorldParent.GetComponent<RectTransform>().sizeDelta = new Vector2(addWidth * totalWorlds, height);
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(width, Parentheight + height);
            ScrollerWorlds.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }
        else
        {
            float sizess = Mathf.Ceil( (float)totalWorlds / 2f);
            SpawnWorldParent.GetComponent<RectTransform>().sizeDelta = new Vector2(addWidth * sizess, addHeight * 2f);
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(1077f, addHeight * 2f);
            transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(1077f, Parentheight + addHeight + 30f);
            ScrollerWorlds.GetComponent<RectTransform>().sizeDelta = new Vector2(1077f, addHeight * 2f);
        }
    }
    public void ViewAllCategoryItems()
    {
        Debug.LogError("View All Items  --- " + CategoryName.text);
        switch (CategoryName.text)
        {
            case "Hot": UIManager.Instance.SetWorldToDisplay(APIURL.Hot); break; 
            case "Game": UIManager.Instance.SetWorldToDisplay(APIURL.GameWorld); break;
            case "New": UIManager.Instance.SetWorldToDisplay(APIURL.AllWorld); break;
            case "Event": UIManager.Instance.SetWorldToDisplay(APIURL.EventWorld); break;
            case "Test": UIManager.Instance.SetWorldToDisplay(APIURL.TestWorld); break;
            case "My World": UIManager.Instance.SetWorldToDisplay(APIURL.MyWorld); break;
            default: UIManager.Instance.SetWorldToDisplay(APIURL.Hot); break;
        }
    }
    public void CategoryScrollerReset()
    {
        ScrollerWorlds.GetComponent<WorldCategoryScroller>().horizontalNormalizedPosition = 0f;
    }
}