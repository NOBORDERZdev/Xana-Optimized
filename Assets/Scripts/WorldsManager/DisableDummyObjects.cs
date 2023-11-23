using UnityEngine;

public class DisableDummyObjects : MonoBehaviour
{
    public GameObject[] dummyObjects;
    private void OnEnable()
    {
        BuilderEventManager.AfterWorldInstantiated += DisableDummyObjectsAfterWorldInstantiated;
    }

    private void OnDisable()
    {
        BuilderEventManager.AfterWorldInstantiated -= DisableDummyObjectsAfterWorldInstantiated;
    }

    private void DisableDummyObjectsAfterWorldInstantiated()
    {
        foreach(GameObject gameObject in dummyObjects)
        {
            gameObject.SetActive(false);
        }
    }
}
