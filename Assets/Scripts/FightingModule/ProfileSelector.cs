using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSelector : MonoBehaviour
{
    public static ProfileSelector _instance;

    public int currentProfile = -1;

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null) _instance = this;
        DontDestroyOnLoad(gameObject);
    }

   
}
