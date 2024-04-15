using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScreenRotationTHA : MonoBehaviour
{
    [SerializeField] Vector3 screenRotation;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_IOS && !UNITY_EDITOR
        this.transform.localRotation = Quaternion.Euler(screenRotation);
#endif
    }
}
