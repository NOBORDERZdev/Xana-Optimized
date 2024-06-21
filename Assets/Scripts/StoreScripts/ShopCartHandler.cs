using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SimpleJSON;
using UnityEngine.Networking;
using System.Text;
using System;
using DigitalRubyShared;
using DG.Tweening;
using System.Linq;

public class ShopCartHandler : MonoBehaviour
{
    public List<ItemDetail> selectedItems;
    public TextMeshProUGUI itemCounter;
    public TextMeshProUGUI totalPriceTxt;
    public float totalPrice;
    public GameObject buyTxt, buyLoading;
    public GameObject cartParentObj; // MainCartPanel
    public GameObject parentObj; // This is the parent object of the cart items
    public GameObject cartItemPrefab;
    public GameObject _CartPanel;
    public GameObject _PurchaseSuccessPanel;
    public GameObject _PurchaseFailPanel;
    public GameObject _LowBalancePanel;
    public Transform _ScallingObj;


    float currentBalance = 0;
    public List<ClothingParts> _oldItemReferences;
    AvatarController ac;

    private void Start()
    {
        ac = GameManager.Instance.mainCharacter.GetComponent<AvatarController>();
    }
    public void UpdateItemReferences()
    {
        _oldItemReferences[0].dressName = ac.wornPant.name;
        _oldItemReferences[1].dressName = ac.wornShirt.name;
        _oldItemReferences[2].dressName = ac.wornHair.name;
        _oldItemReferences[3].dressName = ac.wornShoes.name;
    }


    public void EnableCartPanel()
    {
        if (selectedItems.Count == 0)
        {
            Debug.Log("No items selected");
            return;
        }

        currentBalance = ConstantsHolder.xanaConstants.availableBalance;
        SetCartItems();
    }
    void SetCartItems()
    {
        for (int i = 0; i < selectedItems.Count; i++)
        {
            GameObject _cartObj = Instantiate(cartItemPrefab, parentObj.transform);
            _cartObj.GetComponent<PurchaseableItemHandler>().DataSetter(selectedItems[i], this);
        }
        cartParentObj.SetActive(true);
        _ScallingObj.DOScaleY(1, 0.2f).SetEase(Ease.Linear);
        UpdateTotalCount_Amount();
    }
    public void CloseCartPanel()
    {
        _ScallingObj.DOScaleY(0, 0.2f).SetEase(Ease.Linear).OnComplete(delegate
        {
            cartParentObj.SetActive(false);
            foreach (Transform child in parentObj.transform)
            {
                child.gameObject.SetActive(false);
            }
        });
    }

    public void UpdateTotalCount_Amount()
    {
        totalPrice = 0;
        foreach (ItemDetail item in selectedItems)
        {
            totalPrice += float.Parse(item.price);
        }
        totalPriceTxt.text = totalPrice.ToString();
        itemCounter.text = selectedItems.Count.ToString();
    }
    public void BuyItems()
    {
        if (selectedItems.Count == 0)
        {
            Debug.Log("No items selected");
            return;
        }

        if (totalPrice > currentBalance)
        {
            _LowBalancePanel.SetActive(true);
            return;
        }

        buyTxt.SetActive(false);
        buyLoading.SetActive(true);
        StartCoroutine(BuyItemsCoroutine());
    }
    IEnumerator BuyItemsCoroutine()
    {
        string APIUrl = ConstantsGod.API_BASEURL + ConstantsGod.PURCHASEWITHXENY;

        // Preparing the itemId array correctly for JSON
        var itemIds = selectedItems.ConvertAll(item => item.id).ToArray();
        RequireDataForPurchasing data = new RequireDataForPurchasing
        {
            itemId = itemIds,
            amount = totalPrice
        };

        // Manually constructing the itemId array as a JSON string
        string itemIdJsonArray = "[\"" + string.Join("\", \"", data.itemId) + "\"]";

        // Escaping double quotes for JSON string
        string escapedItemIdJsonArray = itemIdJsonArray.Replace("\"", "\\\"");

        // Constructing the final JSON string
        string jsonData = $"{{\"itemId\": \"{escapedItemIdJsonArray}\",\r\n    \"amount\": {data.amount}}}";

        using (UnityWebRequest request = new UnityWebRequest(APIUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("<color=red> PurchasingYes -- </color> " + request.downloadHandler.text);
                string json = request.downloadHandler.text;
                var _ParsingJson = JSON.Parse(json);
                bool _Success = _ParsingJson["success"].AsBool;
                if (_Success)
                {
                    ConstantsHolder.xanaConstants.isStoreItemPurchasedSuccessfully = true;
                    _PurchaseSuccessPanel.SetActive(true);
                    ConstantsHolder.xanaConstants.availableBalance -= totalPrice;
                    currentBalance = ConstantsHolder.xanaConstants.availableBalance;
                    selectedItems.Clear();
                    itemCounter.text = "0";
                    totalPriceTxt.text = "0";

                    InventoryManager.instance.UpdateUserXeny();
                    //InventoryManager.instance.SelectPanel(0);
                }
                else
                {
                    Debug.Log("<color=red> PurchasingError -- </color> " + request.downloadHandler.text);
                    _PurchaseFailPanel.SetActive(true);
                }
            }
            else
            {
                Debug.Log("<color=red> PurchasingError -- </color> " + request.error);
                _PurchaseFailPanel.SetActive(true);
            }

            //cartParentObj.SetActive(false);
            //_CartPanel.SetActive(false);
            buyTxt.SetActive(true);
            buyLoading.SetActive(false);
        }
    }

    public void DisablePanels(int index)
    {
        if (index == 1)
        {
            _PurchaseSuccessPanel.SetActive(false);
            selectedItems.Clear();
            CloseCartPanel();
            InventoryManager.instance.SelectPanel(0);
        }
        else if (index == 2)
        {
            _PurchaseFailPanel.SetActive(false);
        }
        else if (index == 3)
        {
            _LowBalancePanel.SetActive(false);
        }
    }
    public void RemoveTryonCloth()
    {
        // Refactored to reduce repetitive code and improve readability
        var itemsToCheck = new[] {ac.wornPant, ac.wornShirt, ac.wornHair, ac.wornShoes };
        for (int i = 0; i < itemsToCheck.Length; i++)
        {
            var item = itemsToCheck[i];
            if (item != null && IsDressChanged(item.name))
            {
                StartCoroutine(AddressableDownloader.Instance.DownloadAddressableObj(0, _oldItemReferences[i].dressName, _oldItemReferences[i].itemtype, SaveCharacterProperties.instance.SaveItemList.gender, ac, Color.clear));
            }
        }
    }

    bool IsDressChanged(string dressName)
    {
        // Utilizing LINQ to simplify the check
        return !_oldItemReferences.Any(item => string.Equals(item.dressName, dressName));
    }


    //#region Close Panel On Swipe

    //private SwipeGestureRecognizer swipe1 = new SwipeGestureRecognizer();
    //public SwipeGestureRecognizerDirection lastSwipeMovement;
    //void RegisterNewTouchInput()
    //{
    //    swipe1.StateUpdated += Swipe_Updated;
    //    swipe1.AllowSimultaneousExecution(null);
    //    swipe1.DirectionThreshold = 1f;
    //    swipe1.MinimumSpeedUnits = 1f;
    //    swipe1.PlatformSpecificView = cartParentObj.transform.GetChild(0).gameObject;
    //    swipe1.MinimumNumberOfTouchesToTrack = 1;
    //    swipe1.ThresholdSeconds = 1f;
    //    swipe1.MinimumDistanceUnits = 5f;
    //    swipe1.EndMode = SwipeGestureRecognizerEndMode.EndWhenTouchEnds;
    //    FingersScript.Instance.AddGesture(swipe1);
    //}
    //public void Swipe_Updated(DigitalRubyShared.GestureRecognizer gesture)
    //{
    //    if (swipe1.EndDirection == SwipeGestureRecognizerDirection.Down)
    //    {
    //        lastSwipeMovement = swipe1.EndDirection;
    //    }

    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        if (lastSwipeMovement == SwipeGestureRecognizerDirection.Down)
    //        {
    //            if(cartParentObj.activeInHierarchy)
    //                cartParentObj.SetActive(false);

    //            lastSwipeMovement = SwipeGestureRecognizerDirection.Any;
    //        }
    //    }

    //}
    //#endregion
}

[System.Serializable]
public class RequireDataForPurchasing
{
    public string[] itemId; // Updated to use an array
    public float amount;
}

[System.Serializable]
public class ClothingParts
{
    public string itemtype;
    public string dressName;
}