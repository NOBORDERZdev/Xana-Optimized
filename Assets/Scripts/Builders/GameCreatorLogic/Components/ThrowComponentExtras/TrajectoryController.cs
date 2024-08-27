using System.Net;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryController : MonoBehaviour
{
    public int resolution = 10;
    public int distance = 2;
    private LineRenderer lineRenderer;
    private Vector3[] points;
    public GameObject aimCollsion;
    public GameObject colliderAim;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution;
        points = new Vector3[resolution];
    }

    public void UpdateTrajectory(Vector3 startPosition, Vector3 initialVelocity)
    {
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            t = t * distance;
            Vector3 point = CalculatePointInTrajectory(startPosition, initialVelocity, t);
            points[i] = point;
        }

        bool hitDetected = false; // if any collision detected

        for (int i = 0; i < resolution - 1; i++)
        {
            if (i < resolution - 1)
            {
                RaycastHit hitInfo;
                if (Physics.Linecast(points[i], points[i + 1], out hitInfo))
                {
                    if (colliderAim == null)
                        colliderAim = Instantiate(aimCollsion);
                    if (hitInfo.collider.CompareTag("Item") || hitInfo.collider.CompareTag("Footsteps/grass") || hitInfo.collider.CompareTag("Footsteps/floor"))//"Ground"
                    {
                        colliderAim.transform.position = hitInfo.point;
                        lineRenderer.positionCount = i;
                        hitDetected = true;
                        break;
                    }
                }
            }
        }

        // If no collision detected, Position setting here
        if (!hitDetected)
        {
            colliderAim.transform.position = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        }




        lineRenderer.SetPositions(points);
    }

    Vector3 CalculatePointInTrajectory(Vector3 startPosition, Vector3 initialVelocity, float t)
    {
        float x = startPosition.x + initialVelocity.x * t;
        float y = startPosition.y + initialVelocity.y * t - 0.5f * Physics.gravity.magnitude * t * t;
        float z = startPosition.z + initialVelocity.z * t;

        return new Vector3(x, y, z);
    }

    public void CheckCollision()
    {
        if (colliderAim == null)
            colliderAim = Instantiate(aimCollsion);
        colliderAim.transform.position = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
    }
}