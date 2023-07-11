using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdvancedInputFieldPlugin;

public class InputFieldValidationCheckForSpecialCharacter : MonoBehaviour
{

    [SerializeField] InputField inputField;

    [SerializeField] TMP_InputField inputFieldTMP;

    [SerializeField] AdvancedInputField advanceInputField;


    void OnEnable()
    {
        //Register InputField Event
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(inputValueChanged);
        }
        else if(inputFieldTMP != null)
        {
            inputFieldTMP.onValueChanged.AddListener(inputValueChanged);
        }
        /*else if(advanceInputField != null)
        {
            advanceInputField.OnValueChanged.AddListener(inputValueChanged);
        }*/
    }

    public static string CleanInput(string strIn)
    {
        // Replace invalid characters with empty strings.
        return Regex.Replace(strIn, @"[^a-zA-Z0-9_]", "");
    }

    //Called when Input changes
    void inputValueChanged(string attemptedVal)
    {
        if (inputField != null)
        {
            inputField.text = CleanInput(attemptedVal);
        }
        else if (inputFieldTMP != null)
        {
            inputFieldTMP.text = CleanInput(attemptedVal);
        }  
        else if(advanceInputField != null)
        {
            advanceInputField.Text = CleanInput(attemptedVal);
        }
    }
}