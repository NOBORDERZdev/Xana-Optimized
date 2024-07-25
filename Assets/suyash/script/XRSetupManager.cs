using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRSetupManager : MonoBehaviour
{
    public static XRSetupManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        
    }
}
