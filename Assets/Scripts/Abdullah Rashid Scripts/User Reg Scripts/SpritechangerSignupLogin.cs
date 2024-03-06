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

    private void OnEnable()
    {
        spriteRenderer.sprite = EyeUnchecked;
    }

    void Start()
    {
        textInputField = GetComponent<AdvancedInputField>();
      //  spriteRenderer = GetComponent<Image>();

        // Add listener for text field changes
       textInputField.OnValueChanged.AddListener(OnValueChanged);
    }

    public void OnValueChanged(string newText)
    {
        isEmail = Regex.IsMatch(newText, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);
        // Check if the entered string matches your desired value
       
        if (isEmail)
        {
            // Change the sprite
            spriteRenderer.sprite = EyeChecked;  // Replace with your actual sprite
        }else
        {
            // Change the sprite
            spriteRenderer.sprite = EyeUnchecked; // Replace with your actual sprite
        }
    }
}
