using UnityEngine;
using Photon.Pun;
public class CharacterLights : MonoBehaviour
{
    public GameObject characterLights;
    // Start is called before the first frame update
    void Start()
    {
        if(!GetComponentInParent<PhotonView>().IsMine)
        {
            characterLights.SetActive(false);
        }
    }
}
