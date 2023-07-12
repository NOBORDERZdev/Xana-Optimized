using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JjWorldChanger : MonoBehaviour
{
    [SerializeField] string WorldName;
    [SerializeField] bool HaveMultipleSpwanPoint;
    [SerializeField] JJMussuemEntry mussuemEntry;
    //[SerializeField] SceneManage sceneManage;
    Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PhotonLocalPlayer") && other.GetComponent<PhotonView>().IsMine)
        {
            collider.enabled = false;
            this.StartCoroutine(swtichScene(WorldName));
            //SwtichWorld(WorldName);
        }
    }

    //private void SwtichWorld(string worldName)
    //{
    //    Debug.LogError("SWTICH SCENE CALL");
    //    LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
    //    if (!XanaConstants.xanaConstants.JjWorldSceneChange && !XanaConstants.xanaConstants.orientationchanged)
    //        Screen.orientation = ScreenOrientation.LandscapeLeft;
    //    //XanaConstants.xanaConstants.EnviornmentName = worldName;
    //    //FeedEventPrefab.m_EnvName = worldName;
    //    //Launcher.sceneName = worldName;
    //    if (HaveMultipleSpwanPoint)
    //    {
    //        XanaConstants.xanaConstants.mussuemEntry = mussuemEntry;
    //    }
    //    else
    //    {
    //        XanaConstants.xanaConstants.mussuemEntry = JJMussuemEntry.Null;
    //    }
    //    //PhotonNetwork.LeaveRoom();
    //    //PhotonNetwork.LeaveLobby();
    //    //PhotonNetwork.DestroyAll(true);
    //    // Launcher.instance.working = ScenesList.MainMenu;
    //    // PhotonNetwork.LeaveRoom();
    //    // PhotonNetwork.LeaveLobby();
    //    // PhotonNetwork.DestroyAll(true);
    //    // yield return new WaitForSeconds(1f);
    //    //Photon.Pun.PhotonHandler.levelName = "AddressableScene";
    //    // SceneManager.LoadScene("AddressableScene");
    //    XanaConstants.xanaConstants.JjWorldSceneChange = true;
    //    XanaConstants.xanaConstants.JjWorldTeleportSceneName = worldName;
    //    LoadFromFile.instance._uiReferences.LoadMain();
    //}
    /// <summary>
    /// To Swtich Scene with JJ world Loading
    /// </summary>
    private IEnumerator swtichScene(string worldName)
    {
        Debug.LogError("SWTICH SCENE CALL");
        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        if (!XanaConstants.xanaConstants.JjWorldSceneChange && !XanaConstants.xanaConstants.orientationchanged)
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        //XanaConstants.xanaConstants.EnviornmentName = worldName;
        //FeedEventPrefab.m_EnvName = worldName;
        //Launcher.sceneName = worldName;
        if (HaveMultipleSpwanPoint)
        {
            XanaConstants.xanaConstants.mussuemEntry = mussuemEntry;
        }

        else
        {
            XanaConstants.xanaConstants.mussuemEntry = JJMussuemEntry.Null;
        }
        //PhotonNetwork.LeaveRoom();
        //PhotonNetwork.LeaveLobby();
        //PhotonNetwork.DestroyAll(true);
        // Launcher.instance.working = ScenesList.MainMenu;
        // PhotonNetwork.LeaveRoom();
        // PhotonNetwork.LeaveLobby();
        // PhotonNetwork.DestroyAll(true);
         yield return new WaitForSeconds(1f);
        //Photon.Pun.PhotonHandler.levelName = "AddressableScene";
        // SceneManager.LoadScene("AddressableScene");
        XanaConstants.xanaConstants.JjWorldSceneChange = true;
        XanaConstants.xanaConstants.JjWorldTeleportSceneName = worldName;
        LoadFromFile.instance._uiReferences.LoadMain(false);

    }
}
