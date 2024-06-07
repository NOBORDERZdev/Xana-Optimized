using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

        currentBalance = 1;
        cartPanel.SetActive(true);


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
        yield return new WaitForSeconds(0);
        
    }
}
