using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJObjectPooler : MonoBehaviour
{
    public GameObject frame;
    public GameObject spotLight;
    public int initialPoolSize = 10;
    public List<GameObject> pooledObjectsforFrame;
    public List<GameObject> pooledObjectsforSpotLight;

    private void Start()
    {
        pooledObjectsforFrame = new List<GameObject>();

        for (int i = 0; i < initialPoolSize; i++)    //for frame
        {
            GameObject obj = Instantiate(frame);
            obj.SetActive(false);
            pooledObjectsforFrame.Add(obj);
        }
        for (int i = 0; i < initialPoolSize; i++)    // for spotlight
        {
            GameObject obj = Instantiate(spotLight);
            obj.SetActive(false);
            pooledObjectsforSpotLight.Add(obj);
        }
    }

    public GameObject GetPooledObjectFrame()
    {
        for (int i = 0; i < pooledObjectsforFrame.Count; i++)    
        {
            if (!pooledObjectsforFrame[i].activeInHierarchy)
            {
                return pooledObjectsforFrame[i];
            }
        }

        // If we haven't found an inactive object, create a new one and add it to the pool
        GameObject newObj = Instantiate(frame);
        newObj.SetActive(false);
        pooledObjectsforFrame.Add(newObj);
        return newObj;
    }
    public GameObject GetPooledObjectSpotLight()
    {
        for (int i = 0; i < pooledObjectsforSpotLight.Count; i++)    
        {
            if (!pooledObjectsforSpotLight[i].activeInHierarchy)
            {
                return pooledObjectsforSpotLight[i];
            }
        }

        GameObject newObj = Instantiate(spotLight);
        newObj.SetActive(false);
        pooledObjectsforSpotLight.Add(newObj);
        return newObj;
    }

}
