using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class gizmoHelper : MonoBehaviour
{
    public float radius = 0.1f;
    GUIStyle style = new GUIStyle();
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, radius);
        style.normal.textColor = Color.red;
      //  Handles.Label(transform.position, transform.name, style);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
