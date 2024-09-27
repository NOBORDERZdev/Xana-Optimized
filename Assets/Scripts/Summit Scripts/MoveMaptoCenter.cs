using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

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
    public TMPro.TMP_Text totalVisitCount;


    private void OnEnable()
    {
        totalVisitCount.text = "" + ConstantsHolder.visitorCount;
    }
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

        if (ind == 176) // This is ninga DAO 
        {
            arratInd = 170;
        }
        else if (ind == 177) // This is SMBC 
        {
            arratInd = 171;
        }
        else if (ind >= 167) // There is Somedome which are handles differently
        {
            ind += 3;
            Debug.Log("Modify Item Clicked: " + ind);
        }

        grandChildPing = MapHighlightObjs[arratInd];
        string areaName = grandChildPing.name;
        StartCoroutine(MoveChildToCenterOfMainScreen());
        EnableSelectedImage(arratInd);

        // Get the Thumbnail URL
        var dome = dataManager.summitData.domes.FirstOrDefault(d => d.id == (ind + 1));
        if (dome != null)
        {
            if (!string.IsNullOrEmpty(dome.world360Image))
            {
                string ThumbnailUrl = dome.world360Image+"?width=512?height=256";
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

        if(ind == 176) // Ninga DAO
            DomeMinimapDataHolder.OnSetDomeId?.Invoke(168, areaName);
        if (ind == 177) // SMBC
            DomeMinimapDataHolder.OnSetDomeId?.Invoke(178, areaName);
        else 
            DomeMinimapDataHolder.OnSetDomeId?.Invoke(ind + 1, areaName);
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


    int selectedCategoryIndex = -1;
    public void ExpandChild(int _Index)
    {
        selectedCategoryIndex = _Index;

        //Disable Other Categories
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

        if (childStatus)
            selectedCategoryIndex = _Index;
        else
            selectedCategoryIndex = -1;

        CategoriesController(_Index, childStatus);

        Invoke(nameof(AddDelay), timeDelay);
    }
    void CategoriesController(int ind, bool status)
    {
        Vector3 localPos = CetegoryObjects[ind].localPosition;
        for (int i = 1; i < CetegoryObjects[ind].childCount; i++)
        {
            CetegoryObjects[ind].GetChild(i).gameObject.SetActive(status);
        }

    }
    void AddDelay()
    {
        float normalizedPos = 0.0f;
        switch (selectedCategoryIndex)
        {
            case 0:
            case 1:
                normalizedPos = 1.0f;
                break;

            case 2:
                normalizedPos = 0.9f;
                break;

            case 3:
                normalizedPos = 0.93f;
                break;

            case 4:
            case 5:
                normalizedPos = 0.8f;
                break;

            case 6:
                normalizedPos = 0.88f;
                break;

            case 7:
                normalizedPos = 0.0f;
                break;

            case 8:
                normalizedPos = 0.47f;
                break;

            case 9:
                normalizedPos = 0.26f;
                break;

            case 10:
                normalizedPos = 0.3f;
                break;


            default:
                normalizedPos = 1f;
                break;
        }

        scrollRect.verticalNormalizedPosition = normalizedPos;
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