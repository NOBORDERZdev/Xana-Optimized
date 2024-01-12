using AdvancedInputFieldPlugin;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class SpritechangerSignupLogin : MonoBehaviour
{

    public AdvancedInputField textInputField;  // Reference to the text field
    public Image spriteRenderer;  // Reference to the sprite renderer
    public Sprite EyeChecked;                                        // Start is called before the first frame update
    public Sprite EyeUnchecked;
    bool isEmail;


    void Start()
    {
        textInputField = GetComponent<AdvancedInputField>();
      //  spriteRenderer = GetComponent<Image>();

        // Add listener for text field changes
       textInputField.OnValueChanged.AddListener(OnValueChanged);
    }

    // Update is called once per frame
    public void OnValueChanged(string newText)
    {
        isEmail = Regex.IsMatch(newText, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        // Check if the entered string matches your desired value
        if (!isEmail)
        {
            // Change the sprite
            spriteRenderer.sprite = EyeUnchecked;  // Replace with your actual sprite
        }else
        {
            // Change the sprite
            spriteRenderer.sprite = EyeChecked;  // Replace with your actual sprite
        }
    }
}
