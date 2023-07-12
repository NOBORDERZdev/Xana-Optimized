using SuperStar.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NFTtypeClass : MonoBehaviour
{
    public Button OnclickNFT;
    public bool isVideoUrlFromDropbox;
    public ScrollActivityNFT scrollActivity;
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

    // Start is called before the first frame update
    void Start()
    {
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
        StartCoroutine( waitforIndexUpdation());
    }
    IEnumerator waitforIndexUpdation()
    {
        yield return new WaitForSeconds(.01f);
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
                    APIManager.Instance.ResourcesUnloadAssetFile();//UnloadUnusedAssets file call every 15 items.......
                }
            }
        }
        */
    } 
    public void DownloadAndLoadNFT()
    {
        print("When downloading Index is here " + _indexNumber); 
        AssetCache.Instance.EnqueueOneResAndWait(UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsURL[_indexNumber], UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsURL[_indexNumber], (success) =>
        {
            if (success)
            {
                AssetCache.Instance.LoadSpriteIntoImage(OnclickNFT.image, UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsURL[_indexNumber], changeAspectRatio: true);
                    //tt AssetCache.Instance.LoadTexture2DIntoRawImage(imgFeedRaw, FeedData.thumbnail, changeAspectRatio: true);
                    isImageSuccessDownloadAndSave = true;
            }
        });
    }

    public void TaskOnClick()
    {
        Debug.Log("On Nft Click debug");
        NFTDetailSlider = NftDataScript.Instance.ScrollerObj;
        NFTDetailSlider.GetComponent<ScrollActivityNFT>().NFTURL = UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsURL[_indexNumber];
        if (UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsDescriptions[_indexNumber] != null)
        {
            NFTDetailSlider.GetComponent<ScrollActivityNFT>().NFTDescriptionImage.text = UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsDescriptions[_indexNumber];
        }
        NFTDetailSlider.GetComponent<ScrollActivityNFT>().NFTNametext.text = UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsName[_indexNumber];
        NFTDetailSlider.GetComponent<ScrollActivityNFT>()._NFTIndex = _indexNumber;

        //  if(CollectionAddress)
        if (APIBaseUrlChange.instance.IsXanaLive)
        {
            if (NftDataScript.Instance.EquipCollectionAddresses.Contains(UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.CollectionAddress[_indexNumber]))
            {
              //  NFTDetailSlider.GetComponent<ScrollActivityNFT>()._AttributeData = UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj._Attributes[_indexNumber];
                NFTDetailSlider.GetComponent<ScrollActivityNFT>()._NFTID = UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list[_indexNumber].nftId;
                if (UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list[_indexNumber].nftId == PlayerPrefs.GetInt("nftID"))
                {
                    NFTDetailSlider.GetComponent<ScrollActivityNFT>().SubButtonText.text = "Unequip";
                    NFTDetailSlider.GetComponent<ScrollActivityNFT>().subButtonTextToCheck = "Unequip";
                }
                else
                {
                    NFTDetailSlider.GetComponent<ScrollActivityNFT>().SubButtonText.text = "Equip";
                    NFTDetailSlider.GetComponent<ScrollActivityNFT>().subButtonTextToCheck = "Equip";
                }
            }
            else if (NftDataScript.Instance.ComingSoonCollectionAddresses.Contains(UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.CollectionAddress[_indexNumber]))
            {
                NFTDetailSlider.GetComponent<ScrollActivityNFT>().SubButtonText.text = "Coming Soon";
                NFTDetailSlider.GetComponent<ScrollActivityNFT>().subButtonTextToCheck = "Coming Soon";
            }
            else
            {
                NFTDetailSlider.GetComponent<ScrollActivityNFT>().SubButtonText.text = "Unavailable ";
            }  
        }  
        else
        {
            if (NftDataScript.Instance.TestnetEquipCollectionAddresses.Contains(UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.CollectionAddress[_indexNumber]))
            {
                if (UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list[_indexNumber].nftId == PlayerPrefs.GetInt("nftID"))
                {
                    NFTDetailSlider.GetComponent<ScrollActivityNFT>().SubButtonText.text = "Unequip";
                    NFTDetailSlider.GetComponent<ScrollActivityNFT>().subButtonTextToCheck = "Unequip";
                }
                else
                {
                    NFTDetailSlider.GetComponent<ScrollActivityNFT>().SubButtonText.text = "Equip";
                    NFTDetailSlider.GetComponent<ScrollActivityNFT>().subButtonTextToCheck = "Equip";
                }

              //  NFTDetailSlider.GetComponent<ScrollActivityNFT>()._AttributeData = NFTClass.attribute;
                NFTDetailSlider.GetComponent<ScrollActivityNFT>()._NFTID = UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTlistdata.list[_indexNumber].nftId;
            }
            else if (NftDataScript.Instance.TestnetComingSoonCollectionAddresses.Contains(UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.CollectionAddress[_indexNumber]))
            {
                NFTDetailSlider.GetComponent<ScrollActivityNFT>().SubButtonText.text = "Coming Soon";
                NFTDetailSlider.GetComponent<ScrollActivityNFT>().subButtonTextToCheck = "Coming Soon";
            }
            else
            {
                NFTDetailSlider.GetComponent<ScrollActivityNFT>().SubButtonText.text = "Unavailable ";
                NFTDetailSlider.GetComponent<ScrollActivityNFT>().subButtonTextToCheck = "Unavailable ";
            }  
        }
  
        NFTDetailSlider.SetActive(true);
        scrollActivity = NftDataScript.Instance.ScrollerObj.GetComponent<ScrollActivityNFT>();
        scrollActivity.BottomToTop();
        scrollActivity.enabled = false;
    }
}
