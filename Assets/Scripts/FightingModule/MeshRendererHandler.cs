using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererHandler : MonoBehaviour
{
    [SerializeField] MeshRenderer parentMeshRenderer;
    [SerializeField] MeshRenderer netMeshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        if (!parentMeshRenderer)
        {
            parentMeshRenderer = GetComponent<MeshRenderer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("camHandler"))
        {
            parentMeshRenderer.enabled = false;
            netMeshRenderer.enabled = false;
            //if (this.transform.childCount > 0) {
            //    for (int i = 0; i < transform.childCount; i++) {
            //        transform.GetChild(i).gameObject.SetActive(false);
            //    }
            //}
        }
    } 
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("camHandler"))
        {
            parentMeshRenderer.enabled = true;
            netMeshRenderer.enabled = true;
            //if (this.transform.childCount > 0)
            //{
            //    for (int i = 0; i < transform.childCount; i++)
            //    {
            //        transform.GetChild(i).gameObject.SetActive(true);
            //    }
            //}
        }
    }
}
