using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class SMBCNPC : MonoBehaviour
{
    public string PlanetName;
    string _npcMsg;

    private void Start()
    {
        Debug.Log("NPC function calling");
        SMBCManager.Instance?.InitNPCText(this);
    }

    public void Init(List<string> npcText)
    {
        foreach (var item in npcText)
        {
            _npcMsg += "- " + TextLocalization.GetLocaliseTextByKey(item) + "\n";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.OnNarrationCollisionEnter?.Invoke(_npcMsg, false, false);
        }
    }
}
