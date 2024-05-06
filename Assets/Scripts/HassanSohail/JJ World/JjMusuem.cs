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
    private PlayerController player;

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

    private void OnEnable()
    {
        SetPlayerPos();
    }


    void SetPlayerPos()
    {
        SetPlayerPos(ConstantsHolder.xanaConstants.mussuemEntry);
    }
    /// <summary>
    /// Set player position in room according to the point on which the player enter
    /// </summary>
    public void SetPlayerPos(JJMussuemEntry mussuemEntry) {
        player = ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>();
        if (ReferencesForGamePlay.instance.m_34player.gameObject.GetComponentInChildren<PhotonView>().IsMine)
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
    //        SetPlayerPos(ConstantsHolder.xanaConstants.mussuemEntry);
    //    }
    //}

    IEnumerator changePos(Transform destinationPoint)
    {
        //LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        //LoadingHandler.Instance.UpdateLoadingSliderForJJ(Random.Range(0.7f, 0.85f), 4f, false);
       // yield return new WaitForSeconds(.02f);
        manager.allowTeleportation = false;
        player.allowTeleport = false;
        player.m_IsMovementActive = false;
        //StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
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

        ReferencesForGamePlay.instance.PlayerParent.transform.position = new Vector3(0, 0, 0);
        ReferencesForGamePlay.instance.MainPlayerParent.transform.position = new Vector3(0, 0, 0);
        ReferencesForGamePlay.instance.MainPlayerParent.transform.position = tempSpawn;
        ReferencesForGamePlay.instance.MainPlayerParent.transform.eulerAngles = destinationPoint.eulerAngles;

        yield return new WaitForSeconds(.8f);
        ReferencesForGamePlay.instance.MainPlayerParent.transform.position = tempSpawn;
        player.m_IsMovementActive = true;
        // isAlreadyRunning = true;
        manager.allowTeleportation = true;
        player.allowTeleport = true;
        if (ConstantsHolder.xanaConstants.mussuemEntry == JJMussuemEntry.Astro)
        {
            GameplayEntityLoader.instance.StartCoroutine(GameplayEntityLoader.instance.setPlayerCamAngle(180f, 0.5f));
        }
        else
        {
            GameplayEntityLoader.instance.StartCoroutine(GameplayEntityLoader.instance.setPlayerCamAngle(0f, 0.5f));
        }

       // yield return new WaitForSeconds(.15f);
       // LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
    }

}

public enum JJMussuemEntry
{
    Null,
    Astro,
    Rental
}