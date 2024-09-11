using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
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
        Debug.Log("Sectror trigger status  " + ConstantsHolder.TempDiasableMultiPartPhoton + "  shifting " + MutiplayerController.instance.isShifting);
        yield return new WaitUntil(() => (!ConstantsHolder.TempDiasableMultiPartPhoton && !MutiplayerController.instance.isShifting));
       


        MutiplayerController.instance.Ontriggered(Name);
    }


    public void UpdateMultisector()
    {
        if (ConstantsHolder.xanaConstants.EnviornmentName == "XANA Summit")
        {
            ConstantsHolder.MultiSectionPhoton = true;
        }
        else
        {
            ConstantsHolder.MultiSectionPhoton = false;
            ConstantsHolder.DiasableMultiPartPhoton = false;
            ConstantsHolder.TempDiasableMultiPartPhoton = false;
        }
    }
}
