using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveMaptoCenter : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float moveDuration = 1.0f;
    public List<GameObject> MapHighlightObjs;

    public GameObject mainScreen; // Reference to the parent (Main Screen)
    public GameObject childObject; // Reference to the child object containing the Grandchild
    private GameObject grandChildPing; // Reference to the Grandchild named "Ping"

    public List<Transform> CetegoryObjects;
    public List<CategoriesDomeInfo> CategoriesDomeInfos;
    public GameObject NameItemPrefab;

    public Image selectedWorldBannerImage;
    public GameObject goBtn;

    void Start()
    {
        for (int i = 0; i < MapHighlightObjs.Count; i++)
        {
            int index = i;
            MapHighlightObjs[i].GetComponent<Button>().onClick.AddListener(() => ItemClicked(index));
        }

        InitializeSubBtns();
    }


    void InitializeSubBtns()
    {
        Transform parentObj = CetegoryObjects[0];
        for (int i = 0; i < CategoriesDomeInfos.Count; i++)
        {
            parentObj = CetegoryObjects[i];
            for (int j = 0; j < CategoriesDomeInfos[i].MyDomes.Count; j++)
            {
                GameObject newObj = Instantiate(NameItemPrefab, parentObj);
                newObj.GetComponent<MapItemName>().SetItemName(CategoriesDomeInfos[i].MyDomes[j]);
                newObj.GetComponent<MapItemName>().manager = this;
            }
        }
    }


    public void ItemClicked(int ind)
    {
        Debug.Log("Item Clicked: " + ind);
        grandChildPing = MapHighlightObjs[ind];
        StartCoroutine(MoveChildToCenterOfMainScreen());
        EnableSelectedImage(ind);
    }
    IEnumerator MoveChildToCenterOfMainScreen()
    {
        Debug.Log("Moving Child to Center of Main Screen");
        Vector3 startPosition = childObject.transform.position;
        Vector3 targetPosition = mainScreen.transform.position - grandChildPing.transform.position + startPosition;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            childObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is set
        childObject.transform.position = targetPosition;
    }
    void EnableSelectedImage(int _SelectedImage)
    {
        foreach (GameObject obj in MapHighlightObjs)
        {
            obj.GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
        }

        MapHighlightObjs[_SelectedImage].GetComponent<Image>().color = new Color(1, 1, 1, 1f);
    }

    public void ExpandChild(int _Index)
    {

        // Disable Other Categories
        for (int i = 0; i < CetegoryObjects.Count; i++)
        {
            if (i != _Index)
            {
                CategoriesController(i, false);
            }
        }

        // Get the status of the child
        bool childStatus = CetegoryObjects[_Index].GetChild(1).gameObject.activeSelf;
        // Reverse the Status
        childStatus = !childStatus;

        CategoriesController(_Index, childStatus);

        // Move the ScrollRect to the top
        scrollRect.verticalNormalizedPosition = 1f;
    }

    void CategoriesController(int ind, bool status)
    {
        for (int i = 1; i < CetegoryObjects[ind].childCount; i++)
        {
            CetegoryObjects[ind].GetChild(i).gameObject.SetActive(status);
        }
    }
}

[System.Serializable]
public class CategoriesDomeInfo
{
    public string categoryName;
    public int categoryIndex;
    public List<int> MyDomes;
}