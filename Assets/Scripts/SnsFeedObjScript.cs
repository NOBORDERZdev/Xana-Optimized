using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnsFeedObjScript : MonoBehaviour
{
    public GameObject sns1;
    private AdditiveScenesManager additiveSceneManger;

    public bool isTest;
    public GameObject testingObj;

    void Awake()
    {
        if (isTest)
        {
            testingObj.SetActive(true);
            sns1.SetActive(true);
            return;
        }

        additiveSceneManger = FindObjectOfType<AdditiveScenesManager>();
        additiveSceneManger.SNSmodule = this.sns1;

       // FindObjectOfType<BottomTabManager>().LoaderShow(false);
    }
}