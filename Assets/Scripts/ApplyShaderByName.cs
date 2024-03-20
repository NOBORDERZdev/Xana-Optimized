using UnityEngine;

public class ApplyShaderByName : MonoBehaviour
{
    public Shader _shader;
    public bool isMeshRendrer;
    public string shaderName;
    public bool isTerrain = false;
    public bool isSpriteRenderer = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (isMeshRendrer)
        {
            gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find(shaderName);
        }
        else if (isTerrain)
        {
            gameObject.GetComponent<Terrain>().materialTemplate.shader = Shader.Find(shaderName);
        }
        else if (isSpriteRenderer)
            gameObject.GetComponent<SpriteRenderer>().material.shader = Shader.Find(shaderName);
        else
        {
            gameObject.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find(shaderName);
        }
    }
}
