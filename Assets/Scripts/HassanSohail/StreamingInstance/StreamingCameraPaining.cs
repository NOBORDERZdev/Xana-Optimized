using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class StreamingCameraPaining : MonoBehaviour
{
    public GameObject lookObj;

    private void Start()
    {
        InvokeRepeating(nameof(Paining),1,5);
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
        Vector3 temp= gameObject.transform.position; 
        int rand = Random.Range(0,2);

        switch (rand)
        {
            case 0: // for left
                temp += new Vector3(-2,0,0);
                break;
            case 1: // for right
                temp += new Vector3(2,0,0);
                break;
            //case 2: // for top
            //    temp += new Vector3(0,2,0);
            //    break;
            //case 3: // for bottom
            //    temp += new Vector3(0,-2,0);
            //    break;
            default:
                temp += new Vector3(-2,0,0);
                break;
        }
        gameObject.transform.DOMove(temp,1.5f,false);


        //lookObj.transform.DOMove();
    }
}
