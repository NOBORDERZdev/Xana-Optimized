using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MaterialClonner : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer mesh;
    private void Awake()
    {
        //CloneMaterial();
    }

    void CloneMaterial() {
         Material materialTemp = Instantiate(mesh.sharedMaterials[0]);
        if (GetComponentInParent<PhotonView>())
        {
            string playerID = GetComponentInParent<PhotonView>().ViewID.ToString();
            materialTemp.name = mesh.sharedMaterials[0].name + playerID;
            mesh.material = materialTemp;
        }
        else {
            materialTemp.name = mesh.sharedMaterials[0].name + "new";
            mesh.material = materialTemp;
        }
    }
}
