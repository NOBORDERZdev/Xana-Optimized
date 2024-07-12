using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XanaWorldInfoHolder : MonoBehaviour
{
    public string worldJson;
    public Transform assetsParent;

    // Start is called before the first frame update
    private void OnEnable()
    {
        XanaWorldDownloader.assetParentStatic = assetsParent;
        BuilderEventManager.AfterPlayerInstantiated += StartDownloadingAssets;
    }


    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= StartDownloadingAssets;
    }

    void StartDownloadingAssets()
    {
        //Debug.LogError("started downloading from here........");
        BuilderEventManager.XanaMapDataDownloaded?.Invoke(worldJson);
    }

    public static void UnSubscribeEvent()
    {
        Action.Remove(BuilderEventManager.AfterPlayerInstantiated.GetInvocationList()[0], BuilderEventManager.AfterPlayerInstantiated.GetInvocationList()[0]);
    }
}
