using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snssmsObjScript : MonoBehaviour
{
    public GameObject sns2;
    private AdditiveScenesManager additiveSceneManger;

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

        additiveSceneManger = FindObjectOfType<AdditiveScenesManager>();
        additiveSceneManger.SNSMessage = this.sns2;

        //FindObjectOfType<AdditiveScenesManager>().SNSMessage = this.sns2;
    }
}