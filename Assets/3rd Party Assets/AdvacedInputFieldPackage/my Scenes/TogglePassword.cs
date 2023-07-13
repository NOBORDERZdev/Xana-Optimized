using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedInputFieldPlugin;
public class TogglePassword : MonoBehaviour
{
    public AdvancedInputField Password;
    public bool OnBool;
    
    // Start is called before the first frame update
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
        }
        else
        {
          Password.ContentType = ContentType.STANDARD;
        }
     

    }
}
