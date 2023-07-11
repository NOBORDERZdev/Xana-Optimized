using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionCheck : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("PhotonLocalPlayer"))
        {
            gameObject.SetActive(false);
            return;
        }
        StartCoroutine(TurnOffPhysics(3.0f));
    }

    private IEnumerator TurnOffPhysics(float f)
    {
        yield return new WaitForSeconds(f);
        rb.isKinematic = true;
        GetComponent<MapAtlasUV>().MapAtlasUVs();
    }
}
