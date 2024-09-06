using UnityEngine;
using UnityEngine.UI;

public class BlockSpaceInput : MonoBehaviour
{
    private InputField inputField;

    private void Start()
    {
        inputField = GetComponent<InputField>();

        if (inputField != null)
        {
            // Attach the validation function to the onValueChanged event
            inputField.onValueChanged.AddListener(OnInputValueChanged);
        }
    }

    private void OnInputValueChanged(string newValue)
    {
        // Remove spaces from the input value
        string newValueWithoutSpaces = newValue.Replace(" ", "");

        // Set the input field's text without spaces
        inputField.text = newValueWithoutSpaces;
    }
}
