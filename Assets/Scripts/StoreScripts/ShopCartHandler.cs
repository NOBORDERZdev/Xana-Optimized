using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static InventoryManager;
using UnityEditor.PackageManager;
using UnityEngine.Networking;
using System.Security.Policy;

public class ShopCartHandler : MonoBehaviour
{
    public List<ItemDetail> selectedItems;
    public GameObject cartPanel;
    public GameObject cartItemPrefab;
    public GameObject parentObj;
    public TextMeshProUGUI itemCounter; 
    public TextMeshProUGUI totalPriceTxt;
    public float totalPrice;
    public GameObject buyTxt, buyLoading;

    public GameObject _PurchaseSuccessPanel;
    public GameObject _PurchaseFailPanel;
    public GameObject _LowBalancePanel;

    float currentBalance = 1000;


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
        cartPanel.SetActive(true);
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
        var result = string.Join(",", selectedItems.ConvertAll(item => item.id).ToArray());
        result = "[" + result + "]";
       
        RequireDataForPurchasing data = new RequireDataForPurchasing
        {
            itemId = result,
            amount = totalPrice
        };

        string jsonData = JsonUtility.ToJson(data);
        string json1 = "{\"itemId\":\"[2499,2751]\",\"amount\":2.0}";

        Debug.Log("Json Data : " + jsonData);
        Debug.Log("Dummy Data : " + jsonData);
                
        using (UnityWebRequest www = UnityWebRequest.Post(APIUrl, json1))
        {
            www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log("<color=red> PurchasingError -- </color> " + www.downloadHandler.text);
            }
        }

        #region Method 2
        //UnityWebRequest request = new UnityWebRequest(APIUrl, "POST");
        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        //request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        //request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        //request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        //yield return request.SendWebRequest();
        //if (request.result == UnityWebRequest.Result.Success)
        //{
        //    Debug.Log("Data sent successfully");
        //    // Optionally handle response here
        //}
        //else
        //{
        //    Debug.LogError("Error sending data: " + request.error);
        //}
        #endregion

        #region Method 3
        //string APIUrl2 = ConstantsGod.API_BASEURL + ConstantsGod.PURCHASEWITHXENY;
        //WWWForm form = new WWWForm();
        //// Add itemId and amount to the form
        //for (int i = 0; i < selectedItems.Count; i++)
        //{
        //    form.AddField("itemId", selectedItems[i].id);
        //}
        //form.AddField("itemId", selectedItems[0].id);
        //form.AddField("amount", totalPrice.ToString());

        //// Send the request
        //using (UnityWebRequest www = UnityWebRequest.Post(APIUrl, form))
        //{
        //    www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        //    yield return www.SendWebRequest();

        //    if (www.result == UnityWebRequest.Result.Success)
        //    {
        //        Debug.Log(www.downloadHandler.text);
        //    }
        //    else
        //    {
        //        Debug.Log("<color=red> Purchasing Issue -- </color>" + www.downloadHandler.text);
        //    }
        //}
        #endregion
    }


    //IEnumerator BuyItemsCoroutine()
    //{
    //    string APIUrl = ConstantsGod.API_BASEURL + ConstantsGod.PURCHASEWITHXENY;

    //    // Create a new WWWForm
    //    WWWForm form = new WWWForm();

    //    // Add itemId and amount to the form
    //    //for (int i = 0; i < selectedItems.Count; i++)
    //    //{
    //    //    form.AddField("itemId", selectedItems[i].id);
    //    //}
    //    form.AddField("itemId", selectedItems[0].id);
    //    form.AddField("amount", totalPrice.ToString());

    //    // Send the request
    //    using (UnityWebRequest www = UnityWebRequest.Post(APIUrl, form))
    //    {
    //        www.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
    //        yield return www.SendWebRequest();

    //        if (www.result == UnityWebRequest.Result.Success)
    //        {
    //            Debug.Log(www.downloadHandler.text);
    //        }
    //        else
    //        {
    //            Debug.Log("<color=red> Purchasing Issue -- </color>" + www.downloadHandler.text);
    //        }
    //    }
    //}


}

public class RequireDataForPurchasing
{
    public string itemId;
    public float amount;

    //public RequireDataForPurchasing(string[] itemId, float amount)
    //{
    //    this.itemId = itemId;
    //    this.amount = amount;
    //}

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}