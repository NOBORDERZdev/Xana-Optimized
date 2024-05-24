using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipNameScreen : MonoBehaviour
{
    [SerializeField] private GameObject _popup; // Reference to the popup GameObject
    private float _showTime = 2.0f; // Time to show the popup (2 seconds)

    public void OnButtonClick()
    {
        _popup.SetActive(true); // Show the popup
        StartCoroutine(HidePopup()); // Start coroutine to hide after 2 seconds
    }

    IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(_showTime); // Wait for 2 seconds
        _popup.SetActive(false); // Hide the popup
    }
}