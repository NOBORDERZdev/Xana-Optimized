using UnityEngine;

public class VolcanicWorldManager : MonoBehaviour
{
    [SerializeField] private GameObject _ground;
    private void OnEnable()
    {
        BuilderEventManager.OnSMBCQuizWrongAnswer += DisableGround;
    }
    private void OnDisable()
    {
        BuilderEventManager.OnSMBCQuizWrongAnswer -= DisableGround;
    }

    private void DisableGround()
    {
        _ground.SetActive(false);
    }
}
