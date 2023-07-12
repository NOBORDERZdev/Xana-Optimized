using MD_Plugin;
using UnityEngine;

public class RefreshMeshCollider : MonoBehaviour
{
    void Start() => transform.GetComponent<MD_MeshColliderRefresher>().MeshCollider_UpdateMeshCollider();
}
