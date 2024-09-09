using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailEntryTPPHandler : MonoBehaviour
{
    public List<GameObject> TeleportPoints = new List<GameObject>();

    private void OnEnable()
    {
        if (ConstantsHolder.isFromXANASummit)
        {
            foreach (GameObject obj in TeleportPoints)
            {
                obj.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject obj in TeleportPoints)
            {
                obj.SetActive(false);
            }
        }
    }
}
