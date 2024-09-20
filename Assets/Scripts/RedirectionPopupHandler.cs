using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedirectionPopupHandler : MonoBehaviour
{
    public TMPro.TextMeshProUGUI RedirectionText;
    public GameObject RedirectionPopup;

    private string RedirectUrl;
    private void OnEnable()
    {
        BuilderEventManager.OpenRedirectionPopup += OpenPopup;
    }

    private void OnDisable()
    {
        BuilderEventManager.OpenRedirectionPopup -= OpenPopup;
    }

    public void OnContinue()
    {
        Application.OpenURL(RedirectUrl);
        OnCancel();
    }

    void OpenPopup(string url, string msg)
    {
        RedirectUrl = url;
        RedirectionPopup.SetActive(true);

        if (!string.IsNullOrEmpty(msg))
            RedirectionText.text = msg;
    }

    public void OnCancel()
    {
        RedirectionPopup.SetActive(false);
    }
}
