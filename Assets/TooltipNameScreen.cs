using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipNameScreen : MonoBehaviour
{
    [SerializeField] private GameObject popup; // Reference to the popup GameObject
    private float showTime = 2.0f; // Time to show the popup (2 seconds)

    public void OnButtonClick()
    {
        popup.SetActive(true); // Show the popup
        StartCoroutine(HidePopup()); // Start coroutine to hide after 2 seconds
    }

    IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(showTime); // Wait for 2 seconds
        popup.SetActive(false); // Hide the popup
    }
}