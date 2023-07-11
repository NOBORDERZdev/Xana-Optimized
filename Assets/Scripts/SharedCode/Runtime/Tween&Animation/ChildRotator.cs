using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildRotator : MonoBehaviour
{
    public Transform thisTransform;
    List<Transform> children = new List<Transform>();
    List<Vector3> rots = new List<Vector3>();
    public float rotationSpeed = 10;

    void OnEnable()
    {
        thisTransform = transform;
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
                rots.Add(new Vector3(Random.Range(-1.01f, 1.01f), Random.Range(-1.01f, 1.01f), Random.Range(-1.01f, 1.01f)) * rotationSpeed);
            }
        } 
    }

    void Update()
    {
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] == null) continue;
            children[i].Rotate(rots[i]); 
        }
    } 
}
