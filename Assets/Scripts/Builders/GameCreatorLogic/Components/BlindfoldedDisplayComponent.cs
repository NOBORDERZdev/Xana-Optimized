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

    bool notTriggerOther = false;

    public void Init(BlindfoldedDisplayComponentData blindfoldedDisplayComponentData)
    {
        this.blindfoldedDisplayComponentData = blindfoldedDisplayComponentData;
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
            GamificationComponentData.instance.buildingDetect.StopSpecialItemComponent();
            GamificationComponentData.instance.playerControllerNew.NinjaComponentTimerStart(0);
            GamificationComponentData.instance.playerControllerNew.isThrow = false;
            BuilderEventManager.OnAvatarInvisibilityComponentCollisionEnter?.Invoke(blindfoldedDisplayComponentData.blindfoldSliderValue);
            raycast = GamificationComponentData.instance.raycast;

            Physics.IgnoreLayerCollision(9, 22, true);

            if (blindfoldedDisplayComponentData.footprintPaintAvatar)
            {
                SetFootPrinting(other.gameObject);
            }
            else if (blindfoldedDisplayComponentData.invisibleAvatar)
            {
                SetAvatarInvisibility();
            }
        }
    }

    private void SetFootPrinting(GameObject other)
    {
        Transform shoes = GamificationComponentData.instance.buildingDetect.playerShoes.transform;
        //shoes.localPosition = Vector3.forward * 0.761f;
        Rigidbody rb = null;
        shoes.TryGetComponent(out rb);
        if (rb == null)
            rb = shoes.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        if (shoes.transform.childCount == 0)
        {
            var tempobj = Instantiate(GamificationComponentData.instance.FootSteps[0], shoes);
            tempobj.transform.localPosition = Vector3.up * 0.0207f;
        }

        skinMesh = GamificationComponentData.instance.playerControllerNew.GetComponentsInChildren<SkinnedMeshRenderer>();

        //other.gameObject.GetComponent<PlayerControllerNew>().isThrow = false;
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
        yield return new WaitForSeconds(blindfoldedDisplayComponentData.blindfoldSliderValue);

        BuilderEventManager.DeactivateAvatarInivisibility?.Invoke();
        notTriggerOther = false;
        RaycastHit hit;
        if (Physics.Raycast(raycast.transform.position + new Vector3(0, 100, 0), -raycast.transform.up, out hit, 200))
        {
            if (hit.collider.CompareTag("Item"))
            {
                //Debug.Log("Not Null");
                BuilderEventManager.ReSpawnPlayer?.Invoke();
                notTriggerOther = true;

                Toast.Show("The avatar is now locked inside the object due to the avatar invisibility effect, so it will restart from the current point.");
            }
        }

        footstepsBool = false;

        for (int i = 0; i < skinMesh.Length; i++)
        {
            skinMesh[i].enabled = true;
        }


        GamificationComponentData.instance.buildingDetect.playerFreeCamConsole.enabled = true;
        GamificationComponentData.instance.buildingDetect.playerFreeCamConsoleOther.enabled = true;
        rr.enabled = false;
        rr.transform.GetChild(0).gameObject.SetActive(false);

        //CanvasComponenetsManager._instance.avatarInvisiblityText.gameObject.SetActive(false);
        BuilderEventManager.OnAvatarInvisibilityComponentCollisionEnter?.Invoke(0);
        if (!notTriggerOther)
        {
            Physics.IgnoreLayerCollision(9, 22, false);
        }
        notTriggerOther = false;

        Destroy(this.gameObject);
    }

    IEnumerator BackToAvatarVisiblityHologram()
    {
        yield return new WaitForEndOfFrame();
        BuilderEventManager.ActivateAvatarInivisibility?.Invoke();
        yield return new WaitForSeconds(blindfoldedDisplayComponentData.blindfoldSliderValue);

        BuilderEventManager.DeactivateAvatarInivisibility?.Invoke();
        notTriggerOther = false;
        RaycastHit hit;
        if (Physics.Raycast(raycast.transform.position + new Vector3(0, 100, 0), -raycast.transform.up, out hit, 200))
        {
            if (hit.collider.CompareTag("Item"))
            {
                //Debug.Log("Not Null");
                BuilderEventManager.ReSpawnPlayer?.Invoke();
                notTriggerOther = true;
                Toast.Show("The avatar is now locked inside the object due to the avatar invisibility effect, so it will restart from the current point.");
            }
        }

        BuilderEventManager.OnAvatarInvisibilityComponentCollisionEnter?.Invoke(0);
        if (!notTriggerOther)
        {
            Physics.IgnoreLayerCollision(9, 22, false);
        }
        notTriggerOther = false;
        Destroy(this.gameObject);
    }
}