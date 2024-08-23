using UnityEngine;

public class SubWorldsHandler : MonoBehaviour
{
    public XANASummitDataContainer XANASummitDataContainer;
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

}
