using TMPro;
using System;
using SimpleJSON;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using static StayTimeTrackerForSummit;

public class DomeMinimapDataHolder : MonoBehaviour
{
    [SerializeField]
    private StayTimeTrackerForSummit _stayTimeTrackerForSummit;
    public Sprite HighlightedSprite;
    public List<DomeDataForMap> MapDomes;
    public List<DomeDataForMap> MapDomes_portrait;
    public List<TextMeshProUGUI> VistedCount;

    public GameObject ConfirmationPopup;
    public GameObject ConfirmationPopup_Portrait;

    public static Action<OnTriggerSceneSwitch> OnInitDome;
    public static Action<int,string> OnSetDomeId;

    private Dictionary<int, Transform> _allInitDomes = new Dictionary<int, Transform>();
    private Transform _playerTransform;
    private int _clickedDomeID;
    private string _clickedDomeArea;
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
            if(_domeId>8&& _domeId<37)
            {
                MutiplayerController.instance.Ontriggered("GrassLand");
            }else if(_domeId>40&& _domeId<69)
            {
                MutiplayerController.instance.Ontriggered("Sea");
            }else if(_domeId>68&& _domeId<97)
            {
                MutiplayerController.instance.Ontriggered("Solid");
            }else if(_domeId>100&& _domeId<129)
            {
                MutiplayerController.instance.Ontriggered("Sand");
            }
            else
            {
                MutiplayerController.instance.Ontriggered("Default");
            }

            // Attempt to find "Player Spawner" or default to first child if not found
            Transform domePos = domeTransform.Find("Player Spawner") ?? domeTransform.GetChild(0);
            ConstantsHolder.isTeleporting = true;
            playerTransform.position = domePos.position;
            playerTransform.rotation = Quaternion.Euler(0f, domePos.rotation.eulerAngles.y, 0f);
            Invoke("SetTeleportingFalse", 3f);
        }
        else
        {
            Debug.Log($"Dome with ID {_domeId} not found.");
        }
    }
   

    void SetTeleportingFalse()
    {
        ConstantsHolder.isTeleporting = false;
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
        if (ActionManager.IsAnimRunning)
        {
            ActionManager.StopActionAnimation?.Invoke();

            //  EmoteAnimationHandler.Instance.StopAnimation();
            //  EmoteAnimationHandler.Instance.StopAllCoroutines();
        }

        TeleportPlayerToSelectedDome(_clickedDomeID, _playerTransform);
        CallAnalyticsFromMinimapTeleport();
    }
    void CallAnalyticsFromMinimapTeleport()
    {
        if (Enum.GetNames(typeof(SummitAreaTrigger)).Any(name => _clickedDomeArea.Contains(name)))
        {
            _clickedDomeArea = Enum.GetNames(typeof(SummitAreaTrigger)).FirstOrDefault(name => _clickedDomeArea.Contains(name));
            if (_stayTimeTrackerForSummit != null)
            {
                if (_clickedDomeArea != _stayTimeTrackerForSummit.SummitAreaName)
                {
                    _stayTimeTrackerForSummit.SummitAreaName = _clickedDomeArea;
                    string eventName = "XS_TV_" + _stayTimeTrackerForSummit.SummitAreaName;
                    GlobalConstants.SendFirebaseEventForSummit(eventName, ConstantsHolder.userId);
                    _stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea = true;
                    _stayTimeTrackerForSummit.StartTrackingTime();
                }
            }
        }
    }
    public void SetDomeID(int DomeId, string areaName)
    {
        _clickedDomeID = DomeId;
        _clickedDomeArea = areaName;
        if (_playerTransform == null)
            _playerTransform = GameplayEntityLoader.instance.mainController.transform;
    }
}