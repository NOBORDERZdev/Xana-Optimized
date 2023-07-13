using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class WarpFunctionComponent : ItemComponent
{
    private WarpFunctionComponentData warpFunctionComponentData;
    public static bool isPortalUsed;
    CharacterController characterControllerNew;

    public void Init(WarpFunctionComponentData warpFunctionComponentData)
    {
        this.warpFunctionComponentData = warpFunctionComponentData;
    }

    private void Start()
    {
        isPortalUsed = false;

        if (warpFunctionComponentData.isWarpPortalEnd)
        {
            PortalSystemEndPoint endpointToUpdate =
                warpFunctionComponentData.warpPortalDataEndPoint.Find(endpoint => endpoint.indexPortalEndKey == warpFunctionComponentData.warpPortalEndKeyValue);

            endpointToUpdate.portalEndLocation = new Vector3(transform.position.x, endpointToUpdate.portalEndLocation.y, transform.position.z);

        }
        else if (warpFunctionComponentData.isWarpPortalStart)
        {
            PortalSystemStartPoint startPointToUpdate =
                warpFunctionComponentData.warpPortalDataStartPoint.Find(startKeypoint => startKeypoint.indexPortalStartKey == warpFunctionComponentData.warpPortalStartKeyValue);

            startPointToUpdate.portalStartLocation = new Vector3(transform.position.x, startPointToUpdate.portalStartLocation.y, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision _other)
    {

        if (_other.gameObject.CompareTag("Player") || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            characterControllerNew = ReferrencesForDynamicMuseum.instance.MainPlayerParent.GetComponent<CharacterController>();

            if (warpFunctionComponentData.isWarpPortalStart && !isPortalUsed)
            {
                isPortalUsed = true;
                for (int i = 0; i < warpFunctionComponentData.warpPortalDataEndPoint.Count; i++)
                {
                    if ((warpFunctionComponentData.warpPortalStartKeyValue == warpFunctionComponentData.warpPortalDataEndPoint[i].indexPortalEndKey) && warpFunctionComponentData.warpPortalStartKeyValue != "Select Key")
                    {
                        characterControllerNew.enabled = false;
                        GamificationComponentData.instance.buildingDetect.CameraEffect();
                        GamificationComponentData.instance.playerControllerNew.transform.localPosition = warpFunctionComponentData.warpPortalDataEndPoint[i].portalEndLocation;
                        //Debug.LogError(warpFunctionComponentData.warpPortalDataEndPoint[i].portalEndLocation);
                        characterControllerNew.enabled = true;

                        Debug.Log("Start portal");
                    }
                }
            }


            else if (warpFunctionComponentData.isWarpPortalEnd && !isPortalUsed)
            {
                isPortalUsed = true;
                for (int i = 0; i < warpFunctionComponentData.warpPortalDataStartPoint.Count; i++)
                {
                    if ((warpFunctionComponentData.warpPortalEndKeyValue == warpFunctionComponentData.warpPortalDataStartPoint[i].indexPortalStartKey) && warpFunctionComponentData.warpPortalEndKeyValue != "Select Key")
                    {
                        characterControllerNew.enabled = false;
                        GamificationComponentData.instance.buildingDetect.CameraEffect();
                        GamificationComponentData.instance.playerControllerNew.transform.localPosition = warpFunctionComponentData.warpPortalDataStartPoint[i].portalStartLocation;
                        characterControllerNew.enabled = true;
                        Debug.Log(" End portal");

                    }
                }

            }

            StartCoroutine(PositionUpdating());
        }
    }

    IEnumerator PositionUpdating()
    {
        yield return new WaitForSeconds(4);

        isPortalUsed = false;

    }

}