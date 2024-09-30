using UnityEngine;

public class ApplyShaderByName : MonoBehaviour
{
    public Shader _shader;
    public bool _IsSpriteRenderer;
    public bool isMeshRendrer;
    public string shaderName;
    public bool isTerrain=false;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (isMeshRendrer)
        {
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material.shader = Shader.Find(shaderName);
            }
        }
        else if (isTerrain)
        {
            var terrain = gameObject.GetComponent<Terrain>();
            if (terrain != null)
            {
                terrain.materialTemplate.shader = Shader.Find(shaderName);
            }
        }
        else if (_IsSpriteRenderer)
        {
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.material.shader = Shader.Find(shaderName);
            }
        }
        else
        {
            var particleSystemRenderer = gameObject.GetComponent<ParticleSystemRenderer>();
            if (particleSystemRenderer != null)
            {
                particleSystemRenderer.material.shader = Shader.Find(shaderName);
            }
        }
    }
}
