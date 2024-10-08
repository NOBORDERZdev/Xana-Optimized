using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using static XANASummitDataContainer;

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
            
            if(index > 166) // 3 Domes are skipped and user for Internal
                index += 3;
            
            if (index >= 173)
                index += 3;

            MapHighlightObjs[i].GetComponent<Button>().onClick.AddListener(() => ItemClicked_Icon(index));
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

                String domePrefix = "";
                if (domesDictionary.TryGetValue(domeIdToCheck, out string domeName))
                    domePrefix = domeName;

                if(!string.IsNullOrWhiteSpace(domePrefix))
                    newObj.GetComponent<MapItemName>().SetItemName(domePrefix, domeIdToCheck, false);
                else
                    newObj.GetComponent<MapItemName>().SetItemName("MD-", domeIdToCheck, true);
                    //newObj.GetComponent<MapItemName>().SetItemName(CategoriesDomeInfos[i].DomeNamePrefix[j], domeIdToCheck, true);
            }
        }
    }

    public void ItemClicked_Icon(int ind) // This is the function that is called when the Map Icon button is clicked
    {
        Debug.Log("Icon Clicked: " + ind);

        int arratInd = ind;

        if (ind == 176) // This is Penpenz
        {
            arratInd = 170;
            ind = 173;
        }
        else if (ind == 177) // This is Ninga Dao
        {
            arratInd = 171;
            ind -= 1;
        }
        else if (ind == 178) // This is SMBC
        {
            arratInd = 172;
            ind -= 1;
        }
        else if (ind > 166) // 3 Index are skipped and used for Internal
        {
            arratInd -= 3;
            Debug.Log("Modify Item Clicked: " + arratInd);
        }

        NextStep(ind, arratInd);
    }
    public void ItemClicked(int ind) // This is the function that is called when the Name button is clicked
    {
        Debug.Log("Name 2 Clicked: " + ind);

        int arratInd = ind;

        if (ind == 176) // This is ninga DAO 
        {
            arratInd = 171;
        }
        else if (ind == 177) // This is SMBC 
        {
            arratInd = 172;
        }
        else if (ind > 166) // 3 Index are skipped and used for Internal
        {
            arratInd -= 3;
            Debug.Log("Modify Item Clicked: " + arratInd);
        }

        NextStep(ind, arratInd);
    }
    void NextStep(int ind, int arratInd)
    {
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
                string ThumbnailUrl = dome.world360Image + "?width=512?height=256";
                StartCoroutine(DownloadTexture(ThumbnailUrl));
                selectedWorldBannerImage.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                selectedWorldBannerImage.transform.parent.gameObject.SetActive(false);
            }
            int worldId = dome.worldType == true ? dome.builderWorldId : dome.worldId;
            StartCoroutine(GetVisitorCount(worldId.ToString()));
        }
        else
        {
            selectedWorldBannerImage.transform.parent.gameObject.SetActive(false);
        }

        goBtn.SetActive(true);

        if (ind == 176) // Ninga DAO
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
    IEnumerator GetVisitorCount(string worldId)
    {
        string apiUrl = ConstantsGod.API_BASEURL + ConstantsGod.VISITORCOUNT + worldId;
        UnityWebRequest www = UnityWebRequest.Get(apiUrl);
        www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        www.SendWebRequest();
        while (!www.isDone)
            yield return null;
        string str = www.downloadHandler.text;
        VisitorInfo visitorInfo = JsonUtility.FromJson<VisitorInfo>(str);
        if (visitorInfo.success)
            totalVisitCount.text = "" + visitorInfo.data.total_visit;
        else
            totalVisitCount.text = "" + 100;
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
                normalizedPos = 0.93f;
                break;

            case 3:
            case 5:
            case 8:
                normalizedPos = 0.86f;
                break;

            case 4:
                normalizedPos = 0.76f;
                break;

            case 6:
                normalizedPos = 0.27f;
                break;

            case 7:
                normalizedPos = 0.57f;
                break;

            case 9:
                normalizedPos = 0.23f;
                break;

            case 10:
                normalizedPos = 0.78f;
                break;

            case 11:
                normalizedPos = 0.68f;
                break;

            case 12:
                normalizedPos = 0.0f;
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