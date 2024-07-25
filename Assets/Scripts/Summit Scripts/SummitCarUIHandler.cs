using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummitCarUIHandler : MonoBehaviour
{
    public static SummitCarUIHandler instance;

    [SerializeField]
    private GameObject JoyStick, UIObjects;

    private void Awake()
    {if (instance == null)
            instance = this;
        else DestroyImmediate(this);
    }
    
    public void UpdateUIelement(bool enable)
    {
        JoyStick.SetActive(enable);
        UIObjects.SetActive(enable);
    }


}
