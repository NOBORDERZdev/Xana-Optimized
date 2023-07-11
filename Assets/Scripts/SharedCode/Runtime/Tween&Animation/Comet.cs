using UnityEngine;
using System.Collections;

public class Comet : MonoBehaviour 
{
    public float fuel;
    public Vector3 selfDirection;
    public Vector3 selfDirectionRange;
    public Transform target;
    public float targetSpeed = 10;

    Vector3 randomSelfDir;
    Vector3 tempPos;
    public float leftFuel;
    void OnEnable()
    {
        leftFuel = fuel;
        randomSelfDir = selfDirection + new Vector3(Random.Range(-selfDirectionRange.x, selfDirectionRange.x), 
                                                Random.Range(-selfDirectionRange.y, selfDirectionRange.y), 
                                                Random.Range(-selfDirectionRange.z, selfDirectionRange.z));
    }

	void Update () 
    {
        tempPos = transform.position;
        if (leftFuel>0)
        {
            tempPos += randomSelfDir * Time.deltaTime * leftFuel / fuel;
            leftFuel --;
        }
        if (target!=null)
        {
            tempPos += (target.position - transform.position).normalized * targetSpeed * Time.deltaTime;
        }
        transform.position = tempPos;
	}
}
