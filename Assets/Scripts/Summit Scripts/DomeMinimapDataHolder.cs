using TMPro;
using System;
using SimpleJSON;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;

public class DomeMinimapDataHolder : MonoBehaviour
{
    public GameObject mapBaseObj;
    public Sprite highlightedSprite;
    public List<DomeDataForMap> MapDomes;
    public List<TextMeshProUGUI> vistedCount;

    public GameObject ConfirmationPopup;
    public GameObject ConfirmationPopup_Portrait;

    public static Action<OnTriggerSceneSwitch> OnInitDome;
    Dictionary<int, Transform> allInitDomes = new Dictionary<int, Transform>();

    Transform playerTransform;
    int clickedDomeID;
    int totalDomeCount = 128;


    private void OnEnable()
    {
        allInitDomes.Clear();
        OnInitDome += UpdateDomeList;
        if(ConstantsHolder.xanaConstants.EnviornmentName.Equals("XANA Summit"))
        {
            GetVisitedDomeData();
            mapBaseObj.SetActive(true);
        }
    }
    private void OnDisable()
    {
        OnInitDome -= UpdateDomeList;
    }
    void UpdateDomeList(OnTriggerSceneSwitch domeObj)
    {
        if (!allInitDomes.ContainsKey(domeObj.domeId))
            allInitDomes.Add(domeObj.domeId, domeObj.transform);
    }
    public async Task GetVisitedDomeData()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GETVISITDOMES;
        string result = await GetAsyncRequest(url);
        
        Debug.Log("VisitedWorld: Result: " + result);
        if (string.IsNullOrEmpty(result))
        {
            Debug.LogError("Error fetching dome data.");
            return;
        }
        var jsonNode = JSON.Parse(result);
        var domeVisits = jsonNode["domeVisits"];
        HashSet<int> domeIDs = domeVisits.Children.Select(d => d["domeId"].AsInt).ToHashSet();

        string visitedText = $"({domeIDs.Count}/{totalDomeCount})";
        foreach (var _text in vistedCount)
        {
            _text.text = visitedText;
        }
        foreach (var item in MapDomes)
        {
            if (domeIDs.Contains(item.domeId))
            {
                if (item.myImage == null)
                {
                    item.myImage = item.GetComponent<UnityEngine.UI.Image>();
                }
                item.myImage.sprite = highlightedSprite;
            }
        }
    }
    private async Task<string> GetAsyncRequest(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Get Dome Visit : "+ www.error);
                return null;
            }
            else
            {
                return www.downloadHandler.text;
            }
        }
    }
    public void OnClickTeleportPlayerDomePosition(int domeId)
    {
        if(playerTransform == null)
            playerTransform = GameplayEntityLoader.instance.mainController.transform;
        clickedDomeID = domeId;
        ConfirmationPanelHandling(true);
    }
    void TeleportPlayerToSelectedDome(int _domeId, Transform playerTransform)
    {
        if (allInitDomes.TryGetValue(_domeId, out Transform domeTransform))
        {
            // Attempt to find "Player Spawner" or default to first child if not found
            Transform domePos = domeTransform.Find("Player Spawner") ?? domeTransform.GetChild(0);
            playerTransform.position = domePos.position;
            playerTransform.rotation = Quaternion.Euler(0f, domePos.rotation.eulerAngles.y, 0f);
        }
        else
        {
            Debug.Log($"Dome with ID {_domeId} not found.");
        }
    }
   
    public void ConfirmationPanelHandling(bool status)
    {
        ConfirmationPopup.SetActive(status);
        ConfirmationPopup_Portrait.SetActive(status);
    }
    public void OnClickYesBtn()
    {
        ConfirmationPanelHandling(false);
        ReferencesForGamePlay.instance.FullScreenMapStatus(false);

        TeleportPlayerToSelectedDome(clickedDomeID, playerTransform);
    }
}