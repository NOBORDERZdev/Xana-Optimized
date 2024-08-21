using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubWorldsHandler : MonoBehaviour
{
    public XANASummitDataContainer XANASummitDataContainer;
    // Start is called before the first frame update
    void OnEnable()
    {
        BuilderEventManager.AfterWorldInstantiated += AddSubWorld;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterWorldInstantiated -= AddSubWorld;
    }

    void AddSubWorld()
    {
        if (ConstantsHolder.HaveSubWorlds)
        {
            for (int i = 0; i < XANASummitDataContainer.summitData.domes.Count; i++)
            {
                if (ConstantsHolder.domeId == XANASummitDataContainer.summitData.domes[i].id)
                {
                    for (int j = 0; j < XANASummitDataContainer.summitData.domes[i].SubWorlds.Count; j++)
                    {
                        if (j < XANASummitDataContainer.SceneTeleportingObjects.Count)
                        {
                            XANASummitDataContainer.SceneTeleportingObjects[j].gameObject.AddComponent<OnTriggerSceneSwitch>();
                            XANASummitDataContainer.SceneTeleportingObjects[j].gameObject.GetComponent<OnTriggerSceneSwitch>().DomeId = -1;
                            if (XANASummitDataContainer.summitData.domes[i].SubWorlds[j].builderWorld)
                                XANASummitDataContainer.SceneTeleportingObjects[j].gameObject.GetComponent<OnTriggerSceneSwitch>().WorldId = XANASummitDataContainer.summitData.domes[i].SubWorlds[j].builderSubWorldId;
                            else
                                XANASummitDataContainer.SceneTeleportingObjects[j].gameObject.GetComponent<OnTriggerSceneSwitch>().WorldId = XANASummitDataContainer.summitData.domes[i].SubWorlds[j].selectWorld.id.ToString();
                        }
                    }
                    return;
                }
            }
        }
    }

}
