using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {

    public Vector3 speed;

	void Update () {
        transform.eulerAngles += speed * Time.deltaTime;
	}
}
