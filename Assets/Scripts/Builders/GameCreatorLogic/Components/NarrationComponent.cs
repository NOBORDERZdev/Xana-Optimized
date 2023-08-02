using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

public class NarrationComponent : ItemComponent
{
    [SerializeField]
    private NarrationComponentData narrationComponentData;
    public static IEnumerator currentCoroutine;

    public bool isCoroutineRunning = false;
    int i = 0;

    public void Init(NarrationComponentData narrationComponentData)
    {
        this.narrationComponentData = narrationComponentData;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            string msg = "";
            if (narrationComponentData.narrationsData.Length == 0)
                msg = "Narration data is empty";
            else
                msg = narrationComponentData.narrationsData;

            if (narrationComponentData.onStoryNarration)
            {
                BuilderEventManager.OnNarrationCollisionEnter?.Invoke(msg, true);
            }
            else if (narrationComponentData.onTriggerNarration)
            {
                BuilderEventManager.OnNarrationCollisionEnter?.Invoke(msg, false);
            }
        }
    }
}