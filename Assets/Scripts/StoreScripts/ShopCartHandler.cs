using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SimpleJSON;
using UnityEngine.Networking;
using System.Text;
using System;
using DigitalRubyShared;

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

    float currentBalance = 1000;

    private void Start()
    {
        RegisterNewTouchInput();
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
           GameObject _cartObj =  Instantiate(cartItemPrefab, parentObj.transform);
            _cartObj.GetComponent<PurchaseableItemHandler>().DataSetter(selectedItems[i], this);     
        }
        cartParentObj.SetActive(true);
        UpdateTotalCount_Amount();
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

        string jsonData = JsonUtility.ToJson(data);
        Debug.Log("Selected Object Json Data : " + jsonData);

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
                    _PurchaseSuccessPanel.SetActive(true);
                    ConstantsHolder.xanaConstants.availableBalance -= totalPrice;
                    currentBalance = ConstantsHolder.xanaConstants.availableBalance;
                    selectedItems.Clear();
                    itemCounter.text = "0";
                    totalPriceTxt.text = "0";

                    InventoryManager.instance.UpdateUserXeny();
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

            _CartPanel.SetActive(false);
            buyTxt.SetActive(true);
            buyLoading.SetActive(false);
        }
    }


   public void DisablePanels(int index)
    {
        if (index == 1)
        {
            _PurchaseSuccessPanel.SetActive(false);
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


    #region Close Panel On Swipe

    private SwipeGestureRecognizer swipe1 = new SwipeGestureRecognizer();
    public SwipeGestureRecognizerDirection lastSwipeMovement;
    void RegisterNewTouchInput()
    {
        swipe1.StateUpdated += Swipe_Updated;
        swipe1.AllowSimultaneousExecution(null);
        swipe1.DirectionThreshold = 1f;
        swipe1.MinimumSpeedUnits = 1f;
        swipe1.PlatformSpecificView = cartParentObj.transform.GetChild(0).gameObject;
        swipe1.MinimumNumberOfTouchesToTrack = 1;
        swipe1.ThresholdSeconds = 1f;
        swipe1.MinimumDistanceUnits = 5f;
        swipe1.EndMode = SwipeGestureRecognizerEndMode.EndWhenTouchEnds;
        FingersScript.Instance.AddGesture(swipe1);
    }
    public void Swipe_Updated(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (swipe1.EndDirection == SwipeGestureRecognizerDirection.Down)
        {
            lastSwipeMovement = swipe1.EndDirection;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (lastSwipeMovement == SwipeGestureRecognizerDirection.Down)
            {
                if(cartParentObj.activeInHierarchy)
                    cartParentObj.SetActive(false);

                lastSwipeMovement = SwipeGestureRecognizerDirection.Any;
            }
        }

    }
    #endregion
}

[System.Serializable]
public class RequireDataForPurchasing
{
    public string[] itemId; // Updated to use an array
    public float amount;
}