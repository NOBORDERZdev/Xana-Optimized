using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorManager : MonoBehaviour
{
 
    public static SectorManager Instance;

    public Coroutine routine;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);  
        }
        if (ConstantsHolder.xanaConstants.EnviornmentName == "XANA Summit")
        {
            ConstantsHolder.MultiSectionPhoton = true;
        }
        else {
            ConstantsHolder.MultiSectionPhoton = false;
            ConstantsHolder.DiasableMultiPartPhoton = false;
            ConstantsHolder.TempDiasableMultiPartPhoton = false;
        }
    }

    public void Triggered()
    {

        if(routine != null)
        {
            StopCoroutine(routine);
        }

    }

    public void TriggeredExit(string Name)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        
       routine = StartCoroutine(WaitBeforeHandover(Name));


    }

    public IEnumerator WaitBeforeHandover(string Name)
    {
        if(ConstantsHolder.DiasableMultiPartPhoton) {   yield break;    }
        while (ConstantsHolder.TempDiasableMultiPartPhoton==true || MutiplayerController.instance.isShifting)
        {
            yield return new WaitForSeconds(2);
        }

        yield return new WaitForSeconds(1);

        MutiplayerController.instance.Ontriggered(Name);
    }

}
