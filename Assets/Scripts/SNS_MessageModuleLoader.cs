using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SNS_MessageModuleLoader : MonoBehaviour
{
    public GameObject sns2;
    private AdditiveScenesLoader additiveSceneManger;

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

        additiveSceneManger = FindObjectOfType<AdditiveScenesLoader>();
        additiveSceneManger.SNSMessage = this.sns2;

        //FindObjectOfType<AdditiveScenesLoader>().SNSMessage = this.sns2;
    }
}