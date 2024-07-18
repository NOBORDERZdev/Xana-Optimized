using UnityEngine;

public class TextureOffset125 : MonoBehaviour
{
    public Material material; // The URP Lit material
    public float interval = 0.5f; // Time interval for offset change, public so you can modify it

    private float nextChangeTime = 0f;
    private Vector2 currentOffset;
    private float textureWidth = 1000f; // The width of your texture
    private float jumpWidth = 125f; // The width of each jump

    void Start()
    {
        if (material == null)
        {
            Debug.LogError("Material not assigned.");
            return;
        }

        currentOffset = material.mainTextureOffset;
    }

    void Update()
    {
        if (Time.time >= nextChangeTime)
        {
            nextChangeTime = Time.time + interval;
            currentOffset.x += jumpWidth / textureWidth;

            // Wrap the offset around if it exceeds 1.0
            if (currentOffset.x >= 1.0f)
            {
                currentOffset.x -= 1.0f;
            }

            material.mainTextureOffset = currentOffset;
        }
    }
}
