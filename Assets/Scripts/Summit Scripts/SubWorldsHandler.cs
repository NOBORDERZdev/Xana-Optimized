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

    }

    void GetSubWorldDetail()
    {

    }

}
