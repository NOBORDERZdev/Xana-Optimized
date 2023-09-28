using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class StreamingCameraPaining : MonoBehaviour
{
    public GameObject lookObj;
    int no=0;
    public bool focusOnScreen;
    public bool smallPaining;
    private void OnEnable()
    {
        InvokeRepeating(nameof(Paining),1,2.5f);
    }

    private void Update()
    {
        if (lookObj)
        {
            gameObject.transform.LookAt(lookObj.transform);
        }
    }

    public void Paining() // Paining
    {
        print("CAM " +gameObject.name +"index is "+ no );
        Vector3 temp= gameObject.transform.position; 
        int adjust ;
        if (smallPaining)
        {
            adjust =2;
        }
        else
        {
            adjust =1;
        }
        int rand =no /*Random.Range(0,3)*/;
        int Zoom = Random.Range(-1,2)/adjust;
        switch (rand)
        {
            case 0: // for left
                temp += new Vector3(-1/adjust,0,Zoom);
                break;
            case 1: // for right
                temp += new Vector3(1/adjust,0,Zoom);
                break;
            case 2: // for top
                temp += new Vector3(0, 0.5f/adjust, Zoom);
                break;
            //case 3: // for bottom
            //    temp += new Vector3(0, -0.5f, Zoom);
            //    break;
            default:
                temp += new Vector3(-1/adjust,0,Zoom);
                break;
        }
        gameObject.transform.DOMove(temp,1.5f/adjust,false);
        if (no < 3)
        {
            no++;
        }
        else
        {
            no=0;
        }

        //lookObj.transform.DOMove();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

}
