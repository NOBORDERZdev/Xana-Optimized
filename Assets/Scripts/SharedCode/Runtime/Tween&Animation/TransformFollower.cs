using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransformFollower : MonoBehaviour 
{
    public Transform target;
    [Range(0,1)]
    public float speed = .1f;
    Vector3 lastPos;

    void OnEnable()
    {
        //if (lastPos.Equals(Vector3.zero))
        {
            lastPos = transform.position;
        }
    }

    void Update()
    {
        if (target!=null && speed>0)
        {
            transform.position = Vector3.Lerp(lastPos, target.position, speed);
            lastPos = transform.position;
        }
    }
} 