using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUser : MonoBehaviour
{
    public Transform targ;
    public Vector3 Offset;
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targ.position + Offset, 0.5f);
    }
}
