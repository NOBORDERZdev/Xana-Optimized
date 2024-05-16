using Photon.Pun;
using PhysicsCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UGCItemsData;

public class ExtraBuilderWorldSpwanner : MonoBehaviour
{
    [SerializeField] List<ExtraObj> extraObjList;
    private IEnumerator Start()
    { 
        yield return new WaitForSeconds(10);
        if (PhotonNetwork.IsMasterClient)
        {
            if (ConstantsHolder.xanaConstants.isXanaPartyWorld)
            {
                foreach (var extraObj in extraObjList)
                {
                    var multiplayerObject = PhotonNetwork.InstantiateRoomObject(extraObj.obj, extraObj.pos, Quaternion.Euler(extraObj.rot));
                    //var obj = Instantiate(extraObj.obj/*, extraObj.pos, Quaternion.Euler(extraObj.rot)*/);
                    //obj.transform.SetParent(transform);
                }
            }
        }
    }

}
[Serializable]
public class ExtraObj{ 
    public string obj;
    public Vector3 pos;
    public Vector3 rot;
}
