using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticTarget : MonoBehaviour
{
    public Transform thisTransform; 
    public Transform targetTransform;
    List<Transform> children = new List<Transform>();
    List<float> lfs = new List<float>();
    List<Vector3> dvs = new List<Vector3>(); 
    public Vector3 maxDeviation = new Vector3(10,10,0);
    public float initialEnergy = 0;
    public float staticEnergy = 0;
    public float kineticEnergy = 1;
    float energy = 0;

    void OnEnable()
    {  
        energy = initialEnergy; 
        thisTransform = transform;
        lastPos = thisTransform.position;
        OnTransformChildrenChanged();
    }

    void OnTransformChildrenChanged()
    {
        Transform tmp = null;
        for (int i = 0; i < thisTransform.childCount; i++)
        {
            tmp = thisTransform.GetChild(i);
            if (!children.Contains(tmp))
            {
                children.Add(tmp);
                lfs.Add(Random.Range(.03f, .3f));
                dvs.Add(new Vector3(Random.Range(-maxDeviation.x, maxDeviation.x), Random.Range(-maxDeviation.y, maxDeviation.y), Random.Range(-maxDeviation.z, maxDeviation.z))); 
            }
        }
    }

    Vector3 vari; 
    Vector3 lastPos;
    void Update()
    { 
        energy += (targetTransform.position - lastPos).magnitude * kineticEnergy;
        //energy = Mathf.Clamp(energy -Time.deltaTime*5, 0, energy);
        energy = Mathf.Lerp(energy, staticEnergy, .1f);
        lastPos = targetTransform.position;
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] == null) continue;
            vari = dvs[i] * Time.deltaTime * Mathf.Clamp(energy, 0, energy);
            children[i].position = Vector3.Lerp(children[i].position, targetTransform.position + vari, lfs[i]);
            if ((children[i].position - targetTransform.position).magnitude < .1f) children[i].gameObject.SetActive(false);
            else children[i].gameObject.SetActive(true);
        }
        energy -= Time.deltaTime;
    }
}
