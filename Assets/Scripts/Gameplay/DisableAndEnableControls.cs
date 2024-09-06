using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAndEnableControls : MonoBehaviour
{
    public List<GameObject> disableObjectOnEnableOfthis;
    bool isSetToValue = false;
   

    private void OnEnable()
    {
        if (!isSetToValue)
        {
            if (ConstantsHolder.xanaConstants.IsMuseum)
            {
                if (ReferencesForGamePlay.instance.disableObjects.Length > 0)
                {
                    disableObjectOnEnableOfthis.Clear();
                    foreach (GameObject go in ReferencesForGamePlay.instance.disableObjects)
                    {
                        disableObjectOnEnableOfthis.Add(go);
                    }
                }
            }
            isSetToValue = true;
        }
        foreach(GameObject go in disableObjectOnEnableOfthis)
        {
            if(go != null )
            go.SetActive(false);
        }
    }
    private void OnDisable()
    {
        foreach (GameObject go in disableObjectOnEnableOfthis)
        {
            if(go != null)
            go.SetActive(true);
        }
    }
}
