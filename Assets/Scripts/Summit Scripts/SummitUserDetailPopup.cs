using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummitUserDetailPopup : MonoBehaviour
{
    public GameObject userProfilePopup;

    public TMPro.TextMeshProUGUI userName;
    public TMPro.TextMeshProUGUI company;
    public TMPro.TextMeshProUGUI role;
    public TMPro.TextMeshProUGUI Interest;
    public TMPro.TextMeshProUGUI wantToConnectWith;
    public TMPro.TextMeshProUGUI freeSelfIntroduction;
    public RawImage QRImage;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void OnMouseDown()
    {
        Debug.LogError("On Mouse down ");
       // SetPopUpActive();
    }

    void GetUserInfo()
    {

    }

    void SetPopUpActive()
    {
        userProfilePopup.SetActive(true);
    }

}
