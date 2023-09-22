using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererHandler : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        if (!meshRenderer)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("camHandler"))
        {
            print("Main Cam Triggered Enter");
            meshRenderer.enabled = false;
            if (this.transform.childCount > 0) {
                for (int i = 0; i < transform.childCount; i++) {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    } 
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("camHandler"))
        {
            print("Main Cam Triggered Exit");
            meshRenderer.enabled = true;
            if (this.transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }
}
