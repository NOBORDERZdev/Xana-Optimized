using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchaseableItemHandler : MonoBehaviour
{
    public Image iconImage;
    public Image selectImageIcon;
    public Sprite selected, unselected;
    public TextMeshProUGUI itemPriceTxt;

    ItemDetail _MyData;
    public ShopCartHandler _ShopCartHandler;
    public PurchaseableItemHandler(ItemDetail myData)
    {
        Init(myData);
    }
    

    void Init(ItemDetail initData)
    {
        _MyData = initData;
        itemPriceTxt.text = _MyData.price.ToString();
        selectImageIcon.sprite = selected;
        DownloadIcon(_MyData.iconLink); 
    }
    void DownloadIcon(string iconLink)
    {

    }

    public void ItemSelectUnselect()
    {
        selectImageIcon.sprite = selected;
        _ShopCartHandler.selectedItems.Remove(_MyData);
        _ShopCartHandler.UpdateTotalCount_Amount();
    }


}
