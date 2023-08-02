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
                Transform shoes = GamificationComponentData.instance.buildingDetect.playerShoes.transform;
                //shoes.localPosition = Vector3.forward * 0.761f;
                shoes.gameObject.AddComponent<Rigidbody>().isKinematic = true;
                for (int i = 0; i < shoes.childCount; i++)
                {
                    Destroy(shoes.GetChild(i).gameObject);
                }
                foreach (GameObject goFootStep in GamificationComponentData.instance.FootSteps)
                {
                    var tempobj = Instantiate(goFootStep, shoes);
                    tempobj.transform.localPosition = Vector3.zero;
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

                RingbufferFootSteps[] ringbufferFootSteps = other.gameObject.GetComponentsInChildren<RingbufferFootSteps>();
                for (int i = 0; i < ringbufferFootSteps.Length; i++)
                {
                    ringbufferFootSteps[0].enabled = true;
                    ringbufferFootSteps[0].transform.GetChild(0).gameObject.SetActive(true);
                }
                //Debug.Log("BlindFolded Value : " + blindfoldedDisplayComponentData.blindfoldSliderValue);
                StartCoroutine(BackToVisible(ringbufferFootSteps));

            }
            else if (blindfoldedDisplayComponentData.invisibleAvatar)
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
        }
    }

    IEnumerator BackToVisible(RingbufferFootSteps[] rr)
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

        for (int i = 0; i < rr.Length; i++)
        {
            rr[0].enabled = false;
            rr[0].transform.GetChild(0).gameObject.SetActive(false);
        }

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