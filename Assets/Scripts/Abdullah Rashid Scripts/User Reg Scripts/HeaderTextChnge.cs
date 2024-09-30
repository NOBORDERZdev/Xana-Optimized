using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HeaderTextChnge : MonoBehaviour
{
    public TextMeshProUGUI TextComponent;
    public string TextToChange;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (ConstantsHolder.xanaConstants.LoggedInAsGuest) {
            TextComponent.text = TextToChange;
        }
    }
}
