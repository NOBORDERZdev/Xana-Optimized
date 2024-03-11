using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRaycastBlocker : MonoBehaviour
{
    [System.Serializable]
    public class RayBlock
    {
        public string name;
        public GameObject actualObj;
        public GameObject[] rayBlockerObjects;
    }
    public RayBlock[] rayBlock;

    private void Awake()
    {
        for (int i=0; i< rayBlock.Length; i++) {
            rayBlock[i].actualObj.SetActive(false);
            for (int j=0; j < rayBlock[i].rayBlockerObjects.Length; j++)
                rayBlock[i].rayBlockerObjects[j].SetActive(false);
        }
    }

    //public void EnableSpecificObject(int num)
    //{
    //    Debug.LogError("BlockerInd: " + num);
    //    rayBlockerObjects[num].SetActive(true);
    //}

}
