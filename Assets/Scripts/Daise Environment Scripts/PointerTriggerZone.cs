using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PointerTriggerZone : MonoBehaviour
{
    [SerializeField]
    private Transform PointerUI;
    [SerializeField]
    private Transform InGameUI;

    private Dictionary<string, GameObject> pointerUIElements;
    private Dictionary<string, GameObject> inGameUIElements;

    private void Awake() {
        pointerUIElements = new Dictionary<string, GameObject>();
        inGameUIElements = new Dictionary<string, GameObject>();

        foreach (Transform child in PointerUI) {
            pointerUIElements.Add(child.name, child.gameObject);
        }

        foreach (Transform child in InGameUI) {
            inGameUIElements.Add(child.name, child.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PhotonLocalPlayer") && other.GetComponent<PhotonView>().IsMine)
        {
            
            if (pointerUIElements.ContainsKey(name))            
            {
                if (!pointerUIElements[name].activeSelf)
                {
                    pointerUIElements[name].SetActive(true);
                }
            }
            else
            {
                if (inGameUIElements.ContainsKey(name))
                {
                    if (!inGameUIElements[name].activeSelf)
                    {
                        inGameUIElements[name].SetActive(true);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PhotonLocalPlayer") && other.GetComponent<PhotonView>().IsMine)
        {
            if (pointerUIElements.ContainsKey(name))
            {
                if (pointerUIElements[name].activeSelf)
                {
                    pointerUIElements[name].SetActive(false);
                }
            }
            else
            {
                if (inGameUIElements.ContainsKey(name))
                {
                    if (inGameUIElements[name].activeSelf)
                    {
                        inGameUIElements[name].SetActive(false);
                    }
                }
            }
        }
    }
}
