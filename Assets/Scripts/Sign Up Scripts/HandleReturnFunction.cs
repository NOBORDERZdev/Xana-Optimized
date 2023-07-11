using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;
public class HandleReturnFunction : MonoBehaviour
{
    // Start is called before the first frame update

     public ShiftCode ShiftCodeField;
 
    void Start()
    {
        
    }

    public void OnReturnFirst()
    {
          ShiftCodeField.SelectTextFromReturn();    
    }      
    // Update is called once per frame
    void Update()
    {
        
    }

    public void onReturnName()
    {
     }
}
