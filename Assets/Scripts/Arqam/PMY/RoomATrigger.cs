using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomATrigger : MonoBehaviour
{
    public UnityEvent onPerformAction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PhotonLocalPlayer"))
            onPerformAction.Invoke();
    }

}
