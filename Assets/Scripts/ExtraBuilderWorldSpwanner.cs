using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraBuilderWorldSpwanner : MonoBehaviour
{
    [SerializeField] List<ExtraObj> extraObjList;
    private void Start()
    {
        if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
        {
            foreach (var extraObj in extraObjList)
            {
                var obj = Instantiate(extraObj.obj/*, extraObj.pos, Quaternion.Euler(extraObj.rot)*/);
                //obj.transform.SetParent(transform);
            }
        }
        

    }

}
[Serializable]
public class ExtraObj{ 
    public GameObject obj;
    public Vector3 pos;
    public Vector3 rot;
}
