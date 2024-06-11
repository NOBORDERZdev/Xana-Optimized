using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToLive : MonoBehaviour
{
    [SerializeField]
    private float stayTime = 3f;

    private void OnEnable()
    {
        Invoke(nameof(Visibility), stayTime);
    }

    
    private void Visibility()
    {
        this.gameObject.SetActive(false);
    }

}
