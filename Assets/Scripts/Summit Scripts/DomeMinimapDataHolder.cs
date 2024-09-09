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
    public Sprite HighlightedSprite;
    public List<DomeDataForMap> MapDomes;
    public List<DomeDataForMap> MapDomes_portrait;
    public List<TextMeshProUGUI> VistedCount;

    public GameObject ConfirmationPopup;
    public GameObject ConfirmationPopup_Portrait;

    public static Action<OnTriggerSceneSwitch> OnInitDome;
    public static Action<int> OnSetDomeId;

    private Dictionary<int, Transform> _allInitDomes = new Dictionary<int, Transform>();
    private Transform _playerTransform;
    private int _clickedDomeID;
    private int _totalDomeCount = 128;


    private void OnEnable()
    {
        OnInitDome += UpdateDomeList;
        OnSetDomeId += SetDomeID;

        SummitSceneReloaded();
    }
    private void OnDisable()
    {
        OnInitDome -= UpdateDomeList;
        OnSetDomeId -= SetDomeID;
    }

    public void SummitSceneReloaded()
    {
        _allInitDomes.Clear();
        if (ConstantsHolder.xanaConstants.EnviornmentName.Equals("XANA Summit"))
        {
            GetVisitedDomeData();
        }
    }
    void UpdateDomeList(OnTriggerSceneSwitch domeObj)
    {
        if (!_allInitDomes.ContainsKey(domeObj.DomeId))
            _allInitDomes.Add(domeObj.DomeId, domeObj.transform);
        else
        {
            _allInitDomes[domeObj.DomeId]= domeObj.transform;
        }
    }
    public async Task GetVisitedDomeData()
    {
        string url = ConstantsGod.API_BASEURL + ConstantsGod.GETVISITDOMES;
        string result = await GetAsyncRequest(url);
        
        Debug.Log("VisitedWorld: Result: " + result);
        if (string.IsNullOrEmpty(result))
        {
            Debug.Log("<color=red>Error fetching dome data. Might be the user is Guest</color>");
            return;
        }
        var jsonNode = JSON.Parse(result);
        var domeVisits = jsonNode["domeVisits"];
        HashSet<int> _VisitedDomeIDs = domeVisits.Children.Select(d => d["domeId"].AsInt).ToHashSet();

        string visitedText = $"({_VisitedDomeIDs.Count}/{_totalDomeCount})";
        VistedCount.ForEach(_text => _text.text = $"({_VisitedDomeIDs.Count}/{_totalDomeCount})");

        // Combine MapDomes and MapDomes_portrait into one collection for processing
        var allDomes = MapDomes.Concat(MapDomes_portrait);
        foreach (var item in allDomes)
        {
            if (_VisitedDomeIDs.Contains(item.domeId))
            {
                item.MyImage = item.MyImage ?? item.GetComponent<UnityEngine.UI.Image>();
                item.MyImage.sprite = HighlightedSprite;
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
        if(_playerTransform == null)
            _playerTransform = GameplayEntityLoader.instance.mainController.transform;
        _clickedDomeID = domeId;
        ConfirmationPanelHandling(true);
    }
    void TeleportPlayerToSelectedDome(int _domeId, Transform playerTransform)
    {
        if (_allInitDomes.TryGetValue(_domeId, out Transform domeTransform))
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
        if (GiantWheelManager.Instance != null && GiantWheelManager.Instance.CarAdded)
            return;

        ConfirmationPanelHandling(false);
        ReferencesForGamePlay.instance.FullScreenMapStatus(false);

        TeleportPlayerToSelectedDome(_clickedDomeID, _playerTransform);
    }

    public void SetDomeID(int DomeId)
    {
        _clickedDomeID = DomeId;
        if (_playerTransform == null)
            _playerTransform = GameplayEntityLoader.instance.mainController.transform;
    }
}