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
    public string subButtonTextToCheck = "Equip";
    public CanvasScaler canUi;
    public string CollectionAddressType;
    public int _NFTIndex;
    public int _NFTID;
    //Hardik Add 
    public GameObject EquipPopup;
    public GameObject SidePanel;
    public TMPro.TextMeshProUGUI EquipUIText;
    //End

    //Script Refereces 
    [Header("Scripts References")]
    public OwnedNFTContainer _OwnedNFTDataObj;

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

        if (!UserPassManager.Instance.CheckSpecificItem("EquipButton"))
        {
            //UserPassManager.Instance.PremiumUserUI.SetActive(true);

            print("Please Upgrade to Premium account");
            return;
        }
        EquipPopup.SetActive(true);
        EquipPopup.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        EquipUIText.text = "";
        EquipPopup.transform.GetChild(0).GetChild(2).GetComponent<Button>().interactable = EquipPopup.transform.GetChild(0).GetChild(3).GetComponent<Button>().interactable = false;
        if (subButtonTextToCheck == "Equip")
        {
            EquipUIText.text= "Equiping...";
            Task<bool> task = UserLoginSignupManager.instance._web3APIforWeb2.CheckSpecificNFTAndReturnAsync((_NFTID).ToString());
            bool _IsInOwnerShip = await task;
            if (!_IsInOwnerShip)
            {
                NftDataScript.Instance.NftTransferedPanel.SetActive(true);
                return;
            }
            else
            {
                print("NFT is in your OwnerShip Enjoy");
            }
            PlayerPrefs.SetInt("nftID", _NFTID);
            PlayerPrefs.SetInt("Equiped", _NFTID);
            PlayerPrefs.Save();
            //print(_NFTID);
            //print(PlayerPrefs.GetInt("Equiped"));
            //print("Equip Button Clicked");
            SubButtonText.text = "Unequip";
            SubButtonText.text = TextLocalization.GetLocaliseTextByKey("Unequip");
            subButtonTextToCheck = "Unequip";
            ConstantsHolder.xanaConstants.isNFTEquiped = true;
            SaveAttributesInFile();
            BoxerNFTEventManager.OnNFTequip?.Invoke(true);
            SidePanel.SetActive(false);
            EquipPopup.SetActive(true);
            EquipPopup.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            EquipUIText.text = " Equip Successfully !";
            EquipUIText.text = TextLocalization.GetLocaliseTextByKey("Equip Successfully");
            EquipPopup.transform.GetChild(0).GetChild(2).GetComponent<Button>().interactable = EquipPopup.transform.GetChild(0).GetChild(3).GetComponent<Button>().interactable = true;
        }
        else if (subButtonTextToCheck == "Unequip")
        {
            EquipUIText.text = "Unequiping...";
            SubButtonText.text = "Equip";
            SubButtonText.text = TextLocalization.GetLocaliseTextByKey("Equip");
            subButtonTextToCheck = "Equip";
            PlayerPrefs.DeleteKey("Equiped");
            PlayerPrefs.DeleteKey("nftID");
            ConstantsHolder.xanaConstants.isNFTEquiped = false;
            BoxerNFTEventManager.OnNFTUnequip?.Invoke();
            SwitchToShoesHirokoKoshino.Instance.DisableAllLighting();
            SidePanel.SetActive(false);
            EquipPopup.SetActive(true);
            EquipPopup.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            EquipUIText.text = "Unequipped Successfully.";
            EquipUIText.text = TextLocalization.GetLocaliseTextByKey("Unequipped Successfully.");
            EquipPopup.transform.GetChild(0).GetChild(2).GetComponent<Button>().interactable = EquipPopup.transform.GetChild(0).GetChild(3).GetComponent<Button>().interactable = true;
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
        // GameManager.Instance.UiManager.ShowFooter(true);
    }
    public void WaitForOpenWorldPage()
    {
        Debug.Log("WaitForOpenWorldPage");
        ScrollController.transform.parent.GetComponent<ScrollActivityNFT>().enabled = true;
    }

    void SaveAttributesInFile()
    {
        Debug.LogError("SaveAttributesInFile: " + _NFTIndex);
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

        nftAttributes.id = _OwnedNFTDataObj._Attributes[_NFTIndex].id.ToString();
        nftAttributes.Gloves = _OwnedNFTDataObj._Attributes[_NFTIndex].Gloves;
        nftAttributes.Glasses = _OwnedNFTDataObj._Attributes[_NFTIndex].glasses;
        nftAttributes.Full_Costumes = _OwnedNFTDataObj._Attributes[_NFTIndex].Full_Costumes;
        nftAttributes.Chains = _OwnedNFTDataObj._Attributes[_NFTIndex].Chains;
        nftAttributes.Hairs = _OwnedNFTDataObj._Attributes[_NFTIndex].hairs;
        nftAttributes.Face_Tattoo = _OwnedNFTDataObj._Attributes[_NFTIndex].Face_Tattoo;
        nftAttributes.Forehead_Tattoo = _OwnedNFTDataObj._Attributes[_NFTIndex].Forehead_Tattoo;
        nftAttributes.Chest_Tattoo = _OwnedNFTDataObj._Attributes[_NFTIndex].Chest_Tattoo;
        nftAttributes.Arm_Tattoo = _OwnedNFTDataObj._Attributes[_NFTIndex].Arm_Tattoo;
        nftAttributes.Legs_Tattoo = _OwnedNFTDataObj._Attributes[_NFTIndex].legs_Tattoo;
        nftAttributes.Shoes = _OwnedNFTDataObj._Attributes[_NFTIndex].Shoes;
        nftAttributes.Mustache = _OwnedNFTDataObj._Attributes[_NFTIndex].Mustache;
        nftAttributes.Pants = _OwnedNFTDataObj._Attributes[_NFTIndex].Pants;
        nftAttributes.Eyebrows = _OwnedNFTDataObj._Attributes[_NFTIndex].Eyebrows;
        nftAttributes.Lips = _OwnedNFTDataObj._Attributes[_NFTIndex].Lips;
        nftAttributes.Heads = _OwnedNFTDataObj._Attributes[_NFTIndex].head;
        nftAttributes.Eye_Shapes = _OwnedNFTDataObj._Attributes[_NFTIndex].Eye_Shapes;
        nftAttributes.Skin = _OwnedNFTDataObj._Attributes[_NFTIndex].Skin;
        nftAttributes.Eye_Lense = _OwnedNFTDataObj._Attributes[_NFTIndex].Eye_lense;
        nftAttributes.Eyelid = _OwnedNFTDataObj._Attributes[_NFTIndex].Eyelid;
        nftAttributes.profile = _OwnedNFTDataObj._Attributes[_NFTIndex].profile;
        nftAttributes.speed = _OwnedNFTDataObj._Attributes[_NFTIndex].speed;
        nftAttributes.stamina = _OwnedNFTDataObj._Attributes[_NFTIndex].stamina;
        nftAttributes.punch = _OwnedNFTDataObj._Attributes[_NFTIndex].punch;
        nftAttributes.kick = _OwnedNFTDataObj._Attributes[_NFTIndex].kick;
        nftAttributes.defence = _OwnedNFTDataObj._Attributes[_NFTIndex].defence;
        nftAttributes.special_move = _OwnedNFTDataObj._Attributes[_NFTIndex].special_move;
        string attributesJson = JsonUtility.ToJson(nftAttributes);
        File.WriteAllText(Application.persistentDataPath + ConstantsHolder.xanaConstants.NFTBoxerJson, attributesJson);
    }
}