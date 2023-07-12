using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

 

public class ShiftCode : MonoBehaviour
{
    private bool Show;
    public string Tempstring;
    //public MobileInputField FieldPassword;
    //public MobileInputField FieldStandard;

    void Start()
    {
        //FieldStandard.gameObject.SetActive(false);
        //FieldPassword.gameObject.SetActive(true);
        //FieldPassword.GetComponent<InputField>().text = "";
         Show = false;
     }
    private void OnEnable()
    {
        Show = false;
      //  FieldStandard.gameObject.SetActive(false);
      //  FieldPassword.gameObject.SetActive(true);
      ////  StartCoroutine(WaitAndActive());
      //  FieldPassword.GetComponent<InputField>().text = "";
     // FieldStandard.GetComponent<InputField>().text = "";

      }
    
    


    IEnumerator WaitAndActive()
    {
        yield return new WaitForSeconds(.1f);
     }

    public void TakePasswordFromSavePassword(string L_Password)
    {
        if (!Show)
        {
           //  FieldPassword.Text = L_Password;
        }
        else
        {
       //   FieldStandard.Text = L_Password;
        }       
    }

    public void EmptyPassword()
    {
        if (!Show)
        {
          //  FieldPassword.Text = "";
        }
        else
        {
           // FieldStandard.Text = "";
        }
    }

    public void ToggleShowPassword()
    {
        if(!Show)
        {
         //   Tempstring = FieldPassword.Text;
        }
        else
        {
          //  Tempstring = FieldStandard.Text;
          }   

        Show = !Show;
  
        if (Show)
        {
         //   FieldStandard.gameObject.SetActive(true);
         //   FieldPassword.gameObject.SetActive(false);
         }
        else
        {
        //    FieldStandard.gameObject.SetActive(false);
         //   FieldPassword.gameObject.SetActive(true);
         }
       // FieldPassword.Text = Tempstring;
      // FieldStandard.Text = Tempstring;
         //  this.gameObject.GetComponent<MobileInputField>().Text = Tempstring;   
    }
    public void SetText()
    {
        //Debug.Log("hello world" + FieldPassword.Text);
        //if (!Show)
        //{
        //    Tempstring = FieldPassword.Text;
        //}
        //else
        //{
        //    Tempstring = FieldStandard.Text;
        //}



    }
public string GetText()
    {
        if (!Show)
        {
         //   Tempstring = FieldPassword.Text;
        }
        else
        {
          //  Tempstring = FieldStandard.Text;
        }
        return Tempstring;
      }
    public void SelectTextFromReturn()
    {
        if (!Show)
        {
          //   FieldPassword.SelectOtherField();
        }
        else
        {
          //   FieldStandard.SelectOtherField();
        }
    }



    IEnumerator ActiveAndDeactiveField()
    {
           yield return new WaitForEndOfFrame();
          
    }  
  

}
