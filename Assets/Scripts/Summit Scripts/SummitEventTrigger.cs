using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummitEventTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BuilderEventManager.LoadSummitScene?.Invoke();   
    }
}
