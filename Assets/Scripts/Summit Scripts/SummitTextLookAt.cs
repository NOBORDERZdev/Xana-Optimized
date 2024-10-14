using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummitTextLookAt : MonoBehaviour
{
    private Transform localTrans;
    private Transform target;
    // Start is called before the first frame update
    void Start()
    {
        localTrans = GetComponent<Transform>();
        if (ConstantsHolder.isPenguin && XANAPartyCameraController.instance)
            target = XANAPartyCameraController.instance.cinemachine.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
            localTrans.LookAt(2 * localTrans.position - target.position);
        else
            target = XANAPartyCameraController.instance?.cinemachine.transform;
    }
}
