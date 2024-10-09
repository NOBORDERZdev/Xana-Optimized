using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class NinjaSwordSyncing : MonoBehaviourPun
{
    private Transform swordHook, swordhandHook;
    Transform parentTransfrom;
    Animator anim;
    bool isDrawSword;
    Player player;
    bool isInitiated=false;

    private void OnEnable()
    {
        if (photonView.IsMine)
            return;
        if (!GamificationComponentData.instance.withMultiplayer)
        {
            gameObject.SetActive(false);
            return;
        }
        NinjaSwordInit();
    }

    [PunRPC]
    void NinjaSwordInit()
    {
        this.parentTransfrom = FindPlayerusingPhotonView(photonView).transform;
        this.transform.SetParent(parentTransfrom);
        this.transform.localPosition = Vector3.zero;
        this.transform.localScale = Vector3.one;
        swordhandHook = GetComponentInParent<IKMuseum>().m_SelfieStick.transform.parent;
        swordHook = GetComponentInParent<CharacterBodyParts>().pelvisBoneNewCharacter.transform;
        anim = GetComponentInParent<IKMuseum>().GetComponent<Animator>();
        this.transform.SetParent(swordHook, false);
        this.transform.localPosition = new Vector3(-0.17f, 0.06f, 0.03f);
        this.transform.localRotation = new Quaternion(0.89543f, -0.21528f, 0.28035f, -0.27066f);
        isInitiated = true;
    }

    [PunRPC]
    void SwordHolding(bool isDrawSword)
    {
        if (swordhandHook == null || swordHook == null || parentTransfrom == null)
            NinjaSwordInit();

        if (photonView.Owner == player)
        {
            this.isDrawSword = isDrawSword;
            StartCoroutine(SwordHolding());
        }
    }
    IEnumerator SwordHolding()
    {
        while (!isInitiated)
        {
            yield return new WaitForEndOfFrame();
        }
        if (!isDrawSword)
        {
            anim.CrossFade("Withdrawing", 0.2f);
            yield return new WaitForSecondsRealtime(1.3f);
            this.transform.SetParent(swordHook, false);
            //this.transform.localPosition = new Vector3(-0.155000004f, 0.0500000007f, 0.023f);
            //this.transform.localRotation = new Quaternion(-0.149309605f, -0.19390057f, 0.966789007f, 0.0736774057f);
            this.transform.localPosition = new Vector3(-0.17f, 0.06f, 0.03f);
            this.transform.localRotation = new Quaternion(0.89543f, -0.21528f, 0.28035f, -0.27066f);
        }
        if (isDrawSword)
        {
            anim.CrossFade("SheathingSword", 0.2f);
            yield return new WaitForSecondsRealtime(0.8f);
            this.transform.SetParent(swordhandHook, false);
            yield return new WaitForSecondsRealtime(0.1f);
            //this.transform.localPosition = new Vector3(0.0729999989f, -0.0329999998f, -0.0140000004f);
            //this.transform.localRotation = new Quaternion(0.725517809f, 0.281368196f, -0.0713528395f, 0.623990953f);
            this.transform.localPosition = new Vector3(0.0370000005f, 0.0729999989f, 0.0120000001f);
            Quaternion newRotation = Quaternion.Euler(new Vector3(104.94f, 65.328f, 153.11f));
            this.transform.localRotation = newRotation;

        }
    }

    [PunRPC]
    void NinjaAttackSync(int attackno)
    {
        if (photonView.Owner == player)
        {
            StopCoroutine(nameof(NinjaAttack));
            StartCoroutine(NinjaAttack(attackno));
        }
    }

    IEnumerator NinjaAttack(int attackno)
    {
        while (!isInitiated)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return null;
        if (attackno == 1)
            anim.CrossFade("NinjaAttack", 0.1f);
        else if (attackno == 2)
            anim.CrossFade("NinjaAmimationSlash3", 0.1f);
        else if (attackno == 3)
            anim.CrossFade("Sword And Shield Attack", 0.1f);
    }

    GameObject FindPlayerusingPhotonView(PhotonView pv)
    {
        Player player = pv.Owner;
        foreach (GameObject playerObject in MutiplayerController.instance.playerobjects)
        {
            PhotonView _photonView = playerObject.GetComponent<PhotonView>();
            if (_photonView.Owner == player && _photonView.GetComponent<AvatarController>())
            {
                this.player = _photonView.Owner;
                return playerObject;
            }
        }
        return null;
    }
}
