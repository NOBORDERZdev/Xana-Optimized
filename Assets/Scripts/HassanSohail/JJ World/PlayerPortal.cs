using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GlobalConstants;

public class PlayerPortal : MonoBehaviour
{
    #region EditorReference
    [SerializeField] bool isLocked;
    //[SerializeField] JjWorldMusuemManager manager;
    [SerializeField] Transform destinationPoint;

    public enum PortalType { None, Enter, Exit, Teleport }
    public PortalType currentPortal = PortalType.None;
    public JJMuseumInfoManager ref_JJMuseumInfoManager;
    public float cam_XValue = -50f;
    #endregion
    #region PrivateVar
    // private PlayerControllerNew player;
    private ReferrencesForDynamicMuseum referrencesForDynamicMuseum;
    Collider colider;
    string firebaseEventName = "";
    #endregion
    #region PrivateFunc
    string customFirebaseEvent = "";

    private void Start()
    {
        referrencesForDynamicMuseum = ReferrencesForDynamicMuseum.instance;
       // player = referrencesForDynamicMuseum.MainPlayerParent.GetComponent<PlayerControllerNew>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (/*manager.allowTeleportation && */(other.CompareTag("PhotonLocalPlayer") /*|| other.CompareTag("Player")*/) && destinationPoint != null /*&& isAlreadyRunning */ /*&& player.allowTeleport*/)
        {
            print("player enter : " + transform.parent.parent.name);
            
            // For NFT Click
            JjInfoManager.Instance.analyticMuseumID = transform.parent.name;
            if (transform.parent.parent.name.Contains("Rental"))
            {
                string tempString = JjInfoManager.Instance.analyticMuseumID;
                int ind = int.Parse(tempString.Split('_').Last());
                JjInfoManager.Instance.analyticMuseumID= "Space_" + ind;
            }

            // For EnterPortal
            if(currentPortal == PortalType.None || currentPortal == PortalType.Teleport)
            {
                if(transform.parent.name.Contains("Astroboy"))
                customFirebaseEvent = FirebaseTrigger.WP_Infoboard_Atom + "_" + name;
                else
                    customFirebaseEvent = FirebaseTrigger.WP_Infoboard_Rental + "_" + name;
            }
            else if (currentPortal == PortalType.Enter)
            {
                if (ref_JJMuseumInfoManager.transform.parent.name.Contains("Astroboy"))
                    customFirebaseEvent = FirebaseTrigger.WP_EachRoom_Atom + "_" + ref_JJMuseumInfoManager.name;
                else
                    customFirebaseEvent = FirebaseTrigger.WP_EachRoom_Rental + "_" + ref_JJMuseumInfoManager.name;
            }

            if (other.GetComponent<PhotonView>().IsMine)
            {
                SendFirebaseEvent(customFirebaseEvent);
                //CallAnalyticsEvent();
                this.StartCoroutine(Teleport());
            }
          
        }
    }

   


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
            if (currentPortal == PortalType.Enter)
            {
                UnloadPreviousData();
                ref_JJMuseumInfoManager.InitJJMuseumInfoManager();
            }
            else if (currentPortal == PortalType.Exit)
            {
                UnloadPreviousData();
                JjInfoManager.Instance.IntJjInfoManager();
            }
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
            LoadFromFile.instance.StartCoroutine(LoadFromFile.instance.setPlayerCamAngle(cam_XValue, 0.5f));
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
    void UnloadPreviousData()
    {
        foreach (Texture txt in JjInfoManager.Instance.NFTLoadedSprites)
            Destroy(txt);
        JjInfoManager.Instance.NFTLoadedSprites.Clear();
        foreach (RenderTexture rnd in JjInfoManager.Instance.NFTLoadedVideos)
            Destroy(rnd);
        JjInfoManager.Instance.NFTLoadedVideos.Clear();
        foreach (GameObject nftInfo in ref_JJMuseumInfoManager.NftPlaceholder)
        {
            nftInfo.GetComponent<JJVideoAndImage>().imgVideo16x9.SetActive(false);
            nftInfo.GetComponent<JJVideoAndImage>().imgVideo9x16.SetActive(false);
            nftInfo.GetComponent<JJVideoAndImage>().imgVideo1x1.SetActive(false);
            nftInfo.GetComponent<JJVideoAndImage>().imgVideo4x3.SetActive(false);
        }
        if (JjInfoManager.Instance.videoRenderObject)
            JjInfoManager.Instance.videoRenderObject.SetActive(false);

        foreach (GameObject obj in JJFrameManager.instance.ref_JJObjectPooler.pooledObjectsforFrame)
            obj.SetActive(false);
        foreach (GameObject obj in JJFrameManager.instance.ref_JJObjectPooler.pooledObjectsforSpotLight)
            obj.SetActive(false);
    }
    #endregion

}
