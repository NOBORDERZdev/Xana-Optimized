using System;
using UnityEngine;

public class SummitEventTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BuilderEventManager.LoadSummitScene?.Invoke();
        

        //added to resolve pink sky issue
        try
        {
            RenderSettings.skybox.shader = Shader.Find("Skybox/Cubemap");
        }
        catch(Exception e)
        {

        }
    }
}
