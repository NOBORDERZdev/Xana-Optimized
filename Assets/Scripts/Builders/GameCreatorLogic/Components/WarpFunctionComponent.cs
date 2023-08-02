using System.Collections;
using UnityEngine;
using Models;
using Photon.Pun;

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
    }

    private void OnCollisionEnter(Collision _other)
    {

        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            characterControllerNew = ReferrencesForDynamicMuseum.instance.MainPlayerParent.GetComponent<CharacterController>();

            if (warpFunctionComponentData.isWarpPortalStart && !isPortalUsed)
            {
                StartCoroutine(PositionUpdating());
                isPortalUsed = true;
                for (int i = 0; i < warpFunctionComponentData.warpPortalDataEndPoint.Count; i++)
                {
                    if ((warpFunctionComponentData.warpPortalStartKeyValue == warpFunctionComponentData.warpPortalDataEndPoint[i].indexPortalEndKey) && warpFunctionComponentData.warpPortalStartKeyValue != "Select Key")
                    {
                        characterControllerNew.enabled = false;
                        //GamificationComponentData.instance.buildingDetect.CameraEffect();
                        GamificationComponentData.instance.playerControllerNew.transform.localPosition = warpFunctionComponentData.warpPortalDataEndPoint[i].portalEndLocation;
                        characterControllerNew.enabled = true;
                    }
                }
            }
            else if (warpFunctionComponentData.isWarpPortalEnd && warpFunctionComponentData.isReversible && !isPortalUsed)
            {
                StartCoroutine(PositionUpdating());
                isPortalUsed = true;
                for (int i = 0; i < warpFunctionComponentData.warpPortalDataStartPoint.Count; i++)
                {
                    if ((warpFunctionComponentData.warpPortalEndKeyValue == warpFunctionComponentData.warpPortalDataStartPoint[i].indexPortalStartKey) && warpFunctionComponentData.warpPortalEndKeyValue != "Select Key")
                    {
                        characterControllerNew.enabled = false;
                        //GamificationComponentData.instance.buildingDetect.CameraEffect();
                        GamificationComponentData.instance.playerControllerNew.transform.localPosition = warpFunctionComponentData.warpPortalDataStartPoint[i].portalStartLocation;
                        characterControllerNew.enabled = true;
                    }
                }
            }
        }
    }

    IEnumerator PositionUpdating()
    {
        yield return new WaitForSeconds(2);
        isPortalUsed = false;
    }

}