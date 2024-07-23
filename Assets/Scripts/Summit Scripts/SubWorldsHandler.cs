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
        if (ConstantsHolder.haveSubWorlds)
        {
            for (int i = 0; i < XANASummitDataContainer.summitData.domes.Count; i++)
            {
                if (ConstantsHolder.domeId == XANASummitDataContainer.summitData.domes[i].id)
                {
                    for (int j = 0; j < XANASummitDataContainer.summitData.domes[i].SubWorlds.Count; j++)
                    {
                        if (j < BuilderData.sceneTeleportingObjects.Count)
                        {
                            BuilderData.sceneTeleportingObjects[j].gameObject.AddComponent<OnTriggerSceneSwitch>();
                            BuilderData.sceneTeleportingObjects[j].gameObject.AddComponent<OnTriggerSceneSwitch>().domeId = -1;
                            if (XANASummitDataContainer.summitData.domes[i].SubWorlds[j].builderWorld)
                                BuilderData.sceneTeleportingObjects[j].gameObject.AddComponent<OnTriggerSceneSwitch>().worldId = XANASummitDataContainer.summitData.domes[i].SubWorlds[j].builderSubWorldId;
                            else
                                BuilderData.sceneTeleportingObjects[j].gameObject.AddComponent<OnTriggerSceneSwitch>().worldId = XANASummitDataContainer.summitData.domes[i].SubWorlds[j].selectWorld.id.ToString();
                        }
                    }
                    return;
                }
            }
        }
    }

}
