using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchForAvatar : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(true);
    }
    private void OnDisable()
    {
        GameManager.Instance.ActorManager.IdlePlayerAvatorForMenu(false);
    }
}
