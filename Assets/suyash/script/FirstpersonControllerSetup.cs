using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstpersonControllerSetup : MonoBehaviour
{
    public static FirstpersonControllerSetup instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DisablePlayerOnFPS();
    }

    public void DisablePlayerOnFPS()
    {
        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].gameObject.layer != LayerMask.NameToLayer("Arrow") && (transforms[i].gameObject.GetComponent<Renderer>() || transforms[i].gameObject.GetComponent<MeshRenderer>()))
            {
                transforms[i].gameObject.GetComponent<Renderer>().enabled = false;
                if (transforms[i].gameObject.name == "Eye_Left" || transforms[i].gameObject.name == "Eye_Right") //this is written becuase can't disable mesh renderer
                {
                    transforms[i].gameObject.transform.localScale = Vector3.zero;
                }
            }
            else if (transforms[i].GetComponent<CanvasGroup>())
                transforms[i].GetComponent<CanvasGroup>().alpha = 0;
        }
    }
}
