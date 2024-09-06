using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    public float NearValue = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        BuilderEventManager.AfterPlayerInstantiated += UpdateCamNear;
        
    }
    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= UpdateCamNear;
    }

    private void UpdateCamNear()
    {
        GameplayEntityLoader.instance.PlayerCamera.m_Lens.NearClipPlane = NearValue;
    }


}
