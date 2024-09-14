using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
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

    public XANASummitDataContainer dataManager;


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
        var domesDictionary = new Dictionary<int, string>();
        foreach (var dome in dataManager.summitData.domes)
        {
            domesDictionary[dome.id] = dome.name;
        }
        for (int i = 0; i < CategoriesDomeInfos.Count; i++)
        {
            Transform parentObj = CetegoryObjects[i];
            for (int j = 0; j < CategoriesDomeInfos[i].MyDomes.Count; j++)
            {
                GameObject newObj = Instantiate(NameItemPrefab, parentObj);
                newObj.GetComponent<MapItemName>().manager = this;

                int domeIdToCheck = CategoriesDomeInfos[i].MyDomes[j];
                if (domesDictionary.TryGetValue(domeIdToCheck, out string domeName))
                    newObj.GetComponent<MapItemName>().SetItemName(domeName, domeIdToCheck, false);
                else
                    newObj.GetComponent<MapItemName>().SetItemName(CategoriesDomeInfos[i].DomeNamePrefix[j], domeIdToCheck, true);
            }
        }
    }




    public void ItemClicked(int ind)
    {
        Debug.Log("Item Clicked: " + ind);

        int arratInd = ind;
        grandChildPing = MapHighlightObjs[arratInd];
        string areaName=grandChildPing.name;
        StartCoroutine(MoveChildToCenterOfMainScreen());
        EnableSelectedImage(arratInd);

        // Get the Thumbnail URL
        var dome = dataManager.summitData.domes.FirstOrDefault(d => d.id == (ind + 1));
        if (dome != null)
        {
            if (!string.IsNullOrEmpty(dome.world360Image))
            {
                string ThumbnailUrl = dome.world360Image;
                StartCoroutine(DownloadTexture(ThumbnailUrl));
                selectedWorldBannerImage.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                selectedWorldBannerImage.transform.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            selectedWorldBannerImage.transform.parent.gameObject.SetActive(false);
        }

        goBtn.SetActive(true);
        DomeMinimapDataHolder.OnSetDomeId?.Invoke(ind + 1,areaName);
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
    IEnumerator DownloadTexture(string ThumbnailUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(ThumbnailUrl);
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }
        Texture2D texture2D = DownloadHandlerTexture.GetContent(request);
        selectedWorldBannerImage.sprite = ConvertToSprite(texture2D);
    }
    Sprite ConvertToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
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

        Invoke(nameof(AddDelay), timeDelay);
    }
    void CategoriesController(int ind, bool status)
    {
        for (int i = 1; i < CetegoryObjects[ind].childCount; i++)
        {
            CetegoryObjects[ind].GetChild(i).gameObject.SetActive(status);
        }
    }
    void AddDelay()
    {
        // Move the ScrollRect to the top
        scrollRect.verticalNormalizedPosition = 1;
        //scrollRect.DOVerticalNormalizedPos(1, timeDelay).SetEase(Ease.InOutQuad);
    }

    public float timeDelay = 0.02f;
}

[System.Serializable]
public class CategoriesDomeInfo
{
    public string categoryName;
    public int categoryIndex;
    public List<int> MyDomes;
    public List<string> DomeNamePrefix;
}