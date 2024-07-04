using System;
using UnityEngine;

public class ApplySkyBoxShader : MonoBehaviour
{
    public string skyBoxShader;
    // Start is called before the first frame update
    void Start()
    {
        //added to resolve pink sky issue
        try
        {
            RenderSettings.skybox.shader = Shader.Find(skyBoxShader);
        }
        catch (Exception e)
        {

        }
    }
}
