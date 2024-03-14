using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using AdvancedInputFieldPlugin;
using System;

public class savePasswordList : MonoBehaviour
{
    public static savePasswordList instance;
    //public MobileInputField UserNameInputField;
    //public MobileInputField PasswordInputField1;
    //public MobileInputField PasswordInputField2;
    //public ShiftCode PasswordShiftCode;

    public AdvancedInputField UserNameInputField;
    public AdvancedInputField PasswordInputField1;

    public List<string> SearchingList;
    public GameObject UserAccountObj;
    public Transform ParentOfUserAccount;
     public SaveUserName mySaveUserObj;
    public SavePassword mySavePasswordObj;
    public List<GameObject> UserAccountObjList;
    public Toggle saveToggle;  
    private void OnEnable()
    {
        if(instance== null)
        {
            instance = this;
        }

       // Password.on += OnFocusChange;
        //PasswordInputField2.OnFocusChanged += OnFocusChange;
    }

    private void OnDisable()
    {
        //PasswordInputField1.OnFocusChanged -= OnFocusChange;
    //    PasswordInputField2.OnFocusChanged -= OnFocusChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        //if(PlayerPrefs.GetInt("FirstTime") != 1)
        //{
        //    PlayerPrefs.DeleteAll();
        //    PlayerPrefs.SetInt("FirstTime", 1);
        //    PlayerPrefs.Save();
        //}   
        //  //  print(mySaveUserObj.userName.Count);
        // mySaveUserObj = LoadSavedDataOfUserName();
        //mySavePasswordObj = LoadSavedDataofPassword();
        //UserRegisterationManager.instance.SavingBool = saveToggle.isOn;
        //saveToggle.onValueChanged.AddListener(value => UserRegisterationManager.instance.SavingBool = saveToggle.isOn

        //); 
       }
 
    
    void ToggleValueChanged()
    {

    }

    void OnFocusChange(bool focus)
    {
        DeleteONStart();
    }

    public void DeleteONStart()  
    {          
        if (ParentOfUserAccount.childCount > 0)
        { 
            for (int i = 0; i < ParentOfUserAccount.childCount; i++)
            {
                Destroy(ParentOfUserAccount.GetChild(i).gameObject);
             }  
        }
        SearchingList.Clear();
        UserAccountObjList.Clear();
    } 

    public void OnValueChange()
    {
        if (ParentOfUserAccount.childCount > 0)
        {
            for(int i =0; i< ParentOfUserAccount.childCount; i++)
            {
                Destroy(ParentOfUserAccount.GetChild(i).gameObject);
            }   
        }  
         SearchingList.Clear();
        UserAccountObjList.Clear();
          string  input = UserNameInputField.Text;
        
         foreach (string nameInList in mySaveUserObj.userName)
        {
            if (nameInList.Contains(input))   
            {
                  SearchingList.Add(nameInList);
                  GameObject myAccountObj = Instantiate(UserAccountObj, ParentOfUserAccount);
                  myAccountObj.GetComponent<UserAccountSavePassword>().AccountUserName.text = nameInList;
                   int Tempindex= mySaveUserObj.userName.IndexOf(nameInList);
                 myAccountObj.GetComponent<UserAccountSavePassword>().AccountUserPassword.text = mySavePasswordObj.password[Tempindex];
                 UserAccountObjList.Add(myAccountObj);
             }   
        }

        if (UserAccountObjList.Count > 0)
        {

        }
        else
        {
            DeleteONStart();
        }
    }

    public void DisableOnLoginButton()
    {
        if (ParentOfUserAccount.childCount > 0)
        {
            for (int i = 0; i < ParentOfUserAccount.childCount; i++)
            {
                Destroy(ParentOfUserAccount.GetChild(i).gameObject);
            }
        }
        SearchingList.Clear();
        UserAccountObjList.Clear();
    }

    public SaveUserName LoadSavedDataOfUserName()
    {
        SaveUserName saveobj = new SaveUserName();
        string saveJson = "";

        if (PlayerPrefs.HasKey("SaveUserNamePref"))
        {
             saveJson = PlayerPrefs.GetString("SaveUserNamePref");
            saveobj = JsonUtility.FromJson<SaveUserName>(saveJson);
            return saveobj;
        }
        else
        {
            return mySaveUserObj;
        }
      //  print(saveJson);
    }    

    public SavePassword LoadSavedDataofPassword()
    {
        SavePassword savePassobj = new SavePassword();
        string savePassJson = "";

        if (PlayerPrefs.HasKey("SavePasswordPref"))
        {
            savePassJson = PlayerPrefs.GetString("SavePasswordPref");
            savePassobj = JsonUtility.FromJson<SavePassword>(savePassJson);
            return savePassobj;
        }
        else
        {
            return mySavePasswordObj;
        }
    }   

    public void PutDataInFields(string userName , string Password)
    {
         UserNameInputField.Text = userName;
        PasswordInputField1.Text = Password;
      //  PasswordShiftCode.TakePasswordFromSavePassword(Password);

        if (ParentOfUserAccount.childCount > 0)
        {
            for (int i = 0; i < ParentOfUserAccount.childCount; i++)
            {
                Destroy(ParentOfUserAccount.GetChild(i).gameObject);
            }
        }

    }

    public  void saveData(string L_userName , String L_Password)
    {
        if (!UserRegisterationManager.instance.SavingBool)
        {
            return;
        }
        string userNameItemTxt = L_userName;
        string PasswordItemTxt = L_Password;     

        if (userNameItemTxt == "" || PasswordItemTxt == "")
        {
          //  print("Fields should not be empty");
            return;
        }      
        if (PlayerPrefs.HasKey("SaveUserNamePref"))
        {
            mySaveUserObj= LoadSavedDataOfUserName();
            mySavePasswordObj = LoadSavedDataofPassword ();

            if (mySaveUserObj.userName.Contains(userNameItemTxt))
            {
             //   print("Already existed on index " + mySaveUserObj.userName.IndexOf(userNameItemTxt));
                int tempIndex = mySaveUserObj.userName.IndexOf(userNameItemTxt);
                mySaveUserObj.userName[tempIndex] = userNameItemTxt;
                mySavePasswordObj.password[tempIndex] = PasswordItemTxt;
            }
            else
            {
                mySaveUserObj.userName.Add(userNameItemTxt);
                mySavePasswordObj.password.Add(PasswordItemTxt);
            }
        }
        else
        {
          //  print(userNameItemTxt);
          //  print(mySaveUserObj.userName.Count);
             mySaveUserObj.userName.Add(userNameItemTxt);
            mySavePasswordObj.password.Add(PasswordItemTxt);
        }

        string saveUserNameJson = JsonUtility.ToJson(mySaveUserObj);
        string savePasswordJson = JsonUtility.ToJson(mySavePasswordObj);  

        PlayerPrefs.SetString("SaveUserNamePref", saveUserNameJson);
        PlayerPrefs.SetString("SavePasswordPref", savePasswordJson);
        PlayerPrefs.Save();       
    }
 

    public void saveDataFromForgetPassword(string L_userName, String L_Password)
    {
        if (!UserRegisterationManager.instance.SavingBool)
        {
            return;
        }
 
        string userNameItemTxt = L_userName;
        string PasswordItemTxt = L_Password;

        if (userNameItemTxt == "" || PasswordItemTxt == "")
        {
          //  print("Fields should not be empty");
            return;
        }
        if (PlayerPrefs.HasKey("SaveUserNamePref"))
        {
            mySaveUserObj = LoadSavedDataOfUserName();
            mySavePasswordObj = LoadSavedDataofPassword();

            if (mySaveUserObj.userName.Contains(userNameItemTxt))
            {
              //  print("Change Password Of Index" + mySaveUserObj.userName.IndexOf(userNameItemTxt));
                int tempIndex = mySaveUserObj.userName.IndexOf(userNameItemTxt);
                mySaveUserObj.userName[tempIndex] = userNameItemTxt;
                mySavePasswordObj.password[tempIndex] = PasswordItemTxt;
            }
        }   

        string saveUserNameJson = JsonUtility.ToJson(mySaveUserObj);
        string savePasswordJson = JsonUtility.ToJson(mySavePasswordObj);

        PlayerPrefs.SetString("SaveUserNamePref", saveUserNameJson);
        PlayerPrefs.SetString("SavePasswordPref", savePasswordJson);
        PlayerPrefs.Save();
    }


    [System.Serializable]
    public class SaveUserName
    {
        public List<string> userName;

        internal SaveUserName Trim()
        {
            throw new NotImplementedException();
        }
    }

    [System.Serializable]
    public class SavePassword
    {
        public List<string> password;
    }  
 }
