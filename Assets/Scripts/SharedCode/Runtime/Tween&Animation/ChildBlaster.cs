using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildBlaster : MonoBehaviour
{
    public Transform thisTransform;
    List<Transform> children = new List<Transform>();
    List<float> energy = new List<float>();
    List<Vector3> dirs = new List<Vector3>();
    List<bool> ress = new List<bool>();
    public float minEnergy = .1f;
    public float maxEnergy = 1f; 
    public Vector2 xRange;
    public Vector2 yRange;
    public Vector2 zRange;

    void OnEnable()
    {
        thisTransform = transform;
        OnTransformChildrenChanged();
    }

    public void DuplicateFirstChild()
    {
        Instantiate(thisTransform.GetChild(0).gameObject, thisTransform);
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
                energy.Add(maxEnergy);
                ress.Add(true);
                dirs.Add(new Vector3(Random.Range(xRange.x, xRange.y), Random.Range(yRange.x, yRange.y), Random.Range(zRange.x, zRange.y)));
            }
        } 
    }

    void Update()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] == null) continue;
            if (ress[i])
            {
                children[i].localPosition = Vector3.zero;
                ress[i] = false;
            }
            children[i].localPosition += (dirs[i] * energy[i]); 
            energy[i] = Mathf.Lerp(energy[i], minEnergy, 0.1f);
        }
    }

    float randomUnit
    {
        get
        {
            return Random.Range(-1.01f, 1.01f);
        }
    }
}
