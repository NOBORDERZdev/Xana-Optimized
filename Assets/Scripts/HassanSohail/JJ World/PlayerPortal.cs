using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortal : MonoBehaviour
{
    #region EditorReference
    [SerializeField] bool isLocked;
    //[SerializeField] JjWorldMusuemManager manager;
    [SerializeField] Transform destinationPoint;
    #endregion
    #region PrivateVar
   // private PlayerControllerNew player;
    private ReferrencesForDynamicMuseum referrencesForDynamicMuseum;
    Collider colider;
    #endregion
    #region PrivateFunc
    private void Start()
    {
        referrencesForDynamicMuseum = ReferrencesForDynamicMuseum.instance;
       // player = referrencesForDynamicMuseum.MainPlayerParent.GetComponent<PlayerControllerNew>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (/*manager.allowTeleportation && */(other.CompareTag("PhotonLocalPlayer") /*|| other.CompareTag("Player")*/) && destinationPoint != null /*&& isAlreadyRunning */ /*&& player.allowTeleport*/)
        {
            print("player enter");
            if (other.GetComponent<PhotonView>().IsMine)
            {
                this.StartCoroutine(Teleport());
            }
          
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (manager.allowTeleportation && !player.allowTeleport)
    //    {
    //        player.allowTeleport = true;
    //    }
    //}

    /// <summary>
    /// Teleport player from one point to other with loading effect
    /// </summary>
    /// <returns></returns>
    IEnumerator Teleport()
    {
        if (!isLocked)
        {
            //manager.allowTeleportation = false;
            //player.allowTeleport = false;
            referrencesForDynamicMuseum.MainPlayerParent.GetComponent<PlayerControllerNew>().m_IsMovementActive = false;
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
            yield return new WaitForSeconds(.5f);
            RaycastHit hit;
        CheckAgain:
            if (Physics.Raycast(destinationPoint.position, destinationPoint.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.tag == "PhotonLocalPlayer" /*&& hit.collider.gameObject.tag == "Player"*/)
                {
                    destinationPoint.position = new Vector3(destinationPoint.position.x + Random.Range(-2, 2), destinationPoint.position.y, destinationPoint.position.z + Random.Range(-2, 2));
                    goto CheckAgain;
                }
                else if (hit.collider.gameObject.tag != "GroundFloor")
                {
                    destinationPoint.position = new Vector3(destinationPoint.position.x + Random.Range(-2, 2), destinationPoint.position.y, destinationPoint.position.z + Random.Range(-2, 2));

                    goto CheckAgain;
                }
            }
            else
            {
                destinationPoint.position = new Vector3(destinationPoint.position.x + Random.Range(-2, 2), destinationPoint.position.y, destinationPoint.position.z + Random.Range(-2, 2));
                goto CheckAgain;
            }
            yield return new WaitForSeconds(.4f);

            //referrencesForDynamicMuseum.PlayerParent.transform.position = new Vector3(0, 0, 0);
            //referrencesForDynamicMuseum.MainPlayerParent.transform.position = new Vector3(0, 0, 0);
            referrencesForDynamicMuseum.MainPlayerParent.transform.eulerAngles = destinationPoint.eulerAngles;
            referrencesForDynamicMuseum.MainPlayerParent.transform.position = destinationPoint.position;
            yield return new WaitForSeconds(.8f);
            referrencesForDynamicMuseum.MainPlayerParent.transform.position = destinationPoint.position;
            referrencesForDynamicMuseum.MainPlayerParent.GetComponent<PlayerControllerNew>().m_IsMovementActive = true;
            // isAlreadyRunning = true;
            //manager.allowTeleportation = true;
            LoadFromFile.instance.StartCoroutine(LoadFromFile.instance.setPlayerCamAngle(-50f, 0.5f));
            yield return new WaitForSeconds(.15f);
            //player.allowTeleport = true;
            LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.Out));
        }
        else
        {
            if (SNSNotificationManager.Instance != null)
                SNSNotificationManager.Instance.ShowNotificationMsg("Coming soon");
            yield return null;
        }
    }

    #endregion

}
