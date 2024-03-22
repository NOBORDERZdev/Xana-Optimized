using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleEnabler : MonoBehaviour
{
    public GameObject sns1;
    private AdditiveScenesController additiveSceneManger;

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

        //additiveSceneManger = FindObjectOfType<AdditiveScenesController>();
        GameManager.Instance.additiveScenesManager.SNSmodule = this.sns1;

       // FindObjectOfType<HomeFooterTabCanvas>().LoaderShow(false);
    }
}