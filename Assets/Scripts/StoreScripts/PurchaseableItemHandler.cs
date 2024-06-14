using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEditor;

public class PurchaseableItemHandler : MonoBehaviour
{
    public Image iconImage;
    public Image selectImageIcon;
    public GameObject loadingSpriteImage;
    public Sprite selected, unselected;
    public TextMeshProUGUI itemPriceTxt;

    ItemDetail _MyData;
    public ShopCartHandler _ShopCartHandler;

    List<ItemDetail> _selectedItemInCart;


    void Start()
    {
        _selectedItemInCart = _ShopCartHandler.selectedItems;
        StartCoroutine(AddIconSprite(_MyData.iconLink));
    }
    IEnumerator AddIconSprite(string iconLink)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(iconLink))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
                yield break;
            }

            Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
            if (iconImage && tex != null)
            {
                iconImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                iconImage.gameObject.SetActive(true);
            }

            loadingSpriteImage.SetActive(false);
        }
    }


    public void DataSetter(ItemDetail myData,ShopCartHandler _shopInstance)
    {
        _ShopCartHandler = _shopInstance;
        Init(myData);
    }
    void Init(ItemDetail initData)
    {
        _MyData = initData;
        itemPriceTxt.text = _MyData.price.ToString();
        selectImageIcon.sprite = selected;
        loadingSpriteImage.SetActive(true);
    }
    public void ItemSelectUnselect()
    {
        if (selectImageIcon.sprite == selected)
        {
            selectImageIcon.sprite = unselected;
            _selectedItemInCart.Remove(_MyData);
        }
        else
        {
            selectImageIcon.sprite = selected;
            _selectedItemInCart.Add(_MyData);
        }
       
        _ShopCartHandler.UpdateTotalCount_Amount();
    }
}
