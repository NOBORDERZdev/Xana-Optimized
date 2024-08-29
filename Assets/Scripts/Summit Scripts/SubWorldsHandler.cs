using System.Threading.Tasks;
using UnityEngine;

public class SubWorldsHandler : MonoBehaviour
{
    public GameObject SubworldListParent;
    public Transform ContentParent;
    public GameObject SubworldPrefab;

    public XANASummitDataContainer XANASummitDataContainer;
    public XANASummitSceneLoading XANASummitSceneLoadingInstance;
    // Start is called before the first frame update
    void OnEnable()
    {
        //BuilderEventManager.AfterWorldInstantiated += AddSubWorld;
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += AddSubWorld;
    }

    private void OnDisable()
    {
        //BuilderEventManager.AfterWorldInstantiated -= AddSubWorld;
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= AddSubWorld;
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
        SubworldListParent.SetActive(true);
        for (int i = 0; i < domeGeneralData.SubWorlds.Count; i++)
        {
            GameObject temp = Instantiate(SubworldPrefab,ContentParent);
            SubWorldPrefab _SubWorldPrefab = temp.GetComponent<SubWorldPrefab>();
            if (domeGeneralData.SubWorlds[i].officialWorld)
            {
                _SubWorldPrefab.WorldId = domeGeneralData.SubWorlds[i].selectWorld.id;
                _SubWorldPrefab.ThumbnailUrl = domeGeneralData.SubWorlds[i].selectWorld.icon;
                _SubWorldPrefab.WorldName.text = domeGeneralData.SubWorlds[i].selectWorld.label;
            }
            else
            {
                _SubWorldPrefab.WorldId = int.Parse(domeGeneralData.SubWorlds[i].builderSubWorldId);
            }

            _SubWorldPrefab.PlayerReturnPosition = PlayerReturnPosition;
            _SubWorldPrefab.SubWorldPrefabButton.onClick.AddListener(OnSubworldOpen);
            _SubWorldPrefab.Init();
        }
        if (domeGeneralData.SubWorlds.Count > 0)
            return new Task<bool>(() =>true);
        else
            return new Task<bool>(() => false);
    }


    void OnSubworldOpen()
    {
        foreach (Transform t in ContentParent)
            Destroy(t.gameObject);

        SubworldListParent.SetActive(false);
    }
   
}
