using UnityEngine;
using System.Collections;

public class ChipsStackAnim : MonoBehaviour 
{
    public float rangeMin = 10, rangeMax=20;
    public Transform stack; 
    public float spacing = 10;
    public AnimationCurve spacingCurve = AnimationCurve.Linear(0,0,1,1);


    public float initialDleay = 0;
    public float duration = 1;
    public bool startOnEnable = true;  
    public AnimationCurve curve = new AnimationCurve(new Keyframe[]{new Keyframe(0,0), new Keyframe(0.5f,1), new Keyframe(1,0)});

    float randomRange, timeElapsed, lf;

    void OnEnable()
    {
        if (startOnEnable) StartTween();
    }

    void OnDisable()
    {
        EndTween();
    }

    public void StartTween()
    {
        StopCoroutine("Tween");
        StartCoroutine("Tween");
    }

    public void EndTween()
    {
        StopCoroutine("Tween");
    }

    IEnumerator Tween() 
    {
        randomRange = Random.Range(rangeMin, rangeMax);
        timeElapsed = 0;
        ApplyVal();
        if(initialDleay>0)yield return new WaitForSeconds(initialDleay); 
        while (timeElapsed<duration)
        {
            timeElapsed += Time.deltaTime;
            ApplyVal ();
            yield return null;
        }
        timeElapsed = duration;
        ApplyVal ();  
    }

    void ApplyVal()
    {
        lf = curve.Evaluate(timeElapsed / duration);
        float cc = transform.childCount;
        for (int i = 0; i < cc; i++)
        {
            transform.GetChild(i).localPosition = new Vector3(
                0, 
                Mathf.Lerp(i*spacing, i*spacing + randomRange*spacingCurve.Evaluate(i/cc), lf), 
                0); 
        }
    }
}
