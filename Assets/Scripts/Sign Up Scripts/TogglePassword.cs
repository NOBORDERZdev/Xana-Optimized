using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedInputFieldPlugin;
using UnityEngine.UI;
public class TogglePassword : MonoBehaviour
{
    public AdvancedInputField Password;
    public bool OnBool;
    public Sprite ActiveEye;
    public Sprite UnActiveEye;
    public Image myimage;
    
    
    private void OnEnable()
    {
        myimage.sprite = UnActiveEye;
    }
   
    void Start()
    {
        
        OnBool = true;
       
    }

    public void TogglePasswordhere()
    {
       
        OnBool = !OnBool;
        print(OnBool);
        if (OnBool)
        {
            Password.ContentType = ContentType.PASSWORD;
            myimage.sprite = UnActiveEye;


        }
        else
        {
          Password.ContentType = ContentType.STANDARD;
            myimage.sprite = ActiveEye;
        }
     

    }
}
