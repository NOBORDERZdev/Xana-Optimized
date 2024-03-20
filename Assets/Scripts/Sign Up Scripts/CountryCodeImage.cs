using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CountryCodeImage : MonoBehaviour
{
    public Text CountryText;
    public Text CodeText;
    public int IndexID;
    private Button myBtn;

    
    void Start()
    {
        myBtn = this.GetComponent<Button>();
        myBtn.onClick.AddListener(SelectCountry);
    }

    void SelectCountry()
    {
        //UserRegisterationManager.instance.SignUpPanal.transform.GetChild(5).gameObject.SetActive(true);     
        //UserRegisterationManager.instance.SignUpPanal.transform.GetChild(7).gameObject.SetActive(true);
        //UserRegisterationManager.instance.SignUpPanal.transform.GetChild(6).transform.GetChild(4).gameObject.SetActive(true);
        CountryCodeDropDown.instance.CountrySelectedHere(IndexID);
    }   
      

}
