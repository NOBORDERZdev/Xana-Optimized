using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    public static KeyboardManager instance;

    private void Awake()
    {
        instance = this;
    }
}
