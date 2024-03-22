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
            GamePlayUIHandler.inst.ref_PlayerControllerNew.m_IsMovementActive = false;
            if (ReferrencesForDynamicMuseum.instance.m_34player)
            {
                ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SoundEffects>().PlaySoundEffects(SoundEffects.Sounds.PortalSound);
            }
            triggerObject = other.gameObject;
            if (isEnteringPopup)
                GamePlayUIHandler.inst.EnableJJPortalPopup(this.gameObject, 0);
            else
                GamePlayUIHandler.inst.EnableJJPortalPopup(this.gameObject, 1);
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

        if (XanaConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Lobby"))
        {
            XanaConstantsHolder.xanaConstants.isFromXanaLobby = true;
        }

        // LoadingController.Instance.UpdateLoadingSliderForJJ(Random.Range(0.1f, 0.19f), 1f, false);
        LoadingController.Instance.StartCoroutine(LoadingController.Instance.TeleportFader(FadeAction.In));
        if (!XanaConstantsHolder.xanaConstants.JjWorldSceneChange && !XanaConstantsHolder.xanaConstants.orientationchanged)
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        //XanaConstantsHolder.xanaConstants.EnviornmentName = worldName;
        //FeedEventPrefab.m_EnvName = worldName;
        //Launcher.sceneName = worldName;

        // Added by WaqasAhmad
        // For Live User Count
        if (ServerBaseURlHandler.instance.IsXanaLive)
        {
            XanaConstantsHolder.xanaConstants.customWorldId = MainNet;
        }
        else
        {
            XanaConstantsHolder.xanaConstants.customWorldId = testNet;
        }
        //

        if (isMusuem)
        {
            XanaConstantsHolder.xanaConstants.IsMuseum = true;
            if (ServerBaseURlHandler.instance.IsXanaLive)
            {
                XanaConstantsHolder.xanaConstants.MuseumID = MainNet.ToString();
            }
            else
            {
                XanaConstantsHolder.xanaConstants.MuseumID = testNet.ToString();
            }
        }
        else if (isBuilderWorld)
        {
            XanaConstantsHolder.xanaConstants.isBuilderScene = true;
            if (ServerBaseURlHandler.instance.IsXanaLive)
            {
                XanaConstantsHolder.xanaConstants.builderMapID = MainNet;
            }
            else
            {
                XanaConstantsHolder.xanaConstants.builderMapID = testNet;
            }
        }
        else // FOR JJ WORLD
        {
            if (HaveMultipleSpwanPoint)
            {
                XanaConstantsHolder.xanaConstants.mussuemEntry = mussuemEntry;
            }
            else
            {
                XanaConstantsHolder.xanaConstants.mussuemEntry = JJMussuemEntry.Null;
            }
        }
        yield return new WaitForSeconds(1f);
        XanaConstantsHolder.xanaConstants.JjWorldSceneChange = true;
        XanaConstantsHolder.xanaConstants.JjWorldTeleportSceneName = worldName;
        GameplayEntityLoader.instance._uiReferences.LoadMain(false);


    }


    private bool checkWorldComingSoon(string worldName)
    {
        if (!UserPassManager.Instance.CheckSpecificItem(worldName, true))
        {

            return false;
        }
        else
        {
            return true;
        }
    }
}
