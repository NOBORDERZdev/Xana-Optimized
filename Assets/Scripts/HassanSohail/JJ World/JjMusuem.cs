using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JjMusuem : MonoBehaviour
{
    [SerializeField] Transform AstroEntery, RentalEntery;
    [SerializeField] JjWorldMusuemManager manager;
    public static JjMusuem Instance { get; private set; }
    //  private Transform destinationPoint;
    private PlayerControllerNew player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        // force fully potraite if the player is in potraite in previous jj world
        
    }

    private void Start()
    {
        //if (player.gameObject.GetComponentInChildren<PhotonView>().IsMine)
        //    SetPlayerPos(XanaConstants.xanaConstants.mussuemEntry);
    }


    /// <summary>
    /// Set player position in room according to the point on which the player enter
    /// </summary>
    public void SetPlayerPos(JJMussuemEntry mussuemEntry) {
        player = ReferrencesForDynamicMuseum.instance.MainPlayerParent.GetComponent<PlayerControllerNew>();
        if (ReferrencesForDynamicMuseum.instance.m_34player.gameObject.GetComponentInChildren<PhotonView>().IsMine)
        {
            switch (mussuemEntry)
            {
                case JJMussuemEntry.Astro:
                    StartCoroutine(changePos(AstroEntery.transform));
                    break;
                case JJMussuemEntry.Rental:
                    StartCoroutine(changePos(RentalEntery.transform));
                    break;
                default:
                    break;
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (manager.allowTeleportation && (other.CompareTag("PhotonLocalPlayer") /*|| other.CompareTag("Player")*/) && player.allowTeleport)
    //    {
    //        print("player enter");
    //        SetPlayerPos(XanaConstants.xanaConstants.mussuemEntry);
    //    }
    //}

    IEnumerator changePos(Transform destinationPoint) {
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        yield return new WaitForSeconds(.02f);
        manager.allowTeleportation = false;
        player.allowTeleport = false;
        player.m_IsMovementActive = false;
       // StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        yield return new WaitForSeconds(.2f);
        Vector3 tempSpawn = destinationPoint.position;
        RaycastHit hit;
    CheckAgain:
        if (Physics.Raycast(tempSpawn, destinationPoint.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "PhotonLocalPlayer" /*&& hit.collider.gameObject.tag == "Player"*/)
            {
                tempSpawn = new Vector3(tempSpawn.x + Random.Range(-2, 2), tempSpawn.y, tempSpawn.z + Random.Range(-2, 2));
                goto CheckAgain;
            }
            else if (hit.collider.gameObject.tag != "GroundFloor")
            {
                tempSpawn = new Vector3(tempSpawn.x + Random.Range(-2, 2), tempSpawn.y, tempSpawn.z + Random.Range(-2, 2));
                goto CheckAgain;
            }
        }
        else
        {
            tempSpawn = new Vector3(tempSpawn.x + Random.Range(-2, 2), tempSpawn.y, tempSpawn.z + Random.Range(-2, 2));
            goto CheckAgain;
        }
        yield return new WaitForSeconds(.4f);

        ReferrencesForDynamicMuseum.instance.PlayerParent.transform.position = new Vector3(0, 0, 0);
        ReferrencesForDynamicMuseum.instance.MainPlayerParent.transform.position = new Vector3(0, 0, 0);
        ReferrencesForDynamicMuseum.instance.MainPlayerParent.transform.position = tempSpawn;
        ReferrencesForDynamicMuseum.instance.MainPlayerParent.transform.eulerAngles = destinationPoint.eulerAngles;

        yield return new WaitForSeconds(.8f);
        ReferrencesForDynamicMuseum.instance.MainPlayerParent.transform.position = tempSpawn;
        player.m_IsMovementActive = true;
        // isAlreadyRunning = true;
        manager.allowTeleportation = true;
        player.allowTeleport = true;
        if (XanaConstants.xanaConstants.mussuemEntry == JJMussuemEntry.Astro)
        {
            LoadFromFile.instance.StartCoroutine(LoadFromFile.instance.setPlayerCamAngle(180f, 0.5f));
        }
        else
        {
            LoadFromFile.instance.StartCoroutine(LoadFromFile.instance.setPlayerCamAngle(0f, 0.5f));
        }

        yield return new WaitForSeconds(.15f);
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
    }

}

public enum JJMussuemEntry
{
    Null,
    Astro,
    Rental
}