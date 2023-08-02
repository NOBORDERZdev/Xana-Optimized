using UnityEngine;
using Models;
using Photon.Pun;

public class CollectibleComponent : ItemComponent
{

    private bool activateComponent = true;

    public void Init(CollectibleComponentData collectibleComponentData)
    {
        activateComponent = true;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            gameObject.SetActive(false);
            if (activateComponent)
            {
                activateComponent = false;
                //Toast.Show(XanaConstants.collectibleMsg);
            }
        }
    }
}