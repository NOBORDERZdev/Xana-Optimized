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

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("UserRegisterationManager.instance.nftlsit"+ UserRegisterationManager.instance.nftlsit);
        // getLocalStorageEmote();
    }

    public void ResetNftData()
    {
        if (ContentPanel.transform.childCount > 0)
        {
            foreach (Transform obj in ContentPanel.transform)
            {
                //if (obj.gameObject.GetComponent<RawImage>().texture != null)
                //{
                //    obj.gameObject.GetComponent<RawImage>().texture = null;
                //}
                //obj.gameObject.SetActive(false);
                Destroy(obj.gameObject);
            }
            Resources.UnloadUnusedAssets();
            //UserRegisterationManager.instance.NFTIDList.Clear();
            //UserRegisterationManager.instance.NFTList.Clear();
            // UserRegisterationManager.instance._web3APIforWeb2.OwnedNFTPageNumb = 1;
            //if (MyProfileDataManager.Instance!=null)
            //{
            //    MyProfileDataManager.Instance.OnScrollNFT();
            //}
        }
    }

    public void getLocalStorageNft()
    {
        if (!string.IsNullOrEmpty(UserRegisterationManager.instance.XanaliaUserTokenId))
        {
            if (!UserRegisterationManager.instance.nftlist.Contains("Something went wrong"))
            {
                NftLoadingPenal.SetActive(true);
                NoNftyet.SetActive(false);
                NoNftyet.GetComponent<TMPro.TextMeshProUGUI>().text = string.Empty;
                nftloading.SetActive(true);

                if (File.Exists(GetStringFolderPath()) && !string.IsNullOrEmpty(File.ReadAllText(GetStringFolderPath())))
                {
                    try
                    {
                        AssetBundle.UnloadAllAssetBundles(false);
                        Resources.UnloadUnusedAssets();
                        NftData bean = Gods.DeserializeJSON<NftData>(File.ReadAllText(GetStringFolderPath()));
                        if (bean.data.Count > 0)
                        {
                            for (int i = 0; i < bean.data.Count; i++)
                            {
                                NftObject = Instantiate(ListItemPrefab);
                                NftObject.transform.SetParent(ContentPanel.transform);
                                NftObject.transform.localPosition = Vector3.zero;
                                NftObject.transform.localScale = Vector3.one;
                                NftObject.transform.localRotation = Quaternion.identity;
                                NftObject.transform.GetChild(0).gameObject.SetActive(true);

                                StartCoroutine(LoadSpriteEnv(bean.data[i].thumbnailUrl + "?tr=w-350,h-350", NftObject.transform.GetChild(0).gameObject, i));

                                if (bean.data[i].price != null && !string.IsNullOrEmpty(bean.data[i].price))
                                {
                                    NftObject.transform.GetChild(1).gameObject.SetActive(true);
                                }
                                else
                                {
                                    NftObject.transform.GetChild(2).gameObject.SetActive(true);
                                }

                                if (bean.data[i].metaData.properties.type.Equals("movie"))
                                {
                                    NftObject.transform.GetChild(3).gameObject.SetActive(true);
                                }
                                else
                                {
                                    NftObject.transform.GetChild(3).gameObject.SetActive(false);
                                }

                                OwnedNftLoadButton OBC = NftObject.GetComponent<OwnedNftLoadButton>();

                                if (OBC == null)
                                {
                                    OBC = NftObject.AddComponent<OwnedNftLoadButton>();
                                }

                                OBC.Initializ(bean.data[i].thumbnailUrl + "?tr=w-350,h-350", bean.data[i].owner, bean.data[i].metaData.name, bean.data[i].metaData.description, bean.data[i].metaData.properties.type, bean.data[i].nftLink, this, NftDeatilsPage
                                   );
                                //if (GameManager.currentLanguage.Equals("ja"))
                                //{
                                //    OBC.Initializ(bean.data[i].thumbnailUrl, bean.data[i].owner, bean.data[i].creator, bean.data[i].ja_nft_description, this,NftDeatilsPage
                                //   );
                                //}
                                //else
                                //{
                                //    Debug.Log("data description===="+ bean.data[i].en_nft_description);
                                //    OBC.Initializ(bean.data[i].thumbnailUrl, bean.data[i].owner, bean.data[i].creator, bean.data[i].en_nft_description, this, NftDeatilsPage
                                //   );
                                //}
                            }
                        }
                        else
                        {
                            Debug.Log("call hua else data");
                            NftLoadingPenal.SetActive(true);
                            NoNftyet.SetActive(true);
                            NoNftyet.GetComponent<TMPro.TextMeshProUGUI>().text = TextLocalization.GetLocaliseTextByKey("NFT data not found");
                            nftloading.SetActive(false);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Nft Response not parsing Error====" + e);
                        NftLoadingPenal.SetActive(true);
                        NoNftyet.SetActive(true);
                        NoNftyet.GetComponent<TMPro.TextMeshProUGUI>().text = TextLocalization.GetLocaliseTextByKey("NFT data not found");
                        nftloading.SetActive(false);
                    }
                }
                else
                {
                    try
                    {
                        AssetBundle.UnloadAllAssetBundles(false);
                        Resources.UnloadUnusedAssets();
                        NftData bean = Gods.DeserializeJSON<NftData>(UserRegisterationManager.instance.nftlist);
                        if (bean != null)
                        {
                            if (bean.data.Count > 0)
                            {
                                for (int i = 0; i < bean.data.Count; i++)
                                {
                                    NftObject = Instantiate(ListItemPrefab);
                                    NftObject.transform.SetParent(ContentPanel.transform);
                                    NftObject.transform.localPosition = Vector3.zero;
                                    NftObject.transform.localScale = Vector3.one;
                                    NftObject.transform.localRotation = Quaternion.identity;
                                    NftObject.transform.GetChild(0).gameObject.SetActive(true);

                                    StartCoroutine(LoadSpriteEnv(bean.data[i].thumbnailUrl + "?tr=w-350,h-350", NftObject.transform.GetChild(0).gameObject, i));

                                    if (bean.data[i].price != null && !string.IsNullOrEmpty(bean.data[i].price))
                                    {
                                        NftObject.transform.GetChild(1).gameObject.SetActive(true);
                                    }
                                    else
                                    {
                                        NftObject.transform.GetChild(2).gameObject.SetActive(true);
                                    }

                                    if (bean.data[i].metaData.properties.type.Equals("movie"))
                                    {
                                        NftObject.transform.GetChild(3).gameObject.SetActive(true);
                                    }
                                    else
                                    {
                                        NftObject.transform.GetChild(3).gameObject.SetActive(false);
                                    }

                                    OwnedNftLoadButton OBC = NftObject.GetComponent<OwnedNftLoadButton>();

                                    if (OBC == null)
                                    {
                                        OBC = NftObject.AddComponent<OwnedNftLoadButton>();
                                    }

                                    OBC.Initializ(bean.data[i].thumbnailUrl + "?tr=w-350,h-350", bean.data[i].owner, bean.data[i].metaData.name, bean.data[i].metaData.description, bean.data[i].metaData.properties.type, bean.data[i].nftLink, this, NftDeatilsPage
                                       );
                                    //if (GameManager.currentLanguage.Equals("ja"))
                                    //{
                                    //    OBC.Initializ(bean.data[i].thumbnailUrl, bean.data[i].owner, bean.data[i].creator, bean.data[i].ja_nft_description, this,NftDeatilsPage
                                    //   );
                                    //}
                                    //else
                                    //{
                                    //    Debug.Log("data description===="+ bean.data[i].en_nft_description);
                                    //    OBC.Initializ(bean.data[i].thumbnailUrl, bean.data[i].owner, bean.data[i].creator, bean.data[i].en_nft_description, this, NftDeatilsPage
                                    //   );
                                    //}
                                }
                            }
                            else
                            {
                                Debug.Log("call hua else data");
                                NftLoadingPenal.SetActive(true);
                                NoNftyet.SetActive(true);
                                NoNftyet.GetComponent<TMPro.TextMeshProUGUI>().text = TextLocalization.GetLocaliseTextByKey("NFT data not found");
                                nftloading.SetActive(false);
                            }
                        }
                        else
                        {
                            StartCoroutine(NftGet(2f));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Nft response not parsing error in store file====" + e);
                        NftLoadingPenal.SetActive(true);
                        NoNftyet.SetActive(true);
                        NoNftyet.GetComponent<TMPro.TextMeshProUGUI>().text = TextLocalization.GetLocaliseTextByKey("NFT data not found");
                        nftloading.SetActive(false);
                    }
                }
            }
            else
            {
                Debug.Log("call hua else data");
                NftLoadingPenal.SetActive(true);
                NoNftyet.SetActive(true);
                NoNftyet.GetComponent<TMPro.TextMeshProUGUI>().text = TextLocalization.GetLocaliseTextByKey("NFT data not found");
                nftloading.SetActive(false);
            }
        }
        else
        {
            Debug.Log("call hua else data");
            NftLoadingPenal.SetActive(true);
            NoNftyet.SetActive(true);
            NoNftyet.GetComponent<TMPro.TextMeshProUGUI>().text = TextLocalization.GetLocaliseTextByKey("NFT data not found");
            nftloading.SetActive(false);
        }
    }

    IEnumerator NftGet(float delay)
    {
        yield return new WaitForSeconds(delay);
        getLocalStorageNft();
    }

    IEnumerator LoadSpriteEnv(string ImageUrl, GameObject thumbnail, int i)
    {
        if (i == 0)
        {
            NftLoadingPenal.SetActive(false);
            NoNftyet.SetActive(false);
            nftloading.SetActive(false);
            NoNftyet.GetComponent<TMPro.TextMeshProUGUI>().text = string.Empty;
        }
        Debug.Log("image url====" + ImageUrl);
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
        }
        else
        {
            if (ImageUrl.Equals(""))
            {
            }
            else
            {
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(ImageUrl);
                www.SendWebRequest();
                while (!www.isDone)
                {
                    yield return null;
                }
                Texture2D thumbnailTexture = DownloadHandlerTexture.GetContent(www);
                if (thumbnailTexture != null)
                {
                    thumbnailTexture.Compress(true);
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                    }
                    else
                    {
                        sprite = Sprite.Create(thumbnailTexture, new Rect(0, 0, thumbnailTexture.width, thumbnailTexture.height), new Vector2(0, 0));
                        if (thumbnail != null)
                        {
                            thumbnail.GetComponent<Image>().sprite = sprite;
                        }
                    }
                }
                www.Dispose();
            }
        }
    }


    public void currentSelection()
    {
        if (ContentPanel.transform.childCount == 0)
        {
            getLocalStorageNft();
        }
    }

    public string GetStringFolderPath()
    {
        return (Application.persistentDataPath + "/NftData.txt");
    }


    public class NftData
    {
        public bool success { get; set; }
        public List<NftList> data { get; set; }
        public int count { get; set; }
    }

    public class NftList
    {
        //   public string _id { get; set; }
        public MetaData metaData { get; set; }
        //public string tokenId { get; set; }
        //public string chain { get; set; }
        //public int approval { get; set; }
        //public int timestamp { get; set; }
        //public int is_test { get; set; }
        //public int rating { get; set; }
        //public string newtokenId { get; set; }
        public string price { get; set; }
        ////  public int? sortPrice { get; set; }
        //public object sortPrice2 { get; set; }
        //public int like_count { get; set; }
        //public int like { get; set; }
        public string thumbnailUrl { get; set; }
        public string owner { get; set; }
        //public string checkMinter { get; set; }
        public string nftLink { get; set; }
        //public string creator { get; set; }
        //public List<CreatorObj> creatorObj { get; set; }
        //public string en_nft_description { get; set; }
        //public string en_nft_name { get; set; }
        //public string ja_nft_description { get; set; }
        //public string ja_nft_name { get; set; }
        //public string ko_nft_description { get; set; }
        //public string ko_nft_name { get; set; }
        //public string zh_ch_nft_description { get; set; }
        //public string zh_ch_nft_name { get; set; }
        //public string zh_nft_description { get; set; }
        //public string zh_nft_name { get; set; }
        //public bool? blind { get; set; }
        //public string buyId { get; set; }
        //public string collection { get; set; }
        //public bool? offchain { get; set; }
        //public string oldCollection { get; set; }
        //public string seriesId { get; set; }
        public SellNFT sellNFT { get; set; }
    }

    public class Properties
    {
        public string type { get; set; }
    }

    public class MetaData
    {
        public string name { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public Properties properties { get; set; }
        //public object totalSupply { get; set; }
        //public string externalLink { get; set; }
        //public string thumbnft { get; set; }
        //public string imageOld { get; set; }
    }

    public class SellNFT
    {
        //   public string _id { get; set; }
        //   public string seller { get; set; }
        //   public int boxId { get; set; }
        //   // public int seriesId { get; set; }
        //   public string orignalPrice { get; set; }
        //   public int currencyType { get; set; }
        //   public string collection { get; set; }
        //   public string owner { get; set; }
        //   public string baseCurrency { get; set; }
        //   // public string name { get; set; }
        //   public string tokenId { get; set; }
        //   public bool offchain { get; set; }
        //   public string price { get; set; }
        //   public string priceConversion { get; set; }
        //   public bool buy { get; set; }
        ////   public object sellDateTime { get; set; }
        //   public string buyTxHash { get; set; }
        ////   public object buyDateTime { get; set; }
        //   public string mainChain { get; set; }
        //   public string buyBy { get; set; }
        //   public bool tokenCollectionFlag { get; set; }
        //   public string buybaseCurrency { get; set; }
        //   public string buyCurrencyType { get; set; }
        //   public string buyCalculated { get; set; }
    }

    public class CreatorObj
    {
        //   public string _id { get; set; }
        //   public string username { get; set; }
        //   public string email { get; set; }
        //   public string role { get; set; }
        ////   public object emailVerificationToken { get; set; }
        //   public bool emailVerified { get; set; }
        //   public string about { get; set; }
        //   public string address { get; set; }
        //   public string firstName { get; set; }
        //   public string lastName { get; set; }
        //   public string name { get; set; }
        //   public string phoneNumber { get; set; }
        //   public bool resetPasswordStart { get; set; }
        //   public string profile_image { get; set; }
        //   public Links links { get; set; }
        //   public int followers { get; set; }
        //   public string en_about { get; set; }
        //   public string ja_about { get; set; }
        //   public string ko_about { get; set; }
        //   public string zh_about { get; set; }
        //   public bool featured { get; set; }
        //   public string zh_ch_about { get; set; }
        //   public string stripeCustomerIdold1 { get; set; }
        //   public string stripeCustomerId { get; set; }
        //   public string title { get; set; }
        //   public int transalte_again { get; set; }
        //   public string referralCode { get; set; }
        //   public int following { get; set; }
        //   public string userNftRole { get; set; }
        //   public bool passwordShared { get; set; }
        //   public bool dataShared2 { get; set; }
        //   public bool dataShared3 { get; set; }
        //   public List<string> userNftRoleArr { get; set; }
    }

    public class Links
    {
        //public string facebook { get; set; }
        //public string website { get; set; }
        //public string discord { get; set; }
        //public string twitter { get; set; }
        //public string instagram { get; set; }
        //public string youtube { get; set; }
        //public string zoomLink { get; set; }
    }
}