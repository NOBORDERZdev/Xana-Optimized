using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RFMDevmodeManager : MonoBehaviour
{
    public static RFMDevmodeManager instance { get; private set; }

    public bool devMode = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }


}
