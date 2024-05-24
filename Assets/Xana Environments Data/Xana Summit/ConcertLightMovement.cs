using UnityEngine;

public class ConcertLightMovement : MonoBehaviour
{
    public GameObject[] gameObjects; // Array of game objects to move
    public float rotationSpeed = 1.0f; // Rotation speed
    public float minAngleY = -90.0f; // Minimum angle for rotation around Y-axis (horizontal)
    public float maxAngleY = 90.0f; // Maximum angle for rotation around Y-axis (horizontal)
    public float minAngleX = -45.0f; // Minimum angle for rotation around X-axis (vertical)
    public float maxAngleX = 45.0f; // Maximum angle for rotation around X-axis (vertical)
    public float changeInterval = 2.0f; // Time interval in seconds for changing direction

    private float timer = 0.0f;
    private Vector3[] targetRotations;

    void Start()
    {
        // Initialize target rotations for each game object
        targetRotations = new Vector3[gameObjects.Length];
        for (int i = 0; i < gameObjects.Length; i++)
        {
            targetRotations[i] = GetRandomRotation();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeInterval)
        {
            timer = 0.0f;
            for (int i = 0; i < gameObjects.Length; i++)
            {
                targetRotations[i] = GetRandomRotation();
            }
        }

        for (int i = 0; i < gameObjects.Length; i++)
        {
            RotateGameObject(gameObjects[i], targetRotations[i]);
        }
    }

    Vector3 GetRandomRotation()
    {
        float randomAngleY = Random.Range(minAngleY, maxAngleY);
        float randomAngleX = Random.Range(minAngleX, maxAngleX);
        return new Vector3(randomAngleX, randomAngleY, 0); // Rotation around the X and Y axes
    }

    void RotateGameObject(GameObject gameObject, Vector3 targetRotation)
    {
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
        gameObject.transform.localRotation = Quaternion.RotateTowards(
            gameObject.transform.localRotation,
            targetQuaternion,
            rotationSpeed * Time.deltaTime
        );
    }
}
