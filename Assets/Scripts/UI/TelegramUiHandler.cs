using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TelegramUiHandler : MonoBehaviour
{
    public List<GameObject> TelegramUIObj;

    private void Start()
    {
        StartCoroutine(TelegramFunctionality());
    }

    IEnumerator TelegramFunctionality()
    {
        yield return new WaitForEndOfFrame();
        if (ConstantsHolder.xanaConstants.TelegramLoginBtnStatus)
        {
            // print("IS true " + walletFunctionalitybool);
            foreach (GameObject go in TelegramUIObj)
            {
                go.SetActive(true);
            }
        }
        else
        {
            //print("IS false " + walletFunctionalitybool);
            foreach (GameObject go in TelegramUIObj)
            {
                go.SetActive(false);
            }
        }
    }
}

public class DatumFeature
{
    public int id;
    public string app_name;
    public string version;
    public bool is_active;
    public FeatureList feature_list;
}

public class FeatureList
{
    public bool SummitApp;
    public bool WalletBtn;
    public bool Xsummitbg;
    public bool XanaChatFlag;
    public bool TelegramLogin;
    public bool DomeHeaderInfo;
    public bool wall;
    public bool Enter;
}

public class RootFeatureData
{
    public bool success;
    public List<DatumFeature> data = new List<DatumFeature> ();
    public string msg;
}