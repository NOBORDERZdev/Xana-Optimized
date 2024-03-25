using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SNS_SMSModuleHandler : MonoBehaviour
{
    public GameObject sns2;
    private AdditiveScenesController additiveSceneManger;

    public bool isTest;
    public GameObject testingObj;
   
    void Awake()
    {
        if (isTest)
        {
            testingObj.SetActive(true);
            sns2.SetActive(true);
            return;
        }

        additiveSceneManger = FindObjectOfType<AdditiveScenesController>();
        additiveSceneManger.SNSMessage = this.sns2;

        //FindObjectOfType<AdditiveScenesController>().SNSMessage = this.sns2;
    }
}