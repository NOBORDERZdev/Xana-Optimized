using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGuestBackButton : MonoBehaviour
{
    public GameObject BackFromAvatarPanelButton;
    private void OnEnable()
    {
       if (ConstantsHolder.xanaConstants.LoggedInAsGuest && ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
        {
            if (BackFromAvatarPanelButton != null)
            {
                BackFromAvatarPanelButton.SetActive(true);
            }

        }

    }
    private void OnDisable()
    {
        if (ConstantsHolder.xanaConstants.LoggedInAsGuest && ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
        {
            if (BackFromAvatarPanelButton != null)
            {
                BackFromAvatarPanelButton.SetActive(false);
            }

        }
    }
}
