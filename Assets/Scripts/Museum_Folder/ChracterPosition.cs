using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class ChracterPosition : MonoBehaviour
{
    public static ChracterPosition instance;
    public GameObject NewPos, Object_to_close, Object_to_open;
    public Vector3 spawnPos;
    public bool enetered;

    public static string currSpwanPos;

    private bool isAlreadyRunning=true;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (this.GetComponent<ChracterPosition>().enabled)
            enetered = true;
        else
            enetered = false;


    }


    private void OnTriggerEnter(Collider other)
    {
        if(isAlreadyRunning)
        {
            
            if (!enetered)
                return;
            
            if ((other.gameObject.tag == "PhotonLocalPlayer" || other.gameObject.tag == "Player") && other.GetComponent<PhotonView>().IsMine)
            {
                if (ReferencesForGamePlay.instance.m_34player)
                {
                    ReferencesForGamePlay.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.PortalSound);
                }
                if (!UserPassManager.Instance.CheckSpecificItem("idol_Villa", false))
                {
                    UserPassManager.Instance.vipPassUI.SetActive(true);
                    PlayerCameraController.instance.DisAllowControl();
                    return;
                }
                isAlreadyRunning = false;
                if (NewPos.name.Contains("OutSide"))
                {
                    StartCoroutine(DelayForOutSide());
                }
                else
                {
                    StartCoroutine(DelayInEnterRoom());
                }
            }
        }
    }


    IEnumerator DelayInEnterRoom()
    {
        ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = false;
        yield return StartCoroutine(LoadingHandler.Instance.FadeIn());
        Object_to_open.SetActive(true);
        currSpwanPos = Object_to_open.name;
        yield return new WaitForSeconds(.5f);

        Vector3 tempSpawn = spawnPos;

        RaycastHit hit;
        CheckAgain:
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(tempSpawn, NewPos.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
            if (hit.collider.gameObject.tag == "PhotonLocalPlayer")
            {
                tempSpawn = new Vector3(tempSpawn.x + Random.Range(-2, 2), tempSpawn.y, tempSpawn.z + Random.Range(-2, 2));
                goto CheckAgain;
            }
            else if(hit.collider.gameObject.tag!="GroundFloor")
            {
                tempSpawn = new Vector3(tempSpawn.x + Random.Range(-2, 2), tempSpawn.y, tempSpawn.z + Random.Range(-2, 2));
                goto CheckAgain;
            }

            //spawnPos = new Vector3(spawnPos.x, hit.point.y, spawnPos.z);
        }
        else
        {
            tempSpawn = new Vector3(tempSpawn.x + Random.Range(-2, 2), tempSpawn.y, tempSpawn.z + Random.Range(-2, 2));
            goto CheckAgain;
        }
        yield return new WaitForSeconds(.4f);


        ReferencesForGamePlay.instance.PlayerParent.transform.position = new Vector3(0, 0, 0);
        ReferencesForGamePlay.instance.MainPlayerParent.transform.position = new Vector3(0, 0, 0);
        ReferencesForGamePlay.instance.MainPlayerParent.transform.position = tempSpawn;
      
        yield return new WaitForSeconds(.5f);
        ReferencesForGamePlay.instance.MainPlayerParent.transform.position = tempSpawn;
        yield return StartCoroutine(LoadingHandler.Instance.FadeOut());
        ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = true;
        isAlreadyRunning = true;
        Object_to_close.SetActive(false);
    }


    IEnumerator DelayForOutSide()
    {
        ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = false;
        yield return StartCoroutine(LoadingHandler.Instance.FadeIn());
        Object_to_open.SetActive(true);
        currSpwanPos = Object_to_open.name;
        yield return new WaitForSeconds(.1f);

        RaycastHit hit;
        CheckAgain:
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(spawnPos, NewPos.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
            if (hit.collider.gameObject.tag == "PhotonLocalPlayer")
            {
                spawnPos = new Vector3(spawnPos.x + Random.Range(-2, 2), spawnPos.y, spawnPos.z + Random.Range(-2, 2));
                goto CheckAgain;
            }

            //spawnPos = new Vector3(spawnPos.x, hit.point.y, spawnPos.z);
        }
        yield return new WaitForSeconds(.4f);

        ReferencesForGamePlay.instance.PlayerParent.transform.position = new Vector3(0, 0, 0);
        ReferencesForGamePlay.instance.MainPlayerParent.transform.position = new Vector3(0, 0, 0);
        ReferencesForGamePlay.instance.MainPlayerParent.transform.position = spawnPos;

        yield return new WaitForSeconds(.5f);
        ReferencesForGamePlay.instance.MainPlayerParent.transform.position = spawnPos;
        GameplayEntityLoader.instance.SetAxis();
        yield return StartCoroutine(LoadingHandler.Instance.FadeOut());
        ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = true;
        isAlreadyRunning = true;
        Object_to_close.SetActive(false);
    }


    //public void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    if (currMainPlayer == null)
    //        currMainPlayer = GameObject.FindGameObjectWithTag("Player").gameObject;

    //    if (currMainPlayer != null)
    //    {
    //        currMainPlayer.GetComponentInChildren<PhotonView>().RPC("ChangePlayerParentOnStart", RpcTarget.All);
    //    }
    //    //throw new System.NotImplementedException();
    //}

    //public void OnPlayerLeftRoom(Player otherPlayer)
    //{
    //    //throw new System.NotImplementedException();
    //}

    //public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    //{
    //    //throw new System.NotImplementedException();
    //}

    //public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    //{
    //    //throw new System.NotImplementedException();
    //}

    //public void OnMasterClientSwitched(Player newMasterClient)
    //{
    //    //throw new System.NotImplementedException();
    //}
}
