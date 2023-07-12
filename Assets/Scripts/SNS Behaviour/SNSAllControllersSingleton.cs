using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SNSAllControllersSingleton : MonoBehaviour
{
    public static bool isCreated;
    public bool isSingleton;


    private void Awake()
    {
        if (isSingleton)
        {
            if (!isCreated)
            {
                DontDestroyOnLoad(this.gameObject);
                isCreated = true;
            }
            else
            {
                Destroy(this.gameObject);
            }

            /*if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);*/
        }
    }
}