using UnityEngine;

public class ApplyShaderByName : MonoBehaviour
{
    public Shader _shader;
    public bool isMeshRendrer;
    public string shaderName;
    // Start is called before the first frame update
    void OnEnable()
    {
        if(isMeshRendrer)
        {
            gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find(shaderName);
        }
        else
        {
            gameObject.GetComponent<ParticleSystemRenderer>().material.shader = Shader.Find(shaderName);
        }
    }
}
