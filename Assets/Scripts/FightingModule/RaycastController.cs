using System.Collections.Generic;
using UnityEngine;

public class RaycastController : MonoBehaviour
{
    //public float maxDistance = 100f;
    //private GameObject lastHitRing;
    #region Single RayCast
    //private void Update()
    //{
    //    Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
    //    RaycastHit hit;

    //    Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.blue);

    //    if (Physics.Raycast(ray, out hit, maxDistance))
    //    {
    //        if (hit.collider.CompareTag("ring"))
    //        {
    //            print("Hitting Ring");
    //            if (lastHitRing != null)
    //            {
    //                lastHitRing.SetActive(true);
    //            }

    //            lastHitRing = hit.collider.gameObject;
    //            lastHitRing.SetActive(false);
    //        }
    //    }
    //}
    #endregion

    #region 3 Lines Raycast
    //private GameObject[] rings; // Store references to the ring objects
    //private List<GameObject> ringsToTurnOn = new List<GameObject>();

    //void Start()
    //{
    //    // Initialize the array with a size of 3
    //    rings = new GameObject[3];
    //}

    //void Update()
    //{
    //    CastRays();
    //}

    //void CastRays()
    //{
    //    // Cast rays from the camera at 0 degrees (center), 45 degrees left, and 45 degrees right
    //    for (int i = 0; i < 3; i++)
    //    {
    //        Vector3 rayDirection = Quaternion.Euler(0, i * 45, 0) * transform.forward;
    //        RaycastHit hit;

    //        if (Physics.Raycast(transform.position, rayDirection, out hit, maxDistance))
    //        {
    //            if (hit.collider.CompareTag("ring"))
    //            {
    //                // Store the reference to the ring object
    //                rings[i] = hit.collider.gameObject;

    //                // Turn off the ring object
    //                hit.collider.gameObject.SetActive(false);

    //                // Add it to the list of rings to turn on later
    //                ringsToTurnOn.Add(hit.collider.gameObject);
    //            }
    //        }
    //        else
    //        {
    //            // If the ray doesn't hit anything, reset the reference to null
    //            rings[i] = null;
    //         //   TurnOnStoredRings();
    //        }
    //        Debug.DrawRay(transform.position, rayDirection * maxDistance, Color.red);

    //    }
    //}

    //void TurnOnStoredRings()
    //{
    //    // Turn on any stored ring objects that are not being hit by the rays
    //    for (int i = 0; i < ringsToTurnOn.Count; i++)
    //    {
    //        if (ringsToTurnOn[i] != null && !IsRayHittingRing(ringsToTurnOn[i]))
    //        {
    //            ringsToTurnOn[i].SetActive(true);
    //            ringsToTurnOn.RemoveAt(i);
    //            i--;
    //        }
    //    }
    //}

    //bool IsRayHittingRing(GameObject ring)
    //{
    //    // Check if any of the stored rays hit the specified ring
    //    for (int i = 0; i < 3; i++)
    //    {
    //        if (rings[i] == ring)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
    #endregion

    #region SphereCast
    //public float radius;
    //public LayerMask layerMask;
    //RaycastHit hit;


    //void Update()
    //{
    //    Cast();
    //}

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position + transform.forward * maxDistance, radius);
    //}

    //void Cast()
    //{
    //    if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, maxDistance, layerMask))
    //    {
    //        //   Debug.Log(hit.collider.gameObject);
    //        hit.collider.gameObject.SetActive(false);
    //    }
    //    else {
    //        print("NAH");
    //    }
    //}
    #endregion
}
