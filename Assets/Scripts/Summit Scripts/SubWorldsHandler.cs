using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using static XANASummitDataContainer;

public class SubWorldsHandler : MonoBehaviour
{
    [SerializeField]
    private StayTimeTrackerForSummit _stayTimeTrackerForSummit;
    private string _domeID;
    private bool _isBuilderWorld;
    public bool IsEnteringInSubWorld = false;
    public GameObject SubworldListParent;
    public Transform ContentParent;
    public GameObject SubworldPrefab;

    [Header("Description Panel Objects")]
    public GameObject DescriptionPanelParent;
    public Image ThumbnailImage;
    public TMPro.TextMeshProUGUI WorldName;
    public TMPro.TextMeshProUGUI WorldDescription;
    public TMPro.TextMeshProUGUI WorldCreatorName;
    public TMPro.TextMeshProUGUI WorldType;
    public TMPro.TextMeshProUGUI WorldCategory;
    public TMPro.TextMeshProUGUI WorldEstTime;
    public TMPro.TextMeshProUGUI DomeId;
    public Button EnterButton;
    public GameObject EnterButtonAnimation;
    public Button BackButton;
    
    public XANASummitDataContainer XANASummitDataContainer;
    public XANASummitSceneLoading XANASummitSceneLoadingInstance;

    public static Action<Sprite,string, string,string, string, string, string, string,Vector3, OfficialWorldDetails,bool> OpenSubWorldDescriptionPanel;
    //public static int CurrentlyLoadedDomes;

    private string worldId;
    private Vector3 playerReturnPosition;
    private List<GameObject> subworldsList = new List<GameObject>();
    public OfficialWorldDetails selectedWold;

    // Start is called before the first frame update
    void OnEnable()
    {
        //BuilderEventManager.AfterWorldInstantiated += AddSubWorld;
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += AddSubWorld;
        OpenSubWorldDescriptionPanel += OpenDescirptionPanel;

        EnterButton.onClick.AddListener(EnterWorld);
        BackButton.onClick.AddListener(OnBack);

    }

    private void OnDisable()
    {
        //BuilderEventManager.AfterWorldInstantiated -= AddSubWorld;
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= AddSubWorld;
        OpenSubWorldDescriptionPanel -= OpenDescirptionPanel;

        EnterButton.onClick.RemoveListener(EnterWorld);
        BackButton.onClick.RemoveListener(OnBack);
    }


    /// <summary>
    /// Code looks complicated because we have handled all condition like we more number of subworlds added at backend side in scene does not have same number of teleport object and vice versa
    /// </summary>
    void AddSubWorld()
    {
        if (ConstantsHolder.HaveSubWorlds)
        {
            for (int i = 0; i < XANASummitDataContainer.summitData.domes.Count; i++)
            {
                if (ConstantsHolder.domeId == XANASummitDataContainer.summitData.domes[i].id)
                {
                    for (int j = 0; j < XANASummitDataContainer.SceneTeleportingObjects.Count; j++)
                    {
                        //if (j < XANASummitDataContainer.SceneTeleportingObjects.Count)
                        //{
                            int subworldIndex = XANASummitDataContainer.SceneTeleportingObjects[j].GetComponent<SummitSubWorldIndex>().SubworldIndex;
                            if (subworldIndex < XANASummitDataContainer.summitData.domes[i].SubWorlds.Count)
                            {
                                XANASummitDataContainer.SceneTeleportingObjects[j].gameObject.AddComponent<OnTriggerSceneSwitch>();
                                XANASummitDataContainer.SceneTeleportingObjects[j].gameObject.GetComponent<OnTriggerSceneSwitch>().DomeId = -1;
                                if (XANASummitDataContainer.summitData.domes[i].SubWorlds[subworldIndex].builderWorld)
                                    XANASummitDataContainer.SceneTeleportingObjects[j].gameObject.GetComponent<OnTriggerSceneSwitch>().WorldId = XANASummitDataContainer.summitData.domes[i].SubWorlds[subworldIndex].builderSubWorldId;
                                else
                                    XANASummitDataContainer.SceneTeleportingObjects[j].gameObject.GetComponent<OnTriggerSceneSwitch>().WorldId = XANASummitDataContainer.summitData.domes[i].SubWorlds[subworldIndex].selectWorld.id.ToString();
                            }
                        //}
                    }
                    return;
                }
            }
        }
    }



    public Task<bool> CreateSubWorldList(XANASummitDataContainer.DomeGeneralData domeGeneralData, Vector3 PlayerReturnPosition)
    {
        //if (ConstantsHolder.domeId != domeGeneralData.id)
        //{
        //    ClearOldData();
        //}
        //else if(ContentParent.transform.childCount>0)
        //{
        //    SubworldListParent.SetActive(true);
        //    return new Task<bool>(() => true);
        //}
        //CurrentlyLoadedDomes = ConstantsHolder.domeId;
        ClearOldData();
        SubworldListParent.SetActive(true);
        for (int i = 0; i < domeGeneralData.SubWorlds.Count; i++)
        {
            GameObject temp = Instantiate(SubworldPrefab,ContentParent);
            SubWorldPrefab _SubWorldPrefab = temp.GetComponent<SubWorldPrefab>();
            if (domeGeneralData.SubWorlds[i].officialWorld)
                _SubWorldPrefab.WorldId = domeGeneralData.SubWorlds[i].selectWorld.id;
            else
                _SubWorldPrefab.WorldId = int.Parse(domeGeneralData.SubWorlds[i].builderSubWorldId);
            
            _SubWorldPrefab.SubWorldName = domeGeneralData.SubWorlds[i].selectWorld.label;
            _SubWorldPrefab.WorldDescription = domeGeneralData.SubWorlds[i].selectWorld.description;
            _SubWorldPrefab.CreatorName = domeGeneralData.SubWorlds[i].selectWorld.creatorName;
            _SubWorldPrefab.WorldType = domeGeneralData.SubWorlds[i].selectWorld.subWorldType;
            _SubWorldPrefab.WorldCategory = domeGeneralData.SubWorlds[i].selectWorld.subWorldCategory;
            _SubWorldPrefab.WorldTimeEstimate = domeGeneralData.SubWorlds[i].selectWorld.label;
            _SubWorldPrefab.WorldDomeId = domeGeneralData.id.ToString();
            _SubWorldPrefab.ThumbnailUrl = domeGeneralData.SubWorlds[i].selectWorld.icon;
            _SubWorldPrefab.WorldName.text = domeGeneralData.SubWorlds[i].selectWorld.label;
            _SubWorldPrefab.subworlddata = domeGeneralData.SubWorlds[i].selectWorld;
            _SubWorldPrefab.IsBuilderWorld = domeGeneralData.SubWorlds[i].builderWorld;

            _SubWorldPrefab.PlayerReturnPosition = PlayerReturnPosition;
            _SubWorldPrefab.Init();
            subworldsList.Add(temp);
        }
        if (domeGeneralData.SubWorlds.Count > 0)
            return new Task<bool>(() =>true);
        else
            return new Task<bool>(() => false);
    }

    void OpenDescirptionPanel(Sprite thumbnailImage,string _worldId,string worldName,string worldDesCription,string creatorName,string worldType,string worldCategory,string worldDomeId,Vector3 _playerReturnPosition, OfficialWorldDetails details,bool isBuilderWorld)
    {/*
        ThumbnailImage.sprite = thumbnailImage;
        WorldName.text = worldName;
        WorldDescription.text = worldDesCription;
        WorldCreatorName.text = worldName;
        WorldType.text = worldType;
        WorldCategory.text = worldCategory;
        DomeId.text = worldDomeId;

        worldId = _worldId;
        playerReturnPosition = _playerReturnPosition;

        DescriptionPanelParent.SetActive(true);*/
        worldId = _worldId;
        _isBuilderWorld = isBuilderWorld;
        _domeID = worldDomeId;
        IsEnteringInSubWorld = true;
        BuilderEventManager.LoadSceneByName?.Invoke(worldId, _playerReturnPosition);
        selectedWold = details;
      
    }

    public void CloseSubWorldList()
    {
        SubworldListParent.SetActive(false);
    }

    public void EnterWorld()
    {
        BuilderEventManager.LoadSceneByName?.Invoke(worldId, playerReturnPosition);
        XANASummitSceneLoading.setPlayerPositionDelegate+=OnEnteredIntoWorld;
        EnterButton.interactable = false;
        BackButton.interactable = false;
        EnterButtonAnimation.SetActive(true);
        //_isEnteringInSubWorld = true;
    }

   public void OnEnteredIntoWorld()
    {
        SubworldListParent.SetActive(false);
        DescriptionPanelParent.SetActive(false);
        EnterButton.interactable = true;
        BackButton.interactable = true;
        EnterButtonAnimation.SetActive(false);
        //if (_isEnteringInSubWorld)
        //{
        //    _isEnteringInSubWorld = false;
        //    CallAnalyticsForSubWorlds();
        //}
    }

    public void OnBack()
    {
        DescriptionPanelParent.SetActive(false);
    }

    void ClearOldData()
    {
        int x = subworldsList.Count;
        for (int i=0;i<x; i++)
            Destroy(subworldsList[i]);

        subworldsList.Clear();
        SubworldListParent.SetActive(false);
    }
    public void CallAnalyticsForSubWorlds()
    {
        if (_stayTimeTrackerForSummit != null)
        {
            if (_stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea)
                _stayTimeTrackerForSummit.IsTrackingTimeForExteriorArea = false;
            _stayTimeTrackerForSummit.DomeId = int.Parse(_domeID);
            _stayTimeTrackerForSummit.DomeWorldId = int.Parse(worldId);
            _stayTimeTrackerForSummit.IsBuilderWorld = _isBuilderWorld;
            string eventName;
            if (_isBuilderWorld)
                eventName = "TV_Dome_" + _stayTimeTrackerForSummit.DomeId + "_BW_" + _stayTimeTrackerForSummit.DomeWorldId;
            else
                eventName = "TV_Dome_" + _stayTimeTrackerForSummit.DomeId + "_XW_" + _stayTimeTrackerForSummit.DomeWorldId;
            GlobalConstants.SendFirebaseEventForSummit(eventName);
            _stayTimeTrackerForSummit.StartTrackingTime();
        }
    }


}
