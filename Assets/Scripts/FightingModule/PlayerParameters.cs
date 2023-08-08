using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerParameters : MonoBehaviourPun
{
    [SerializeField] PlayerController playerController;
    [SerializeField] TakeDamage takeDamage;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SetParameters", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void SetParameters()
    {
        playerController.playerSpeed = CustomParameters.instance.playerSpeed;
        playerController.range = CustomParameters.instance.range;
        playerController.specialBarFill = CustomParameters.instance.specialBarFill;
        playerController.grabDamage = CustomParameters.instance.grabDamage;
        takeDamage.startHealth = CustomParameters.instance.playerHealth;
        takeDamage.punchPower = CustomParameters.instance.punchPower;
        takeDamage.kickPower = CustomParameters.instance.kickPower;
        takeDamage.damageIncTime = CustomParameters.instance.damageIncTime;
    }
}
