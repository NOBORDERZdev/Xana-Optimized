using UnityEngine;
using System.Collections;

public class MoveToChildren : MonoBehaviour {
	public Transform parent;
	public float speed = 1;
	public bool loop;
	public bool beginOnEnable;
	Transform currentTarget;

	void OnEnable()
	{
		if (beginOnEnable) 
		{
			Begin ();
		}
	}

	public void Begin() 
	{
		StopCoroutine ("Movement");
		StartCoroutine ("Movement");
	}

	IEnumerator Movement()
	{
		Vector3 startPos = Vector3.zero; 
		Vector3 endPos = Vector3.zero; 
		float t = 0, et = 0;
		do { 
			for (int i = 0; i < parent.childCount; i++) 
			{ 
				currentTarget = parent.GetChild(i);
				startPos = i > 0 ? parent.GetChild(i-1).position : transform.position;
				endPos = currentTarget.position;
				et = Vector3.Distance (startPos, endPos) / speed;
				t = 0;
				while (t < et) 
				{
					transform.position = Vector3.Lerp (startPos, endPos, t / et);
					t+=Time.deltaTime;
					yield return null;
				}
			}
		} while (loop) ;
	}
}