using System.Collections;
using UnityEngine;
using Models;
using Photon.Pun;

public class BlindfoldedDisplayComponent : ItemComponent
{
    BlindfoldedDisplayComponentData blindfoldedDisplayComponentData;
    public static bool footstepsBool = false;
    [SerializeField] GameObject raycast;

    SkinnedMeshRenderer[] skinMesh;
    Collider[] childCollider;
    MeshRenderer[] childMesh;
    string RuntimeItemID = "";

    bool notTriggerOther = false;
    RingbufferFootSteps rr;

    GameObject invisibleAvatar;
    GameObject footPrintAvatar;

    public void Init(BlindfoldedDisplayComponentData blindfoldedDisplayComponentData)
    {
        this.blindfoldedDisplayComponentData = blindfoldedDisplayComponentData;
        RuntimeItemID = this.GetComponent<XanaItem>().itemData.RuntimeItemID;
    }

    private void Start()
    {
        Physics.IgnoreLayerCollision(9, 22, false);

        childCollider = transform.GetComponentsInChildren<Collider>();
        childMesh = transform.GetComponentsInChildren<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (GamificationComponentData.instance.withMultiplayer)
            {
                GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.Others, RuntimeItemID, Constants.ItemComponentType.none);
            }

            ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.Invisible);
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
            GamificationComponentData.instance.activeComponent = this;
        }
    }

    private void SetFootPrinting()
    {
        GamificationComponentData.instance.isBlindfoldedFootPrinting = true;
        Transform shoes = GamificationComponentData.instance.buildingDetect.playerShoes.transform;
        //shoes.localPosition = Vector3.forward * 0.761f;
        Rigidbody rb = null;
        shoes.TryGetComponent(out rb);
        if (rb == null)
            rb = shoes.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        //if (shoes.transform.childCount == 0)
        //{
        footPrintAvatar = PhotonNetwork.Instantiate("Footprint", Vector3.zero, Quaternion.identity);
        footPrintAvatar.transform.SetParent(shoes);
        footPrintAvatar.transform.localPosition = Vector3.up * 0.0207f;
        footPrintAvatar.transform.localEulerAngles = Vector3.zero;
        //}

        skinMesh = GamificationComponentData.instance.playerControllerNew.GetComponentsInChildren<SkinnedMeshRenderer>();

        //other.gameObject.GetComponent<PlayerController>().isThrow = false;
        footstepsBool = true;
        for (int i = 0; i < childCollider.Length; i++)
        {
            childCollider[i].enabled = false;
        }


        for (int i = 0; i < childMesh.Length; i++)
        {
            childMesh[i].enabled = false;
        }

        for (int i = 0; i < skinMesh.Length; i++)
        {
            skinMesh[i].enabled = false;
        }

        GamificationComponentData.instance.buildingDetect.playerFreeCamConsole.enabled = false;
        GamificationComponentData.instance.buildingDetect.playerFreeCamConsoleOther.enabled = false;

        RingbufferFootSteps ringbufferFootStep = shoes.gameObject.GetComponentInChildren<RingbufferFootSteps>();
        //for (int i = 0; i < ringbufferFootSteps.Length; i++)
        //{
        ringbufferFootStep.enabled = true;
        ringbufferFootStep.transform.GetChild(0).gameObject.SetActive(true);
        //}
        //Debug.Log("BlindFolded Value : " + blindfoldedDisplayComponentData.blindfoldSliderValue);
        StartCoroutine(BackToVisible(ringbufferFootStep));
    }

    private void SetAvatarInvisibility()
    {
        for (int i = 0; i < childCollider.Length; i++)
        {
            childCollider[i].enabled = false;
        }

        for (int i = 0; i < childMesh.Length; i++)
        {
            childMesh[i].enabled = false;
        }
        StartCoroutine(BackToAvatarVisiblityHologram());
    }

    IEnumerator BackToVisible(RingbufferFootSteps rr)
    {
        this.rr = rr;
        yield return new WaitForSeconds(blindfoldedDisplayComponentData.blindfoldSliderValue);
        DeactivateAvatarInivisibility();
    }

    IEnumerator BackToAvatarVisiblityHologram()
    {
        //yield return new WaitForEndOfFrame();
        BuilderEventManager.ActivateAvatarInivisibility?.Invoke();
        invisibleAvatar = PhotonNetwork.Instantiate("InvisibleAvatar", Vector3.zero, Quaternion.identity);
        yield return new WaitForSeconds(blindfoldedDisplayComponentData.blindfoldSliderValue);
        DeactivateAvatarInivisibility();
    }

    private void DeactivateAvatarInivisibility()
    {
        BuilderEventManager.OnAvatarInvisibilityComponentCollisionEnter?.Invoke(0);
        BuilderEventManager.DeactivateAvatarInivisibility?.Invoke();

        if (invisibleAvatar)
            PhotonNetwork.Destroy(invisibleAvatar.GetPhotonView());
        if (footPrintAvatar)
            PhotonNetwork.Destroy(footPrintAvatar.GetPhotonView());

        notTriggerOther = false;
        //RaycastHit hit;
        //if (Physics.Raycast(raycast.transform.position + new Vector3(0, 100, 0), -raycast.transform.up, out hit, 200))
        //{
        //    if (hit.collider.CompareTag("Item"))
        //    {
        //        //Debug.Log("Not Null");
        //        BuilderEventManager.ReSpawnPlayer?.Invoke();
        //        notTriggerOther = true;

        //        Toast.Show("The avatar is now locked inside the object due to the avatar invisibility effect, so it will restart from the current point.");
        //    }
        //}

        if (blindfoldedDisplayComponentData.footprintPaintAvatar)
        {
            footstepsBool = false;

            for (int i = 0; i < skinMesh.Length; i++)
            {
                skinMesh[i].enabled = true;
            }


            GamificationComponentData.instance.buildingDetect.playerFreeCamConsole.enabled = true;
            GamificationComponentData.instance.buildingDetect.playerFreeCamConsoleOther.enabled = true;
            if (rr != null)
            {
                rr.enabled = false;
                rr.transform.GetChild(0).gameObject.SetActive(false);
            }
            GamificationComponentData.instance.isBlindfoldedFootPrinting = false;
        }

        GamificationComponentData.instance.activeComponent = null;
        //CanvasComponenetsManager._instance.avatarInvisiblityText.gameObject.SetActive(false);
        if (!notTriggerOther)
        {
            Physics.IgnoreLayerCollision(9, 22, false);
        }
        notTriggerOther = false;
        this.gameObject.SetActive(false);

    }

    #region BehaviourControl

    private void StartComponent()
    {
        //GamificationComponentData.instance.buildingDetect.StopSpecialItemComponent();
        //GamificationComponentData.instance.playerControllerNew.NinjaComponentTimerStart(0);
        //GamificationComponentData.instance.playerControllerNew.isThrow = false;
        BuilderEventManager.OnAvatarInvisibilityComponentCollisionEnter?.Invoke(blindfoldedDisplayComponentData.blindfoldSliderValue);
        raycast = GamificationComponentData.instance.raycast;
        if (!gameObject.activeInHierarchy)
            this.gameObject.SetActive(true);

        //Physics.IgnoreLayerCollision(9, 22, true);

        if (blindfoldedDisplayComponentData.footprintPaintAvatar)
        {
            SetFootPrinting();
        }
        else if (blindfoldedDisplayComponentData.invisibleAvatar)
        {
            SetAvatarInvisibility();
        }
    }
    private void StopComponent()
    {
        //ther is no Stop because in this case the player have no colliders they are invisible
        //BuilderEventManager.OnAvatarInvisibilityComponentCollisionEnter?.Invoke(0);
        if (GamificationComponentData.instance.activeComponent != null)
            DeactivateAvatarInivisibility();
    }

    public override void StopBehaviour()
    {
        if (!isPlaying)
            return;
        isPlaying = false;
        StopComponent();
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
        _componentType = Constants.ItemComponentType.BlindfoldedDisplayComponent;
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