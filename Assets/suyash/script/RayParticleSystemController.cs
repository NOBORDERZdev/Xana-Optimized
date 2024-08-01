using UnityEngine;

public class RayParticleSystemController : MonoBehaviour
{
    public GameObject startParticlePrefab; // Assign your start point particle prefab in the inspector
    public GameObject endParticlePrefab;   // Assign your end point particle prefab in the inspector

    private GameObject startParticleInstance;
    private GameObject endParticleInstance;
    private LineRenderer lineRenderer;     // Assuming you use a LineRenderer for the ray

    public float minRayLengthForParticles = 0.1f; // Minimum ray length for particles to be visible
    public float endParticleScale = 0.1f;         // Constant scale for the end particle

    private void Start()
    {
        // Instantiate the start and end particle systems
        startParticleInstance = Instantiate(startParticlePrefab);
        endParticleInstance = Instantiate(endParticlePrefab);

        // Get the LineRenderer component (or any other component you're using for the ray)
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        // Ensure LineRenderer is valid and has enough positions
        if (lineRenderer != null && lineRenderer.positionCount > 1)
        {
            // Update the position of the start particle system to be at the start of the ray
            Vector3 startPoint = lineRenderer.GetPosition(0);
            startParticleInstance.transform.position = startPoint;

            // Calculate the endpoint based on the LineRenderer positions
            Vector3 endPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
            endParticleInstance.transform.position = endPoint;
        }

        // Handle visibility and scaling of particles
        HandleVisibilityAndEffects();
    }

    private void HandleVisibilityAndEffects()
    {
        // Check if the ray is visible and long enough
        if (lineRenderer != null && lineRenderer.positionCount > 1)
        {
            bool isRayVisible = lineRenderer.enabled;
            float rayLength = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(lineRenderer.positionCount - 1));

            // Set particles active based on ray length
            startParticleInstance.SetActive(isRayVisible && rayLength > minRayLengthForParticles);
            endParticleInstance.SetActive(isRayVisible && rayLength > minRayLengthForParticles);

            // Set constant scale for end particle to maintain consistent size
            endParticleInstance.transform.localScale = Vector3.one * endParticleScale;
        }
    }

    private void OnDestroy()
    {
        // Cleanup particle system instances when this script is destroyed
        if (startParticleInstance != null)
        {
            Destroy(startParticleInstance);
        }

        if (endParticleInstance != null)
        {
            Destroy(endParticleInstance);
        }
    }
}
