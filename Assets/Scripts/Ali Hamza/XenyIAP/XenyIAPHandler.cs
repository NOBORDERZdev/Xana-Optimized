using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;
using System;

[Serializable]
public class ConsumableItem
{
    public string name;
    public string id;
    public string desc;
    public float price;
}
[Serializable]
public class SubscriptionItem
{
    public string name;
    public string id;
    public string desc;
    public float price;
    public int timeDuration; // in days
}

public class XenyIAPHandler : MonoBehaviour, IStoreListener
{
    public Button allButton;
    public Button subscriptionButton;
    public Button xenyButton;

    public GameObject[] SubscriptionButtons;
    public GameObject[] XenyButtons;

    public Color normalButtonColor;
    public Color selectedButtonColor;


    public TextMeshProUGUI xenyBalanceText;
    public int xenyBalance = 0;


    public ConsumableItem[] consumableItems;
    public SubscriptionItem subscriptionItem;

    public Data data;
    public Payload payload;
    public PayloadData payloadData;

    public TextMeshProUGUI purchaseTokenText;

    IStoreController storeController;

    private void Start()
    {
        allButton.onClick.AddListener(OnAllButtonClicked);
        subscriptionButton.onClick.AddListener(OnSubscriptionButtonClicked);
        xenyButton.onClick.AddListener(OnXenyButtonClicked);
        OnAllButtonClicked();

        xenyBalance = PlayerPrefs.GetInt("XenyBalance", 0);
        xenyBalanceText.text = xenyBalance.ToString();


        SetupBuilder();
    }

    void SetupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (ConsumableItem item in consumableItems)
        {
            builder.AddProduct(item.id, ProductType.Consumable);
        }
        builder.AddProduct(subscriptionItem.id, ProductType.Subscription);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        print("Initialized");
        storeController=controller;


        CheckSubscription(subscriptionItem.id);
    }

    #region XenyIAP

    public void BuyXenyCoins(int amount)
    {
        string productID = "";
        //AddXenyCoins(amount);
        switch (amount)
        {
            case 100:
                productID = consumableItems[0].id;
                break;
            case 500:
                productID = consumableItems[1].id;
                break;
            case 1050:
                productID = consumableItems[2].id;
                break;
            case 2200:
                productID = consumableItems[3].id;
                break;
            case 6000:
                productID = consumableItems[4].id;
                break;
        }
        storeController.InitiatePurchase(productID);
    }
    public void BuySubscription()
    {
        //AddXenyCoins(amount);
        storeController.InitiatePurchase(subscriptionItem.id);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        //Retrieve the purchased product
        var product = purchaseEvent.purchasedProduct;

        print("Purchase Complete: " + product.definition.id);

        string receipt = product.receipt;
        data = JsonUtility.FromJson<Data>(receipt);
        payload = JsonUtility.FromJson<Payload>(data.Payload);
        payloadData =JsonUtility.FromJson<PayloadData>(payload.json);
        int quantity = payloadData.quantity;
        purchaseTokenText.text = "purchase Token: "+payloadData.purchaseToken;
        Debug.Log("Purchase Token: " + payloadData.purchaseToken);
        Debug.LogError("Purchase Token: " + payloadData.purchaseToken);

        if(product.definition.id == consumableItems[0].id)
            AddXenyCoins(100,quantity);
        else if (product.definition.id == consumableItems[1].id)
            AddXenyCoins(500, quantity);
        else if (product.definition.id == consumableItems[2].id)
            AddXenyCoins(1050, quantity);
        else if (product.definition.id == consumableItems[3].id)
            AddXenyCoins(2200, quantity);
        else if (product.definition.id == consumableItems[4].id)
            AddXenyCoins(6000, quantity);
        //else if (product.definition.id == subscriptionItem.id)
        //    AddXenyCoins(300);

        return PurchaseProcessingResult.Complete;
    }

    private void AddXenyCoins(int amount, int quantity=1)
    {
        xenyBalance += (amount * quantity);
        xenyBalanceText.text = xenyBalance.ToString();
        PlayerPrefs.SetInt("XenyBalance", xenyBalance);
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        print("Initialization Failed"+ error);
    }

    

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        print("Purchase Failed: " + product.definition.id + " Reason: " + failureReason);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        print("Initialization Failed: " + error + " Message: " + message);
    }

    void CheckSubscription(string id)
    {
        var subProduct = storeController.products.WithID(id);
        if (subProduct != null)
        {
            try
            {
                if(subProduct.hasReceipt)
                {
                    var subManager = new SubscriptionManager(subProduct, null);
                    var info = subManager.getSubscriptionInfo();
                    print("Subscription Info: " + info.getPurchaseDate() + " " + info.getExpireDate());

                    if(info.isSubscribed()==Result.True)
                    {
                        print("We are subscribed");
                    }
                    else
                    {
                        print("Un Subscribed");
                    }
                }
                else
                {
                    print("No Receipt Found");
                }
            }
            catch (Exception e)
            {
                print("It only work for Google store, app store, amazon store, you are using fake store");
            }
        }

    }
    

    #endregion
    public void OnAllButtonClicked()
    {
        SetActiveStates(true, true);
        UpdateButtonColors(allButton);
    }

    public void OnSubscriptionButtonClicked()
    {
        SetActiveStates(true, false);
        UpdateButtonColors(subscriptionButton);
    }

    public void OnXenyButtonClicked()
    {
        SetActiveStates(false, true);
        UpdateButtonColors(xenyButton);
    }

    private void SetActiveStates(bool subscriptionActive, bool xenyActive)
    {
        foreach (GameObject button in SubscriptionButtons)
        {
            button.SetActive(subscriptionActive);
        }
        foreach (GameObject button in XenyButtons)
        {
            button.SetActive(xenyActive);
        }
    }
    private void UpdateButtonColors(Button selectedButton)
    {
        Button[] buttons = { allButton, subscriptionButton, xenyButton };

        foreach (Button button in buttons)
        {
            bool isSelected = button == selectedButton;
            button.GetComponent<Image>().color = isSelected ? selectedButtonColor : normalButtonColor;
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = isSelected ? Color.white : Color.black;
        }
    }

    public void OnBackButtonClicked()
    {
        gameObject.SetActive(false);
    }

    [Serializable]
    public class PayloadData
    {
        public string orderId;
        public string packageName;
        public string productId;
        public long purchaseTime;
        public int purchaseState;
        public string purchaseToken;
        public int quantity;
        public bool acknowledged;
    }
    [Serializable]
    public class Payload
    {
        public string json;
        public string signature;
        public List<SkuDetails> skuDetails;
        public PayloadData payloadData;
    }
    [Serializable]
    public class SkuDetails
    {
        public string productId;
        public string type;
        public string title;
        public string name;
        public string iconUrl;
        public string description;
        public string price;
        public long price_amount_micros;
        public string price_currency_code;
        public string skuDetailsToken;
    }

    [Serializable]
    public class Data
    {
        public string Payload;
        public string Store;
        public string TransactionID;
    }

}
