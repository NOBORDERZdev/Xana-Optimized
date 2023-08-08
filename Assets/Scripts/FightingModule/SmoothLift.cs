/// <summary>
/// This Script is used to Set player position for VS Screen
/// </summary>

using UnityEngine;
using System.Collections;

public class SmoothLift : MonoBehaviour
{
    public Vector3 targetTransform; // The desired position to move the object towards
    public float duration = 1f; // The time it takes to reach the target position

    private Vector3 startPosition;
    private float elapsedTime = 0f;

    private void Start()
    {
        startPosition = transform.position;
        StartCoroutine(LiftObject());
    }

    private IEnumerator LiftObject()
    {
        elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the normalized time (0 to 1)
            float t = elapsedTime / duration;

            // Interpolate the position between start and target using Lerp
            transform.localPosition = Vector3.Lerp(startPosition, targetTransform, t);

            // Increase the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null; // Wait for the next frame
        }

        // Ensure the object reaches the exact target position
        transform.localPosition = targetTransform;
    }
}
