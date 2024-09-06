using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchForStore : MonoBehaviour
{

    public static bool isCustomizationPanelOpen = false;
    private void OnEnable()
    {
        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
        GameManager.Instance.userAnimationPostFeature.GetComponent<UserPostFeature>().ActivatePostButtbleHome(false);
    }
    private void OnDisable()
    {
        if (isCustomizationPanelOpen)
            return;

        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(false);
        GameManager.Instance.userAnimationPostFeature.GetComponent<UserPostFeature>().ActivatePostButtbleHome(true);
    }
}
