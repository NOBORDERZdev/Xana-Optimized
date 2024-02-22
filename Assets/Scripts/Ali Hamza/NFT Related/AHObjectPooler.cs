using Toyota;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AHObjectPooler : MonoBehaviour
{
    public GameObject frame1x1;
    public GameObject frame4x3;
    public GameObject frame16x9;
    public GameObject frame9x16;
    public GameObject frame;
    public GameObject spotLight;
    public int initialPoolSize = 10;
    public List<GameObject> pooledObjectsforFrame;
    public List<GameObject> pooledObjectsforSpotLight;

    private void Start()
    {
        pooledObjectsforFrame = new List<GameObject>();

        for (int i = 0; i < initialPoolSize; i++)    //for frame 1x1
        {
            GameObject obj = Instantiate(frame1x1);
            obj.SetActive(false);
            pooledObjectsforFrame.Add(obj);
        }
        for (int i = 0; i < initialPoolSize; i++)    //for frame 4x3
        {
            GameObject obj = Instantiate(frame4x3);
            obj.SetActive(false);
            pooledObjectsforFrame.Add(obj);
        }
        for (int i = 0; i < initialPoolSize; i++)    //for frame 16x9
        {
            GameObject obj = Instantiate(frame16x9);
            obj.SetActive(false);
            pooledObjectsforFrame.Add(obj);
        }
        for (int i = 0; i < initialPoolSize; i++)    //for frame 9x16
        {
            GameObject obj = Instantiate(frame9x16);
            obj.SetActive(false);
            pooledObjectsforFrame.Add(obj);
        }
    }

    public GameObject GetPooledObjectFrame(PMY_Ratio _imgVideoRatio)
    {
        for (int i = 0; i < pooledObjectsforFrame.Count; i++)
        {
            if (!pooledObjectsforFrame[i].activeInHierarchy)
            {
                if (_imgVideoRatio == PMY_Ratio.OneXOneWithoutDes || _imgVideoRatio == PMY_Ratio.OneXOneWithDes)
                {
                    if (pooledObjectsforFrame[i].CompareTag("Frame1x1"))
                    {
                        return pooledObjectsforFrame[i];
                    }
                }
                else if (_imgVideoRatio == PMY_Ratio.FourXThreeWithoutDes || _imgVideoRatio == PMY_Ratio.FourXThreeWithDes)
                {
                    if (pooledObjectsforFrame[i].CompareTag("Frame4x3"))
                    {
                        return pooledObjectsforFrame[i];
                    }
                }
                else if (_imgVideoRatio == PMY_Ratio.SixteenXNineWithoutDes || _imgVideoRatio == PMY_Ratio.SixteenXNineWithDes)
                {
                    if (pooledObjectsforFrame[i].CompareTag("Frame16x9"))
                    {
                        return pooledObjectsforFrame[i];
                    }
                }
                else if (_imgVideoRatio == PMY_Ratio.NineXSixteenWithoutDes || _imgVideoRatio == PMY_Ratio.NineXSixteenWithDes)
                {
                    if (pooledObjectsforFrame[i].CompareTag("Frame9x16"))
                    {
                        return pooledObjectsforFrame[i];
                    }
                }
            }
        }


        // If we haven't found an inactive object, create a new one and add it to the pool
        if(_imgVideoRatio == PMY_Ratio.OneXOneWithoutDes || _imgVideoRatio == PMY_Ratio.OneXOneWithDes)
            frame= frame1x1;
        else if(_imgVideoRatio == PMY_Ratio.FourXThreeWithoutDes || _imgVideoRatio == PMY_Ratio.FourXThreeWithDes)
            frame = frame4x3;
        else if (_imgVideoRatio == PMY_Ratio.SixteenXNineWithoutDes || _imgVideoRatio == PMY_Ratio.SixteenXNineWithDes)
            frame = frame16x9;
        else if (_imgVideoRatio == PMY_Ratio.NineXSixteenWithoutDes || _imgVideoRatio == PMY_Ratio.NineXSixteenWithDes)
            frame = frame9x16;

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
