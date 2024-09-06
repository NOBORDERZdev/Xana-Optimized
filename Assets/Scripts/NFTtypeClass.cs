using SuperStar.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NFTtypeClass : MonoBehaviour
{
    public Button OnclickNFT;
    public bool isVideoUrlFromDropbox;
    public bool VideoNFT;
    public GameObject VideoIcon;
    public GameObject NFTDetailSlider;
    /*public string ImageURL;
    public string NFTDescription;
    public string NFTName;
    public string CollectionAddress;
    public int NFTID;
    public UserNFTlistClass.Attribute _Attribute;*/
    //   public UserNFTlistClass.List NFTClass = new UserNFTlistClass.List();
    public bool isImageSuccessDownloadAndSave = false;
    public bool isReleaseFromMemoryOrNot = false;
    public bool isOnScreen;//check object is on screen or not
    public bool isVisible = false;
    float lastUpdateCallTime;
    bool isClearAfterMemory = false;
    public int _indexNumber;


    //NFT data Holder Scriptable Object
    public OwnedNFTContainer _OwnedNFTDataObj;
    public ScrollActivityNFT scrollActivityNFTRef;
    // Start is called before the first frame update
    void Start()
    {
        if (OnclickNFT.gameObject)
            OnclickNFT.onClick.AddListener(TaskOnClick);
    }
    private void OnEnable()
    {
        if (VideoNFT)
        {
            VideoIcon.SetActive(true);

        }
        else
        {
            VideoIcon.SetActive(false);
        }
        StartCoroutine(waitforIndexUpdation());
    }
    IEnumerator waitforIndexUpdation()
    {
        yield return new WaitForSeconds(.03f);
        DownloadAndLoadNFT();
    }
    private void Update()
    {
        /*
        lastUpdateCallTime += Time.deltaTime;
        if (lastUpdateCallTime > 0.3f)//call every 0.4 sec
        {
            Vector3 mousePosNormal = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
            Vector3 mousePosNR = Camera.main.ScreenToViewportPoint(mousePosNormal);

            if (mousePosNR.y >= -0.1f && mousePosNR.y <= 1.1f)
            {
                isOnScreen = true;
            }
            else
            {
                isOnScreen = false;
            }

            lastUpdateCallTime = 0;
        }

        if (isVisible && isOnScreen)//this is check if object is visible on camera then load feed or video one time
        {
            isVisible = false;
            //Debug.LogError("Image download starting one time");
            DownloadAndLoadNFT();
        }
        else if (isImageSuccessDownloadAndSave)
        {
            if (isOnScreen)
            {
                if (isReleaseFromMemoryOrNot)
                {
                    isReleaseFromMemoryOrNot = false;
                    if (!string.IsNullOrEmpty(UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsURL[_indexNumber]))
                    {
                        if (AssetCache.Instance.HasFile(UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsURL[_indexNumber]))
                        {
                            AssetCache.Instance.LoadSpriteIntoImage(OnclickNFT.image, UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsURL[_indexNumber], changeAspectRatio: true);
                            //CheckAndSetResolutionOfImage(imgFeed.sprite);
                            //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.image, changeAspectRatio: true);
                        }
                    }
                }
            }
            else
            {
                if (!isReleaseFromMemoryOrNot)
                {
                    //realse from memory 
                    isReleaseFromMemoryOrNot = true;
                    //Debug.LogError("remove from memory");
                    AssetCache.Instance.RemoveFromMemory(OnclickNFT.image.sprite);
                    //tt AssetCache.Instance.RemoveFromMemory(FeedData.image, true);
                    OnclickNFT.image.sprite = null;
                    //tt imgFeedRaw.texture = null;
                    //Resources.UnloadUnusedAssets();//every clear.......
                    //Caching.ClearCache();
                    SNS_APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
                }
            }
        }
        */
    }
    public void DownloadAndLoadNFT()
    {
        /*Debug.LogError("When downloading Index is here " + _indexNumber);
        Debug.LogError("NFT Object Name: " + gameObject.name);*/
        AssetCache.Instance.EnqueueOneResAndWait(_OwnedNFTDataObj.NFTsURL[_indexNumber], _OwnedNFTDataObj.NFTsURL[_indexNumber], (success) =>
        {
            if (success)
            {
                AssetCache.Instance.LoadSpriteIntoImage(OnclickNFT.image, _OwnedNFTDataObj.NFTsURL[_indexNumber], changeAspectRatio: true);
                //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.thumbnail, changeAspectRatio: true);
                isImageSuccessDownloadAndSave = true;
            }
        });
    }

    private bool IsNFTCollectionBreakingDown;

    public void CheckNftCollection()
    {
        if (_OwnedNFTDataObj.NFTlistdata.list[_indexNumber].collection.name == "XANA x BreakingDown")
        {
            Debug.Log("Current NFT Colelction is Breaking Down");
            IsNFTCollectionBreakingDown = true;
            PlayerPrefs.SetInt("IsNFTCollectionBreakingDown", 1);
        }
        else if (_OwnedNFTDataObj.NFTlistdata.list[_indexNumber].collection.name == "HIROKO KOSHINO")
        {
            IsNFTCollectionBreakingDown = false;
            PlayerPrefs.SetInt("IsNFTCollectionBreakingDown", 2);
            Debug.Log("Current NFT Colelction is HIROKO KOSHINO");
        }
    }

    public void TaskOnClick()
    {
        Debug.Log("On Nft Click debug");
        NFTDetailSlider = NftDataScript.Instance.ScrollerObj;
        scrollActivityNFTRef.NFTURL = _OwnedNFTDataObj.NFTsURL[_indexNumber];
        if (_OwnedNFTDataObj.NFTsDescriptions[_indexNumber] != null)
        {
            scrollActivityNFTRef.NFTDescriptionImage.text = _OwnedNFTDataObj.NFTsDescriptions[_indexNumber];
        }
        scrollActivityNFTRef.NFTNametext.text = _OwnedNFTDataObj.NFTsName[_indexNumber];
        scrollActivityNFTRef._NFTIndex = _indexNumber;

        //  if(CollectionAddress)
        if (APIBasepointManager.instance.IsXanaLive)
        {
            if (NftDataScript.Instance.EquipCollectionAddresses.Contains(_OwnedNFTDataObj.CollectionAddress[_indexNumber]))
            {
                //  NFTDetailSlider.GetComponent<ScrollActivityNFT>()._AttributeData = UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj._Attributes[_indexNumber];
                scrollActivityNFTRef._NFTID = _OwnedNFTDataObj.NFTlistdata.list[_indexNumber].nftId;
                if (_OwnedNFTDataObj.NFTlistdata.list[_indexNumber].nftId == PlayerPrefs.GetInt("nftID"))
                {
                    scrollActivityNFTRef.SubButtonText.text = "Unequip";
                    scrollActivityNFTRef.subButtonTextToCheck = "Unequip";
                }
                else
                {
                    scrollActivityNFTRef.SubButtonText.text = "Equip";
                    scrollActivityNFTRef.subButtonTextToCheck = "Equip";
                }
            }
            else if (NftDataScript.Instance.ComingSoonCollectionAddresses.Contains(_OwnedNFTDataObj.CollectionAddress[_indexNumber]))
            {
                scrollActivityNFTRef.SubButtonText.text = "Coming Soon";
                scrollActivityNFTRef.subButtonTextToCheck = "Coming Soon";
            }
            else
            {
                scrollActivityNFTRef.SubButtonText.text = "Unavailable ";
            }
        }
        else
        {
            if (NftDataScript.Instance.TestnetEquipCollectionAddresses.Contains(_OwnedNFTDataObj.CollectionAddress[_indexNumber]))
            {
                if (_OwnedNFTDataObj.NFTlistdata.list[_indexNumber].nftId == PlayerPrefs.GetInt("nftID"))
                {
                    scrollActivityNFTRef.SubButtonText.text = "Unequip";
                    scrollActivityNFTRef.subButtonTextToCheck = "Unequip";
                }
                else
                {
                    scrollActivityNFTRef.SubButtonText.text = "Equip";
                    scrollActivityNFTRef.subButtonTextToCheck = "Equip";
                }

                //  NFTDetailSlider.GetComponent<ScrollActivityNFT>()._AttributeData = NFTClass.attribute;
                scrollActivityNFTRef._NFTID = _OwnedNFTDataObj.NFTlistdata.list[_indexNumber].nftId;
            }
            else if (NftDataScript.Instance.TestnetComingSoonCollectionAddresses.Contains(_OwnedNFTDataObj.CollectionAddress[_indexNumber]))
            {
                scrollActivityNFTRef.SubButtonText.text = "Coming Soon";
                scrollActivityNFTRef.subButtonTextToCheck = "Coming Soon";
            }
            else
            {
                scrollActivityNFTRef.SubButtonText.text = "Unavailable ";
                scrollActivityNFTRef.subButtonTextToCheck = "Unavailable ";
            }
        }

        NFTDetailSlider.SetActive(true);
        scrollActivityNFTRef.BottomToTop();
        scrollActivityNFTRef.enabled = false;

        CheckNftCollection();
    }
}
