using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NftDataScript : MonoBehaviour
{
    private GameObject NftObject;
    public GameObject ContentPanel;
    public GameObject ListItemPrefab;
    public GameObject NftDeatilsPage;
    public GameObject NoNftyet;
    private Sprite sprite;
    public GameObject nftloading;
    public GameObject NftLoadingPenal;
    public static NftDataScript Instance;
    public GameObject ScrollerObj;
    public GameObject NftTransferedPanel;
    public GameObject NftWorldEquipPanel;

    public List<string> EquipCollectionAddresses;
    public List<string> ComingSoonCollectionAddresses;
    public List<string> UnAvailableCollectionAddresses;

    public List<string> TestnetEquipCollectionAddresses;
    public List<string> TestnetComingSoonCollectionAddresses;

    private void Awake()
    {
        NoNftyet.SetActive(false);
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(this);
        }
    }

    public void ResetNftData()
    {
        if (ContentPanel.transform.childCount > 0)
        {
            foreach (Transform obj in ContentPanel.transform)
            {
                Destroy(obj.gameObject);
            }
            Resources.UnloadUnusedAssets();
        }
    }
}