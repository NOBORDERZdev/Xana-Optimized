using UnityEngine;

public class DisableDummyObjects : MonoBehaviour
{
    public GameObject[] dummyObjects;
    private void OnEnable()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated += DisableDummyObjectsAfterWorldInstantiated;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterWorldOffcialWorldsInatantiated -= DisableDummyObjectsAfterWorldInstantiated;
    }

    private void DisableDummyObjectsAfterWorldInstantiated()
    {
        foreach(GameObject gameObject in dummyObjects)
        {
            gameObject.SetActive(false);
        }
    }
}
