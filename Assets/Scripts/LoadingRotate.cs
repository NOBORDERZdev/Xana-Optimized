using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingRotate : MonoBehaviour
{
    bool rotate = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!rotate)
        {
            this.transform.DORotate(new Vector3(0, 0, 180), .5f).OnComplete(delegate ()
            {
                rotate = !rotate;
                Start();
            });
        }
        else
        {
            this.transform.DORotate(new Vector3(0, 0, 360), .5f).OnComplete(delegate ()
            {
                rotate = !rotate;
                Start();
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
