using UnityEngine;

public class FindMeshRadius : MonoBehaviour
{
    //private void Start()
    //{
    //    // Get the MeshFilter component from the GameObject
    //    MeshFilter meshFilter = GetComponent<MeshFilter>();

    //    if (meshFilter != null)
    //    {
    //        // Get the mesh from the MeshFilter
    //        Mesh mesh = meshFilter.sharedMesh;

    //        // Calculate the radius
    //        float radius = CalculateMeshRadius(mesh);

    //        Debug.Log("Mesh Radius: " + radius);
    //    }
    //    else
    //    {
    //        Debug.LogError("MeshFilter component not found!");
    //    }
    //}

    //private float CalculateMeshRadius(Mesh mesh)
    //{
    //    Vector3[] vertices = mesh.vertices;
    //    Vector3 center = transform.position;
    //    float maxDistanceSqr = 0.0f;

    //    foreach (Vector3 vertex in vertices)
    //    {
    //        float distanceSqr = (vertex + center).sqrMagnitude;
    //        maxDistanceSqr = Mathf.Max(maxDistanceSqr, distanceSqr);
    //    }

    //    return Mathf.Sqrt(maxDistanceSqr);
    //}
}
