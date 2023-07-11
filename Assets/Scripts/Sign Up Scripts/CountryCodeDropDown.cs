using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using UnityEngine.Networking;
using System;
using System.Text;
using Newtonsoft.Json;
using System.IO;

 
[System.Serializable]
public class GameInfoList
{
    public GameInfo gameInfo;
    public string[] countries;
    public List<GameInfo> list = new List<GameInfo>();
}
[System.Serializable]
public class GameInfo
{
    public string code;
    public string name;
}

public class CountryCodeDropDown : MonoBehaviour
{
    public static CountryCodeDropDown instance;
    public string myjson;
    public Dropdown dropDownCountryCode;
    private List<string> DropDownList = new List<string>();
    public GameObject CountryCodePanal;
    public Text CodeText;
 
  //  public MobileInputField NumberField;     
     //  public GameObject CountryCodeObj;
     //public GameObject CountryCodePanel;
     //GameInfoList gameInfoList;
    [System.Serializable]
    public class GameInfoList
    {
        public List<GameInfo> countries;
    }
    GameInfoList Items;
    private void Awake()
    {
        instance = this;
      //  print(myjson);
      //  dropDownCountryCode.options.Clear();
      //  DropDownList.Clear();
        Items = JsonUtility.FromJson<GameInfoList>(myjson);
      //  print(Items.countries.Count);   
       // for (int i = 0; i < Items.countries.Count; i++)
      //  {
            //GameObject L_CountryObj = Instantiate(CountryCodeObj, CountryCodePanel.transform);
            //L_CountryObj.GetComponent<CountryCodeImage>().CountryText.text = Items.countries[i].name;
            //L_CountryObj.GetComponent<CountryCodeImage>().CodeText.text = Items.countries[i].code;
            //L_CountryObj.GetComponent<CountryCodeImage>().IndexID = i;
          //  DropDownList.Add(Items.countries[i].name.ToString() + "             " + Items.countries[i].code.ToString());
      //  }
      //  print("List of others " + DropDownList.Count);   
      //  dropDownCountryCode.AddOptions(DropDownList);
    }
    private void Start()
    {
        // CodeText.text = Items.countries[dropDownCountryCode.value].code;   
        // dropDownCountryCode.onValueChanged.AddListener(delegate {  
        //    DropdownValueChanged(dropDownCountryCode);
        //});   
    }
    public void CountrySelectedHere(int TakeIndex)
    {
        CodeText.text = Items.countries[TakeIndex].code;
        //  Exampletext.text = Items.countries[TakeIndex].code;
        // NumberField.gameObject.SetActive(true); 
          CountryCodePanal.SetActive(false);
     }                       
    public void OpenCountryCodePanal()
    {
        CountryCodePanal.SetActive(true);
        //NumberField.gameObject.SetActive(false);
      //  print("number field");  
    }
    public void ClosePanal()
    {
        //NumberField.gameObject.SetActive(true);
        CountryCodePanal.SetActive(false);
    }
        //Ouput the new value of the Dropdown into Text
        void DropdownValueChanged(Dropdown change)
    {
        //print(change.value.ToString());
        //var Index = change.value;
        //CodeText.text = Items.countries[Index].code;
        //CountryCodePanal.SetActive(false);
     }   
}
