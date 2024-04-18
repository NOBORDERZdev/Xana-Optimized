using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableDisable : MonoBehaviour
{
    public UnityEvent OnEnabled;
    public UnityEvent OnDisabled;

    private void OnEnable()
    {
        if (AvatarCustomizationManager.Instance != null)
        {
            AvatarCustomizationManager.Instance.m_CanRotateCharacter = false;
        }
        OnEnabled.Invoke();
    }

    private void OnDisable()
    {
        if (AvatarCustomizationManager.Instance != null)
        {
            AvatarCustomizationManager.Instance.m_CanRotateCharacter = true;
        }
        OnDisabled.Invoke();
    }

    public void ClosePopUp()
    {
        OnDisabled.Invoke();
        GameManager.Instance.UiManager.ShowFooter(true);
        StartCoroutine(WaitForPopup());
    }
    IEnumerator WaitForPopup()
    {
        yield return new WaitForSeconds(0.34f);

        gameObject.SetActive(false);
    }
}
