using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordEntry : MonoBehaviour
{
    public static PasswordEntry instance; // Singleton instance
    public List<TMP_InputField> passwordInput; // Input field for entering password
    public Button submitButton; // Button to submit the password
    public List<string> collectedKeywords = new List<string>(); // List to hold collected keywords
    public List<TMP_InputField> inputFields; // List of input fields for displaying collected keywords
    public List<string> requiredPasswordSequence; // The required sequence of keywords for the password
    public TextMeshProUGUI updateFeedbackText; // Text for displaying feedback messages
    private void Start()
    {
        instance = this; // Initialize singleton instance
        //submitButton.onClick.AddListener(SubmitPassword); // Add listener to submit button
        UpdateInputFields(); // Initial update of input fields
    }

    public void SubmitPassword()
    {
        // Check if the collected keywords match the required sequence
        CheckPassword();
    }

    public void CollectKeyword(string keyword)
    {
        // Collect keyword if not already collected
        if (!collectedKeywords.Contains(keyword))
        {
            collectedKeywords.Add(keyword); // Add keyword to the list
            UpdateInputFields(); // Update input fields with the new keyword
            Debug.Log($"Collected Keyword: {keyword}"); // Log collected keyword
        }
    }

    public void CheckPassword()
    {
        if (ArePasswordsMatching())
        {
            UpdateFeedback("All Passwords Correct! Access Granted.", Color.green);
        }
        else
        {
            UpdateFeedback("One or More Passwords Incorrect! Access Denied.", Color.red);
        }
    }
    private bool ArePasswordsMatching()
    {
        // Check if the number of input fields matches the number of required password strings
        if (passwordInput.Count != requiredPasswordSequence.Count)
        {
            return false; // Return false if counts do not match
        }

        // Compare each TMP_InputField text with the corresponding required password
        for (int i = 0; i < passwordInput.Count; i++)
        {
            // Trim the text to avoid leading or trailing whitespace issues
            string inputPassword = passwordInput[i].text.Trim();

            // Compare the input password with the required password sequence
            if (inputPassword != requiredPasswordSequence[i])
            {
                return false; // Return false if any input does not match the corresponding required password
            }
        }

        return true; // All inputs match the required passwords
    }
    private void MatchPassWithAdmin()
    {
       
    }
    private void UpdateInputFields()
    {
        // Clear existing input field values
        foreach (var inputField in inputFields)
        {
            inputField.text = ""; // Clear the text in each input field
        }

        // Update input fields with collected keywords
        for (int i = 0; i < collectedKeywords.Count && i < inputFields.Count; i++)
        {
            inputFields[i].text = collectedKeywords[i]; // Set the keyword in the corresponding input field
        }
    }

    private void UpdateFeedback(string message, Color color)
    {
        updateFeedbackText.text = message; // Update feedback message
        updateFeedbackText.color = color; // Change feedback text color
    }
}