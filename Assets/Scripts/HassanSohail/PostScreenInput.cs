using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PostScreenInput : MonoBehaviour
{  
   [SerializeField] TMP_Text ShowText;
   [SerializeField] TMP_InputField inputField;
   public void TextChange(){
        ShowText.text = "";
        ShowText.text = inputField.text;
   }

}
