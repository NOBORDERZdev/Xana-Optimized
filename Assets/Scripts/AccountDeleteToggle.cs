using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountDeleteToggle : MonoBehaviour
{
    public Toggle _toggle;
    public Button _button;
   public void ChangeButtonOnToggle()
   {
        if (_toggle.isOn)
            _button.interactable = true;
        else
            _button.interactable = false;

   }
}
