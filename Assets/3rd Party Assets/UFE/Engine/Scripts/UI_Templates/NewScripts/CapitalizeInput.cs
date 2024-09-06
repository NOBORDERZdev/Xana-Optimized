using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CapitalizeInput : MonoBehaviour
{
    [SerializeField]
    private Text textField;

    private void Start()
    {
        textField = GetComponent<Text>();
        // Capitalize the initial text in the input field
        textField.text = textField.text.ToUpper();
    }
}
