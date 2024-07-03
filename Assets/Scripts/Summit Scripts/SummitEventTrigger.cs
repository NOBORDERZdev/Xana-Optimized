using System;
using UnityEngine;

public class SummitEventTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BuilderEventManager.LoadSummitScene?.Invoke();
    }
}
