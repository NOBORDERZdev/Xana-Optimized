using System.Collections;
using UnityEngine;
using Models;
using Photon.Pun;

public class WarpFunctionComponent : ItemComponent
{
    internal WarpFunctionComponentData warpFunctionComponentData;
    public static bool isPortalUsed;
    CharacterController characterControllerNew;

    public void Init(WarpFunctionComponentData warpFunctionComponentData)
    {
        this.warpFunctionComponentData = warpFunctionComponentData;

        GamificationComponentData.instance.warpComponentList.Add(this);
    }

    private void Start()
    {
        isPortalUsed = false;
    }

    private void OnCollisionEnter(Collision _other)
    {

        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            characterControllerNew = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<CharacterController>();


            if (warpFunctionComponentData.isWarpPortalStart && !isPortalUsed)
            {
                StartCoroutine(PositionUpdating());
                ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.PortalSound);
                isPortalUsed = true;
                for (int i = 0; i < warpFunctionComponentData.warpPortalDataEndPoint.Count; i++)
                {
                    if ((warpFunctionComponentData.warpPortalStartKeyValue == warpFunctionComponentData.warpPortalDataEndPoint[i].indexPortalEndKey) && warpFunctionComponentData.warpPortalStartKeyValue != "Select Key")
                    {
                        characterControllerNew.enabled = false;
                        GamificationComponentData.instance.buildingDetect.CameraEffect();
                        GamificationComponentData.instance.playerControllerNew.transform.localPosition = warpFunctionComponentData.warpPortalDataEndPoint[i].portalEndLocation;
                        characterControllerNew.enabled = true;
                    }
                }
            }
            else if (warpFunctionComponentData.isWarpPortalEnd && warpFunctionComponentData.isReversible && !isPortalUsed)
            {
                ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.PortalSound);
                StartCoroutine(PositionUpdating());
                isPortalUsed = true;
                for (int i = 0; i < warpFunctionComponentData.warpPortalDataStartPoint.Count; i++)
                {
                    if ((warpFunctionComponentData.warpPortalEndKeyValue == warpFunctionComponentData.warpPortalDataStartPoint[i].indexPortalStartKey) && warpFunctionComponentData.warpPortalEndKeyValue != "Select Key")
                    {
                        characterControllerNew.enabled = false;
                        GamificationComponentData.instance.buildingDetect.CameraEffect();
                        GamificationComponentData.instance.playerControllerNew.transform.localPosition = warpFunctionComponentData.warpPortalDataStartPoint[i].portalStartLocation;
                        characterControllerNew.enabled = true;
                    }
                }
            }
        }
    }

    IEnumerator PositionUpdating()
    {

        yield return new WaitForSeconds(2f);
        isPortalUsed = false;
    }

    #region BehaviourControl
    private void StartComponent()
    {

    }
    private void StopComponent()
    {


    }

    public override void StopBehaviour()
    {
        if (isPlaying)
        {
            isPlaying = false;
            StopComponent();
        }
    }

    public override void PlayBehaviour()
    {
        isPlaying = true;
        StartComponent();
    }

    public override void ToggleBehaviour()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
            PlayBehaviour();
        else
            StopBehaviour();
    }
    public override void ResumeBehaviour()
    {
        PlayBehaviour();
    }

    public override void AssignItemComponentType()
    {
        _componentType = Constants.ItemComponentType.WarpFunctionComponent;
    }

    public override void CollisionExitBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //CollisionEnter();
    }

    #endregion
}