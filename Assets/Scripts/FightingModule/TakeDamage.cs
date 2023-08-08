using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using DG.Tweening;
using UnityEditor;

public class TakeDamage : MonoBehaviourPun
{
    [SerializeField] PlayerController playerController;
    [SerializeField] CountdownManager countDownManager;

    public float startHealth;
    public float punchPower, kickPower, damageIncTime;
    public GameObject otherPlayer;
    [HideInInspector]public TakeDamage otherPlayerTakeDamage;
    public PlayerController otherPlayerController;
    [HideInInspector]public CountdownManager otherPlayerCountdown;
    [SerializeField] Image healthBar;
    public Toggle[] roundToggles;
    [SerializeField] List<GameObject> comboCollider;
    public float health;
    public int losses = 0;
    FightingModule.CameraLook _camLook;
    bool died;

    [SerializeField] PhotonView _photonView;
    bool isAndroid = false;
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    private void Start()
    {
        if (_photonView.IsMine)
        {
            healthBar = FightingModule.GameManager.instance.firstHealthBar;
            roundToggles = FightingModule.GameManager.instance.firstBarToggles;
        }
        else
        {
            healthBar = FightingModule.GameManager.instance.secondHealthBar;
            roundToggles = FightingModule.GameManager.instance.secondBarToggles;
        }
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        if (damageIncTime > 0)
            StartCoroutine(SetDamageInc());
      
        if (!PhotonNetwork.IsMasterClient)
        {
            if (photonView.IsMine)
                photonView.RPC("SetHealthStats", RpcTarget.All);
        }
        
        GameObject cineMachineObj = GameObject.FindGameObjectWithTag("CineMachine");
        _camLook = cineMachineObj.GetComponent<FightingModule.CameraLook>();

        if (Application.platform == RuntimePlatform.Android) {
            isAndroid = true;
        }

    }

    /// <summary>
    /// The purpose of this mechanism is likely to implement a 
    /// temporary power-up effect during a time-limited phase of the game.
    /// </summary>
    IEnumerator SetDamageInc()
    {
        punchPower = punchPower + (punchPower * 0.05f);
        kickPower = kickPower + (kickPower * 0.05f);
        playerController.grabDamage += (playerController.grabDamage * 0.05f);
        while (countDownManager.remainingTIme > (60 - damageIncTime))
        {
            yield return new WaitForEndOfFrame();
        }
        punchPower = 5f;
        kickPower = 8f;
        playerController.grabDamage = 20f;
        yield return null;
    }

    /// <summary>
    /// Damage Calculations and DIE checks
    /// </summary>
    [PunRPC]
    void SetDamage(float dam,int doShake,float _camShakeWaitTime)
    {
        health -= dam;
        healthBar.DOFillAmount(health/startHealth, 0.5f);
        if (doShake == 1)
        {
            StartCoroutine(CameraShakeEffect(_camShakeWaitTime));
        }
        //  _camLook.ShakeCamera();
        //    healthBar.fillAmount = health / startHealth;
        if (health <= 0f)
        {
            if (!died)
            {
                countDownManager.stopCountDown();
                otherPlayerCountdown.stopCountDown();
                Die();
            }
        }
    }

    /// <summary>
    /// Camera Shake Effect Start and Stop
    /// </summary>
    IEnumerator CameraShakeEffect(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _camLook.ShakeCamera();
        if (isAndroid)
        {
            VibrationManager.Vibrate();
        }
    }

    public void OnTimeOver()
    {
        if (!died)
        {
            if (health < otherPlayerTakeDamage.health)
            {
                Die();
            }
            else
            {
                otherPlayerTakeDamage.Die();
            }
        }
    }

    /// <summary>
    /// This code snippet handles a specific Photon event (eventCodeNumber == 3) 
    /// and triggers a coroutine to leave the Photon room after a short delay, but only for the local player.
    /// </summary>
    byte eventCodeNumber = 3;
    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == eventCodeNumber)
        {
            StartCoroutine(LeaveAfterDelay());
        }
    }

    /// <summary>
    /// When this coroutine is executed and the photonView belongs to the local player, 
    /// it will wait for 2 seconds, and then leave the Photon room.
    /// </summary>
    IEnumerator LeaveAfterDelay()
    {
        if (photonView.IsMine)
        {
            yield return new WaitForSeconds(2f);
            Debug.Log("Leaving Room!");
            PhotonNetwork.LeaveRoom();
        }
    }

    /// <summary>
    /// This code ensures that the "Finish Game" event is raised only once 
    /// and is propagated to all players in the room through Photon Networking.
    /// </summary>
    bool isGameFinishedCalled = false;
    string finishGameVal = "Finish Game";
    void RaiseFinishEvent()
    {
        if (isGameFinishedCalled) return;
        object[] data = new object[] { finishGameVal };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(eventCodeNumber, data, raiseEventOptions, SendOptions.SendReliable);
        isGameFinishedCalled = true;
    }

    /// <summary>
    /// When player dies we call this function and set up rounds mechanism
    /// </summary>
    void Die()
    {
        died = true;
        Debug.Log("You Die!");
        losses++;
        if (losses >= CustomizationCanvasManager._instance._noofRounds)
        {
            otherPlayerTakeDamage.roundToggles[losses - 1].isOn = true;
            if (photonView.IsMine)
            {
               StartCoroutine(FightingModule.GameManager.instance.SetEndPanel("You Loss!"));
               RaiseFinishEvent();
            }
            else
            {
                StartCoroutine(FightingModule.GameManager.instance.SetEndPanel("You Win!"));
            }
        }
        else
        {
            otherPlayerTakeDamage.roundToggles[losses - 1].isOn = true;
            FightingModule.GameManager.instance.ForNextRound();
            StartCoroutine(SettingHealth());
        }
    }

    public IEnumerator SettingHealth()
    {     
        yield return new WaitForSeconds(6f);
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        otherPlayerTakeDamage.health = startHealth;
        otherPlayerTakeDamage.healthBar.fillAmount = health / startHealth;
        playerController.powerBar.fillAmount = 0;
        playerController.powerBar.fillAmount = 0;
        otherPlayerController.powerBar.fillAmount = 0;
        playerController.ResetPlayer();
        otherPlayerController.ResetPlayer();
        died = false;
        yield return null;
    }

    /// <summary>
    /// On round start we set up all health bars and power bars on both ends using below function
    /// </summary>
    [PunRPC]
    void SetHealthStats()
    {
        StartCoroutine(SettingHealth());
    }

    /// <summary>
    /// When colliders placed on hands and feet triggers oppoenent player SetDamage() is called and UI PowerBars are set accordingly
    /// we have another mechanism where if player uses special move and the other player is blocking we still give other player minor damage "chip damage"
    /// All of the above explained mechanism is done in here
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!comboCollider.Contains(other.gameObject))
        {
            if (!playerController.isBlock)
            {
              
                if (other.CompareTag("PunchCombo"))
                {
                    photonView.RPC("SetDamage", RpcTarget.All, punchPower,0,0f);
                    if (!otherPlayerController.isSpecialAttack)
                    {
                        otherPlayerController.photonView.RPC("PowerBar_RPC", RpcTarget.All);
                    }
                }
                if (other.CompareTag("KickCombo"))
                {
                    photonView.RPC("SetDamage", RpcTarget.All, kickPower,0,0f);

                    if (!otherPlayerController.isSpecialAttack)
                    {
                        otherPlayerController.photonView.RPC("PowerBar_RPC", RpcTarget.All);
                    }
                }
            }
            else
            {
                if (otherPlayerController.isSpecialAttack)
                {
                    if (other.CompareTag("PunchCombo"))
                    {
                        photonView.RPC("SetDamage", RpcTarget.All, punchPower / 2,0,0f);
                    }
                    if (other.CompareTag("KickCombo"))
                    {
                        photonView.RPC("SetDamage", RpcTarget.All, kickPower / 2, 0,0f);
                    }
                }
            }
        }
    }
}
