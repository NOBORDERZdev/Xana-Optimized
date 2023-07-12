using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextEffectSeparated : MonoBehaviour {  
	public float defDuration = 1;
	float currDuration;  
	string targetString;
	int saperatorsRequired = 0; 

	public bool beginOnEnable;

	void OnEnable(){
//		Begin ("MEGA WIN!", 0);
		if (beginOnEnable) {
			Begin ();
		}
	}

	void Begin(string val, float duration = 0)
	{
		Set(val, duration);
		Begin ();
	}

	public void Set(string val, float duration = 0)
	{
		currDuration = duration == 0 ? defDuration : duration;
		targetString = val;
		saperatorsRequired = targetString.Length + 1; 
	} 

	void Begin()
	{
//		print (targetString);
		StopCoroutine ("Effect");

		if (transform.childCount < saperatorsRequired) {
			for (int i = transform.childCount; i < saperatorsRequired; i++) {
				transform.GetChild (0).Duplicate (transform); 
			}
		} 
		else {
			for (int i = 0; i < transform.childCount-saperatorsRequired; i++) {
				Destroy(transform.GetChild (transform.childCount-i-1).gameObject); 
			}
		}

		transform.GetChild (0).gameObject.SetActive(false);
		for (int i = 1; i < transform.childCount; i++) {
			transform.GetChild (i).gameObject.SetActive(false);
			transform.GetChild (i).GetComponent<Text> ().text = targetString [i-1].ToString();
		}

		StartCoroutine ("Effect");  
	}

	IEnumerator Effect()
	{
		float t = 0;

		for (int i = 1; i < transform.childCount; i++) {
			transform.GetChild (i).gameObject.SetActive(true);
			yield return new WaitForSeconds (currDuration/transform.childCount);
		}  
	}
}
