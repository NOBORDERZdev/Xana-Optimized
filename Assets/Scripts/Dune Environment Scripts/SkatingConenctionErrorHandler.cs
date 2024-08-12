using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkatingConenctionErrorHandler : MonoBehaviour
{
    private void OnEnable()
    {
        SandGameManager.Instance.EnableSkating();
    }

    private void OnDisable()
    {
        SandGameManager.Instance.DisableSkating();
    }
}
