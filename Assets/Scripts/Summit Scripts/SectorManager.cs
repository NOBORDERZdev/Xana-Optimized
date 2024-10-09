using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorManager : MonoBehaviour
{
 
    public static SectorManager Instance;

    public IEnumerator routine;

    string prevSector = "Default";
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
        if (prevSector != Name) { prevSector = Name; } else { return; }
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        if (MutiplayerController.instance.getSector() == name) { Debug.Log("Discard Sector "); return; }
        routine = WaitBeforeHandover(Name);
        StartCoroutine(routine);


    }

    public IEnumerator WaitBeforeHandover(string Name)
    {
      
        if(ConstantsHolder.DiasableMultiPartPhoton) {   yield break;    }
       
       


        yield return new WaitForSeconds(1);
        Debug.Log("Sectror trigger status  " + ConstantsHolder.TempDiasableMultiPartPhoton + "  shifting " + MutiplayerController.instance.isShifting);
        yield return new WaitUntil(() => (!ConstantsHolder.TempDiasableMultiPartPhoton && !MutiplayerController.instance.isShifting));
        MutiplayerController.instance.Ontriggered(Name);
        routine = null;
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
