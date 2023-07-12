using UnityEngine;
using System.Collections;

public class PathMovement : MonoBehaviour 
{
    public Transform pathPointsParent;
    public float speed = 1;
    public bool startOnEnable; 

    Transform nextTarget;
    int nextTargetIndex;

    void OnEnable()
    {
        if (startOnEnable)
            StartMovement();
    }

    public void StartMovement()
    {
        if (pathPointsParent == null || pathPointsParent.childCount < 2) return;

        StopCoroutine("Move_c");
        StartCoroutine("Move_c");
    }

    public void StopMovement()
    {
        StopCoroutine("Move_c");
    }

    IEnumerator Move_c() { 
        int nextPointIndex = 1;  
        nextTargetIndex = 0;
        nextTarget = pathPointsParent.GetChild(0);
        Vector3 dir;
        float delta = 0;
        while (true)
        {   
            dir = nextTarget.position - transform.position;
            delta =  speed * Time.deltaTime;
            transform.position += dir.normalized * delta;
            yield return null;
            if (dir.magnitude < delta)
            {
                transform.position = nextTarget.position;
                nextTargetIndex = (nextTargetIndex + 1) % pathPointsParent.childCount;
                nextTarget = pathPointsParent.GetChild(nextTargetIndex);
                print(nextTargetIndex);
                yield return null;
            }
        }
    }
}
