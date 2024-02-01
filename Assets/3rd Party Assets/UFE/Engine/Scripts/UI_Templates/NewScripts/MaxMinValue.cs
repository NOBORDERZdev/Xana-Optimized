using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxMinValue : MonoBehaviour
{
    public int minValue = 0;
    public int maxValue = 100;

    private InputField inputField;

    private void Start()
    {
        inputField = GetComponent<InputField>();

        if (inputField != null)
        {
            // Attach the validation function to the onValueChanged event
            inputField.onEndEdit.AddListener(OnInputValueChanged);
        }
    }

    private void OnInputValueChanged(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            // If no value is entered, set the input field to the minimum value
            inputField.text = minValue.ToString();
        }
        else if (int.TryParse(newValue, out int intValue))
        {
            // Validate the input value within the specified range
            intValue = Mathf.Clamp(intValue, minValue, maxValue);

            // Update the input field text with the validated value
            inputField.text = intValue.ToString();
        }
        else
        {
            // Clear the input field if the input is not a valid integer
            inputField.text = "";
        }
    }
}
