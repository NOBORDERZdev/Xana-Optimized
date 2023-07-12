using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UserAccountSavePassword : MonoBehaviour
{
    public Text AccountUserName;
    public Text AccountUserPassword;
    private string Password;
    public int IndexID;
    private Button myBtn;  
    // Start is called before the first frame update
    void Start()
    {
        // string oldString="";
        Password = AccountUserPassword.text;
        AccountUserPassword.text = new string('*', AccountUserPassword.text.Length);  
          myBtn = this.GetComponent<Button>();
        myBtn.onClick.AddListener(SelectAccount);
    }
    void SelectAccount()
    {
        savePasswordList.instance.PutDataInFields(AccountUserName.text, Password);
        print(AccountUserName.text);
        print(Password);
          // CountryCodeDropDown.instance.CountrySelectedHere(IndexID);
    }  
    // Update is called once per frame
    void Update()
    {
        
    }
}
