using UnityEngine;
using System.Collections;

public class AutoMove : MonoBehaviour {

    public Vector3 directionAbs;
    public Transform directionRef;
    public bool randomDirection;
    Vector3 directionRandom;
    public float speed; 


    Vector3 direction {
        get
        {
            if (randomDirection) return directionRandom; 

            if (directionRef == null) return directionAbs; 

            return directionRef.forward;
        }
    }

    void OnEnable() {
        directionRandom = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    void Update()
    {
        transform.localPosition += direction * speed * Time.deltaTime;
    }
}
