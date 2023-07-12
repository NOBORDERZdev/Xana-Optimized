using UnityEngine;
using System.Collections;

public class AutoDisable : MonoBehaviour {
    public float wait;
    void OnEnable() {
        StartCoroutine(cr());
    }
	IEnumerator cr (){
        yield return new WaitForSeconds(wait);
        gameObject.SetActive(false);
    }
}
