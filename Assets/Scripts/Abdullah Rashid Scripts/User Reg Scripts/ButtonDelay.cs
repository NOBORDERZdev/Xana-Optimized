using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDelay : MonoBehaviour
{
    public Button myButton; // Assign your button in the inspector

    private void Start()
    {
        this.myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnClick); // Add listener for click
    }

    void OnClick()
    {
        myButton.interactable = false; // Disable button interaction
        StartCoroutine(EnableButtonAfterDelay(10f)); // Start coroutine to wait
    }

    IEnumerator EnableButtonAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime); // Wait for specified time
        myButton.interactable = true; // Enable button interaction again
    }
}
