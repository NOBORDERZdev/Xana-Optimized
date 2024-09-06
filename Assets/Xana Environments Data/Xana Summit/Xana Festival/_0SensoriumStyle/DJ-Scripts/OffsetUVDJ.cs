using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OffsetUVDJ : MonoBehaviour
{
    public float offsetSpeed = 0.1f; // Speed of UV offset

    private Renderer _renderer;
    private Vector2 _currentOffset;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _currentOffset = _renderer.material.GetTextureOffset("_BaseMap");
    }

    void Update()
    {
        // Update the X value of the UV offset
        _currentOffset.y += offsetSpeed * Time.deltaTime;

        // Apply the updated offset to the material
        _renderer.material.SetTextureOffset("_BaseMap", _currentOffset);
    }
}
