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
            gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find(shaderName);
        }
        else if (isTerrain)
        {
            gameObject.GetComponent<Terrain>().materialTemplate.shader = Shader.Find(shaderName);
        }
        else if (_IsSpriteRenderer)
        {
            gameObject.GetComponent<SpriteRenderer>().material.shader = Shader.Find(shaderName);
        }
        else
        {
            gameObject.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find(shaderName);
        }
    }
}
