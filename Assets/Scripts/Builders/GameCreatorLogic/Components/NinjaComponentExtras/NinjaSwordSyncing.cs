using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NinjaSwordSyncing : MonoBehaviourPun
{
    private Transform swordHook, swordhandHook;
    Transform parentTransfrom;
    Animator anim;
    bool isDrawSword;

    private void OnEnable()
    {
        if (photonView.IsMine)
            return;
        if (!GamificationComponentData.instance.withMultiplayer)
            gameObject.SetActive(false);
    }
    [PunRPC]
    void NinjaSwordInit(int pvID)
    {
        this.parentTransfrom = PhotonView.Find(pvID).transform;
        this.transform.SetParent(parentTransfrom);
        this.transform.localPosition = Vector3.zero;
        this.transform.localScale = Vector3.one;
        swordhandHook = GetComponentInParent<IKMuseum>().m_SelfieStick.transform.parent;
        swordHook = GetComponentInParent<CharcterBodyParts>().PelvisBone.transform;
        anim = GetComponentInParent<IKMuseum>().GetComponent<Animator>();
    }

    [PunRPC]
    void SwordHolding(bool isDrawSword, int pvID)
    {
        this.isDrawSword = isDrawSword;

        if (swordhandHook == null || swordHook == null)
            NinjaSwordInit(pvID);

        StartCoroutine(SwordHolding());

    }
    IEnumerator SwordHolding()
    {
        if (!isDrawSword)
        {
            anim.CrossFade("Withdrawing", 0.2f);
            yield return new WaitForSecondsRealtime(1.3f);
            this.transform.SetParent(swordHook, false);
            this.transform.localPosition = new Vector3(-0.149000004f, 0.0500000007f, 0.023f);
            this.transform.localRotation = new Quaternion(-0.149309605f, -0.19390057f, 0.966789007f, 0.0736774057f);
        }
        if (isDrawSword)
        {
            anim.CrossFade("SheathingSword", 0.2f);
            yield return new WaitForSecondsRealtime(0.8f);
            this.transform.SetParent(swordhandHook, false);
            yield return new WaitForSecondsRealtime(0.1f);
            this.transform.localPosition = new Vector3(0.0729999989f, -0.0329999998f, -0.0140000004f);
            this.transform.localRotation = new Quaternion(0.725517809f, 0.281368196f, -0.0713528395f, 0.623990953f);
        }
    }

    [PunRPC]
    void NinjaAttackSync(int attackno)
    {
        StopCoroutine(nameof(NinjaAttack));
        StartCoroutine(NinjaAttack(attackno));
    }

    IEnumerator NinjaAttack(int attackno)
    {
        yield return null;
        if (attackno == 1)
            anim.CrossFade("NinjaAttack", 0.1f);
        else if (attackno == 2)
            anim.CrossFade("NinjaAmimationSlash3", 0.1f);
        else if (attackno == 3)
            anim.CrossFade("Sword And Shield Attack", 0.1f);
    }
}
