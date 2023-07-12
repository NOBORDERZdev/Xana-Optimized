using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OwnedNftLoadButton : MonoBehaviour
{
    [HideInInspector] public string thumb;
    [HideInInspector] public string owner;
    [HideInInspector] public string creator;
    [HideInInspector] public string des;
    [HideInInspector] public string type;
    [HideInInspector] public string _NftDetailsopen;
    [HideInInspector] public NftDataScript controller;
    public GameObject nftDeatilsPage;
    public static Sprite nftImage;
    
    public void Initializ(string _thumb, string _owner, string _creator, string _description, string _type, string NftDetailsopen, NftDataScript ctrlr, GameObject nftDetails)
    {
        thumb = _thumb;
        owner = _owner;
        controller = ctrlr;
        creator = _creator;
        des = _description;
        type = _type;
        _NftDetailsopen = NftDetailsopen;
        nftDeatilsPage = nftDetails;
    }

    public void OnButtonClick()
    {
        PlayerPrefs.SetString(ConstantsGod.NFTTHUMB, thumb);
        PlayerPrefs.SetString(ConstantsGod.NFTOWNER, owner);
        PlayerPrefs.SetString(ConstantsGod.NFTCREATOR, creator);
        PlayerPrefs.SetString(ConstantsGod.NFTDES, des);
        PlayerPrefs.SetString(ConstantsGod.NFTLINK, _NftDetailsopen);
        PlayerPrefs.SetString(ConstantsGod.NFTTYPE, type);
        nftImage = gameObject.transform.GetChild(0).GetComponent<Image>().sprite;
        nftDeatilsPage.SetActive(true);
    }
}