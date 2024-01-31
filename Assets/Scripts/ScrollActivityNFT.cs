using DG.Tweening;
using SuperStar.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class ScrollActivityNFT : MonoBehaviour
{
    [Header("For World Icons Scroll")]
    public ScrollRect ScrollController;
    public GameObject btnback;
    public float normalized;
    private int lastindex = 1;
    public string NFTURL;
     public Image NFTImage;
    public TMP_Text NFTDescriptionImage;  
    public Text NFTNametext;
    public Text SubButtonText;
    [HideInInspector]
    public string subButtonTextToCheck= "Equip";
    public CanvasScaler canUi;
    public string CollectionAddressType;    
  //  public UserNFTlistClass.Attribute _AttributeData;
    // public UserNFTlistClass.Attribute _AttributeData;
    public int _NFTIndex;
    public int _NFTID;
    //Hardik Add 
    public GameObject EquipPopup;
    public GameObject SidePanel;
    public TMPro.TextMeshProUGUI EquipUIText;
    public OwnedNFTContainer onwNFTContainer;
    //End
    private void Awake()
    {
        ScrollController.verticalNormalizedPosition = 3.5f;
    }

    //Worked by Abdullah & Riken
    private void OnDisable()
    {  
       // canUi.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        ScrollController.verticalNormalizedPosition = 3.5f;
        ScrollController.movementType = ScrollRect.MovementType.Elastic;
        lastindex = 1;
    }
    private void OnEnable()
    {
      // canUi.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;  
         AssetCache.Instance.EnqueueOneResAndWait(NFTURL, NFTURL, (success) =>
        {
            if (success)
            {
                AssetCache.Instance.LoadSpriteIntoImage(NFTImage, NFTURL, changeAspectRatio: true);
                // CheckAndSetResolutionOfImage(imgFeed.sprite);
                //  isImageSuccessDownloadAndSave = true;
            }    
            else
            {
               Debug.Log("Download Failed");
            }
        });  
        ScrollController.movementType = ScrollRect.MovementType.Elastic;
        lastindex = 1;

        //playBtn.onClick.RemoveAllListeners();
        //playBtn.onClick.AddListener(PlayBtnClicked);
    }
  
    public async void EquipBtnClicked()
    {

        if (!PremiumUsersDetails.Instance.CheckSpecificItem("EquipButton"))
        {
            //PremiumUsersDetails.Instance.PremiumUserUI.SetActive(true);

            print("Please Upgrade to Premium account");
            return;
        }
         print("NFT btn clicked here");  
         if(subButtonTextToCheck == "Equip")
        {
             Task<bool> task = UserRegisterationManager.instance._web3APIforWeb2.CheckSpecificNFTAndReturnAsync((_NFTID).ToString());
            bool _IsInOwnerShip = await task;
              print("_IsInOwnerShip :: " + _IsInOwnerShip);
            if (!_IsInOwnerShip)
            {
                print("Show UI NFT not available");
                NftDataScript.Instance.NftTransferedPanel.SetActive(true);
                return;
            }
            else
            {
                print("NFT is in your OwnerShip Enjoy");
            }
             print("_NFTID :: " + _NFTID.ToString());  
             PlayerPrefs.SetInt("nftID", _NFTID);
            PlayerPrefs.SetInt("Equiped", _NFTID);  
            PlayerPrefs.Save();
            //print(_NFTID);
            //print(PlayerPrefs.GetInt("Equiped"));
            //print("Equip Button Clicked");
            print("Attributes Data Showing Here");
            SubButtonText.text = "Unequip";
            SubButtonText.text = TextLocalization.GetLocaliseTextByKey("Unequip");
            subButtonTextToCheck = "Unequip";
            XanaConstants.xanaConstants.isNFTEquiped = true;
            SaveAttributesInFile();
            BoxerNFTEventManager.OnNFTequip?.Invoke(true);
            SidePanel.SetActive(false);
            EquipPopup.SetActive(true);
            EquipUIText.text = " Equip Successfully !";
            EquipUIText.text = TextLocalization.GetLocaliseTextByKey("Equip Successfully");
        }
        else if (subButtonTextToCheck == "Unequip")
        {
 
            SubButtonText.text = "Equip";
            SubButtonText.text = TextLocalization.GetLocaliseTextByKey("Equip");
            subButtonTextToCheck = "Equip";  
            PlayerPrefs.DeleteKey("Equiped");
            PlayerPrefs.DeleteKey("nftID");
            XanaConstants.xanaConstants.isNFTEquiped = false;
            BoxerNFTEventManager.OnNFTUnequip?.Invoke();
            SwitchToShoesHirokoKoshinoNFT.Instance.DisableAllLighting();
            SidePanel.SetActive(false);
            EquipPopup.SetActive(true);
            EquipUIText.text = "Unequipped Successfully.";
            EquipUIText.text = TextLocalization.GetLocaliseTextByKey("Unequipped Successfully.");
        }
        else  
        {
            print("Not Available");
        }
    }
 

    private void Update()
    {
        if (ScrollController.verticalNormalizedPosition > 1f)
        {
            ScrollController.movementType = ScrollRect.MovementType.Unrestricted;

            if (Input.touchCount > 0)
            {

            }
            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended))
            {
                ScrollController.movementType = ScrollRect.MovementType.Unrestricted;
                StartCoroutine(ExampleCoroutine());
                lastindex = 2;
            }
        }
        else if (ScrollController.verticalNormalizedPosition < 1f)
        {
            ScrollController.movementType = ScrollRect.MovementType.Elastic;
            if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)) && lastindex == 0 && ScrollController.verticalNormalizedPosition < 0.99f && ScrollController.verticalNormalizedPosition > 0)
            {
                DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 1, 0.1f).SetEase(Ease.Linear);
                lastindex = 1;
            }
            else if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)) && lastindex == 1 && ScrollController.verticalNormalizedPosition < 0.99f && ScrollController.verticalNormalizedPosition > 0)
            {
                DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 0, 0.1f).SetEase(Ease.Linear);
                lastindex = 0;
            }
        }
    }
    public void Closer()
    {
        if (ScrollController.verticalNormalizedPosition < 0.001f)
        {
            btnback.SetActive(true);
        }
        else
        {
            btnback.SetActive(false);
        }
        normalized = ScrollController.verticalNormalizedPosition;
    }

    Coroutine IEBottomToTopCoroutine;
    public void BottomToTop()
    {
        if (IEBottomToTopCoroutine == null)
        {
            IEBottomToTopCoroutine = StartCoroutine(IEBottomToTop());
        }
    }
    public IEnumerator IEBottomToTop()
    {
        ScrollController.verticalNormalizedPosition = 3.5f;
        yield return new WaitForSeconds(0.2f);
        DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 1, 0.2f).SetEase(Ease.Linear).OnComplete(WaitForOpenWorldPage);
        IEBottomToTopCoroutine = null;
    }
    IEnumerator ExampleCoroutine()
    {
        DOTween.To(() => ScrollController.verticalNormalizedPosition, x => ScrollController.verticalNormalizedPosition = x, 3.5f, 0.2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.2f);
        this.gameObject.SetActive(false);
       // UIManager.Instance.ShowFooter(true);
    }
    public void WaitForOpenWorldPage()
    {
        Debug.Log("WaitForOpenWorldPage");
        ScrollController.transform.parent.GetComponent<ScrollActivityNFT>().enabled = true;
    }

   void SaveAttributesInFile()
    {
        BoxerNFTDataClass nftAttributes = new BoxerNFTDataClass();
        nftAttributes.isNFTAquiped = true;
        /*
         nftAttributes.id = _AttributeData.id.ToString();
         nftAttributes.Gloves = _AttributeData.Gloves;
         nftAttributes.Glasses = _AttributeData.glasses;
         nftAttributes.Full_Costumes = _AttributeData.Full_Costumes;
         nftAttributes.Chains = _AttributeData.Chains;
         nftAttributes.Hairs = _AttributeData.hairs;
         nftAttributes.Face_Tattoo = _AttributeData.Face_Tattoo;
         nftAttributes.Forehead_Tattoo = _AttributeData.Forehead_Tattoo;
         nftAttributes.Chest_Tattoo = _AttributeData.Chest_Tattoo;
         nftAttributes.Arm_Tattoo = _AttributeData.Arm_Tattoo;
         nftAttributes.Legs_Tattoo =_AttributeData.legs_Tattoo;
         nftAttributes.Shoes = _AttributeData.Shoes;
         nftAttributes.Mustache = _AttributeData.Mustache;
         nftAttributes.Pants = _AttributeData.Pants;
         nftAttributes.Eyebrows = _AttributeData.Eyebrows;
         nftAttributes.Lips = _AttributeData.Lips;
         nftAttributes.Heads = _AttributeData.head;
         nftAttributes.Eye_Shapes = _AttributeData.Eye_Shapes;
         nftAttributes.Skin = _AttributeData.Skin;
         nftAttributes.Eye_Lense = _AttributeData.Eye_lense;
         nftAttributes.Eyelid = _AttributeData.Eyelid;
         */
        //UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj.NFTsURL[_indexNumber]

        // nftAttributes.id = UserRegisterationManager.instance._web3APIforWeb2._OwnedNFTDataObj._Attributes[_NFTIndex].id.ToString();
         nftAttributes.id = onwNFTContainer.getDetails.list[_NFTIndex].attribute.id.ToString();



         nftAttributes.Gloves = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Gloves;
        nftAttributes.Glasses = onwNFTContainer.getDetails.list[_NFTIndex].attribute.glasses;
        nftAttributes.Full_Costumes = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Full_Costumes;
        nftAttributes.Chains = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Chains;
        nftAttributes.Hairs = onwNFTContainer.getDetails.list[_NFTIndex].attribute.hairs;
        nftAttributes.Face_Tattoo = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Face_Tattoo;
        nftAttributes.Forehead_Tattoo = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Forehead_Tattoo;
        nftAttributes.Chest_Tattoo = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Chest_Tattoo;
        nftAttributes.Arm_Tattoo = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Arm_Tattoo;
        nftAttributes.Legs_Tattoo = onwNFTContainer.getDetails.list[_NFTIndex].attribute.legs_Tattoo;
        nftAttributes.Shoes = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Shoes;
        nftAttributes.Mustache = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Mustache;
        nftAttributes.Pants = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Pants;
        nftAttributes.Eyebrows = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Eyebrows;
        nftAttributes.Lips = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Lips;
        nftAttributes.Heads = onwNFTContainer.getDetails.list[_NFTIndex].attribute.head;
        nftAttributes.Eye_Shapes = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Eye_Shapes;
        nftAttributes.Skin = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Skin;
        nftAttributes.Eye_Lense = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Eye_lense;
        nftAttributes.Eyelid = onwNFTContainer.getDetails.list[_NFTIndex].attribute.Eyelid;
        nftAttributes.profile = onwNFTContainer.getDetails.list[_NFTIndex].attribute.profile;
        nftAttributes.speed = onwNFTContainer.getDetails.list[_NFTIndex].attribute.speed;
        nftAttributes.stamina = onwNFTContainer.getDetails.list[_NFTIndex].attribute.stamina;
        nftAttributes.punch = onwNFTContainer.getDetails.list[_NFTIndex].attribute.punch;
        nftAttributes.kick = onwNFTContainer.getDetails.list[_NFTIndex].attribute.kick;
        nftAttributes.defence = onwNFTContainer.getDetails.list[_NFTIndex].attribute.defence;
        nftAttributes.special_move = onwNFTContainer.getDetails.list[_NFTIndex].attribute.special_move;

        ConnectionManagemenet.punch = nftAttributes.punch;
        ConnectionManagemenet.kick = nftAttributes.kick;
        ConnectionManagemenet.special_move = nftAttributes.special_move;

        string attributesJson = JsonUtility.ToJson(nftAttributes);      
        File.WriteAllText(Application.persistentDataPath + XanaConstants.xanaConstants.NFTBoxerJson, attributesJson);


    }
}