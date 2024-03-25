using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScreenRotation : MonoBehaviour
{
    [SerializeField] Vector3 screenRotation;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        this.transform.localRotation = Quaternion.Euler(screenRotation);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
