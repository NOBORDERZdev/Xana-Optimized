using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JjWorldChanger : MonoBehaviour
{
    public string WorldName;
    [SerializeField] bool HaveMultipleSpwanPoint;
    [SerializeField] JJMussuemEntry mussuemEntry;
    [Header("Xana Musuem")]
    public bool isMusuem;
    public int testNet;
    public int MainNet;
    [Header("Builder")]
    public bool isBuilderWorld;

    Collider collider;

    bool reSetCollider = false;

    private GameObject triggerObject;
    public bool isEnteringPopup;
    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerObject = other.gameObject;
        if (triggerObject.CompareTag("PhotonLocalPlayer") && triggerObject.GetComponent<PhotonView>().IsMine)
        {
            CanvasButtonsHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = false;
            if (ReferrencesForDynamicMuseum.instance.m_34player)
            {
                ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.PortalSound);
            }
            triggerObject = other.gameObject;
            if (isEnteringPopup)
                CanvasButtonsHandler.inst.EnableJJPortalPopup(this.gameObject, 0);
            else
                CanvasButtonsHandler.inst.EnableJJPortalPopup(this.gameObject, 1);
        }

    }
    public void RedirectToWorld()
    {
        if (triggerObject.CompareTag("PhotonLocalPlayer") && triggerObject.GetComponent<PhotonView>().IsMine)
        {
            //Gautam Commented below code due to shows pop up again and again.

            //collider.enabled = false;
            if (checkWorldComingSoon(WorldName) || isBuilderWorld)
            {
                this.StartCoroutine(swtichScene(WorldName));
            }
            //else
            //{
            //    this.StartCoroutine(ResetColider());
            //}
        }
    }

    IEnumerator ResetColider()
    {
        yield return new WaitForSeconds(1f);
        collider.enabled = true;
    }


    /// <summary>
    /// To Swtich Scene with JJ world Loading
    /// </summary>
    private IEnumerator swtichScene(string worldName)
    {
        if (worldName.Contains(" : "))
        {
            string name = worldName.Replace(" : ", string.Empty);
            worldName = name;
        }

        if (XanaConstants.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            XanaConstants.xanaConstants.isFromXanaLobby = true;
        }

        // LoadingHandler.Instance.UpdateLoadingSliderForJJ(Random.Range(0.1f, 0.19f), 1f, false);
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        if (!XanaConstants.xanaConstants.JjWorldSceneChange && !XanaConstants.xanaConstants.orientationchanged)
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        //XanaConstants.xanaConstants.EnviornmentName = worldName;
        //FeedEventPrefab.m_EnvName = worldName;
        //Launcher.sceneName = worldName;

        // Added by WaqasAhmad
        // For Live User Count
        if (APIBaseUrlChange.instance.IsXanaLive)
        {
            XanaConstants.xanaConstants.customWorldId = MainNet;
        }
        else
        {
            XanaConstants.xanaConstants.customWorldId = testNet;
        }
        //

        if (isMusuem)
        {
            XanaConstants.xanaConstants.IsMuseum = true;
            if (APIBaseUrlChange.instance.IsXanaLive)
            {
                XanaConstants.xanaConstants.MuseumID = MainNet.ToString();
            }
            else
            {
                XanaConstants.xanaConstants.MuseumID = testNet.ToString();
            }
        }
        else if (isBuilderWorld)
        {
            XanaConstants.xanaConstants.isBuilderScene = true;
            if (APIBaseUrlChange.instance.IsXanaLive)
            {
                XanaConstants.xanaConstants.builderMapID = MainNet;
            }
            else
            {
                XanaConstants.xanaConstants.builderMapID = testNet;
            }
        }
        else // FOR JJ WORLD
        {
            if (HaveMultipleSpwanPoint)
            {
                XanaConstants.xanaConstants.mussuemEntry = mussuemEntry;
            }
            else
            {
                XanaConstants.xanaConstants.mussuemEntry = JJMussuemEntry.Null;
            }
        }
        yield return new WaitForSeconds(1f);
        XanaConstants.xanaConstants.JjWorldSceneChange = true;
        XanaConstants.xanaConstants.JjWorldTeleportSceneName = worldName;
        LoadFromFile.instance._uiReferences.LoadMain(false);


    }


    private bool checkWorldComingSoon(string worldName)
    {
        if (!PremiumUsersDetails.Instance.CheckSpecificItem(worldName, true))
        {

            return false;
        }
        else
        {
            return true;
        }
    }
}
