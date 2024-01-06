using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchForStore : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
        GameManager.Instance.userAnimationPostFeature.GetComponent<UserPostFeature>().ActivatePostButtbleHome(false);
    }
    private void OnDisable()
    {
        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(false);
        GameManager.Instance.userAnimationPostFeature.GetComponent<UserPostFeature>().ActivatePostButtbleHome(true);
    }
}
